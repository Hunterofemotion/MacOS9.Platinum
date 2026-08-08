using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Muestrario: cada familia de controles en su propio panel, con todas las
/// variaciones que la biblioteca sabe dibujar.
/// </summary>
/// <remarks>
/// Es distinto de los cuatro programas de muestra y no los reemplaza. Aquellos
/// enseñan cómo se ve un control dentro de una ventana con forma de aplicación
/// real; este enseña qué variantes existen, que es lo que se necesita al elegir
/// una. Un catálogo no dice si algo funciona en su sitio y una aplicación no
/// enseña las variantes que no usa.
///
/// Los paneles se arman en código y no en marcado porque casi todo es la misma
/// pieza repetida con una propiedad distinta: en XAML serían cien renglones que
/// difieren en una palabra.
/// </remarks>
public partial class Muestrario : PlatinumWindow
{
    private static readonly string CarpetaIconos =
        Path.Combine(AppContext.BaseDirectory, "Recursos", "IconosPixel");

    private readonly (string Nombre, string Icono, string Nota, Func<UIElement> Panel)[] familias;

    public Muestrario()
    {
        InitializeComponent();

        familias =
        [
            ("Medidores", "estado-r2c2", "Tres lecturas del mismo dato, en los dos sentidos.", PanelMedidores),
            ("Teclas", "base-r4c2", "Normal, por omisión, apagada, con icono y de barra.", PanelTeclas),
            ("Campos", "base-r1c4", "Editable, solo lectura, apagado, de varias líneas.", PanelCampos),
            ("Elección", "arch-r3c5", "Casillas, opciones excluyentes y menús.", PanelEleccion),
            ("Listas", "arch-r4c1", "Con columnas, sin columnas y en árbol.", PanelListas),
            ("Estado", "estado-r2c3", "Avance, testigos y avisos.", PanelEstado),
            ("Tiempo", "ofi-r1c2", "Flechitas, campo de fecha y hora, calendario.", PanelTiempo),
            ("Cajas", "base-r1c1", "Recuadros, plegables, pestañas y separadores.", PanelCajas),
            ("Transporte", "repro-r2c4", "Visor de tablero, signos de reproducción y barra de posición.", PanelTransporte),
        ];

        foreach (var (nombre, icono, _, _) in familias)
        {
            Riel.Items.Add(new PlatinumNavItem
            {
                Text = nombre,
                Icon = Pieza(icono, 32),
            });
        }

        Riel.SelectedIndex = 0;
    }

    private static BitmapImage? Pieza(string nombre, int lado)
    {
        string ruta = Path.Combine(CarpetaIconos, $"{nombre}-{lado}.png");
        if (!File.Exists(ruta)) { return null; }

        var mapa = new BitmapImage();
        mapa.BeginInit();
        mapa.CacheOption = BitmapCacheOption.OnLoad;
        mapa.UriSource = new Uri(ruta);
        mapa.EndInit();
        return mapa;
    }

    private void AlElegirFamilia(object sender, SelectionChangedEventArgs e)
    {
        int i = Riel.SelectedIndex;
        if (i < 0 || i >= familias.Length) { return; }

        var (nombre, _, nota, panel) = familias[i];
        Recuadro.Header = nombre;
        LineaFamilia.Text = nombre;
        LineaNota.Text = nota;

        Panel.Children.Clear();
        Panel.Children.Add(panel());
    }

    // ---- Ayudantes de acomodo ----------------------------------------------

    /// <summary>Título de un bloque dentro del panel, con su línea grabada.</summary>
    private static UIElement Bloque(string titulo, UIElement contenido)
    {
        var caja = new StackPanel { Margin = new Thickness(0, 0, 0, 18) };

        caja.Children.Add(new TextBlock
        {
            Text = titulo,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 2),
        });
        caja.Children.Add(new Separator { Margin = new Thickness(0, 0, 0, 8) });
        caja.Children.Add(contenido);

        return caja;
    }

    private static StackPanel Fila(params UIElement[] piezas)
    {
        var fila = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 8),
        };

        foreach (UIElement pieza in piezas)
        {
            if (pieza is FrameworkElement fe && fe.Margin.Right == 0)
            {
                fe.Margin = new Thickness(fe.Margin.Left, fe.Margin.Top, 14, fe.Margin.Bottom);
            }
            fila.Children.Add(pieza);
        }

        return fila;
    }

    private static StackPanel Columna(params UIElement[] piezas)
    {
        var columna = new StackPanel();
        foreach (UIElement pieza in piezas) { columna.Children.Add(pieza); }
        return columna;
    }

    /// <summary>Pie de una pieza: qué variante es, para no tener que adivinarlo.</summary>
    private static UIElement Rotulo(string texto, UIElement pieza)
    {
        // Pegado a la izquierda: una pieza con ancho fijo dentro de un contenedor
        // estirado queda centrada por omisión, y en una columna de ejemplos eso
        // deja cada renglón arrancando en un sitio distinto.
        var caja = new StackPanel
        {
            Margin = new Thickness(0, 0, 14, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        caja.Children.Add(pieza);
        caja.Children.Add(new TextBlock
        {
            Text = texto,
            Margin = new Thickness(0, 5, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        return caja;
    }

    // El ancho tope es lo que la hace cortar de renglón: sin él, una fila de
    // ejemplos más ancha que el panel estira todo y la nota se sale en una sola
    // línea en lugar de partirse.
    private static TextBlock Nota(string texto) => new()
    {
        Text = texto,
        TextWrapping = TextWrapping.Wrap,
        MaxWidth = 620,
        HorizontalAlignment = HorizontalAlignment.Left,
        Margin = new Thickness(0, 0, 0, 12),
    };
}
