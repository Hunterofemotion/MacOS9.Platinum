using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>Cómo se usan las franjas de color.</summary>
public enum LevelBandMode
{
    /// <summary>
    /// El relleno sube hasta el valor y toma el color de la franja en la que cae.
    /// De un vistazo se ve cuánto hay y si está bien.
    /// </summary>
    Fill,

    /// <summary>
    /// Las franjas pintan el carril completo como escala fija y un indicador marca
    /// dónde está el valor. Se ve dónde están los umbrales sin leer la leyenda, a
    /// costa de que el nivel se lea del indicador y no del área llena.
    /// </summary>
    Zones,

    /// <summary>
    /// Gráfica de bala: las franjas al fondo, atenuadas, y encima una barra de
    /// medida más angosta que llega hasta el valor. Da las dos cosas a la vez —el
    /// nivel y los umbrales— que es lo que <see cref="Fill"/> y <see cref="Zones"/>
    /// dan por separado.
    /// </summary>
    /// <remarks>
    /// El diseño es de Stephen Few. Admite además una marca de objetivo, que es lo
    /// que lo vuelve útil para comparar contra una meta y no solo contra umbrales.
    /// </remarks>
    Bullet
}

/// <summary>
/// Medidor de nivel: un carril que se llena, con escala graduada y franjas de
/// color declaradas por quien lo usa.
/// </summary>
/// <remarks>
/// Se dibuja entero en <see cref="OnRender"/> y no con una plantilla, como el
/// resto de las piezas del tema que dependen del píxel físico. Las marcas de la
/// escala tienen que caer enteras —a 200 % una plantilla las deja unas de un
/// píxel y otras de dos— y el triángulo indicador es el mismo problema que
/// resuelve <see cref="ArrowGlyph"/>. Una plantilla solo expondría cosas que no
/// deben variar.
///
/// El control no sabe si más es mejor. Un búfer lleno es bueno y una ocupación
/// alta es mala, así que el color vive en las franjas que declara la aplicación
/// y no aquí.
/// </remarks>
public class PlatinumLevelGauge : FrameworkElement
{
    private readonly List<LevelBand> franjas = [];

    public PlatinumLevelGauge()
    {
        Bands = [];
    }

    // ---- Valor -------------------------------------------------------------

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    // ---- Franjas -----------------------------------------------------------

    public static readonly DependencyProperty BandsProperty =
        DependencyProperty.Register(nameof(Bands), typeof(FreezableCollection<LevelBand>),
            typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Las franjas de color, de la más baja a la más alta.</summary>
    public FreezableCollection<LevelBand> Bands
    {
        get => (FreezableCollection<LevelBand>)GetValue(BandsProperty);
        set => SetValue(BandsProperty, value);
    }

    public static readonly DependencyProperty BandModeProperty =
        DependencyProperty.Register(nameof(BandMode), typeof(LevelBandMode), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(LevelBandMode.Fill, FrameworkPropertyMetadataOptions.AffectsRender));

    public LevelBandMode BandMode
    {
        get => (LevelBandMode)GetValue(BandModeProperty);
        set => SetValue(BandModeProperty, value);
    }

    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Meta contra la que se compara, dibujada como una marca cruzada en el modo
    /// <see cref="LevelBandMode.Bullet"/>. Sin asignar no se dibuja.
    /// </summary>
    /// <remarks>
    /// Es distinto de un umbral: el umbral parte el rango en franjas y la meta es
    /// un punto al que se quiere llegar. Un mismo medidor puede tener los dos.
    /// </remarks>
    public double Target
    {
        get => (double)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public static readonly DependencyProperty MeasureBrushProperty =
        DependencyProperty.Register(nameof(MeasureBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Color de la barra de medida de la gráfica de bala.</summary>
    public Brush? MeasureBrush
    {
        get => (Brush?)GetValue(MeasureBrushProperty);
        set => SetValue(MeasureBrushProperty, value);
    }

    // ---- Escala e indicador ------------------------------------------------

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(Orientation.Vertical,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty ShowScaleProperty =
        DependencyProperty.Register(nameof(ShowScale), typeof(bool), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public bool ShowScale
    {
        get => (bool)GetValue(ShowScaleProperty);
        set => SetValue(ShowScaleProperty, value);
    }

    public static readonly DependencyProperty ScaleStepProperty =
        DependencyProperty.Register(nameof(ScaleStep), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(25d,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Cada cuánto va una marca con su número.</summary>
    public double ScaleStep
    {
        get => (double)GetValue(ScaleStepProperty);
        set => SetValue(ScaleStepProperty, value);
    }

    public static readonly DependencyProperty MinorTicksProperty =
        DependencyProperty.Register(nameof(MinorTicks), typeof(int), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Marcas sin número entre dos con número. Le dan lectura de regla.</summary>
    public int MinorTicks
    {
        get => (int)GetValue(MinorTicksProperty);
        set => SetValue(MinorTicksProperty, value);
    }

    public static readonly DependencyProperty ShowReadoutProperty =
        DependencyProperty.Register(nameof(ShowReadout), typeof(bool), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public bool ShowReadout
    {
        get => (bool)GetValue(ShowReadoutProperty);
        set => SetValue(ShowReadoutProperty, value);
    }

    public static readonly DependencyProperty UnitProperty =
        DependencyProperty.Register(nameof(Unit), typeof(string), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata("%",
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Lo que va después del número, en la cifra y en la escala.
    /// </summary>
    /// <remarks>
    /// Va como texto suelto y no dentro de un formato numérico porque en .NET el
    /// signo de porcentaje dentro del formato multiplica el valor por cien, y ese
    /// es un error que se descubre tarde.
    /// </remarks>
    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly DependencyProperty DecimalsProperty =
        DependencyProperty.Register(nameof(Decimals), typeof(int), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    public int Decimals
    {
        get => (int)GetValue(DecimalsProperty);
        set => SetValue(DecimalsProperty, value);
    }

    // ---- Medidas y tintas --------------------------------------------------

    public static readonly DependencyProperty TrackThicknessProperty =
        DependencyProperty.Register(nameof(TrackThickness), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(30d,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Ancho del carril en vertical, alto en horizontal.</summary>
    public double TrackThickness
    {
        get => (double)GetValue(TrackThicknessProperty);
        set => SetValue(TrackThicknessProperty, value);
    }

    public static readonly DependencyProperty ReadoutFontSizeProperty =
        DependencyProperty.Register(nameof(ReadoutFontSize), typeof(double), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Tamaño de la cifra. Sin asignar se calcula, y no es el mismo en los dos
    /// sentidos.
    /// </summary>
    /// <remarks>
    /// Un número fijo no sirve para los dos: de pie el carril mide ciento y pico de
    /// alto y admite una cifra grande, acostado mide dieciocho y la misma cifra no
    /// cabe —la caja crecía por encima y por debajo del carril hasta tocar el
    /// renglón de la escala—. Acostado el tamaño sale del grosor del carril.
    /// </remarks>
    public double ReadoutFontSize
    {
        get => (double)GetValue(ReadoutFontSizeProperty);
        set => SetValue(ReadoutFontSizeProperty, value);
    }

    private double TamanoCifra()
    {
        if (!double.IsNaN(ReadoutFontSize)) { return ReadoutFontSize; }

        return Orientation == Orientation.Vertical
            ? FontSize * 1.7
            : TrackThickness * 0.62;
    }

    public static readonly DependencyProperty ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner(typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public static readonly DependencyProperty BackgroundProperty =
        Panel.BackgroundProperty.AddOwner(typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Lo que se ve del carril sin llenar.</summary>
    public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public static readonly DependencyProperty BorderBrushProperty =
        Control.BorderBrushProperty.AddOwner(typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public static readonly DependencyProperty WellShadowBrushProperty =
        DependencyProperty.Register(nameof(WellShadowBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Sombra interior arriba y a la izquierda: es lo que hunde el carril.</summary>
    public Brush? WellShadowBrush
    {
        get => (Brush?)GetValue(WellShadowBrushProperty);
        set => SetValue(WellShadowBrushProperty, value);
    }

    public static readonly DependencyProperty WellShadowDeepBrushProperty =
        DependencyProperty.Register(nameof(WellShadowDeepBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// El tono exterior de la sombra interior. Son dos y no uno porque así lo hace
    /// el campo de texto del tema, y un medidor junto a un campo tiene que leerse
    /// hundido lo mismo que él.
    /// </summary>
    public Brush? WellShadowDeepBrush
    {
        get => (Brush?)GetValue(WellShadowDeepBrushProperty);
        set => SetValue(WellShadowDeepBrushProperty, value);
    }

    public static readonly DependencyProperty WellHighlightBrushProperty =
        DependencyProperty.Register(nameof(WellHighlightBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? WellHighlightBrush
    {
        get => (Brush?)GetValue(WellHighlightBrushProperty);
        set => SetValue(WellHighlightBrushProperty, value);
    }

    public static readonly DependencyProperty ReadoutShadowBrushProperty =
        DependencyProperty.Register(nameof(ReadoutShadowBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Sombra interior de la caja de la cifra, arriba y a la izquierda.
    /// </summary>
    /// <remarks>
    /// Va aparte de la del carril porque las dos piezas no piden lo mismo: la caja
    /// es un registro y se lee mejor rehundida como un campo, y el carril es una
    /// superficie de color a la que la sombra solo le quita área.
    ///
    /// De un solo tono y no de dos como el campo de texto: la caja mide lo que el
    /// carril de alto, y dos tonos se comerían la mitad.
    /// </remarks>
    public Brush? ReadoutShadowBrush
    {
        get => (Brush?)GetValue(ReadoutShadowBrushProperty);
        set => SetValue(ReadoutShadowBrushProperty, value);
    }

    public static readonly DependencyProperty ReadoutHighlightBrushProperty =
        DependencyProperty.Register(nameof(ReadoutHighlightBrush), typeof(Brush), typeof(PlatinumLevelGauge),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush? ReadoutHighlightBrush
    {
        get => (Brush?)GetValue(ReadoutHighlightBrushProperty);
        set => SetValue(ReadoutHighlightBrushProperty, value);
    }

    public static readonly DependencyProperty FontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner(typeof(PlatinumLevelGauge));

    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly DependencyProperty FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner(typeof(PlatinumLevelGauge));

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // ---- Medida ------------------------------------------------------------

    private const double LargoMarca = 6;
    private const double Aire = 6;
    private const double AnchoIndicador = 7;

    protected override Size MeasureOverride(Size disponible)
    {
        if (Orientation == Orientation.Vertical)
        {
            double etiquetas = ShowScale ? Etiquetas().Max(e => e.Width) + LargoMarca + Aire : 0;
            return new Size(etiquetas + TrackThickness + AnchoCifra(), 120);
        }

        // En horizontal la escala va debajo del carril y la cifra a la derecha, así
        // que cada una gasta un eje distinto. La primera versión usaba la misma
        // cuenta para las dos orientaciones y el número terminaba pintado fuera del
        // control.
        double alto = TrackThickness + 4;
        if (ShowScale) { alto += LargoMarca + Etiquetas().Max(e => e.Height); }

        return new Size(120 + AnchoCifra(), alto);
    }

    /// <summary>Lo que gasta la cifra a un lado del carril, indicador incluido.</summary>
    private double AnchoCifra()
    {
        if (!ShowReadout) { return 0; }

        double indicador = Orientation == Orientation.Vertical ? AnchoIndicador + Aire : 0;
        return Aire + indicador + Cifra(Maximum).Width + (Aire * 2);
    }

    // ---- Pintado -----------------------------------------------------------

    protected override void OnRender(DrawingContext dc)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) { return; }
        if (Maximum <= Minimum) { return; }

        // Las franjas se ordenan aquí y no al declararlas: quien las escribe en
        // XAML no tiene por qué cuidar el orden.
        franjas.Clear();
        if (Bands is not null) { franjas.AddRange(Bands.OrderBy(b => b.To)); }

        double escala = DeviceScale.Of(this).X;
        if (escala <= 0) { escala = 1; }
        double pixel = 1d / escala;
        double grosor = Math.Max(1d, Math.Round(escala)) * pixel;

        if (Orientation == Orientation.Vertical) { Vertical(dc, pixel, grosor); }
        else { Horizontal(dc, pixel, grosor); }
    }

    private void Vertical(DrawingContext dc, double pixel, double grosor)
    {
        double anchoEtiquetas = ShowScale ? Etiquetas().Max(e => e.Width) : 0;
        double izquierda = ShowScale ? anchoEtiquetas + LargoMarca + Aire : 0;

        var carril = new Rect(
            Cuadrar(izquierda, pixel),
            Cuadrar(2, pixel),
            Cuadrar(TrackThickness, pixel),
            Cuadrar(ActualHeight - 4, pixel));

        if (carril.Height <= 4 || carril.Width <= 4) { return; }

        if (ShowScale) { EscalaVertical(dc, carril, anchoEtiquetas, grosor); }

        Pozo(dc, carril, grosor);

        Rect dentro = Interior(carril, grosor);

        double fraccion = Fraccion(Value);

        if (BandMode == LevelBandMode.Bullet)
        {
            BalaVertical(dc, dentro, fraccion, grosor, pixel);
        }
        else if (BandMode == LevelBandMode.Zones)
        {
            ZonasVertical(dc, dentro, pixel);
            MarcaVertical(dc, dentro, fraccion, grosor, pixel);
        }
        else
        {
            double alto = Cuadrar(dentro.Height * fraccion, pixel);
            if (alto > 0)
            {
                dc.DrawRectangle(
                    ColorDe(Value) ?? Foreground, null,
                    new Rect(dentro.X, dentro.Bottom - alto, dentro.Width, alto));
            }
        }

        if (ShowReadout) { CifraVertical(dc, carril, dentro, fraccion, grosor, pixel); }
    }

    private void EscalaVertical(DrawingContext dc, Rect carril, double anchoEtiquetas, double grosor)
    {
        int pasos = (int)Math.Round((Maximum - Minimum) / ScaleStep);
        if (pasos <= 0) { return; }

        var tinta = Foreground;

        for (int i = 0; i <= pasos; i++)
        {
            double valor = Minimum + (i * ScaleStep);
            double y = carril.Bottom - (carril.Height * Fraccion(valor));

            FormattedText texto = Texto(Numero(valor) + Unit, FontSize);
            dc.DrawText(texto, new Point(anchoEtiquetas - texto.Width, y - (texto.Height / 2)));

            dc.DrawRectangle(tinta, null,
                new Rect(anchoEtiquetas + Aire, y - (grosor / 2), LargoMarca, grosor));

            // Marcas menores hacia el paso siguiente, más cortas.
            if (i == pasos || MinorTicks <= 0) { continue; }
            for (int m = 1; m <= MinorTicks; m++)
            {
                double sub = valor + (ScaleStep * m / (MinorTicks + 1));
                double ys = carril.Bottom - (carril.Height * Fraccion(sub));
                dc.DrawRectangle(tinta, null,
                    new Rect(anchoEtiquetas + Aire + (LargoMarca / 2), ys - (grosor / 2),
                        LargoMarca / 2, grosor));
            }
        }
    }

    private void ZonasVertical(DrawingContext dc, Rect dentro, double pixel)
    {
        double desde = Minimum;

        foreach (LevelBand franja in franjas)
        {
            double hasta = Math.Min(franja.To, Maximum);
            if (hasta <= desde) { continue; }

            double y1 = dentro.Bottom - Cuadrar(dentro.Height * Fraccion(hasta), pixel);
            double y2 = dentro.Bottom - Cuadrar(dentro.Height * Fraccion(desde), pixel);

            dc.DrawRectangle(franja.Fill ?? Background, null,
                new Rect(dentro.X, y1, dentro.Width, y2 - y1));

            desde = hasta;
        }
    }

    /// <summary>
    /// Cuánto del grosor del carril ocupa la barra de medida. Angosta a propósito:
    /// si midiera lo mismo que el carril taparía las franjas y la gráfica volvería
    /// a ser un relleno normal.
    /// </summary>
    private const double RazonMedida = 0.42;

    /// <summary>Qué tanto se apagan las franjas para que la medida se despegue.</summary>
    private const double AtenuacionFranjas = 0.42;

    private void BalaVertical(DrawingContext dc, Rect dentro, double fraccion, double grosor, double pixel)
    {
        // Las franjas se atenúan aquí y no en la paleta que declara la aplicación:
        // son las mismas de los otros modos, y pedirle un segundo juego de colores
        // solo para este sería trabajo suyo por una decisión del control.
        dc.PushOpacity(AtenuacionFranjas);
        ZonasVertical(dc, dentro, pixel);
        dc.Pop();

        double ancho = Cuadrar(dentro.Width * RazonMedida, pixel);
        double x = Cuadrar(dentro.X + ((dentro.Width - ancho) / 2), pixel);
        double alto = Cuadrar(dentro.Height * fraccion, pixel);

        if (alto > 0)
        {
            dc.DrawRectangle(MeasureBrush ?? Foreground, null,
                new Rect(x, dentro.Bottom - alto, ancho, alto));
        }

        Objetivo(dc, dentro, grosor, pixel, vertical: true);
    }

    /// <summary>
    /// La marca de la meta: cruza el carril entero para que se lea por encima de la
    /// barra de medida, la alcance o no.
    /// </summary>
    private void Objetivo(DrawingContext dc, Rect dentro, double grosor, double pixel, bool vertical)
    {
        if (double.IsNaN(Target)) { return; }

        double f = Fraccion(Target);

        if (vertical)
        {
            double y = Cuadrar(dentro.Bottom - (dentro.Height * f), pixel);
            dc.DrawRectangle(BorderBrush, null,
                new Rect(dentro.X, y - grosor, dentro.Width, grosor * 2));
        }
        else
        {
            double x = Cuadrar(dentro.X + (dentro.Width * f), pixel);
            dc.DrawRectangle(BorderBrush, null,
                new Rect(x - grosor, dentro.Y, grosor * 2, dentro.Height));
        }
    }

    private void MarcaVertical(DrawingContext dc, Rect dentro, double fraccion, double grosor, double pixel)
    {
        double y = Cuadrar(dentro.Bottom - (dentro.Height * fraccion), pixel);

        // Línea de lado a lado y no un punto: sobre una zona de color, un punto se
        // pierde y una línea negra siempre se ve.
        //
        // Sin realce debajo. Lo llevaba, en blanco fijo, para despegar la marca del
        // color; sobre una franja clara no se veía y sobre una oscura se leía como
        // una raya suelta. Además era el único blanco del control que no salía de
        // la paleta.
        dc.DrawRectangle(BorderBrush, null,
            new Rect(dentro.X, y - grosor, dentro.Width, grosor * 2));
    }

    private void CifraVertical(DrawingContext dc, Rect carril, Rect dentro, double fraccion,
        double grosor, double pixel)
    {
        double y = Cuadrar(dentro.Bottom - (dentro.Height * fraccion), pixel);
        double x = carril.Right + Aire;

        // Triángulo indicador, apuntando al carril
        var punta = new StreamGeometry();
        using (StreamGeometryContext ctx = punta.Open())
        {
            ctx.BeginFigure(new Point(x, y), isFilled: true, isClosed: true);
            ctx.LineTo(new Point(x + AnchoIndicador, y - (AnchoIndicador * 0.8)), true, false);
            ctx.LineTo(new Point(x + AnchoIndicador, y + (AnchoIndicador * 0.8)), true, false);
        }
        punta.Freeze();
        dc.DrawGeometry(Foreground, null, punta);

        FormattedText cifra = Cifra(Value);
        double cajaX = Cuadrar(x + AnchoIndicador + Aire, pixel);
        double cajaAncho = Cuadrar(cifra.Width + (Aire * 2), pixel);
        double cajaAlto = Cuadrar(cifra.Height + Aire, pixel);
        double cajaY = Cuadrar(Math.Max(carril.Top,
            Math.Min(carril.Bottom - cajaAlto, y - (cajaAlto / 2))), pixel);

        var caja = new Rect(cajaX, cajaY, cajaAncho, cajaAlto);
        CajaCifra(dc, caja, grosor);
        dc.DrawText(cifra, new Point(cajaX + Aire, cajaY + (Aire / 2)));
    }

    // ---- Piezas compartidas ------------------------------------------------

    /// <summary>
    /// El mismo pozo del campo de texto: marco negro, dos tonos de sombra arriba y
    /// a la izquierda, y realce blanco en los lados opuestos. Los dos tonos son lo
    /// que da la profundidad; con uno solo el carril se lee plano con un borde
    /// grueso.
    /// </summary>
    private void Pozo(DrawingContext dc, Rect carril, double grosor)
    {
        dc.DrawRectangle(Background, null, carril);
        Marco(dc, carril, grosor);

        // Realce primero, para que la sombra lo pise en la esquina donde se cruzan.
        if (WellHighlightBrush is not null)
        {
            dc.DrawRectangle(WellHighlightBrush, null,
                new Rect(carril.X + grosor, carril.Bottom - (grosor * 2),
                    carril.Width - (grosor * 2), grosor));
            dc.DrawRectangle(WellHighlightBrush, null,
                new Rect(carril.Right - (grosor * 2), carril.Y + grosor,
                    grosor, carril.Height - (grosor * 2)));
        }

        if (WellShadowBrush is not null)
        {
            dc.DrawRectangle(WellShadowBrush, null,
                new Rect(carril.X + grosor, carril.Y + grosor,
                    carril.Width - (grosor * 2), grosor * 2));
            dc.DrawRectangle(WellShadowBrush, null,
                new Rect(carril.X + grosor, carril.Y + grosor,
                    grosor * 2, carril.Height - (grosor * 2)));
        }

        if (WellShadowDeepBrush is not null)
        {
            dc.DrawRectangle(WellShadowDeepBrush, null,
                new Rect(carril.X + grosor, carril.Y + grosor,
                    carril.Width - (grosor * 2), grosor));
            dc.DrawRectangle(WellShadowDeepBrush, null,
                new Rect(carril.X + grosor, carril.Y + grosor,
                    grosor, carril.Height - (grosor * 2)));
        }
    }

    /// <summary>
    /// El área que queda para pintar el nivel. El hueco sale de lo que de verdad se
    /// dibuja: si no hay sombra interior, el relleno arranca pegado al marco en vez
    /// de dejar una franja del color de fondo donde estaba la sombra.
    /// </summary>
    private Rect Interior(Rect carril, double grosor)
    {
        double arriba = grosor + (WellShadowBrush is null && WellShadowDeepBrush is null ? 0 : grosor * 2);
        double abajo = grosor + (WellHighlightBrush is null ? 0 : grosor);

        return new Rect(
            carril.X + arriba,
            carril.Y + arriba,
            carril.Width - arriba - abajo,
            carril.Height - arriba - abajo);
    }

    /// <summary>
    /// La caja de la cifra: fondo, marco y su propio rehundido de un tono.
    /// </summary>
    private void CajaCifra(DrawingContext dc, Rect caja, double grosor)
    {
        dc.DrawRectangle(Background, null, caja);
        Marco(dc, caja, grosor);

        if (ReadoutHighlightBrush is not null)
        {
            dc.DrawRectangle(ReadoutHighlightBrush, null,
                new Rect(caja.X + grosor, caja.Bottom - (grosor * 2),
                    caja.Width - (grosor * 2), grosor));
            dc.DrawRectangle(ReadoutHighlightBrush, null,
                new Rect(caja.Right - (grosor * 2), caja.Y + grosor,
                    grosor, caja.Height - (grosor * 2)));
        }

        if (ReadoutShadowBrush is not null)
        {
            dc.DrawRectangle(ReadoutShadowBrush, null,
                new Rect(caja.X + grosor, caja.Y + grosor, caja.Width - (grosor * 2), grosor));
            dc.DrawRectangle(ReadoutShadowBrush, null,
                new Rect(caja.X + grosor, caja.Y + grosor, grosor, caja.Height - (grosor * 2)));
        }
    }

    private void Marco(DrawingContext dc, Rect caja, double grosor)
    {
        dc.DrawRectangle(BorderBrush, null, new Rect(caja.X, caja.Y, caja.Width, grosor));
        dc.DrawRectangle(BorderBrush, null, new Rect(caja.X, caja.Bottom - grosor, caja.Width, grosor));
        dc.DrawRectangle(BorderBrush, null, new Rect(caja.X, caja.Y, grosor, caja.Height));
        dc.DrawRectangle(BorderBrush, null, new Rect(caja.Right - grosor, caja.Y, grosor, caja.Height));
    }

    private void Horizontal(DrawingContext dc, double pixel, double grosor)
    {
        // El ancho de la cifra se descuenta del carril: si no, el carril se lleva
        // todo el control y el número sale dibujado fuera de sus propios límites.
        var carril = new Rect(
            Cuadrar(2, pixel),
            Cuadrar(2, pixel),
            Cuadrar(ActualWidth - 4 - AnchoCifra(), pixel),
            Cuadrar(TrackThickness, pixel));

        if (carril.Width <= 4 || carril.Height <= 4) { return; }

        if (ShowScale) { EscalaHorizontal(dc, carril, grosor); }

        Pozo(dc, carril, grosor);

        Rect dentro = Interior(carril, grosor);

        double fraccion = Fraccion(Value);

        if (BandMode is LevelBandMode.Zones or LevelBandMode.Bullet)
        {
            bool bala = BandMode == LevelBandMode.Bullet;
            if (bala) { dc.PushOpacity(AtenuacionFranjas); }

            double desde = Minimum;
            foreach (LevelBand franja in franjas)
            {
                double hasta = Math.Min(franja.To, Maximum);
                if (hasta <= desde) { continue; }
                double x1 = dentro.X + Cuadrar(dentro.Width * Fraccion(desde), pixel);
                double x2 = dentro.X + Cuadrar(dentro.Width * Fraccion(hasta), pixel);
                dc.DrawRectangle(franja.Fill ?? Background, null,
                    new Rect(x1, dentro.Y, x2 - x1, dentro.Height));
                desde = hasta;
            }

            if (bala)
            {
                dc.Pop();

                double alto = Cuadrar(dentro.Height * RazonMedida, pixel);
                double y = Cuadrar(dentro.Y + ((dentro.Height - alto) / 2), pixel);
                double largo = Cuadrar(dentro.Width * fraccion, pixel);

                if (largo > 0)
                {
                    dc.DrawRectangle(MeasureBrush ?? Foreground, null,
                        new Rect(dentro.X, y, largo, alto));
                }

                Objetivo(dc, dentro, grosor, pixel, vertical: false);
            }
            else
            {
                double marca = dentro.X + Cuadrar(dentro.Width * fraccion, pixel);
                dc.DrawRectangle(BorderBrush, null,
                    new Rect(marca - grosor, dentro.Y, grosor * 2, dentro.Height));
            }
        }
        else
        {
            double ancho = Cuadrar(dentro.Width * fraccion, pixel);
            if (ancho > 0)
            {
                dc.DrawRectangle(ColorDe(Value) ?? Foreground, null,
                    new Rect(dentro.X, dentro.Y, ancho, dentro.Height));
            }
        }

        if (!ShowReadout) { return; }

        // Sin triángulo indicador, al revés que en vertical: allá la caja flota a
        // la altura del valor y hace falta señalar a cuál. Aquí la caja está fija
        // al final del carril y no apunta a nada.
        //
        // La caja tiene exactamente el alto del carril. Dejándola crecer con su
        // texto sobresalía por arriba y por abajo, y por abajo llegaba a tocar el
        // renglón de la escala: dos rectángulos vecinos de distinto alto y sin
        // separación entre ellos.
        FormattedText cifra = Cifra(Value);
        var caja = new Rect(
            Cuadrar(carril.Right + Aire, pixel),
            carril.Y,
            Cuadrar(cifra.Width + (Aire * 2), pixel),
            carril.Height);

        CajaCifra(dc, caja, grosor);
        dc.DrawText(cifra, new Point(
            caja.X + Aire,
            Cuadrar(caja.Y + ((caja.Height - cifra.Height) / 2), pixel)));
    }

    private void EscalaHorizontal(DrawingContext dc, Rect carril, double grosor)
    {
        int pasos = (int)Math.Round((Maximum - Minimum) / ScaleStep);
        if (pasos <= 0) { return; }

        double arriba = carril.Bottom + LargoMarca;

        for (int i = 0; i <= pasos; i++)
        {
            double valor = Minimum + (i * ScaleStep);
            double x = carril.X + (carril.Width * Fraccion(valor));

            dc.DrawRectangle(Foreground, null,
                new Rect(x - (grosor / 2), carril.Bottom, grosor, LargoMarca));

            // La primera y la última se recargan contra su extremo para que no se
            // salgan del carril.
            FormattedText texto = Texto(Numero(valor) + Unit, FontSize);
            double tx = i == 0 ? x
                : i == pasos ? x - texto.Width
                : x - (texto.Width / 2);
            dc.DrawText(texto, new Point(tx, arriba));

            if (i == pasos || MinorTicks <= 0) { continue; }
            for (int m = 1; m <= MinorTicks; m++)
            {
                double sub = valor + (ScaleStep * m / (MinorTicks + 1));
                double xs = carril.X + (carril.Width * Fraccion(sub));
                dc.DrawRectangle(Foreground, null,
                    new Rect(xs - (grosor / 2), carril.Bottom, grosor, LargoMarca / 2));
            }
        }
    }

    private double Fraccion(double valor) =>
        Math.Clamp((valor - Minimum) / (Maximum - Minimum), 0d, 1d);

    /// <summary>La franja en la que cae el valor. Sin franjas declaradas, ninguna.</summary>
    private Brush? ColorDe(double valor)
    {
        foreach (LevelBand franja in franjas)
        {
            if (valor <= franja.To) { return franja.Fill; }
        }

        return franjas.Count > 0 ? franjas[^1].Fill : null;
    }

    private string Numero(double valor) =>
        valor.ToString("F" + Decimals.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentCulture);

    private IEnumerable<FormattedText> Etiquetas()
    {
        int pasos = Math.Max(1, (int)Math.Round((Maximum - Minimum) / Math.Max(0.0001, ScaleStep)));
        for (int i = 0; i <= pasos; i++)
        {
            yield return Texto(Numero(Minimum + (i * ScaleStep)) + Unit, FontSize);
        }
    }

    private FormattedText Cifra(double valor) => Texto(Numero(valor) + Unit, TamanoCifra(), negrita: true);

    private FormattedText Texto(string valor, double tamano, bool negrita = false) =>
        new(valor,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily, FontStyles.Normal,
                negrita ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal),
            tamano,
            Foreground,
            null,
            TextFormattingMode.Display,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

    private static double Cuadrar(double valor, double pixel) =>
        Math.Round(valor / pixel) * pixel;
}
