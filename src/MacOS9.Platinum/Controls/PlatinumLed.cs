using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Testigo luminoso: el puntito de color que dice si algo está encendido, listo o
/// en falla.
/// </summary>
/// <remarks>
/// El color va como propiedad y el estado como un booleano aparte porque son dos
/// cosas distintas: el color dice de qué habla el testigo y el booleano si está
/// prendido. Apagado toma el gris del tema en lugar de esconderse, que es lo que
/// permite ver que el testigo existe y no está encendido.
/// </remarks>
public class PlatinumLed : Control
{
    static PlatinumLed()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumLed), new FrameworkPropertyMetadata(typeof(PlatinumLed)));
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(PlatinumLed),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Color del testigo encendido.</summary>
    public Brush? Fill
    {
        get => (Brush?)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register(
            nameof(IsOn),
            typeof(bool),
            typeof(PlatinumLed),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }
}
