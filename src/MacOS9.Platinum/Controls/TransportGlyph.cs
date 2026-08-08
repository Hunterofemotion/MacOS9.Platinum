using System;
using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>Qué signo de transporte se dibuja.</summary>
public enum TransportKind
{
    Play,
    Pause,
    Stop,
    Record,
    Rewind,
    FastForward,
    SkipBack,
    SkipForward,
    Eject
}

/// <summary>
/// Los signos de una botonera de reproducción, dibujados a píxel.
/// </summary>
/// <remarks>
/// Son signos y no objetos, así que van planos y sin volumen: darles relieve les
/// quita legibilidad y no aporta nada, porque no hay una cosa real que recordar.
/// Y van sueltos, sin marco propio: el marco lo pone la tecla que los aloja. Los
/// que salieron del generador de imágenes venían dibujados como teclas con bisel,
/// y metidos dentro de una tecla de verdad quedaba tecla dentro de tecla.
///
/// Se dibujan por escalones enteros como <see cref="ArrowGlyph"/> y por el mismo
/// motivo: un triángulo vectorial a doce píxeles sale con los filos lavados.
/// </remarks>
public class TransportGlyph : FrameworkElement
{
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(
            nameof(Kind),
            typeof(TransportKind),
            typeof(TransportGlyph),
            new FrameworkPropertyMetadata(
                TransportKind.Play,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public TransportKind Kind
    {
        get => (TransportKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(TransportGlyph),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? Fill
    {
        get => (Brush?)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    /// <summary>
    /// Alto del signo en píxeles físicos. Impar a propósito: un triángulo de altura
    /// par no tiene punta, tiene meseta. En cero se calcula.
    /// </summary>
    /// <remarks>
    /// Sin asignar sale de <see cref="LogicalSize"/> multiplicado por la escala de
    /// la pantalla, así que el signo mide lo mismo a simple vista al 100 % y al
    /// 200 %. Un número fijo de píxeles físicos, que es lo que hace ArrowGlyph, deja
    /// el signo a la mitad de tamaño en una pantalla escalada; en una flecha de
    /// scrollbar eso da igual porque es diminuta, en una tecla de reproducir no.
    /// </remarks>
    public static readonly DependencyProperty StepsProperty =
        DependencyProperty.Register(
            nameof(Steps),
            typeof(int),
            typeof(TransportGlyph),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public int Steps
    {
        get => (int)GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    public static readonly DependencyProperty LogicalSizeProperty =
        DependencyProperty.Register(
            nameof(LogicalSize),
            typeof(double),
            typeof(TransportGlyph),
            new FrameworkPropertyMetadata(
                11d,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Alto del signo en unidades lógicas, cuando Steps va en cero.</summary>
    public double LogicalSize
    {
        get => (double)GetValue(LogicalSizeProperty);
        set => SetValue(LogicalSizeProperty, value);
    }

    private int AltoEnPixeles()
    {
        if (Steps > 0) { return Impar(Steps); }

        double escala = DeviceScale.Of(this).X;
        if (escala <= 0) { escala = 1; }

        return Impar((int)Math.Round(LogicalSize * escala));
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        InvalidateVisual();
    }

    protected override Size MeasureOverride(Size disponible)
    {
        double escala = DeviceScale.Of(this).X;
        if (escala <= 0) { escala = 1; }

        int alto = AltoEnPixeles();
        int ancho = AnchoDe(alto);

        return new Size(ancho / escala, alto / escala);
    }

    private static int Impar(int valor) => Math.Max(3, valor % 2 == 0 ? valor + 1 : valor);

    /// <summary>Ancho en píxeles que pide cada signo para un alto dado.</summary>
    private int AnchoDe(int alto)
    {
        int medio = (alto / 2) + 1;
        int hueco = Math.Max(2, alto / 5);

        return Kind switch
        {
            TransportKind.Play => medio,
            TransportKind.Stop or TransportKind.Record => alto,
            TransportKind.Pause => (hueco * 3),
            TransportKind.Rewind or TransportKind.FastForward => (medio * 2) + 1,
            TransportKind.SkipBack or TransportKind.SkipForward => medio + hueco + 1,
            TransportKind.Eject => alto,
            _ => alto,
        };
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (Fill is null || ActualWidth <= 0 || ActualHeight <= 0) { return; }

        double escala = DeviceScale.Of(this).X;
        if (escala <= 0) { escala = 1; }
        double pixel = 1d / escala;

        int alto = AltoEnPixeles();
        int ancho = AnchoDe(alto);

        // Se centra en píxeles enteros. Con un sobrante impar el píxel de más se
        // queda del lado inicial, igual que en ArrowGlyph.
        int cajaAncho = (int)Math.Round(ActualWidth * escala);
        int cajaAlto = (int)Math.Round(ActualHeight * escala);
        int x0 = (cajaAncho - ancho) / 2;
        int y0 = (cajaAlto - alto) / 2;

        int medio = (alto / 2) + 1;
        int hueco = Math.Max(2, alto / 5);

        switch (Kind)
        {
            case TransportKind.Play:
                Triangulo(dc, pixel, x0, y0, alto, derecha: true);
                break;

            case TransportKind.Pause:
                Caja(dc, pixel, x0, y0, hueco, alto);
                Caja(dc, pixel, x0 + (hueco * 2), y0, hueco, alto);
                break;

            case TransportKind.Stop:
                Caja(dc, pixel, x0, y0, alto, alto);
                break;

            case TransportKind.Record:
                Disco(dc, pixel, x0, y0, alto);
                break;

            case TransportKind.Rewind:
                Triangulo(dc, pixel, x0, y0, alto, derecha: false);
                Triangulo(dc, pixel, x0 + medio + 1, y0, alto, derecha: false);
                break;

            case TransportKind.FastForward:
                Triangulo(dc, pixel, x0, y0, alto, derecha: true);
                Triangulo(dc, pixel, x0 + medio + 1, y0, alto, derecha: true);
                break;

            case TransportKind.SkipBack:
                Caja(dc, pixel, x0, y0, hueco, alto);
                Triangulo(dc, pixel, x0 + hueco + 1, y0, alto, derecha: false);
                break;

            case TransportKind.SkipForward:
                Triangulo(dc, pixel, x0, y0, alto, derecha: true);
                Caja(dc, pixel, x0 + medio + 1, y0, hueco, alto);
                break;

            case TransportKind.Eject:
                // Triángulo arriba y su base debajo, separados por un renglón: es
                // como se leía el signo de expulsar en la unidad de disco.
                TrianguloArriba(dc, pixel, x0, y0, alto - hueco - 1);
                Caja(dc, pixel, x0, y0 + alto - hueco, alto, hueco);
                break;
        }
    }

    /// <summary>
    /// Triángulo por escalones: cada columna es un píxel de ancho y se acorta de
    /// dos en dos, de manera que la punta cae en un píxel entero.
    /// </summary>
    private void Triangulo(DrawingContext dc, double pixel, int x0, int y0, int alto, bool derecha)
    {
        int pasos = (alto / 2) + 1;

        for (int i = 0; i < pasos; i++)
        {
            int largo = alto - (i * 2);
            int columna = derecha ? x0 + i : x0 + pasos - 1 - i;

            dc.DrawRectangle(Fill, null,
                new Rect(columna * pixel, (y0 + i) * pixel, pixel, largo * pixel));
        }
    }

    private void TrianguloArriba(DrawingContext dc, double pixel, int x0, int y0, int alto)
    {
        for (int i = 0; i < alto; i++)
        {
            int largo = ((i + 1) * 2) - 1;
            int inicio = x0 + (alto - 1 - i);

            dc.DrawRectangle(Fill, null,
                new Rect(inicio * pixel, (y0 + i) * pixel, largo * pixel, pixel));
        }
    }

    private void Caja(DrawingContext dc, double pixel, int x0, int y0, int ancho, int alto) =>
        dc.DrawRectangle(Fill, null, new Rect(x0 * pixel, y0 * pixel, ancho * pixel, alto * pixel));

    /// <summary>
    /// Círculo por renglones. Se dibuja así y no con una elipse porque a doce
    /// píxeles el suavizado de una elipse deja el borde lavado y el punto de grabar
    /// tiene que verse macizo.
    /// </summary>
    private void Disco(DrawingContext dc, double pixel, int x0, int y0, int lado)
    {
        double radio = lado / 2d;
        double centro = radio - 0.5;

        for (int y = 0; y < lado; y++)
        {
            double dy = y - centro;
            double mitad = Math.Sqrt(Math.Max(0, (radio * radio) - (dy * dy)));
            int desde = (int)Math.Round(centro - mitad + 0.5);
            int largo = (int)Math.Round(mitad * 2);
            if (largo <= 0) { continue; }

            dc.DrawRectangle(Fill, null,
                new Rect((x0 + desde) * pixel, (y0 + y) * pixel, largo * pixel, pixel));
        }
    }
}
