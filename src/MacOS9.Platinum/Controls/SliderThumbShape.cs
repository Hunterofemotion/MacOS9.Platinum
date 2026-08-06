using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Pentágono del cursor del deslizador, rasterizado fila por fila en píxeles
/// físicos. Un Path vectorial de 11x14 con coordenadas .5 deja el lado inicial
/// negro nítido y el final lavado a escalas fraccionarias; aquí los dos costados
/// pesan exactamente un píxel.
/// </summary>
public class SliderThumbShape : FrameworkElement
{
    // Diseño en unidades lógicas; se redondea a físico al dibujar.
    private const double BodyLogical = 11d;
    private const double LengthLogical = 14d;
    private const double RectLogical = 8d;

    public SliderThumbShape()
    {
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(SliderThumbShape),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(SliderThumbShape),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>Horizontal apunta hacia abajo; Vertical apunta a la derecha.</summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation),
            typeof(System.Windows.Controls.Orientation),
            typeof(SliderThumbShape),
            new FrameworkPropertyMetadata(
                System.Windows.Controls.Orientation.Horizontal,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public System.Windows.Controls.Orientation Orientation
    {
        get => (System.Windows.Controls.Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        InvalidateMeasure();
        InvalidateVisual();
    }

    /// <summary>
    /// Medidas del pentágono en píxeles físicos. El cono se cierra solo cuando el
    /// sangrado alcanza el centro, así que el largo real lo manda el cuerpo, no la
    /// constante de diseño: con cuerpo 11 el cono ocupa 5 filas, no 6. Medir y
    /// dibujar tienen que salir de aquí o el Track centra el cursor por un tamaño
    /// que nadie pinta y el pentágono queda corrido respecto del carril.
    /// </summary>
    private static (int Body, int Rect, int Length) Metrics(double scale)
    {
        // El cuerpo debe ser impar en píxeles físicos para que la punta caiga en una
        // columna central exacta.
        int body = (int)Math.Round(BodyLogical * scale);
        if ((body & 1) == 0)
        {
            body -= 1;
        }

        int taper = (body + 1) / 2;
        int rect = (int)Math.Round(RectLogical * scale);

        // Si el diseño pide más tramo recto del que cabe, cede el recto.
        int declared = (int)Math.Round(LengthLogical * scale);
        if (rect + taper > declared)
        {
            rect = declared - taper;
        }

        // El bucle de dibujo termina en rect + taper - 2, así que ese es el largo
        // que de verdad se pinta.
        return (body, rect, rect + taper - 1);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double scale = DeviceScale.Of(this).X;
        (int body, _, int length) = Metrics(scale);
        double bodyLogical = body / scale;
        double lengthLogical = length / scale;

        return Orientation == System.Windows.Controls.Orientation.Horizontal
            ? new Size(bodyLogical, lengthLogical)
            : new Size(lengthLogical, bodyLogical);
    }

    protected override void OnRender(DrawingContext dc)
    {
        double scale = DeviceScale.Of(this).X;
        double pixel = 1d / scale;

        (int body, int rect, int length) = Metrics(scale);

        bool horizontal = Orientation == System.Windows.Controls.Orientation.Horizontal;

        // Sangrado por fila a lo largo del eje de la punta: cero en el tramo recto y
        // creciendo de a un píxel por lado en el cono.
        Brush fill = Fill;
        Brush stroke = Stroke;

        for (int i = 0; i < length; i++)
        {
            int inset = i < rect ? 0 : (i - rect + 1);
            int from = inset;
            int to = body - inset;
            if (to <= from)
            {
                break;
            }

            bool tip = (to - from) <= 2 || i == length - 1;

            if (horizontal)
            {
                dc.DrawRectangle(fill, null, new Rect(from * pixel, i * pixel, (to - from) * pixel, pixel));

                if (i == 0 || tip)
                {
                    dc.DrawRectangle(stroke, null, new Rect(from * pixel, i * pixel, (to - from) * pixel, pixel));
                }
                else
                {
                    dc.DrawRectangle(stroke, null, new Rect(from * pixel, i * pixel, pixel, pixel));
                    dc.DrawRectangle(stroke, null, new Rect((to - 1) * pixel, i * pixel, pixel, pixel));
                }
            }
            else
            {
                dc.DrawRectangle(fill, null, new Rect(i * pixel, from * pixel, pixel, (to - from) * pixel));

                if (i == 0 || tip)
                {
                    dc.DrawRectangle(stroke, null, new Rect(i * pixel, from * pixel, pixel, (to - from) * pixel));
                }
                else
                {
                    dc.DrawRectangle(stroke, null, new Rect(i * pixel, from * pixel, pixel, pixel));
                    dc.DrawRectangle(stroke, null, new Rect(i * pixel, (to - 1) * pixel, pixel, pixel));
                }
            }
        }
    }
}
