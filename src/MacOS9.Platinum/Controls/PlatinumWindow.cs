using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Ventana con el chrome de Mac OS 9: close box a la izquierda, zoom box y
/// collapse box a la derecha, barra de título rayada y grow box en la esquina.
/// </summary>
[TemplatePart(Name = PartCloseBox, Type = typeof(Button))]
[TemplatePart(Name = PartZoomBox, Type = typeof(Button))]
[TemplatePart(Name = PartCollapseBox, Type = typeof(Button))]
[TemplatePart(Name = PartGrowBox, Type = typeof(FrameworkElement))]
public class PlatinumWindow : Window
{
    public const string PartCloseBox = "PART_CloseBox";
    public const string PartZoomBox = "PART_ZoomBox";
    public const string PartCollapseBox = "PART_CollapseBox";
    public const string PartGrowBox = "PART_GrowBox";

    // Mensajes de Windows para delegar el redimensionado al sistema, que es lo que
    // da el rectángulo de arrastre y el acople nativos.
    private const int WmSysCommand = 0x0112;
    private const int WmGetMinMaxInfo = 0x0024;
    private const int ScSize = 0xF000;
    private const int WmszBottomRight = 8;
    private const int MonitorDefaultToNearest = 2;

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint hWnd, int flags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfo info);

    // Windows 11 redondea las esquinas de cualquier ventana con marco de vidrio. Una
    // ventana Platinum es de esquinas rectas, así que hay que salirse de esa política
    // de forma explícita.
    private const int DwmWindowCornerPreference = 33;
    private const int DwmCornerDoNotRound = 1;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint hWnd, int attribute, ref int value, int size);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect Work;
        public int Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public NativePoint Reserved;
        public NativePoint MaxSize;
        public NativePoint MaxPosition;
        public NativePoint MinTrackSize;
        public NativePoint MaxTrackSize;
    }

    // Altura de la ventana antes de colapsarla, para poder restaurarla.
    private double expandedHeight;

    static PlatinumWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumWindow),
            new FrameworkPropertyMetadata(typeof(PlatinumWindow)));
    }

    /// <summary>
    /// Indica si la ventana está enrollada a su barra de título (windowshade).
    /// </summary>
    public static readonly DependencyProperty IsCollapsedProperty =
        DependencyProperty.Register(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(PlatinumWindow),
            new FrameworkPropertyMetadata(false, OnIsCollapsedChanged));

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    /// <summary>
    /// Alto de la barra de título. Expuesto porque el grow box y el windowshade
    /// dependen de él, y algunas ventanas de utilería usan una barra más angosta.
    /// </summary>
    public static readonly DependencyProperty TitleBarHeightProperty =
        DependencyProperty.Register(
            nameof(TitleBarHeight),
            typeof(double),
            typeof(PlatinumWindow),
            new FrameworkPropertyMetadata(24d));

    public double TitleBarHeight
    {
        get => (double)GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    /// <summary>
    /// Oculta el grow box en ventanas de tamaño fijo, como los diálogos.
    /// </summary>
    public static readonly DependencyProperty ShowGrowBoxProperty =
        DependencyProperty.Register(
            nameof(ShowGrowBox),
            typeof(bool),
            typeof(PlatinumWindow),
            new FrameworkPropertyMetadata(true));

    public bool ShowGrowBox
    {
        get => (bool)GetValue(ShowGrowBoxProperty);
        set => SetValue(ShowGrowBoxProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(PartCloseBox) is Button close)
        {
            close.Click += (_, _) => Close();
        }

        if (GetTemplateChild(PartZoomBox) is Button zoom)
        {
            // En Platinum el zoom alterna entre el tamaño del usuario y el tamaño
            // ideal de la ventana; maximizar es la equivalencia razonable en Windows.
            zoom.Click += (_, _) => WindowState =
                WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }

        if (GetTemplateChild(PartCollapseBox) is Button collapse)
        {
            collapse.Click += (_, _) => IsCollapsed = !IsCollapsed;
        }

        if (GetTemplateChild(PartGrowBox) is FrameworkElement grow)
        {
            grow.Cursor = Cursors.SizeNWSE;
            grow.MouseLeftButtonDown += OnGrowBoxPressed;
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var source = (HwndSource)PresentationSource.FromVisual(this)!;
        source.AddHook(WindowProc);

        int corners = DwmCornerDoNotRound;
        DwmSetWindowAttribute(source.Handle, DwmWindowCornerPreference, ref corners, sizeof(int));
    }

    private nint WindowProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg != WmGetMinMaxInfo)
        {
            return 0;
        }

        // Sin chrome nativo, al maximizar Windows desborda la ventana por el grosor
        // del borde de redimensionado y ese sobrante recorta las cajas de la barra de
        // título. Aquí se acota el tamaño maximizado al área de trabajo del monitor,
        // que además respeta la barra de tareas.
        nint monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
        if (monitor == 0)
        {
            return 0;
        }

        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfo(monitor, ref info))
        {
            return 0;
        }

        var minMax = Marshal.PtrToStructure<MinMaxInfo>(lParam);

        // Las coordenadas van relativas a la esquina del monitor, no del escritorio.
        minMax.MaxPosition.X = info.Work.Left - info.Monitor.Left;
        minMax.MaxPosition.Y = info.Work.Top - info.Monitor.Top;
        minMax.MaxSize.X = info.Work.Right - info.Work.Left;
        minMax.MaxSize.Y = info.Work.Bottom - info.Work.Top;

        Marshal.StructureToPtr(minMax, lParam, false);
        handled = true;
        return 0;
    }

    private void OnGrowBoxPressed(object sender, MouseButtonEventArgs e)
    {
        if (ResizeMode == ResizeMode.NoResize || IsCollapsed)
        {
            return;
        }

        // Se suelta la captura de WPF antes de ceder el arrastre al sistema; si no,
        // el mouse se queda enganchado al control y el redimensionado no arranca.
        ReleaseCapture();
        SendMessage(
            new WindowInteropHelper(this).Handle,
            WmSysCommand,
            ScSize + WmszBottomRight,
            0);

        e.Handled = true;
    }

    private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (PlatinumWindow)d;

        if ((bool)e.NewValue)
        {
            window.expandedHeight = window.ActualHeight;
            // El marco aporta un píxel arriba y otro abajo de la barra de título.
            window.Height = window.TitleBarHeight + 2;
            window.ResizeMode = ResizeMode.NoResize;
        }
        else
        {
            window.Height = window.expandedHeight;
            window.ResizeMode = ResizeMode.CanResize;
        }
    }
}
