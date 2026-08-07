using System.Windows;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Diálogo de conexión con el banco. Es el único sitio del catálogo donde aparece
/// el campo de contraseña, así que también sirve de prueba de ese control.
/// </summary>
public partial class InstrumentDialog : PlatinumWindow
{
    public InstrumentDialog() => InitializeComponent();

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
