using System.Collections.Generic;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>Renglón de muestra para la lista con columnas.</summary>
public sealed record MailRow(string Sender, string Subject, string Size);

/// <summary>
/// Catálogo visual de los controles del tema Platinum.
/// </summary>
public partial class MainWindow : PlatinumWindow
{
    public IReadOnlyList<MailRow> Messages { get; } =
    [
        new("Alex Morgan", "Project Orion status update", "12K"),
        new("Dana White", "Lunch this week?", "3K"),
        new("Team Sync", "Monthly planning meeting", "7K"),
        new("Chris Johnson", "Re: Budget estimates", "9K"),
        new("J. Anderson", "Quarterly report attached", "28K"),
        new("Melissa Lee", "Client feedback", "5K"),
        new("Billy Chan", "Weekend hiking trip", "4K"),
    ];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Deja el primer campo con el texto seleccionado para poder revisar el
        // resalte sin tener que reproducirlo a mano en cada arranque.
        Loaded += (_, _) =>
        {
            SampleField.Focus();
            SampleField.SelectAll();
        };
    }
}
