using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Ventana de paleta (el «windoid» de Mac OS 9): barra de título angosta toda
/// rayada, close box chico y marco delgado. Es la ventana de las herramientas que
/// flotan sobre el documento.
/// <para>
/// Para que flote sobre su documento hay que darle dueña (<see cref="Window.Owner"/>);
/// sin dueña es una ventana suelta más. No sale en la barra de tareas: en el
/// original las paletas no eran ventanas por derecho propio sino muebles de su
/// aplicación.
/// </para>
/// <para>
/// La regla de encendido también es la del original: una paleta no se apaga por
/// perder el foco frente a su documento —trabajan juntas—, solo cuando la
/// aplicación entera pasa al fondo. La medida es de la aplicación y no de la
/// dueña: se escuchan Activated y Deactivated de <see cref="Application"/>, así
/// da igual cuándo se asigne la dueña o cuál de los documentos mande.
/// </para>
/// </summary>
public class PlatinumPaletteWindow : PlatinumWindow
{
    private const int GwlStyle = -16;
    private const int WsMaximizeBox = 0x00010000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(nint hWnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(nint hWnd, int index, int value);

    // La aplicación empieza al frente desde la perspectiva de una paleta: quien
    // la crea lo hace desde una ventana que manda. Los eventos la mantienen al día.
    private bool applicationIsActive = true;

    static PlatinumPaletteWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumPaletteWindow),
            new FrameworkPropertyMetadata(typeof(PlatinumPaletteWindow)));
    }

    public PlatinumPaletteWindow()
    {
        // Application.Deactivated solo dispara al ceder el frente a OTRA
        // aplicación, no al pasar el foco entre ventanas propias: es exactamente
        // la frontera que la paleta necesita medir. Cada evento dice de qué lado
        // quedó la aplicación; consultar IsActive de las ventanas aquí sería una
        // carrera, porque Activated llega antes de que la ventana elegida lo
        // publique.
        if (Application.Current is Application app)
        {
            app.Activated += OnApplicationActivated;
            app.Deactivated += OnApplicationDeactivated;
            Closed += (_, _) =>
            {
                app.Activated -= OnApplicationActivated;
                app.Deactivated -= OnApplicationDeactivated;
            };
        }
    }

    private void OnApplicationActivated(object? sender, EventArgs e)
    {
        applicationIsActive = true;
        RefreshShowsActive();
    }

    private void OnApplicationDeactivated(object? sender, EventArgs e)
    {
        applicationIsActive = false;
        RefreshShowsActive();
    }

    /// <summary>Encendida mientras ella o cualquier ventana de la aplicación mande.</summary>
    protected override bool ComputeShowsActive() => IsActive || applicationIsActive;

    // La paleta no tiene zoom box ni zoom: en el original el windoid solo se
    // enrollaba. Sin quitar el estilo, el doble clic en la barra y Win+Flecha
    // arriba la mandaban a pantalla completa.
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        nint handle = new WindowInteropHelper(this).Handle;
        _ = SetWindowLong(handle, GwlStyle, GetWindowLong(handle, GwlStyle) & ~WsMaximizeBox);
    }

    // Con cajas de 11 y sin zoom box la barra vive cómoda muy por debajo del piso
    // de la ventana de documento; una paleta de herramientas es angosta por
    // vocación.
    protected override double MinimumChromeWidthDips => 60d;

    protected override double MinimumContentHeightDips => 24d;
}
