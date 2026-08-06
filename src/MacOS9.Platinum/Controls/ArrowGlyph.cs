using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

public enum ArrowDirection
{
    Up,
    Down,
    Left,
    Right,
}

/// <summary>
/// Triángulo de las flechas de la barra de desplazamiento. Se rasteriza fila por fila
/// en píxeles físicos: un triángulo vectorial centrado por layout queda medio píxel
/// corrido cuando la pantalla está a una escala fraccionaria, y a este tamaño ese
/// medio píxel se nota.
/// </summary>
public class ArrowGlyph : FrameworkElement
{
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(ArrowGlyph),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? Fill
    {
        get => (Brush?)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(
            nameof(Direction),
            typeof(ArrowDirection),
            typeof(ArrowGlyph),
            new FrameworkPropertyMetadata(ArrowDirection.Down, FrameworkPropertyMetadataOptions.AffectsRender));

    public ArrowDirection Direction
    {
        get => (ArrowDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// Número de escalones del triángulo, en píxeles físicos. La base mide
    /// <c>2 × Steps − 1</c>, siempre impar, que es lo que permite centrarlo exacto.
    /// </summary>
    public static readonly DependencyProperty StepsProperty =
        DependencyProperty.Register(
            nameof(Steps),
            typeof(int),
            typeof(ArrowGlyph),
            new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsRender));

    public int Steps
    {
        get => (int)GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        InvalidateVisual();
    }

    /// <summary>
    /// Sin esto el elemento mide cero y desaparece: un FrameworkElement que solo pinta
    /// no tiene tamaño propio, y dentro de un contenedor centrado se queda sin espacio.
    /// </summary>
    protected override Size MeasureOverride(Size availableSize)
    {
        double scale = VisualTreeHelper.GetDpi(this).DpiScaleX;
        if (scale <= 0d)
        {
            scale = 1d;
        }

        int steps = Math.Max(1, Steps);
        int span = (steps * 2) - 1;

        return Direction is ArrowDirection.Up or ArrowDirection.Down
            ? new Size(span / scale, steps / scale)
            : new Size(steps / scale, span / scale);
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (Fill is null || ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        double scale = VisualTreeHelper.GetDpi(this).DpiScaleX;
        double pixel = 1d / scale;

        int width = (int)Math.Round(ActualWidth * scale);
        int height = (int)Math.Round(ActualHeight * scale);

        int steps = Math.Max(1, Steps);
        int span = (steps * 2) - 1;

        bool vertical = Direction is ArrowDirection.Up or ArrowDirection.Down;
        int across = vertical ? width : height;
        int along = vertical ? height : width;

        // Sobrante a cada lado. Se reparte de forma que la fila más ancha quede
        // centrada; si sobra un píxel impar, se queda del lado inicial.
        int offsetAcross = (across - span) / 2;
        int offsetAlong = (along - steps) / 2;

        for (int step = 0; step < steps; step++)
        {
            // La fila 0 es la base; cada paso la angosta dos píxeles.
            int run = span - (step * 2);
            int start = offsetAcross + step;

            // Up y Left crecen al revés: la punta va primero.
            int index = Direction is ArrowDirection.Up or ArrowDirection.Left
                ? steps - 1 - step
                : step;

            int position = offsetAlong + index;

            Rect rect = vertical
                ? new Rect(start * pixel, position * pixel, run * pixel, pixel)
                : new Rect(position * pixel, start * pixel, pixel, run * pixel);

            dc.DrawRectangle(Fill, null, rect);
        }
    }
}
