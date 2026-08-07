using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MacOS9.Platinum.Controls;

/// <summary>Qué muestra el campo.</summary>
public enum DateTimeFieldMode
{
    Date,
    Time
}

/// <summary>
/// Campo de fecha u hora al estilo de Mac OS 9: el valor se parte en tramos —día,
/// mes y año, u hora, minuto y meridiano— y se edita de tramo en tramo. Se elige
/// uno con el clic o con las flechas laterales, y se cambia con las flechas de
/// arriba y abajo, con las flechitas de al lado o tecleando dígitos.
///
/// No hay campo libre a propósito: así no existe la fecha inválida a medio
/// escribir, que es el problema que este control resuelve y una caja de texto no.
/// </summary>
[TemplatePart(Name = PartSegments, Type = typeof(Panel))]
[TemplatePart(Name = PartStepper, Type = typeof(PlatinumStepper))]
public class PlatinumDateTimeField : Control
{
    public const string PartSegments = "PART_Segments";
    public const string PartStepper = "PART_Stepper";

    static PlatinumDateTimeField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumDateTimeField), new FrameworkPropertyMetadata(typeof(PlatinumDateTimeField)));
        FocusableProperty.OverrideMetadata(
            typeof(PlatinumDateTimeField), new FrameworkPropertyMetadata(true));
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(DateTime), typeof(PlatinumDateTimeField),
            new FrameworkPropertyMetadata(DateTime.Now,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AlCambiarValor));

    public DateTime Value
    {
        get => (DateTime)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(DateTimeFieldMode), typeof(PlatinumDateTimeField),
            new FrameworkPropertyMetadata(DateTimeFieldMode.Date, AlCambiarValor));

    public DateTimeFieldMode Mode
    {
        get => (DateTimeFieldMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    private static void AlCambiarValor(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PlatinumDateTimeField)d).Repintar();
    }

    private Panel? tramos;
    private int elegido;
    // Dígitos que se llevan tecleados en el tramo actual. Se vacía al cambiar de
    // tramo: escribir "5" y luego "3" en el mes debe dar 05 y después 53 rechazado,
    // no 5 y 3 sueltos.
    private string tecleado = "";

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        tramos = GetTemplateChild(PartSegments) as Panel;

        if (GetTemplateChild(PartStepper) is PlatinumStepper flechas)
        {
            flechas.Stepped += (_, paso) => Mover(paso);
        }

        Repintar();
    }

    // ---- Tramos -----------------------------------------------------------

    // Cada tramo sabe leerse del valor y devolver el valor con ese tramo cambiado.
    private sealed record Tramo(
        string Nombre,
        Func<DateTime, string> Leer,
        Func<DateTime, int, DateTime> Paso,
        Func<DateTime, int, DateTime?> Poner,
        int Digitos);

    private IReadOnlyList<Tramo> Definicion() => Mode == DateTimeFieldMode.Time
        ?
        [
            new Tramo("hora", v => (v.Hour % 12 == 0 ? 12 : v.Hour % 12).ToString("00"),
                (v, p) => v.AddHours(p),
                (v, n) => n is >= 1 and <= 12
                    ? v.Date.AddHours((n % 12) + (v.Hour >= 12 ? 12 : 0)).AddMinutes(v.Minute)
                    : null,
                2),
            new Tramo("minuto", v => v.Minute.ToString("00"),
                (v, p) => v.AddMinutes(p),
                (v, n) => n is >= 0 and <= 59 ? v.Date.AddHours(v.Hour).AddMinutes(n) : null,
                2),
            new Tramo("meridiano", v => v.Hour >= 12 ? "PM" : "AM",
                (v, _) => v.AddHours(v.Hour >= 12 ? -12 : 12),
                (_, _) => null,
                0)
        ]
        :
        [
            new Tramo("día", v => v.Day.ToString("00"),
                (v, p) => v.AddDays(p),
                (v, n) => n >= 1 && n <= DateTime.DaysInMonth(v.Year, v.Month)
                    ? new DateTime(v.Year, v.Month, n, v.Hour, v.Minute, 0)
                    : null,
                2),
            new Tramo("mes", v => v.Month.ToString("00"),
                (v, p) => v.AddMonths(p),
                (v, n) => n is >= 1 and <= 12
                    ? new DateTime(v.Year, n, Math.Min(v.Day, DateTime.DaysInMonth(v.Year, n)), v.Hour, v.Minute, 0)
                    : null,
                2),
            new Tramo("año", v => v.Year.ToString("0000"),
                (v, p) => v.AddYears(p),
                (v, n) => n is >= 1 and <= 9999
                    ? new DateTime(n, v.Month, Math.Min(v.Day, DateTime.DaysInMonth(n, v.Month)), v.Hour, v.Minute, 0)
                    : null,
                4)
        ];

    private string Separador => Mode == DateTimeFieldMode.Time ? ":" : "/";

    // ---- Pintado ----------------------------------------------------------

    private void Repintar()
    {
        if (tramos is null) { return; }

        tramos.Children.Clear();
        IReadOnlyList<Tramo> lista = Definicion();

        for (int i = 0; i < lista.Count; i++)
        {
            if (i > 0 && !(Mode == DateTimeFieldMode.Time && i == lista.Count - 1))
            {
                tramos.Children.Add(new TextBlock
                {
                    Text = Separador,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            else if (i > 0)
            {
                // Antes del meridiano va un espacio, no un separador.
                tramos.Children.Add(new TextBlock
                {
                    Text = " ",
                    VerticalAlignment = VerticalAlignment.Center
                });
            }

            int indice = i;
            var texto = new TextBlock
            {
                Text = lista[i].Leer(Value),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(2, 0, 2, 0),
                Background = indice == elegido && IsKeyboardFocusWithin
                    ? TryFindResource("SelectionStrongBrush") as System.Windows.Media.Brush
                    : null,
                Foreground = indice == elegido && IsKeyboardFocusWithin
                    ? TryFindResource("TextInvertedBrush") as System.Windows.Media.Brush
                    : TryFindResource("TextBrush") as System.Windows.Media.Brush
            };

            texto.MouseLeftButtonDown += (_, e) =>
            {
                elegido = indice;
                tecleado = "";
                Focus();
                Repintar();
                e.Handled = true;
            };

            tramos.Children.Add(texto);
        }
    }

    // ---- Mando ------------------------------------------------------------

    private void Mover(int paso)
    {
        IReadOnlyList<Tramo> lista = Definicion();
        if (elegido < 0 || elegido >= lista.Count) { return; }

        tecleado = "";
        Value = lista[elegido].Paso(Value, paso);
        Focus();
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        Repintar();
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        tecleado = "";
        Repintar();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        IReadOnlyList<Tramo> lista = Definicion();

        switch (e.Key)
        {
            case Key.Up:
                Mover(1);
                e.Handled = true;
                return;

            case Key.Down:
                Mover(-1);
                e.Handled = true;
                return;

            case Key.Left:
                elegido = Math.Max(0, elegido - 1);
                tecleado = "";
                Repintar();
                e.Handled = true;
                return;

            case Key.Right:
                elegido = Math.Min(lista.Count - 1, elegido + 1);
                tecleado = "";
                Repintar();
                e.Handled = true;
                return;
        }

        int digito = Digito(e.Key);
        if (digito >= 0 && elegido >= 0 && elegido < lista.Count && lista[elegido].Digitos > 0)
        {
            Teclear(lista[elegido], digito);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void Teclear(Tramo tramo, int digito)
    {
        string intento = tecleado + digito;
        if (intento.Length > tramo.Digitos) { intento = digito.ToString(); }

        DateTime? nuevo = tramo.Poner(Value, int.Parse(intento, CultureInfo.InvariantCulture));
        if (nuevo is null)
        {
            // El número no cabe en el tramo: se empieza de nuevo con este dígito.
            intento = digito.ToString();
            nuevo = tramo.Poner(Value, digito);
        }

        tecleado = intento;
        if (nuevo is not null) { Value = nuevo.Value; }

        // Con el tramo lleno se pasa al siguiente, como en el original.
        if (tecleado.Length >= tramo.Digitos)
        {
            tecleado = "";
            elegido = Math.Min(Definicion().Count - 1, elegido + 1);
            Repintar();
        }
    }

    private static int Digito(Key tecla)
    {
        if (tecla is >= Key.D0 and <= Key.D9) { return tecla - Key.D0; }
        if (tecla is >= Key.NumPad0 and <= Key.NumPad9) { return tecla - Key.NumPad0; }
        return -1;
    }
}
