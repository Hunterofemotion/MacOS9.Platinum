using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Icono de la tecla que despliega el calendario: una hojita con su cabecera y las
/// casillas de los días. Se dibuja a píxel físico, como las demás figuras del tema,
/// porque a 12 unidades un trazo vectorial sale borroso.
/// </summary>
public class CalendarGlyph : FrameworkElement
{
    public CalendarGlyph()
    {
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        IsHitTestVisible = false;
        Width = 14;
        Height = 13;
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(CalendarGlyph),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public static readonly DependencyProperty HeaderBrushProperty =
        DependencyProperty.Register(nameof(HeaderBrush), typeof(Brush), typeof(CalendarGlyph),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Relleno de la banda superior, la del nombre del mes.</summary>
    public Brush HeaderBrush
    {
        get => (Brush)GetValue(HeaderBrushProperty);
        set => SetValue(HeaderBrushProperty, value);
    }

    protected override void OnRender(DrawingContext dc)
    {
        double pixel = 1d / DeviceScale.Of(this).X;
        double ancho = ActualWidth;
        double alto = ActualHeight;
        if (ancho <= 0 || alto <= 0) { return; }

        var pluma = new Pen(Stroke, pixel);

        // Marco de la hoja.
        dc.DrawRectangle(Brushes.White, pluma,
            new Rect(pixel / 2, pixel / 2, ancho - pixel, alto - pixel));

        // Banda de la cabecera, del alto de tres píxeles.
        double banda = 4 * pixel;
        dc.DrawRectangle(HeaderBrush, null, new Rect(pixel, pixel, ancho - (2 * pixel), banda));
        dc.DrawLine(pluma, new Point(0, banda + pixel + (pixel / 2)),
                            new Point(ancho, banda + pixel + (pixel / 2)));

        // Casillas de los días: tres columnas por dos renglones.
        double casilla = 2 * pixel;
        double hueco = 2 * pixel;
        double x0 = 2 * pixel;
        double y0 = banda + (3 * pixel);
        for (int f = 0; f < 2; f++)
        {
            for (int c = 0; c < 3; c++)
            {
                dc.DrawRectangle(Stroke, null, new Rect(
                    x0 + (c * (casilla + hueco)),
                    y0 + (f * (casilla + hueco)),
                    casilla, casilla));
            }
        }
    }
}
