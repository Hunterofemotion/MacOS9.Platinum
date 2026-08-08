using System.Windows;
using System.Windows.Controls;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Visor de tablero: pantalla oscura con dígitos de fósforo, para un valor que se
/// mira mientras cambia.
/// </summary>
/// <remarks>
/// Es otra familia que la caja de la cifra del medidor, aunque las dos muestren un
/// número. Aquella es un campo claro para un dato que acompaña a otra cosa; esta es
/// una pantalla para un dato que es el asunto —un cronómetro, un contador, una
/// lectura de instrumento—.
///
/// Va con tipografía de ancho fijo porque el contenido cambia sin parar: con ancho
/// variable los dígitos se recorren en cada actualización y el número parece
/// temblar.
/// </remarks>
public class PlatinumDisplay : Control
{
    static PlatinumDisplay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumDisplay), new FrameworkPropertyMetadata(typeof(PlatinumDisplay)));
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlatinumDisplay),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextAlignmentProperty =
        TextBox.TextAlignmentProperty.AddOwner(typeof(PlatinumDisplay),
            new FrameworkPropertyMetadata(TextAlignment.Center));

    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }
}
