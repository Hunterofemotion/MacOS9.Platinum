using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Rayado horizontal de la barra de título. No usa un DrawingBrush con mosaico
/// porque el mosaico se define en unidades independientes del dispositivo: con la
/// pantalla al 150 % cada línea cae sobre un píxel y medio y Windows la interpola,
/// que es lo que hacía verse el rayado como un degradado. Aquí las líneas se
/// dibujan midiendo un píxel físico exacto.
/// </summary>
public class Pinstripe : FrameworkElement
{
    public Pinstripe()
    {
        // Sin suavizado de bordes: las líneas de un píxel deben quedar duras.
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
    }

    public static readonly DependencyProperty LightBrushProperty =
        DependencyProperty.Register(
            nameof(LightBrush),
            typeof(Brush),
            typeof(Pinstripe),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush LightBrush
    {
        get => (Brush)GetValue(LightBrushProperty);
        set => SetValue(LightBrushProperty, value);
    }

    public static readonly DependencyProperty DarkBrushProperty =
        DependencyProperty.Register(
            nameof(DarkBrush),
            typeof(Brush),
            typeof(Pinstripe),
            new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush DarkBrush
    {
        get => (Brush)GetValue(DarkBrushProperty);
        set => SetValue(DarkBrushProperty, value);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        double width = ActualWidth;
        double height = ActualHeight;
        if (width <= 0 || height <= 0)
        {
            return;
        }

        // Un píxel físico expresado en unidades de WPF. A 100 % vale 1, a 150 % vale
        // 0.666..., y así cada franja aterriza justo en el borde de un píxel.
        double pixel = 1d / VisualTreeHelper.GetDpi(this).DpiScaleY;

        Brush light = LightBrush;
        Brush dark = DarkBrush;

        for (double y = 0; y + pixel <= height; y += pixel * 2)
        {
            dc.DrawRectangle(light, null, new Rect(0, y, width, pixel));

            if (y + pixel * 2 <= height)
            {
                dc.DrawRectangle(dark, null, new Rect(0, y + pixel, width, pixel));
            }
        }
    }
}
