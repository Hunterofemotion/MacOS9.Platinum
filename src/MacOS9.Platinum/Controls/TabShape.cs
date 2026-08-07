using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Trapecio de una pestaña: lados inclinados hacia afuera y esquinas superiores
/// redondeadas.
/// </summary>
/// <remarks>
/// Se dibuja igual que un Border con esquinas redondeadas: geometría vectorial con
/// suavizado. Lo único que se añade es ajustar la figura a la rejilla de píxeles
/// físicos, para que los tramos rectos caigan enteros y solo se suavice lo que de
/// verdad lo necesita, que son las diagonales y las curvas.
/// </remarks>
public class TabShape : FrameworkElement
{
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? Fill
    {
        get => (Brush?)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? Stroke
    {
        get => (Brush?)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>Realce interior, pegado por dentro del borde superior.</summary>
    public static readonly DependencyProperty HighlightBrushProperty =
        DependencyProperty.Register(
            nameof(HighlightBrush),
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? HighlightBrush
    {
        get => (Brush?)GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    /// <summary>
    /// Sombra del lado derecho. Cae en el hueco entre dos pestañas y es lo que separa
    /// una de la siguiente.
    /// </summary>
    public static readonly DependencyProperty ShadowBrushProperty =
        DependencyProperty.Register(
            nameof(ShadowBrush),
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? ShadowBrush
    {
        get => (Brush?)GetValue(ShadowBrushProperty);
        set => SetValue(ShadowBrushProperty, value);
    }

    /// <summary>Cuánto se mete el borde superior respecto del inferior, por lado.</summary>
    public static readonly DependencyProperty SlantProperty =
        DependencyProperty.Register(
            nameof(Slant),
            typeof(double),
            typeof(TabShape),
            new FrameworkPropertyMetadata(7d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Slant
    {
        get => (double)GetValue(SlantProperty);
        set => SetValue(SlantProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(double),
            typeof(TabShape),
            new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Cuánto se prolonga la base hacia abajo, en píxeles físicos. Sirve para que la
    /// pestaña activa tape el filo del panel.
    /// </summary>
    public static readonly DependencyProperty BaseExtensionProperty =
        DependencyProperty.Register(
            nameof(BaseExtension),
            typeof(double),
            typeof(TabShape),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double BaseExtension
    {
        get => (double)GetValue(BaseExtensionProperty);
        set => SetValue(BaseExtensionProperty, value);
    }

    /// <summary>
    /// Apaga el suavizado. Los tramos rectos ya caen sobre la rejilla de píxeles, así
    /// que lo único que cambia son las diagonales y las curvas: sin suavizado quedan
    /// escalonadas y duras, con él quedan continuas pero con grises intermedios. A
    /// escalas fraccionarias como 125 % no hay una tercera opción.
    /// </summary>
    /// <remarks>
    /// Es adjunta y heredable a propósito: así se pone una sola vez en la ventana y
    /// baja a todas las pestañas, en vez de tener que tocar cada una.
    /// </remarks>
    public static readonly DependencyProperty SmoothProperty =
        DependencyProperty.RegisterAttached(
            "Smooth",
            typeof(bool),
            typeof(TabShape),
            new FrameworkPropertyMetadata(
                true,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender,
                OnSmoothChanged));

    public static bool GetSmooth(DependencyObject element) => (bool)element.GetValue(SmoothProperty);

    public static void SetSmooth(DependencyObject element, bool value) => element.SetValue(SmoothProperty, value);

    private static void OnSmoothChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TabShape shape)
        {
            return;
        }

        RenderOptions.SetEdgeMode(shape, (bool)e.NewValue ? EdgeMode.Unspecified : EdgeMode.Aliased);
        shape.InvalidateVisual();
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

        double scale = DeviceScale.Of(this).X;
        double pixel = 1d / scale;

        // El contorno mide una unidad lógica, redondeada a píxeles enteros: uno a
        // escala normal y dos al 200 %. Fijarlo en un píxel físico dejaba la pestaña
        // con el filo a la mitad del grosor de los demás marcos del tema, que salen
        // de un BorderThickness de una unidad.
        double grosor = Math.Max(1d, Math.Round(scale)) * pixel;
        double medio = grosor / 2d;

        // Ajuste a la rejilla de píxeles físicos. El trazo se pinta centrado, de ahí
        // el medio grosor: así el filo cae entero sobre una columna de píxeles y no
        // repartido entre dos.
        double left = medio;
        double top = medio;
        double right = (Math.Round(ActualWidth * scale) * pixel) - medio;
        double bottom = Math.Round(ActualHeight * scale) * pixel;

        double slant = Math.Round(Slant * scale) * pixel;
        double radius = Math.Round(CornerRadius * scale) * pixel;

        // Con pestañas angostas el trapecio se cerraría sobre sí mismo.
        double room = (right - left - (radius * 2d)) / 2d;
        if (slant > room)
        {
            slant = Math.Max(0d, room);
        }

        var pen = new Pen(Stroke, grosor);

        dc.DrawGeometry(Fill, null, BuildGeometry(left, top, right, bottom, slant, radius, closed: true));

        if (BaseExtension > 0d)
        {
            // La base se prolonga recta, del ancho completo: alargar la figura
            // inclinaría más los lados y borraría de más a cada costado.
            double extension = Math.Round(BaseExtension * scale) * pixel;
            dc.DrawRectangle(Fill, null, new Rect(left, bottom, right - left, extension));
        }

        // El contorno se dibuja abierto: la pestaña activa se funde con el panel de
        // abajo, así que el lado inferior nunca lleva filo.
        dc.DrawGeometry(null, pen, BuildGeometry(left, top, right, bottom, slant, radius, closed: false));

        if (HighlightBrush is not null)
        {
            dc.DrawLine(
                new Pen(HighlightBrush, grosor),
                new Point(left + slant + radius, top + grosor),
                new Point(right - slant - radius, top + grosor));
        }

        if (ShadowBrush is not null)
        {
            dc.DrawLine(
                new Pen(ShadowBrush, grosor),
                new Point(right - slant + grosor, top + radius),
                new Point(right + grosor, bottom));
        }
    }

    private static Geometry BuildGeometry(
        double left,
        double top,
        double right,
        double bottom,
        double slant,
        double radius,
        bool closed)
    {
        // Empalme tangente: la curva arranca sobre el propio lado inclinado, a una
        // distancia del vértice igual al radio, y usa el vértice como punto de control.
        // Si arranca subiendo en vertical, en la unión con el lado queda un pico.
        double rise = bottom - top;
        double length = Math.Sqrt((slant * slant) + (rise * rise));
        double stepX = length > 0d ? (slant / length) * radius : 0d;
        double stepY = length > 0d ? (rise / length) * radius : 0d;

        var geometry = new StreamGeometry();

        using (StreamGeometryContext ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(left, bottom), isFilled: true, isClosed: closed);

            ctx.LineTo(new Point(left + slant - stepX, top + stepY), isStroked: true, isSmoothJoin: true);
            ctx.QuadraticBezierTo(
                new Point(left + slant, top),
                new Point(left + slant + radius, top),
                isStroked: true,
                isSmoothJoin: true);

            ctx.LineTo(new Point(right - slant - radius, top), isStroked: true, isSmoothJoin: true);
            ctx.QuadraticBezierTo(
                new Point(right - slant, top),
                new Point(right - slant + stepX, top + stepY),
                isStroked: true,
                isSmoothJoin: true);

            ctx.LineTo(new Point(right, bottom), isStroked: true, isSmoothJoin: true);
        }

        geometry.Freeze();
        return geometry;
    }
}
