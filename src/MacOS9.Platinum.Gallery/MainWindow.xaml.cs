using System.Collections.Generic;
using System.Windows;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>Renglón de la tabla de líneas espectrales.</summary>
public sealed record SpectralLine(
    string Line, string Element, string Wavelength, string Intensity, string Status);

/// <summary>
/// Consola ficticia de un espectrómetro de emisión. No mide nada: existe para
/// mostrar el tema completo en una ventana con forma de aplicación real, con
/// parte de los controles dentro de diálogos, que es donde de verdad se usan.
/// </summary>
public partial class MainWindow : PlatinumWindow
{
    public IReadOnlyList<SpectralLine> Lines { get; } =
    [
        new("Cr 267.716", "Chromium", "267.716 nm", "18 420", "Accepted"),
        new("Cr 283.563", "Chromium", "283.563 nm", "9 118", "Accepted"),
        new("Mn 293.306", "Manganese", "293.306 nm", "27 905", "Accepted"),
        new("Mn 403.076", "Manganese", "403.076 nm", "4 662", "Weak"),
        new("Ni 341.476", "Nickel", "341.476 nm", "12 077", "Accepted"),
        new("Ni 352.454", "Nickel", "352.454 nm", "1 340", "Below limit"),
        new("Fe 371.993", "Iron", "371.993 nm", "63 511", "Saturated"),
    ];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Deja el campo con el texto seleccionado para poder revisar el resalte sin
        // reproducirlo a mano en cada arranque.
        Loaded += (_, _) =>
        {
            SampleField.Focus();
            SampleField.SelectAll();
        };
    }

    private void OnCalibrate(object sender, RoutedEventArgs e) =>
        new CalibrationDialog { Owner = this }.ShowDialog();

    private void OnExport(object sender, RoutedEventArgs e) =>
        new ExportDialog { Owner = this }.ShowDialog();

    private void OnInstrument(object sender, RoutedEventArgs e) =>
        new InstrumentDialog { Owner = this }.ShowDialog();
}
