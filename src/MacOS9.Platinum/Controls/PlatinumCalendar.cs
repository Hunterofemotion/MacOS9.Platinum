using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Rejilla de un mes. Va como control aparte del campo de fecha porque también
/// sirve suelto, dentro de un panel, no solo colgando de un menú.
///
/// La rejilla se arma en código: son cuarenta y dos celdas que cambian con cada
/// mes, y declararlas en una plantilla obligaría a nombrarlas una por una.
/// </summary>
[TemplatePart(Name = PartGrid, Type = typeof(Grid))]
[TemplatePart(Name = PartTitle, Type = typeof(TextBlock))]
[TemplatePart(Name = PartPrev, Type = typeof(ButtonBase))]
[TemplatePart(Name = PartNext, Type = typeof(ButtonBase))]
[TemplatePart(Name = PartMonth, Type = typeof(ComboBox))]
[TemplatePart(Name = PartYear, Type = typeof(TextBox))]
[TemplatePart(Name = PartYearStepper, Type = typeof(PlatinumStepper))]
public class PlatinumCalendar : Control
{
    public const string PartGrid = "PART_Grid";
    public const string PartTitle = "PART_Title";
    public const string PartPrev = "PART_Prev";
    public const string PartNext = "PART_Next";
    public const string PartMonth = "PART_Month";
    public const string PartYear = "PART_Year";
    public const string PartYearStepper = "PART_YearStepper";

    static PlatinumCalendar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumCalendar), new FrameworkPropertyMetadata(typeof(PlatinumCalendar)));
    }

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime), typeof(PlatinumCalendar),
            new FrameworkPropertyMetadata(DateTime.Today,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AlCambiar));

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Mes que se está mostrando, que no siempre es el de la fecha elegida.</summary>
    public static readonly DependencyProperty DisplayMonthProperty =
        DependencyProperty.Register(nameof(DisplayMonth), typeof(DateTime), typeof(PlatinumCalendar),
            new FrameworkPropertyMetadata(DateTime.Today, AlCambiar));

    public DateTime DisplayMonth
    {
        get => (DateTime)GetValue(DisplayMonthProperty);
        set => SetValue(DisplayMonthProperty, value);
    }

    /// <summary>Se eligió un día.</summary>
    public event EventHandler<DateTime>? DateChosen;

    private static void AlCambiar(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var calendario = (PlatinumCalendar)d;
        if (e.Property == SelectedDateProperty)
        {
            // Al fijar una fecha de otro mes, el calendario se mueve a ese mes: si
            // no, se elige algo que no está a la vista.
            var nueva = (DateTime)e.NewValue;
            if (nueva.Year != calendario.DisplayMonth.Year || nueva.Month != calendario.DisplayMonth.Month)
            {
                calendario.DisplayMonth = new DateTime(nueva.Year, nueva.Month, 1);
                return;
            }
        }
        calendario.Repintar();
    }

    private Grid? rejilla;
    private TextBlock? titulo;
    private ComboBox? meses;
    private TextBox? anio;

    // Al repintar se reescriben el menú de meses y el campo del año, y eso vuelve a
    // disparar sus eventos. Sin esta marca el calendario se repinta a sí mismo en
    // cadena cada vez que cambia de mes.
    private bool actualizando;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rejilla = GetTemplateChild(PartGrid) as Grid;
        titulo = GetTemplateChild(PartTitle) as TextBlock;
        meses = GetTemplateChild(PartMonth) as ComboBox;
        anio = GetTemplateChild(PartYear) as TextBox;

        // Las flechas de mes anterior y siguiente siguen atendiéndose por si alguien
        // conserva la plantilla vieja: la de la casa ya no las trae.
        if (GetTemplateChild(PartPrev) is ButtonBase atras)
        {
            atras.Click += (_, _) => DisplayMonth = DisplayMonth.AddMonths(-1);
        }
        if (GetTemplateChild(PartNext) is ButtonBase adelante)
        {
            adelante.Click += (_, _) => DisplayMonth = DisplayMonth.AddMonths(1);
        }

        if (meses is not null)
        {
            meses.ItemsSource = NombresDeMes();
            meses.SelectionChanged += (_, _) =>
            {
                if (actualizando || meses.SelectedIndex < 0) { return; }
                DisplayMonth = new DateTime(DisplayMonth.Year, meses.SelectedIndex + 1, 1);
            };
        }

        if (GetTemplateChild(PartYearStepper) is PlatinumStepper flechas)
        {
            flechas.Stepped += (_, direccion) => MoverAnio(direccion);
        }

        if (anio is not null)
        {
            // Se toma el año al soltar el campo y al dar Enter, no en cada tecla:
            // escribiendo "2024" el primer dígito daría el año 2 y saltaría el mes.
            anio.LostFocus += (_, _) => TomarAnio();
            anio.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter) { TomarAnio(); e.Handled = true; }
            };
        }

        Repintar();
    }

    private static string[] NombresDeMes()
    {
        CultureInfo cultura = CultureInfo.CurrentCulture;
        var nombres = new string[12];
        for (int i = 0; i < 12; i++)
        {
            nombres[i] = cultura.TextInfo.ToTitleCase(cultura.DateTimeFormat.MonthNames[i]);
        }
        return nombres;
    }

    private void MoverAnio(int direccion)
    {
        int destino = DisplayMonth.Year + Math.Sign(direccion);
        if (destino is < 1 or > 9999) { return; }
        DisplayMonth = new DateTime(destino, DisplayMonth.Month, 1);
    }

    private void TomarAnio()
    {
        if (anio is null) { return; }

        // Un año imposible devuelve el campo al que estaba en lugar de quedarse en
        // rojo: aquí no hay dónde poner un aviso.
        if (int.TryParse(anio.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int valor)
            && valor is >= 1 and <= 9999)
        {
            DisplayMonth = new DateTime(valor, DisplayMonth.Month, 1);
        }
        else
        {
            anio.Text = DisplayMonth.Year.ToString(CultureInfo.CurrentCulture);
        }
    }

    private void Repintar()
    {
        if (rejilla is null) { return; }

        CultureInfo cultura = CultureInfo.CurrentCulture;
        DateTime mes = new(DisplayMonth.Year, DisplayMonth.Month, 1);

        actualizando = true;
        try
        {
            if (titulo is not null)
            {
                titulo.Text = cultura.TextInfo.ToTitleCase(mes.ToString("MMMM yyyy", cultura));
            }
            if (meses is not null) { meses.SelectedIndex = mes.Month - 1; }
            if (anio is not null) { anio.Text = mes.Year.ToString(cultura); }
        }
        finally
        {
            actualizando = false;
        }

        rejilla.Children.Clear();
        rejilla.ColumnDefinitions.Clear();
        rejilla.RowDefinitions.Clear();

        for (int c = 0; c < 7; c++)
        {
            rejilla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        // Se dibujan las semanas que el mes ocupa de verdad. Un mes que empieza en
        // sábado necesita seis renglones y uno que empieza en domingo cinco; dejar
        // siempre seis colgaba una franja vacía al pie.
        int desfaseInicial = ((int)mes.DayOfWeek - (int)cultura.DateTimeFormat.FirstDayOfWeek + 7) % 7;
        int semanas = (int)Math.Ceiling((desfaseInicial + DateTime.DaysInMonth(mes.Year, mes.Month)) / 7d);

        for (int f = 0; f <= semanas; f++)
        {
            rejilla.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Encabezado con el nombre corto de cada día, empezando por el primero de la
        // semana según la configuración regional.
        DayOfWeek primero = cultura.DateTimeFormat.FirstDayOfWeek;
        string[] nombres = cultura.DateTimeFormat.AbbreviatedDayNames;
        for (int c = 0; c < 7; c++)
        {
            var dia = (DayOfWeek)(((int)primero + c) % 7);
            var casilla = new Border
            {
                Background = TryFindResource("WindowFaceBrush") as Brush,
                BorderBrush = TryFindResource("CalendarRuleBrush") as Brush,
                BorderThickness = new Thickness(0, 0, c == 6 ? 0 : 1, 1),
                Padding = new Thickness(5, 2, 5, 2),
                Child = new TextBlock
                {
                    Text = cultura.TextInfo.ToTitleCase(nombres[(int)dia]),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = TryFindResource("TextDisabledBrush") as Brush
                }
            };
            Grid.SetRow(casilla, 0);
            Grid.SetColumn(casilla, c);
            rejilla.Children.Add(casilla);
        }

        DateTime cursor = mes.AddDays(-desfaseInicial);

        for (int f = 0; f < semanas; f++)
        {
            for (int c = 0; c < 7; c++)
            {
                var celda = Celda(cursor, mes.Month, ultimaColumna: c == 6, ultimaFila: f == semanas - 1);
                Grid.SetRow(celda, f + 1);
                Grid.SetColumn(celda, c);
                rejilla.Children.Add(celda);
                cursor = cursor.AddDays(1);
            }
        }
    }

    private UIElement Celda(DateTime dia, int mesVigente, bool ultimaColumna, bool ultimaFila)
    {
        bool esDelMes = dia.Month == mesVigente;
        bool esElegido = esDelMes && dia.Date == SelectedDate.Date;

        var fondo = TryFindResource("CalendarCellBrush") as Brush;
        var celda = new Border
        {
            Padding = new Thickness(5, 3, 5, 3),
            BorderBrush = TryFindResource("CalendarRuleBrush") as Brush,
            BorderThickness = new Thickness(0, 0, ultimaColumna ? 0 : 1, ultimaFila ? 0 : 1),
            Background = esElegido ? TryFindResource("SelectionStrongBrush") as Brush : fondo,
            Child = new TextBlock
            {
                // Los días de los meses vecinos van en blanco, no apagados: el mes
                // que se está viendo tiene que leerse de un vistazo, y un número
                // gris igual invita a contarlo.
                Text = esDelMes ? dia.Day.ToString(CultureInfo.CurrentCulture) : string.Empty,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = esElegido
                    ? TryFindResource("TextInvertedBrush") as Brush
                    : TryFindResource("TextBrush") as Brush
            }
        };

        if (!esDelMes) { return celda; }

        celda.MouseLeftButtonUp += (_, e) =>
        {
            SelectedDate = new DateTime(dia.Year, dia.Month, dia.Day,
                SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second);
            DateChosen?.Invoke(this, SelectedDate);
            e.Handled = true;
        };

        celda.MouseEnter += (_, _) =>
        {
            if (!esElegido) { celda.Background = TryFindResource("SelectionBrush") as Brush; }
        };
        celda.MouseLeave += (_, _) =>
        {
            if (!esElegido) { celda.Background = fondo; }
        };

        celda.Cursor = Cursors.Arrow;
        return celda;
    }
}
