using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Contorno del botón por omisión. Se dibuja por fuera de sus propios límites, a
/// una separación medida en píxeles físicos. No se resuelve con un Border de margen
/// negativo porque WPF redondea cada borde de la celda por separado y, con la
/// pantalla escalada, el canal sale de un píxel de un lado y de dos del otro.
/// </summary>
public class DefaultRing : FrameworkElement
{
    public DefaultRing()
    {
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        IsHitTestVisible = false;
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(DefaultRing),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>Grosor del contorno, en unidades lógicas.</summary>
    /// <remarks>
    /// En unidades lógicas y no en píxeles físicos, redondeado a píxeles enteros al
    /// dibujar. Medido en físicos, el anillo salía de un píxel en una pantalla al
    /// 200 % mientras todos los demás marcos del tema median dos: la mitad del peso
    /// del resto de la ventana. Es el mismo arreglo que llevó el contorno de las
    /// pestañas.
    /// </remarks>
    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register(
            nameof(Thickness),
            typeof(double),
            typeof(DefaultRing),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    /// <summary>Canal libre entre el botón y el contorno, en unidades lógicas.</summary>
    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(
            nameof(Gap),
            typeof(double),
            typeof(DefaultRing),
            new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Gap
    {
        get => (double)GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>Radio de esquina del botón al que sigue el contorno, en unidades lógicas.</summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(double),
            typeof(DefaultRing),
            new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
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
        if (scale <= 0) { scale = 1; }

        // Un píxel físico expresado en unidades de WPF.
        double pixel = 1d / scale;

        // Las medidas vienen en unidades lógicas y se redondean a píxeles enteros:
        // así el anillo pesa lo mismo que el resto de los marcos a cualquier escala,
        // y sus dos costados salen del mismo grosor.
        double stroke = Math.Max(1d, Math.Round(Thickness * scale)) * pixel;
        double gap = Math.Max(0d, Math.Round(Gap * scale)) * pixel;

        // El trazo se pinta centrado, así que el desplazamiento hacia afuera es el
        // canal más media pluma.
        double offset = gap + (stroke / 2d);

        var rect = new Rect(
            -offset,
            -offset,
            ActualWidth + (offset * 2d),
            ActualHeight + (offset * 2d));

        // El radio crece con la separación para que el contorno quede paralelo a la
        // figura del botón también en las esquinas.
        double radius = (Math.Round(CornerRadius * scale) * pixel) + offset;

        dc.DrawRoundedRectangle(null, new Pen(Stroke, stroke), rect, radius, radius);
    }
}
