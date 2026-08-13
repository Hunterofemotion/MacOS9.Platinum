using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Cursores clásicos de Mac OS 9, dibujados píxel por píxel.
/// <para>
/// El relojito de pulsera es la seña de espera del sistema original y no existe en
/// Windows. Se arma en memoria con el formato .cur en lugar de embarcar archivos:
/// así el dibujo vive en el código como el resto de los glifos del tema y puede
/// crecer a factor entero cuando el monitor escala, con el píxel gordo del original
/// en lugar del borroso del estirado.
/// </para>
/// <para>
/// Uso: <c>Mouse.OverrideCursor = PlatinumCursors.Wait;</c> mientras dura la
/// tarea, y de vuelta a <c>null</c> al terminar.
/// </para>
/// </summary>
public static class PlatinumCursors
{
    // El reloj en 16×16: '#' trazo negro, 'o' blanco, '.' transparente. Correa
    // arriba y abajo, caja redonda con la corona a la derecha y manecillas a las
    // nueve en punto, como lo dibujaba el sistema.
    private static readonly string[] WatchMap =
    [
        ".....######.....",
        ".....#oooo#.....",
        ".....######.....",
        "......####......",
        "....##o#oo##....",
        "....#oo#ooo#....",
        "...#ooo#oooo#...",
        "...#o###oooo##..",
        "...#oooooooo##..",
        "...#oooooooo#...",
        "....#oooooo#....",
        "....##oooo##....",
        "......####......",
        ".....######.....",
        ".....#oooo#.....",
        ".....######.....",
    ];

    private static readonly Dictionary<int, Cursor> Cache = [];
    private static readonly object Lock = new();

    /// <summary>El relojito de espera, al tamaño que pide el monitor principal.</summary>
    public static Cursor Wait => For(ZoomOfApp());

    /// <summary>
    /// El relojito medido contra un elemento concreto, para ventanas que viven en
    /// un monitor con otra escala que el principal.
    /// </summary>
    public static Cursor WaitFor(Visual visual) =>
        For(ZoomOf(VisualTreeHelper.GetDpi(visual).DpiScaleX));

    private static Cursor For(int zoom)
    {
        lock (Lock)
        {
            if (!Cache.TryGetValue(zoom, out Cursor? cursor))
            {
                cursor = Build(zoom);
                Cache[zoom] = cursor;
            }

            return cursor;
        }
    }

    // El factor es entero: a 125 % el cursor queda en sus 16 píxeles de origen
    // (los cursores no participan del layout y un cuarto de píxel solo emborrona);
    // de 150 % en adelante sube a píxel doble.
    private static int ZoomOf(double scale) => Math.Max(1, (int)Math.Round(scale));

    private static int ZoomOfApp()
    {
        Window? window = Application.Current?.MainWindow;
        return window is null ? 1 : ZoomOf(VisualTreeHelper.GetDpi(window).DpiScaleX);
    }

    private static Cursor Build(int zoom)
    {
        const int side = 16;
        int w = side * zoom;
        int h = side * zoom;

        // Un .cur clásico: imagen XOR en BGRA de abajo hacia arriba y máscara AND
        // a un bit con filas alineadas a 32 bits. Con canal alfa la máscara casi
        // no se consulta, pero el formato la exige completa.
        var xor = new byte[w * h * 4];
        int andStride = (w + 31) / 32 * 4;
        var and = new byte[andStride * h];

        for (int y = 0; y < h; y++)
        {
            string row = WatchMap[y / zoom];
            int outY = h - 1 - y;

            for (int x = 0; x < w; x++)
            {
                char cell = row[x / zoom];
                if (cell == '.')
                {
                    and[(outY * andStride) + (x >> 3)] |= (byte)(0x80 >> (x & 7));
                    continue;
                }

                byte v = cell == '#' ? (byte)0 : (byte)255;
                int p = ((outY * w) + x) * 4;
                xor[p] = v;
                xor[p + 1] = v;
                xor[p + 2] = v;
                xor[p + 3] = 255;
            }
        }

        var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true);

        // ICONDIR: tipo 2 es cursor, una sola imagen.
        bw.Write((ushort)0);
        bw.Write((ushort)2);
        bw.Write((ushort)1);

        // ICONDIRENTRY. En cursores los campos de planos y bits llevan el punto
        // caliente; va sobre el centro de la carátula, que es adonde mira quien
        // espera.
        bw.Write((byte)w);
        bw.Write((byte)h);
        bw.Write((byte)0);
        bw.Write((byte)0);
        bw.Write((ushort)(7 * zoom));
        bw.Write((ushort)(7 * zoom));
        bw.Write(40 + xor.Length + and.Length);
        bw.Write(22);

        // BITMAPINFOHEADER: el alto declara el doble porque abarca XOR y AND.
        bw.Write(40);
        bw.Write(w);
        bw.Write(h * 2);
        bw.Write((ushort)1);
        bw.Write((ushort)32);
        bw.Write(0);
        bw.Write(xor.Length + and.Length);
        bw.Write(0);
        bw.Write(0);
        bw.Write(0);
        bw.Write(0);

        bw.Write(xor);
        bw.Write(and);
        bw.Flush();

        ms.Position = 0;
        return new Cursor(ms);
    }
}
