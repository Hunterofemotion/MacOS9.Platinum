using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Entrada del riel de navegación: icono grande con su rótulo debajo.
/// </summary>
/// <remarks>
/// El icono y el texto van como propiedades y no como contenido armado a mano
/// porque el riel entero tiene que salir alineado, igual que en la barra de
/// herramientas. Deriva de <see cref="ListBoxItem"/> para no reinventar la
/// selección, el recorrido con el teclado ni el estado de foco.
/// </remarks>
public class PlatinumNavItem : ListBoxItem
{
    static PlatinumNavItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumNavItem), new FrameworkPropertyMetadata(typeof(PlatinumNavItem)));
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(ImageSource),
            typeof(PlatinumNavItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(PlatinumNavItem),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(PlatinumNavItem),
            new FrameworkPropertyMetadata(32d, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}
