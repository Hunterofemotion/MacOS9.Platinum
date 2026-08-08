using System.Windows;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Una franja del medidor: hasta qué valor llega y de qué color se pinta el
/// relleno mientras el nivel esté dentro de ella.
/// </summary>
/// <remarks>
/// Las franjas van como colección y no como tres propiedades sueltas —bien,
/// regular, mal— porque no todos los medidores tienen tres estados: uno de señal
/// puede tener cinco y uno de carga dos.
///
/// Deriva de <see cref="Freezable"/> para que su pincel se pueda enlazar y para
/// que la franja herede el contexto de datos de quien la aloja, que es lo que
/// permite declararla en XAML dentro del medidor.
/// </remarks>
public class LevelBand : Freezable
{
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(nameof(To), typeof(double), typeof(LevelBand),
            new PropertyMetadata(0d));

    /// <summary>Valor en el que termina la franja.</summary>
    public double To
    {
        get => (double)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(LevelBand),
            new PropertyMetadata(null));

    public Brush? Fill
    {
        get => (Brush?)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new LevelBand();
}
