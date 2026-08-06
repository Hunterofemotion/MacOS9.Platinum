using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Tablero de dos grises a un píxel físico por celda. Sustituye al DrawingBrush con
/// mosaico del canal del scrollbar: un tile de 2 unidades lógicas mide 2.5 píxeles a
/// 125 % y el mosaico se interpola en un moiré de tonos inventados. Aquí cada celda
/// se pinta como rectángulo de coordenadas físicas enteras, así que el punteado
/// queda duro a cualquier escala.
/// </summary>
public class CheckerTexture : FrameworkElement
{
    public CheckerTexture()
    {
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        IsHitTestVisible = false;
    }

    public static readonly DependencyProperty LightBrushProperty =
        DependencyProperty.Register(
            nameof(LightBrush),
            typeof(Brush),
            typeof(CheckerTexture),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush LightBrush
    {
        get => (Brush)GetValue(LightBrushProperty);
        set => SetValue(LightBrushProperty, value);
    }

    public static readonly DependencyProperty DarkBrushProperty =
        DependencyProperty.Register(
            nameof(DarkBrush),
            typeof(Brush),
            typeof(CheckerTexture),
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
        if (ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        (double scale, double scaleY) = DeviceScale.Of(this);
        double pixel = 1d / scale;

        int width = (int)Math.Round(ActualWidth * scale);
        int height = (int)Math.Round(ActualHeight * scaleY);

        // El fondo entero en claro y encima solo las celdas oscuras.
        dc.DrawRectangle(LightBrush, null, new Rect(0d, 0d, width * pixel, height * pixel));

        // Las celdas oscuras van como un mosaico de 2x2 píxeles físicos en vez de un
        // rectángulo por celda: el canal de una barra de ventana son unos 6.600
        // rectángulos que se volvían a emitir en cada cuadro del arrastre. El viewport
        // se declara en unidades absolutas equivalentes a 2 píxeles físicos exactos,
        // que es lo que evita el moiré del mosaico lógico original.
        dc.DrawRectangle(
            Tile(DarkBrush, pixel),
            null,
            new Rect(0d, 0d, width * pixel, height * pixel));
    }

    // El mosaico solo cambia con la escala o con el color, y OnRender se vuelve a
    // ejecutar en cada cuadro del arrastre: sin este cacheo se construía y congelaba
    // un DrawingBrush nuevo por cuadro.
    private Brush? tile;
    private double tilePixel;
    private Brush? tileDark;

    private Brush Tile(Brush dark, double pixel)
    {
        if (tile is not null && tilePixel == pixel && ReferenceEquals(tileDark, dark))
        {
            return tile;
        }

        tile = BuildTile(dark, pixel);
        tilePixel = pixel;
        tileDark = dark;
        return tile;
    }

    private static Brush BuildTile(Brush dark, double pixel)
    {
        var cells = new DrawingGroup();
        cells.Children.Add(new GeometryDrawing(dark, null, new RectangleGeometry(new Rect(0d, 0d, pixel, pixel))));
        cells.Children.Add(new GeometryDrawing(dark, null, new RectangleGeometry(new Rect(pixel, pixel, pixel, pixel))));

        var brush = new DrawingBrush(cells)
        {
            TileMode = TileMode.Tile,
            ViewportUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0d, 0d, pixel * 2d, pixel * 2d),
            ViewboxUnits = BrushMappingMode.Absolute,
            Viewbox = new Rect(0d, 0d, pixel * 2d, pixel * 2d),
            Stretch = Stretch.None,
        };
        brush.Freeze();
        return brush;
    }
}
