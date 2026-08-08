using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Editor de video inventado. Interesa por la pista de tiempo: es un dibujo a mano
/// sobre un lienzo, no un control del tema, y sirve para ver cómo convive lo
/// dibujado con el chrome.
/// </summary>
public partial class VideoWindow : PlatinumWindow
{
    public VideoWindow()
    {
        InitializeComponent();

        Clips.ItemsSource = new[]
        {
            new { Clip = "sunset_take12", Length = "00:42:11" },
            new { Clip = "interview_a", Length = "01:18:04" },
            new { Clip = "b_roll_pier", Length = "00:27:19" },
            new { Clip = "titles_v3", Length = "00:08:00" },
            new { Clip = "credits", Length = "00:36:02" },
        };

        // Las franjas se declaran aquí y no en el marcado porque son las mismas dos
        // veces: el búfer de las dos platinas se juzga con el mismo criterio.
        foreach (PlatinumLevelGauge medidor in new[] { BuferA, BuferB })
        {
            medidor.Bands.Add(new LevelBand { To = 25, Fill = (Brush)FindResource("LedRedBrush") });
            medidor.Bands.Add(new LevelBand { To = 50, Fill = (Brush)FindResource("LedAmberBrush") });
            medidor.Bands.Add(new LevelBand { To = 100, Fill = (Brush)FindResource("LedGreenBrush") });
        }

        Pista.SizeChanged += (_, _) => Dibujar();

        // Los búferes se mueven. Un medidor quieto no enseña lo que hace: lo que
        // hay que ver es el nivel subiendo y bajando, y el relleno cambiando de
        // color al cruzar un umbral.
        //
        // No se anima la propiedad con una animación de WPF: aquí el valor cambia
        // de verdad, que es como llega en una aplicación real. El control no tiene
        // que animar nada, solo redibujarse cuando le cambian el valor.
        var reloj = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        reloj.Tick += (_, _) =>
        {
            BuferA.Value = Deriva(BuferA.Value, 74);
            BuferB.Value = Deriva(BuferB.Value, 34);
        };
        reloj.Start();
    }

    private readonly Random azar = new();

    /// <summary>
    /// Paseo aleatorio que tiende a volver a su centro. Sin ese regreso el nivel
    /// se va a un extremo y se queda ahí, que es justo lo que no se quiere ver.
    /// </summary>
    private double Deriva(double actual, double centro)
    {
        double paso = (azar.NextDouble() - 0.5) * 7;
        double regreso = (centro - actual) * 0.06;
        return Math.Clamp(actual + paso + regreso, 4, 99);
    }

    private void Dibujar()
    {
        Pista.Children.Clear();

        double ancho = Pista.ActualWidth;
        double alto = Pista.ActualHeight;
        if (ancho <= 0 || alto <= 0) { return; }

        // Dos carriles: video arriba, audio abajo, con la línea de tiempo marcada
        // cada doceavo, que es lo que da la lectura de regla.
        double carril = (alto - 12) / 2;

        (double inicio, double largo, Color tinte, string nombre)[] bloques =
        [
            (0.00, 0.22, Color.FromRgb(0x8E, 0x9C, 0xC8), "sunset"),
            (0.22, 0.31, Color.FromRgb(0xA8, 0xB6, 0x8E), "interview"),
            (0.53, 0.18, Color.FromRgb(0xC8, 0xA8, 0x8E), "pier"),
            (0.71, 0.29, Color.FromRgb(0x9C, 0x8E, 0xC8), "credits"),
        ];

        foreach (var (inicio, largo, tinte, nombre) in bloques)
        {
            var bloque = new Border
            {
                Width = largo * ancho,
                Height = carril,
                Background = new SolidColorBrush(tinte),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new TextBlock
                {
                    Text = nombre,
                    Margin = new Thickness(4, 2, 0, 0),
                    FontSize = 10,
                },
            };

            Canvas.SetLeft(bloque, inicio * ancho);
            Canvas.SetTop(bloque, 0);
            Pista.Children.Add(bloque);
        }

        for (int i = 0; i < 40; i++)
        {
            // La onda de audio es determinista a propósito: una captura de patrón
            // no puede depender de números al azar.
            double h = (carril - 6) * (0.25 + (((i * 37) % 17) / 24d));
            var barra = new Rectangle
            {
                Width = (ancho / 40) - 2,
                Height = h,
                Fill = new SolidColorBrush(Color.FromRgb(0x5A, 0x6E, 0x9C)),
            };

            Canvas.SetLeft(barra, (i * ancho / 40) + 1);
            Canvas.SetTop(barra, alto - h);
            Pista.Children.Add(barra);
        }

        for (int i = 0; i <= 12; i++)
        {
            var marca = new Line
            {
                X1 = i * ancho / 12,
                Y1 = carril,
                X2 = i * ancho / 12,
                Y2 = carril + 8,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
            Pista.Children.Add(marca);
        }

        var cabezal = new Line
        {
            X1 = ancho * 0.38, Y1 = 0,
            X2 = ancho * 0.38, Y2 = alto,
            Stroke = new SolidColorBrush(Color.FromRgb(0xC4, 0x2B, 0x21)),
            StrokeThickness = 2,
        };
        Pista.Children.Add(cabezal);
    }
}
