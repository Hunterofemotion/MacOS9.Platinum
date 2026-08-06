using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

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
    public const string PartRoot = "PART_Root";

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
            new FrameworkPropertyMetadata(24d, OnTitleBarHeightChanged));

    public double TitleBarHeight
    {
        get => (double)GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    /// <summary>
    /// Lado del grow box, en unidades de WPF. El grow box se dibuja encima del
    /// contenido, igual que en Mac OS, así que cualquier vista que llegue hasta la
    /// esquina inferior derecha —una barra de desplazamiento, por ejemplo— debe
    /// reservar este espacio o quedará tapada.
    /// </summary>
    public const double GrowBoxSize = 16d;

    /// <summary>
    /// Hace que una unidad de WPF valga exactamente un píxel físico.
    /// </summary>
    /// <remarks>
    /// Platinum está diseñado en píxeles: filos de uno, biseles de uno, rayados de uno.
    /// Con la pantalla al 125 % una unidad de WPF vale 1.25 píxeles, así que ninguna de
    /// esas líneas cae entera y todas se reparten entre dos píxeles. Aplicando la escala
    /// inversa al contenido de la ventana, la rejilla de WPF coincide con la de la
    /// pantalla y todo el tema queda nítido sin trucos por control. El costo es que la
    /// interfaz se ve del tamaño que tendría en una pantalla de la época.
    /// </remarks>
    public static readonly DependencyProperty PixelPerfectProperty =
        DependencyProperty.Register(
            nameof(PixelPerfect),
            typeof(bool),
            typeof(PlatinumWindow),
            new FrameworkPropertyMetadata(false, OnPixelPerfectChanged));

    public bool PixelPerfect
    {
        get => (bool)GetValue(PixelPerfectProperty);
        set => SetValue(PixelPerfectProperty, value);
    }

    private static void OnPixelPerfectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (PlatinumWindow)d;
        window.ApplyPixelPerfectScale();
        window.UpdateCaptionHeight();
    }

    private static void OnTitleBarHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PlatinumWindow)d).UpdateCaptionHeight();
    }

    /// <summary>
    /// Alto de la barra en unidades de WPF. TitleBarHeight se declara en unidades
    /// salvo con PixelPerfect, donde mide píxeles físicos; todo cálculo de layout
    /// tiene que pasar por aquí o unas partes quedan a escala y otras no.
    /// </summary>
    /// <summary>
    /// Filas que el marco aporta por encima de la cara de la barra: el filo negro
    /// exterior y el realce blanco pegado a él (Themes/Window.xaml, PART_Root y su
    /// Border interior). Abajo aporta una sola, el filo negro.
    /// </summary>
    private const double FrameTopDips = 2d;

    private const double FrameBottomDips = 1d;

    private double TitleBarHeightInDips =>
        PixelPerfect
            ? TitleBarHeight / VisualTreeHelper.GetDpi(this).DpiScaleY
            : TitleBarHeight;

    /// <summary>
    /// Mantiene la franja arrastrable del sistema del mismo alto que la barra
    /// visible. El template trae un CaptionHeight fijo de 24 que solo vale para el
    /// caso por omisión: si el consumidor cambia TitleBarHeight, o PixelPerfect
    /// encoge la barra visual, la zona de arrastre quedaría desfasada y robaría
    /// clics a los controles pegados a ella.
    /// </summary>
    private void UpdateCaptionHeight()
    {
        // La franja arrastrable se mide desde el borde de la VENTANA, y la cara de la
        // barra empieza dos unidades más abajo: filo negro del marco y realce blanco
        // pegado a él. Sin sumarlas, las dos últimas filas de la barra de título no
        // arrastran la ventana.
        double caption = TitleBarHeightInDips + FrameTopDips;

        // El WindowChrome del estilo es compartido (y congelado) entre ventanas, así
        // que se instala una instancia propia con la altura calculada.
        WindowChrome.SetWindowChrome(this, new WindowChrome
        {
            CaptionHeight = caption,
            ResizeBorderThickness = new Thickness(4),
            GlassFrameThickness = new Thickness(0),
            CornerRadius = new CornerRadius(0),
            UseAeroCaptionButtons = false,
        });
    }

    private void ApplyPixelPerfectScale()
    {
        if (GetTemplateChild(PartRoot) is not FrameworkElement root)
        {
            return;
        }

        if (!PixelPerfect)
        {
            root.LayoutTransform = Transform.Identity;
            return;
        }

        DpiScale dpi = VisualTreeHelper.GetDpi(this);
        root.LayoutTransform = new ScaleTransform(1d / dpi.DpiScaleX, 1d / dpi.DpiScaleY);
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

        ApplyPixelPerfectScale();
        UpdateCaptionHeight();
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        ApplyPixelPerfectScale();
        UpdateCaptionHeight();
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

        // Tamaño mínimo propio: por debajo de esto el mobiliario de la barra de
        // título (close box, título, zoom y collapse) ya no cabe y se recorta. El
        // sistema solo impone ~110 unidades, insuficiente para este chrome.
        //
        // Enrollada la ventana debe poder bajar hasta la barra: Windows aplica
        // MinTrackSize también al redimensionado por programa mientras el marco
        // conserve WS_THICKFRAME, y el windowshade fija Height antes de pasar a
        // NoResize. Sin esta excepción la ventana se quedaba en el mínimo normal
        // con una franja vacía debajo de la barra.
        DpiScale dpi = VisualTreeHelper.GetDpi(this);
        double barDips = TitleBarHeightInDips;
        int minWidth = (int)Math.Ceiling(150d * dpi.DpiScaleX);
        int minHeight = (int)Math.Ceiling(
            (barDips + (IsCollapsed ? FrameTopDips + FrameBottomDips : 40d)) * dpi.DpiScaleY);
        minMax.MinTrackSize.X = Math.Max(minMax.MinTrackSize.X, minWidth);
        minMax.MinTrackSize.Y = Math.Max(minMax.MinTrackSize.Y, minHeight);

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

    // ResizeMode vigente antes de colapsar, para devolverlo tal cual al expandir:
    // un diálogo NoResize no debe despertar como CanResize.
    private ResizeMode expandedResizeMode = ResizeMode.CanResize;

    private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = (PlatinumWindow)d;

        if ((bool)e.NewValue)
        {
            // Si aún no hay layout (IsCollapsed=True desde XAML), ActualHeight es 0
            // y expandir dejaría la ventana en cero: se conserva el Height declarado.
            window.expandedHeight = window.ActualHeight > 0
                ? window.ActualHeight
                : window.Height;
            window.expandedResizeMode = window.ResizeMode;

            // El marco aporta tres filas al alto, no dos: filo negro exterior arriba,
            // realce blanco pegado a él y filo negro abajo. Con dos, la ventana
            // enrollada recortaba la última fila de la barra de título.
            window.Height = window.TitleBarHeightInDips + FrameTopDips + FrameBottomDips;
            window.ResizeMode = ResizeMode.NoResize;
        }
        else
        {
            window.Height = window.expandedHeight;
            window.ResizeMode = window.expandedResizeMode;
        }
    }
}
