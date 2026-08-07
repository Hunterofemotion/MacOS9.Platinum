using System.Windows;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Diálogo de exportación. Junta campo de texto, menú emergente y casillas con el
/// icono de documento a la izquierda, que es la disposición clásica de un cuadro
/// de guardar de Mac OS.
/// </summary>
public partial class ExportDialog : PlatinumWindow
{
    public ExportDialog() => InitializeComponent();

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
