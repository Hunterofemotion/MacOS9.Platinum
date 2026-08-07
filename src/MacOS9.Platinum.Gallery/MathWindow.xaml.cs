using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Consola de cálculo inventada, para mostrar el tema en un programa de otra clase
/// que el espectrómetro.
/// </summary>
public partial class MathWindow : PlatinumWindow
{
    public MathWindow()
    {
        InitializeComponent();

        Expresiones.ItemsSource = new[]
        {
            new { Name = "f(x)", Body = "a·sin(n·x) + b·x" },
            new { Name = "f'(x)", Body = "a·n·cos(n·x) + b" },
            new { Name = "g(x)", Body = "exp(-x²/2)" },
            new { Name = "h(x)", Body = "ln(1 + x²)" },
            new { Name = "p(x)", Body = "x³ - 2x + 1" },
        };

        Tabla.ItemsSource = Filas();

        // La curva se dibuja cuando el lienzo ya tiene medida: antes del acomodo su
        // tamaño es cero y saldría una línea plana.
        Lienzo.SizeChanged += (_, _) => Trazar();
    }

    private static object[] Filas()
    {
        var filas = new object[9];
        for (int i = 0; i < filas.Length; i++)
        {
            double x = -2 + (i * 0.5);
            double y = (1.25 * Math.Sin(3 * x)) + (0.4 * x);
            double d = (1.25 * 3 * Math.Cos(3 * x)) + 0.4;

            filas[i] = new
            {
                X = x.ToString("0.000", CultureInfo.InvariantCulture),
                Y = y.ToString("0.000000", CultureInfo.InvariantCulture),
                D = d.ToString("0.000000", CultureInfo.InvariantCulture),
                Note = Math.Abs(y) < 0.05 ? "near root" : string.Empty,
            };
        }

        return filas;
    }

    private void Trazar()
    {
        Lienzo.Children.Clear();

        double ancho = Lienzo.ActualWidth;
        double alto = Lienzo.ActualHeight;
        if (ancho <= 0 || alto <= 0) { return; }

        var reticula = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));

        for (double x = 0; x <= ancho; x += ancho / 12)
        {
            Lienzo.Children.Add(new Line
            {
                X1 = x, Y1 = 0, X2 = x, Y2 = alto,
                Stroke = reticula, StrokeThickness = 1,
            });
        }
        for (double y = 0; y <= alto; y += alto / 8)
        {
            Lienzo.Children.Add(new Line
            {
                X1 = 0, Y1 = y, X2 = ancho, Y2 = y,
                Stroke = reticula, StrokeThickness = 1,
            });
        }

        Lienzo.Children.Add(new Line
        {
            X1 = 0, Y1 = alto / 2, X2 = ancho, Y2 = alto / 2,
            Stroke = Brushes.Black, StrokeThickness = 1,
        });
        Lienzo.Children.Add(new Line
        {
            X1 = ancho / 2, Y1 = 0, X2 = ancho / 2, Y2 = alto,
            Stroke = Brushes.Black, StrokeThickness = 1,
        });

        Lienzo.Children.Add(Curva(ancho, alto, x => (1.25 * Math.Sin(3 * x)) + (0.4 * x),
            new SolidColorBrush(Color.FromRgb(0x3A, 0x5F, 0xA8)), 2));
        Lienzo.Children.Add(Curva(ancho, alto, x => Math.Exp(-x * x / 2) * 2,
            new SolidColorBrush(Color.FromRgb(0xC4, 0x2B, 0x21)), 1));
    }

    private static Polyline Curva(double ancho, double alto, Func<double, double> f, Brush tinta, double grosor)
    {
        var linea = new Polyline { Stroke = tinta, StrokeThickness = grosor };

        const double Rango = 6.28;
        const double Escala = 4.5;

        for (int i = 0; i <= 240; i++)
        {
            double x = -Rango + (i * 2 * Rango / 240);
            double y = f(x);

            linea.Points.Add(new Point(
                (x + Rango) / (2 * Rango) * ancho,
                (alto / 2) - (y / Escala * alto / 2)));
        }

        return linea;
    }
}
