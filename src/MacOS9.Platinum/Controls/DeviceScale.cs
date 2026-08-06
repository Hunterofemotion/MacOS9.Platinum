using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Cuántos píxeles físicos mide una unidad de WPF para un elemento concreto.
/// <para>
/// Los controles que se rasterizan a mano (rayado, flechas, tablero y cursor del
/// deslizador) necesitan este número para colocar cada trazo en una fila entera de
/// píxeles. Preguntarle la escala al monitor no basta: cualquier transformación en el
/// camino hasta la raíz visual cambia el tamaño real de la unidad, y este tema instala
/// una —el ScaleTransform de <see cref="PlatinumWindow.PixelPerfect"/>—. Con la escala
/// del monitor a secas, dentro de esa ventana cada celda del tablero caía en
/// coordenadas fraccionarias y volvía el moiré que la rasterización venía a evitar.
/// </para>
/// </summary>
internal static class DeviceScale
{
    public static (double X, double Y) Of(Visual visual)
    {
        DpiScale dpi = VisualTreeHelper.GetDpi(visual);
        PresentationSource source = PresentationSource.FromVisual(visual);

        // Sin fuente de presentación el elemento aún no cuelga de ninguna ventana
        // (o se está renderizando fuera de pantalla): la escala del monitor es lo
        // único que hay y es la respuesta correcta para ese caso.
        if (source?.CompositionTarget is null || source.RootVisual is null)
        {
            return (dpi.DpiScaleX, dpi.DpiScaleY);
        }

        try
        {
            GeneralTransform toRoot = visual.TransformToAncestor(source.RootVisual);
            Point origin = toRoot.Transform(new Point(0d, 0d));
            Point unitX = toRoot.Transform(new Point(1d, 0d));
            Point unitY = toRoot.Transform(new Point(0d, 1d));

            Matrix toDevice = source.CompositionTarget.TransformToDevice;
            double x = Math.Abs(unitX.X - origin.X) * toDevice.M11;
            double y = Math.Abs(unitY.Y - origin.Y) * toDevice.M22;

            // Una transformación degenerada (escala cero, rotación de 90°) dejaría el
            // trazo en cero: en ese caso vale más el dato del monitor que un cero.
            return (x > 0d ? x : dpi.DpiScaleX, y > 0d ? y : dpi.DpiScaleY);
        }
        catch (InvalidOperationException)
        {
            // El elemento no cuelga de esa raíz: pasa entre un cambio de plantilla y
            // el siguiente pase de layout.
            return (dpi.DpiScaleX, dpi.DpiScaleY);
        }
    }
}
