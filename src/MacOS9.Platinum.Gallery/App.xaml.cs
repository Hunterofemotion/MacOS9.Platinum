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

    }

    // Se elige la ventana por argumento y no por menú para poder capturar cada una
    // sin depender de que alguien la abra a mano.
    private void AlArrancar(object sender, StartupEventArgs e)
    {
        string cual = e.Args.Length > 0 ? e.Args[0].ToLowerInvariant() : "spectra";

        Window ventana = cual switch
        {
            "math" => new MathWindow(),
            "video" => new VideoWindow(),
            "notes" => new NotesWindow(),
            _ => new MainWindow(),
        };

        MainWindow = ventana;
        ventana.Show();
    }
}
