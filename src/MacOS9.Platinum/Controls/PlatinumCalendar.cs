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
public class PlatinumCalendar : Control
{
    public const string PartGrid = "PART_Grid";
    public const string PartTitle = "PART_Title";
    public const string PartPrev = "PART_Prev";
    public const string PartNext = "PART_Next";

    private const int Semanas = 6;

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

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rejilla = GetTemplateChild(PartGrid) as Grid;
        titulo = GetTemplateChild(PartTitle) as TextBlock;

        if (GetTemplateChild(PartPrev) is ButtonBase atras)
        {
            atras.Click += (_, _) => DisplayMonth = DisplayMonth.AddMonths(-1);
        }
        if (GetTemplateChild(PartNext) is ButtonBase adelante)
        {
            adelante.Click += (_, _) => DisplayMonth = DisplayMonth.AddMonths(1);
        }

        Repintar();
    }

    private void Repintar()
    {
        if (rejilla is null) { return; }

        CultureInfo cultura = CultureInfo.CurrentCulture;
        DateTime mes = new(DisplayMonth.Year, DisplayMonth.Month, 1);

        if (titulo is not null)
        {
            titulo.Text = cultura.TextInfo.ToTitleCase(
                mes.ToString("MMMM yyyy", cultura));
        }

        rejilla.Children.Clear();
        rejilla.ColumnDefinitions.Clear();
        rejilla.RowDefinitions.Clear();

        for (int c = 0; c < 7; c++)
        {
            rejilla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        for (int f = 0; f <= Semanas; f++)
        {
            rejilla.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Encabezado con la inicial de cada día, empezando por el primero de la
        // semana según la configuración regional.
        DayOfWeek primero = cultura.DateTimeFormat.FirstDayOfWeek;
        string[] nombres = cultura.DateTimeFormat.ShortestDayNames;
        for (int c = 0; c < 7; c++)
        {
            var dia = (DayOfWeek)(((int)primero + c) % 7);
            var texto = new TextBlock
            {
                Text = cultura.TextInfo.ToTitleCase(nombres[(int)dia]),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3),
                Foreground = TryFindResource("TextDisabledBrush") as Brush
            };
            Grid.SetRow(texto, 0);
            Grid.SetColumn(texto, c);
            rejilla.Children.Add(texto);
        }

        // El calendario siempre muestra seis semanas para no cambiar de alto al
        // pasar de mes: un menú que crece y encoge se lee como si saltara.
        int desfase = ((int)mes.DayOfWeek - (int)primero + 7) % 7;
        DateTime cursor = mes.AddDays(-desfase);

        for (int f = 0; f < Semanas; f++)
        {
            for (int c = 0; c < 7; c++)
            {
                rejilla.Children.Add(Celda(cursor, mes.Month));
                Grid.SetRow(rejilla.Children[^1], f + 1);
                Grid.SetColumn(rejilla.Children[^1], c);
                cursor = cursor.AddDays(1);
            }
        }
    }

    private UIElement Celda(DateTime dia, int mesVigente)
    {
        bool esDelMes = dia.Month == mesVigente;
        bool esElegido = dia.Date == SelectedDate.Date;

        var celda = new Border
        {
            Padding = new Thickness(4, 2, 4, 2),
            Background = esElegido
                ? TryFindResource("SelectionStrongBrush") as Brush
                : Brushes.Transparent,
            Child = new TextBlock
            {
                Text = dia.Day.ToString(CultureInfo.CurrentCulture),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = esElegido
                    ? TryFindResource("TextInvertedBrush") as Brush
                    : (esDelMes
                        ? TryFindResource("TextBrush") as Brush
                        : TryFindResource("TextDisabledBrush") as Brush)
            }
        };

        // Los días de los meses vecinos se muestran apagados pero se pueden elegir:
        // esconderlos obligaría a cambiar de mes para tomar un día de la orilla.
        celda.MouseLeftButtonUp += (_, e) =>
        {
            SelectedDate = new DateTime(dia.Year, dia.Month, dia.Day,
                SelectedDate.Hour, SelectedDate.Minute, 0);
            DateChosen?.Invoke(this, SelectedDate);
            e.Handled = true;
        };

        celda.MouseEnter += (_, _) =>
        {
            if (!esElegido) { celda.Background = TryFindResource("SelectionBrush") as Brush; }
        };
        celda.MouseLeave += (_, _) =>
        {
            if (!esElegido) { celda.Background = Brushes.Transparent; }
        };

        celda.Cursor = Cursors.Arrow;
        return celda;
    }
}
