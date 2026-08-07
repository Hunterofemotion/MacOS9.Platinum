using System;
using System.Globalization;
using System.Text;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Coincidencia tolerante entre lo que se escribe y lo que un icono representa.
/// </summary>
/// <remarks>
/// Se compara sin acentos y sin distinguir mayúsculas: quien busca un icono
/// escribe rápido y no va a poner la tilde de "teléfono".
///
/// La puntuación va en escalones y no en una sola medida porque las formas de
/// acertar no valen lo mismo. Que el término empiece con lo escrito es una señal
/// mucho más fuerte que que las letras aparezcan salteadas, y ordenar por una
/// distancia sola mezcla las dos.
/// </remarks>
public static class BusquedaDifusa
{
    /// <summary>Quita acentos y pasa a minúsculas, para comparar manzanas con manzanas.</summary>
    public static string Normalizar(string texto)
    {
        string descompuesto = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var limpio = new StringBuilder(descompuesto.Length);

        foreach (char c in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                limpio.Append(c);
            }
        }

        return limpio.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Qué tanto se parece <paramref name="termino"/> a lo escrito. Cero es que no
    /// se parece nada. Los dos ya vienen normalizados.
    /// </summary>
    public static int Puntuar(string escrito, string termino)
    {
        if (escrito.Length == 0) { return 1; }
        if (termino.Length == 0) { return 0; }

        if (termino == escrito) { return 1000; }

        if (termino.StartsWith(escrito, StringComparison.Ordinal))
        {
            // Entre dos que empiezan igual gana el más corto: "carpeta" antes que
            // "carpeta compartida" cuando se escribió "carp".
            return 800 - Math.Min(199, termino.Length - escrito.Length);
        }

        int donde = termino.IndexOf(escrito, StringComparison.Ordinal);
        if (donde >= 0) { return 600 - Math.Min(199, donde); }

        int salteado = Subsecuencia(escrito, termino);
        if (salteado > 0) { return salteado; }

        // Último recurso: errores de dedo. La tolerancia crece con la palabra,
        // porque una letra mal en tres es otra palabra y en diez es un resbalón.
        int margen = Math.Max(1, termino.Length / 4);
        int distancia = Distancia(escrito, termino, margen);
        return distancia <= margen ? 200 - (distancia * 30) : 0;
    }

    /// <summary>
    /// Las letras de lo escrito aparecen en orden dentro del término, aunque no
    /// pegadas. Puntúa mejor cuanto menos separadas estén.
    /// </summary>
    private static int Subsecuencia(string escrito, string termino)
    {
        int i = 0;
        int primero = -1;
        int ultimo = -1;

        for (int j = 0; j < termino.Length && i < escrito.Length; j++)
        {
            if (termino[j] != escrito[i]) { continue; }
            if (primero < 0) { primero = j; }
            ultimo = j;
            i++;
        }

        if (i < escrito.Length) { return 0; }

        int tramo = ultimo - primero + 1;
        int huecos = tramo - escrito.Length;
        return Math.Max(1, 400 - (huecos * 12) - primero);
    }

    /// <summary>
    /// Distancia de edición con corte: en cuanto pasa del margen se abandona, que
    /// es lo que la hace barata sobre un catálogo entero.
    /// </summary>
    private static int Distancia(string a, string b, int margen)
    {
        if (Math.Abs(a.Length - b.Length) > margen) { return margen + 1; }

        int[] anterior = new int[b.Length + 1];
        int[] actual = new int[b.Length + 1];

        for (int j = 0; j <= b.Length; j++) { anterior[j] = j; }

        for (int i = 1; i <= a.Length; i++)
        {
            actual[0] = i;
            int mejorDelRenglon = actual[0];

            for (int j = 1; j <= b.Length; j++)
            {
                int costo = a[i - 1] == b[j - 1] ? 0 : 1;
                actual[j] = Math.Min(
                    Math.Min(actual[j - 1] + 1, anterior[j] + 1),
                    anterior[j - 1] + costo);

                if (actual[j] < mejorDelRenglon) { mejorDelRenglon = actual[j]; }
            }

            if (mejorDelRenglon > margen) { return margen + 1; }

            (anterior, actual) = (actual, anterior);
        }

        return anterior[b.Length];
    }
}
