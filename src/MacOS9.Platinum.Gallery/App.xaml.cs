using System.Windows;

namespace MacOS9.Platinum.Gallery;

public partial class App : Application
{
    public App()
    {
        // WPF trae dos motores para dibujar la selección de texto. El viejo la pinta
        // como un adorno encima del texto, así que un resalte opaco lo tapa y
        // SelectionTextBrush se ignora. Este interruptor activa el motor nuevo, que
        // pinta el resalte detrás y respeta el color del texto seleccionado.
        // Debe quedar puesto antes de que se muestre el primer campo.
        AppContext.SetSwitch(
            "Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering",
            false);

        // Windows puede estar configurado para alinear los menús a la derecha del
        // puntero, y WPF lo obedece por dentro: la hoja se abría hacia afuera de la
        // ventana. En una barra de Mac OS 9 cae bajo su título.
        PlatinumTheme.UseLeftMenuDrop();
    }
}
