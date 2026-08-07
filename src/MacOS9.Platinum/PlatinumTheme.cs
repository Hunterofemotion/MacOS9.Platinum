using System.Reflection;
using System.Windows;

namespace MacOS9.Platinum;

/// <summary>Ajustes de proceso que el tema no puede hacer desde un diccionario.</summary>
public static class PlatinumTheme
{
    private static bool respetarElSistema;

    /// <summary>
    /// Deja que Windows mande sobre la caída de los menús, en lugar del tema.
    ///
    /// Llamarla antes de crear la primera <c>PlatinumWindow</c>. Después ya no
    /// sirve: para entonces el ajuste ya se aplicó.
    /// </summary>
    /// <remarks>
    /// Existe porque <see cref="UseLeftMenuDrop"/> pisa una preferencia del
    /// usuario. Quien tenga una razón para respetarla la tiene aquí.
    /// </remarks>
    public static void KeepSystemMenuDropAlignment() => respetarElSistema = true;

    /// <summary>
    /// Hace que los menús caigan alineados a la izquierda de su título.
    ///
    /// Windows tiene un ajuste, MenuDropAlignment, que alinea los menús a la
    /// derecha del puntero. Suele venir encendido en equipos configurados para
    /// zurdos, y WPF lo obedece dentro del propio MenuItem: ninguna combinación de
    /// Placement, PlacementTarget ni FlowDirection lo cambia, porque el volteo se
    /// aplica después. El resultado es una hoja de menú que se abre hacia afuera
    /// de la ventana y se sale de la pantalla.
    ///
    /// En una barra de menús de Mac OS 9 la hoja siempre cae bajo su título, así
    /// que el tema necesita el otro comportamiento.
    ///
    /// <c>PlatinumWindow</c> la llama sola en su constructor estático, así que una
    /// aplicación que use la ventana del tema no tiene que hacer nada. Sigue
    /// siendo pública para el caso contrario: una ventana de WPF de siempre que
    /// solo fusiona el diccionario y tiene un menú. Ahí hay que llamarla a mano,
    /// antes de mostrar la primera ventana.
    /// </summary>
    public static void UseLeftMenuDrop()
    {
        if (respetarElSistema) { return; }
        if (!SystemParameters.MenuDropAlignment) { return; }

        // No hay propiedad pública que escribir: el valor vive en un campo estático
        // privado que SystemParameters llena una sola vez desde el sistema.
        FieldInfo? campo = typeof(SystemParameters).GetField(
            "_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

        campo?.SetValue(null, false);
    }
}
