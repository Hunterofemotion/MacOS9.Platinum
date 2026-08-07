using System.Windows;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Diálogo modal de calibración. Muestra el grupo de opciones excluyentes, el
/// deslizador con marcas y la barra de progreso indeterminada, que son piezas que
/// en una ventana principal casi nunca conviven.
/// </summary>
public partial class CalibrationDialog : PlatinumWindow
{
    public CalibrationDialog() => InitializeComponent();

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
