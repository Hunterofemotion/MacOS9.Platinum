using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Tecla de barra de herramientas: icono grande arriba y rótulo debajo, sin marco
/// hasta que el puntero entra. Deriva de Button para no perder el mando por
/// comando, el foco ni la accesibilidad; lo único que agrega es la disposición.
///
/// El icono y el texto van como propiedades y no como contenido armado a mano
/// porque toda la barra tiene que salir alineada: si cada consumidor apila su
/// imagen y su etiqueta, los rótulos quedan a alturas distintas.
/// </summary>
public class PlatinumToolButton : Button
{
    static PlatinumToolButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumToolButton), new FrameworkPropertyMetadata(typeof(PlatinumToolButton)));
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(PlatinumToolButton),
            new PropertyMetadata(null));

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlatinumToolButton),
            new PropertyMetadata("", AlCambiarTexto));

    // El rótulo vive en Text y no en Content, así que la automatización veía la
    // tecla sin nombre: ni un lector de pantalla ni una prueba automática podían
    // referirse a ella. El nombre se copia aquí para que las dos cosas coincidan.
    private static void AlCambiarTexto(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Windows.Automation.AutomationProperties.SetName(d, (string)e.NewValue ?? string.Empty);
    }

    /// <summary>Rótulo bajo el icono.</summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(PlatinumToolButton),
            new PropertyMetadata(32d));

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}
