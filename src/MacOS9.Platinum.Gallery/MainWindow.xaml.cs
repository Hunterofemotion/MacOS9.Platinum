using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Catálogo visual de los controles del tema Platinum.
/// </summary>
public partial class MainWindow : PlatinumWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // Deja el primer campo con el texto seleccionado para poder revisar el
        // resalte sin tener que reproducirlo a mano en cada arranque.
        Loaded += (_, _) =>
        {
            SampleField.Focus();
            SampleField.SelectAll();
        };
    }
}
