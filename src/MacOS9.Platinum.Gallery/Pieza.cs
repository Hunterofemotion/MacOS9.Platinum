using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Carga una pieza del catálogo de mapas de bits por su clave y su tamaño.
/// </summary>
/// <remarks>
/// Las piezas van como archivos sueltos junto al ejecutable y no incrustadas en el
/// ensamblado, así que no se alcanzan con un URI de paquete y hay que abrirlas por
/// ruta. Se guardan en memoria y se suelta el archivo, si no volver a correr el
/// recortador con un programa abierto falla porque el PNG está tomado.
/// </remarks>
public static class Pieza
{
    private static readonly string Carpeta =
        Path.Combine(AppContext.BaseDirectory, "Recursos", "IconosPixel");

    // La misma pieza sale en varias ventanas y el catálogo la pide de golpe: sin
    // memoria se abriría el archivo una vez por uso.
    private static readonly Dictionary<string, BitmapImage?> memoria = [];

    public static BitmapImage? Cargar(string clave, int lado)
    {
        string llave = $"{clave}-{lado}";
        if (memoria.TryGetValue(llave, out BitmapImage? guardada)) { return guardada; }

        string ruta = Path.Combine(Carpeta, $"{llave}.png");
        BitmapImage? mapa = null;

        if (File.Exists(ruta))
        {
            mapa = new BitmapImage();
            mapa.BeginInit();
            mapa.CacheOption = BitmapCacheOption.OnLoad;
            mapa.UriSource = new Uri(ruta);
            mapa.EndInit();
            mapa.Freeze();
        }

        memoria[llave] = mapa;
        return mapa;
    }
}

/// <summary>
/// Permite nombrar una pieza del catálogo desde el marcado:
/// <c>Icon="{local:PiezaIcono lab-r2c3}"</c>.
/// </summary>
/// <remarks>
/// Va como extensión de marcado y no como recurso porque son 509 piezas: un
/// diccionario con todas cargaría medio millar de mapas de bits para usar seis.
/// </remarks>
[MarkupExtensionReturnType(typeof(ImageSource))]
public sealed class PiezaIconoExtension : MarkupExtension
{
    public PiezaIconoExtension()
    {
    }

    public PiezaIconoExtension(string clave) => Clave = clave;

    /// <summary>Clave del catálogo, como aparece en <c>catalogo.tsv</c>.</summary>
    public string Clave { get; set; } = string.Empty;

    /// <summary>Lado en píxeles: 16, 32 o 128.</summary>
    public int Lado { get; set; } = 32;

    public override object? ProvideValue(IServiceProvider serviceProvider) =>
        Pieza.Cargar(Clave, Lado);
}
