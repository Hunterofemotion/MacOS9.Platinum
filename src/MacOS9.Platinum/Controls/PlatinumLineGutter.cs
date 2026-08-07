using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Franja de números de renglón para un cuadro de texto.
/// </summary>
/// <remarks>
/// Se apunta a un <see cref="TextBox"/> por la propiedad <see cref="Target"/> en
/// lugar de envolverlo: el cuadro sigue siendo el del tema, con su marco, su
/// selección y su scrollbar, y la franja solo lee posiciones. Envolverlo obligaría
/// a rehacer la plantilla del cuadro y a mantener dos.
///
/// Los renglones se cuentan como los cuenta el cuadro: por renglón dibujado, así
/// que uno partido por ajuste de línea gasta un número por cada tramo. Es lo que
/// tiene que pasar, porque el número señala una posición en la pantalla.
/// </remarks>
public class PlatinumLineGutter : FrameworkElement
{
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(
            nameof(Target),
            typeof(TextBox),
            typeof(PlatinumLineGutter),
            new FrameworkPropertyMetadata(null, AlCambiarObjetivo));

    public static readonly DependencyProperty ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner(
            typeof(PlatinumLineGutter),
            new FrameworkPropertyMetadata(
                Brushes.Black,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BackgroundProperty =
        Panel.BackgroundProperty.AddOwner(
            typeof(PlatinumLineGutter),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RuleBrushProperty =
        DependencyProperty.Register(
            nameof(RuleBrush),
            typeof(Brush),
            typeof(PlatinumLineGutter),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PaddingProperty =
        Control.PaddingProperty.AddOwner(
            typeof(PlatinumLineGutter),
            new FrameworkPropertyMetadata(
                new Thickness(4, 0, 5, 0),
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty FontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner(typeof(PlatinumLineGutter));

    public static readonly DependencyProperty FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner(typeof(PlatinumLineGutter));

    private ScrollViewer? _visor;

    public PlatinumLineGutter()
    {
        // El enlace al cuadro puede resolverse después de que el cuadro ya se cargó,
        // y entonces su evento Loaded no vuelve a dispararse. Al cargarse la franja
        // se reintenta el enganche y se vuelve a medir: para entonces el cuadro ya
        // sabe cuántos renglones tiene, que es lo que fija el ancho.
        Loaded += (_, _) =>
        {
            if (Target is not null && _visor is null)
                EngancharVisor(Target);

            InvalidateMeasure();
            InvalidateVisual();
        };
    }

    public TextBox? Target
    {
        get => (TextBox?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>Color de la línea que separa la franja del texto.</summary>
    public Brush? RuleBrush
    {
        get => (Brush?)GetValue(RuleBrushProperty);
        set => SetValue(RuleBrushProperty, value);
    }

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    private static void AlCambiarObjetivo(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var franja = (PlatinumLineGutter)d;
        franja.Desconectar((TextBox?)e.OldValue);
        franja.Conectar((TextBox?)e.NewValue);
    }

    private void Conectar(TextBox? cuadro)
    {
        if (cuadro is null) return;

        cuadro.TextChanged += AlCambiarTexto;
        cuadro.SizeChanged += AlRedimensionar;

        // El visor está dentro de la plantilla, así que puede no existir todavía.
        // Si aún no se aplica, se vuelve a intentar cuando el cuadro se cargue y,
        // si para entonces ya se cargó, cuando se cargue la franja.
        if (!EngancharVisor(cuadro) && !cuadro.IsLoaded)
            cuadro.Loaded += AlCargarObjetivo;
    }

    private void Desconectar(TextBox? cuadro)
    {
        if (cuadro is null) return;

        cuadro.TextChanged -= AlCambiarTexto;
        cuadro.SizeChanged -= AlRedimensionar;
        cuadro.Loaded -= AlCargarObjetivo;

        if (_visor is not null)
        {
            _visor.ScrollChanged -= AlDesplazar;
            _visor = null;
        }
    }

    private bool EngancharVisor(TextBox cuadro)
    {
        cuadro.ApplyTemplate();
        _visor = cuadro.Template?.FindName("PART_ContentHost", cuadro) as ScrollViewer;
        if (_visor is null) return false;

        _visor.ScrollChanged += AlDesplazar;
        return true;
    }

    private void AlCargarObjetivo(object sender, RoutedEventArgs e)
    {
        var cuadro = (TextBox)sender;
        cuadro.Loaded -= AlCargarObjetivo;
        EngancharVisor(cuadro);
        InvalidateVisual();
    }

    private void AlCambiarTexto(object sender, TextChangedEventArgs e)
    {
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void AlRedimensionar(object sender, SizeChangedEventArgs e) => InvalidateVisual();

    private void AlDesplazar(object sender, ScrollChangedEventArgs e) => InvalidateVisual();

    protected override Size MeasureOverride(Size disponible)
    {
        // El ancho sale de cuántos dígitos tiene el último renglón, no del que se
        // ve ahora: si creciera al desplazar, el texto se recorrería solo.
        int renglones = Math.Max(1, Target?.LineCount ?? 1);
        var muestra = Texto(new string('0', renglones.ToString(CultureInfo.InvariantCulture).Length));

        double ancho = muestra.Width + Padding.Left + Padding.Right + 1;
        return new Size(ancho, 0);
    }

    protected override void OnRender(DrawingContext dc)
    {
        var caja = new Rect(RenderSize);

        if (Background is not null)
            dc.DrawRectangle(Background, null, caja);

        if (RuleBrush is not null)
        {
            // La línea se cuadra a píxel físico para que no salga difuminada en
            // pantallas escaladas.
            double grosor = 1 / VisualTreeHelper.GetDpi(this).DpiScaleX;
            dc.DrawRectangle(RuleBrush, null,
                new Rect(caja.Right - grosor, 0, grosor, caja.Height));
        }

        var cuadro = Target;
        if (cuadro is null || !cuadro.IsLoaded) return;

        // Mientras el cuadro no ha terminado su acomodo, LineCount vale 0 y sin
        // embargo los dos índices visibles devuelven 0: pedirle el renglón cero a un
        // cuadro con cero renglones revienta. El conteo es el que manda.
        int renglones = cuadro.LineCount;
        if (renglones <= 0) return;

        int primero = Math.Max(0, cuadro.GetFirstVisibleLineIndex());
        int ultimo = Math.Min(renglones - 1, cuadro.GetLastVisibleLineIndex());
        if (ultimo < primero) return;

        // El desfase vertical entre el cuadro y la franja: los dos comparten
        // ventana, así que basta con transformar un punto.
        double desfase = 0;
        if (cuadro.IsVisible && IsVisible)
            desfase = cuadro.TransformToVisual(this).Transform(new Point(0, 0)).Y;

        double interior = caja.Width - Padding.Right - 1;

        for (int i = primero; i <= ultimo; i++)
        {
            int caracter = cuadro.GetCharacterIndexFromLineIndex(i);
            var lugar = cuadro.GetRectFromCharacterIndex(caracter);
            if (double.IsInfinity(lugar.Y)) continue;

            var numero = Texto((i + 1).ToString(CultureInfo.InvariantCulture));
            dc.DrawText(numero, new Point(interior - numero.Width, lugar.Y + desfase));
        }
    }

    // Se usa la sobrecarga con TextFormattingMode porque el tema entero dibuja en
    // modo Display: sin eso los dígitos de la franja caen en fracciones de píxel y
    // se ven de otro grosor que el texto de al lado.
    private FormattedText Texto(string valor) =>
        new(valor,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
            FontSize,
            Foreground,
            null,
            TextFormattingMode.Display,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
}
