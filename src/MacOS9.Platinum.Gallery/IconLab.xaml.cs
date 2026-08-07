using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Catálogo de iconos con búsqueda por lo que la pieza representa.
/// </summary>
/// <remarks>
/// El nombre técnico no sirve para encontrar nada: quien busca el icono de una
/// impresora escribe "imprimir", no "IconPrinter". Por eso cada pieza lleva una
/// lista de palabras en el catálogo, y la búsqueda pesa el nombre y las palabras
/// por igual.
///
/// Catálogo y piezas se leen de archivos sueltos junto al programa: así se corre
/// el recortador, se edita el catálogo y se vuelve a abrir sin recompilar.
/// </remarks>
public partial class IconLab : PlatinumWindow
{
    private static readonly string Carpeta =
        Path.Combine(AppContext.BaseDirectory, "Recursos", "IconosPixel");

    private readonly List<Pieza> catalogo = [];

    public IconLab()
    {
        InitializeComponent();
        Cargar();
        Filtrar();
    }

    /// <summary>Una entrada del catálogo, ya con sus términos normalizados.</summary>
    public sealed class Pieza
    {
        public required string Archivo { get; init; }
        public required string Nombre { get; init; }
        public required string Palabras { get; init; }

        // El nombre y las palabras se guardan aparte porque no valen lo mismo al
        // buscar: "impresora" es el nombre de una pieza y una palabra suelta de
        // otra, y sin la distinción el desempate lo terminaba haciendo el alfabeto.
        public required string[] TerminosNombre { get; init; }
        public required string[] TerminosPalabras { get; init; }

        public double Lado { get; set; } = 32;
        public ImageSource? Estampa { get; set; }
    }

    private static BitmapImage? Mapa(string archivo, int lado)
    {
        string ruta = Path.Combine(Carpeta, $"{archivo}-{lado}.png");
        if (!File.Exists(ruta)) { return null; }

        var mapa = new BitmapImage();
        mapa.BeginInit();
        // Se carga a memoria y se suelta el archivo: si no, volver a correr el
        // recortador con el catálogo abierto falla porque el PNG está tomado.
        mapa.CacheOption = BitmapCacheOption.OnLoad;
        mapa.UriSource = new Uri(ruta);
        mapa.EndInit();
        return mapa;
    }

    private void Cargar()
    {
        string tabla = Path.Combine(Carpeta, "catalogo.tsv");
        if (!File.Exists(tabla))
        {
            LineaCuenta.Text = "Falta catalogo.tsv.";
            Testigo.Fill = (Brush)FindResource("LedRedBrush");
            return;
        }

        foreach (string renglon in File.ReadAllLines(tabla))
        {
            if (string.IsNullOrWhiteSpace(renglon)) { continue; }

            string[] campos = renglon.Split('\t');
            if (campos.Length < 3) { continue; }
            if (campos[0] == "archivo") { continue; }

            string archivo = campos[0].Trim();
            // Una entrada del catálogo sin su recorte no se muestra: es más
            // honesto que una ficha vacía que parece un icono roto.
            if (!File.Exists(Path.Combine(Carpeta, $"{archivo}-32.png"))) { continue; }

            string nombre = campos[1].Trim();
            string palabras = campos[2].Trim();

            catalogo.Add(new Pieza
            {
                Archivo = archivo,
                Nombre = nombre,
                Palabras = palabras,
                TerminosNombre = Partir(nombre),
                TerminosPalabras = Partir(palabras + " " + archivo),
            });
        }
    }

    // El nombre viene pegado en mayúsculas intercaladas (FolderOpen), así que
    // además de los separadores se corta donde arranca cada mayúscula: quien
    // escribe "open" tiene que encontrar FolderOpen.
    private static string[] Partir(string texto)
    {
        var sueltas = new List<string>();

        foreach (string bruto in texto.Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries))
        {
            sueltas.Add(BusquedaDifusa.Normalizar(bruto));

            int inicio = 0;
            for (int i = 1; i <= bruto.Length; i++)
            {
                if (i < bruto.Length && !char.IsUpper(bruto[i])) { continue; }
                if (i - inicio > 1) { sueltas.Add(BusquedaDifusa.Normalizar(bruto[inicio..i])); }
                inicio = i;
            }
        }

        return sueltas.Distinct().ToArray();
    }

    private int LadoElegido() => MenuTamano.SelectedIndex switch
    {
        0 => 16,
        2 => 48,
        _ => 32,
    };

    private void Filtrar()
    {
        int lado = LadoElegido();
        // El de 48 no tiene archivo propio: se sube del recorte grande, que es el
        // único tamaño del que se puede escalar sin que se deshaga.
        int archivo = lado == 48 ? 128 : lado;

        string escrito = BusquedaDifusa.Normalizar(Consulta.Text.Trim());
        string[] partes = escrito.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var encontradas = new List<(Pieza P, int Puntos)>();

        foreach (Pieza pieza in catalogo)
        {
            int total = 0;
            bool todas = true;

            foreach (string parte in partes)
            {
                int mejor = 0;

                foreach (string termino in pieza.TerminosNombre)
                {
                    int puntos = BusquedaDifusa.Puntuar(parte, termino);
                    if (puntos > mejor) { mejor = puntos; }
                }

                // Acertarle a una palabra descriptiva cuenta menos que acertarle al
                // nombre, y menos todavía cuanto más atrás esté escrita: las
                // palabras del catálogo van de lo más propio a lo más lejano. Sin
                // esto, buscar "impresora" traía primero el fax, que la menciona de
                // pasada, y desempataba el alfabeto.
                for (int i = 0; i < pieza.TerminosPalabras.Length; i++)
                {
                    int peso = 60 - Math.Min(36, i * 6);
                    int puntos = BusquedaDifusa.Puntuar(parte, pieza.TerminosPalabras[i]) * peso / 100;
                    if (puntos > mejor) { mejor = puntos; }
                }

                // Cada palabra escrita tiene que acertarle a algo. Con sumar
                // sueltas, escribir "carpeta roja" traería todas las carpetas.
                if (mejor == 0) { todas = false; break; }
                total += mejor;
            }

            if (!todas) { continue; }
            encontradas.Add((pieza, total));
        }

        var lista = encontradas
            .OrderByDescending(e => e.Puntos)
            .ThenBy(e => e.P.Nombre, StringComparer.Ordinal)
            .Select(e =>
            {
                e.P.Lado = lado;
                e.P.Estampa = Mapa(e.P.Archivo, archivo);
                return e.P;
            })
            .ToList();

        Cuadricula.ItemsSource = lista;

        LineaCuenta.Text = partes.Length == 0
            ? $"{catalogo.Count} piezas en el catálogo."
            : $"{lista.Count} de {catalogo.Count}.";

        Testigo.Fill = (Brush)FindResource(
            lista.Count == 0 && partes.Length > 0 ? "LedAmberBrush" : "LedGreenBrush");

        if (lista.Count > 0) { Cuadricula.SelectedIndex = 0; }
        else { LimpiarDetalle(); }
    }

    private void LimpiarDetalle()
    {
        DetalleGrande.Source = null;
        DetalleMedio.Source = null;
        DetalleChico.Source = null;
        DetalleLlave.Text = string.Empty;
        DetallePalabras.Text = string.Empty;
        LineaDetalle.Text = string.Empty;
    }

    private void AlEscribir(object sender, TextChangedEventArgs e) => Filtrar();

    private void AlCambiarTamano(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized) { return; }
        Filtrar();
    }

    private void AlLimpiar(object sender, RoutedEventArgs e)
    {
        Consulta.Text = string.Empty;
        Consulta.Focus();
    }

    private void AlElegir(object sender, SelectionChangedEventArgs e)
    {
        if (Cuadricula.SelectedItem is not Pieza pieza)
        {
            LimpiarDetalle();
            return;
        }

        DetalleGrande.Source = Mapa(pieza.Archivo, 128);
        DetalleMedio.Source = Mapa(pieza.Archivo, 32);
        DetalleChico.Source = Mapa(pieza.Archivo, 16);
        DetalleLlave.Text = $"Icon{pieza.Nombre}   ·   {pieza.Archivo}";
        DetallePalabras.Text = pieza.Palabras;
        LineaDetalle.Text = $"{{StaticResource Icon{pieza.Nombre}32}}";
    }
}
