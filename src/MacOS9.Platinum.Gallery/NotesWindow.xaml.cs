using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Cuaderno de notas inventado. Es la ventana donde se ven juntos el riel de
/// navegación y la franja de números de renglón.
/// </summary>
public partial class NotesWindow : PlatinumWindow
{
    public NotesWindow()
    {
        InitializeComponent();

        Notas.ItemsSource = new[]
        {
            new { Title = "Tide pools at Cabo Pulmo", Edited = "18 Mar" },
            new { Title = "Urchin counts 2025", Edited = "02 Mar" },
            new { Title = "Gear list", Edited = "27 Feb" },
            new { Title = "Boat schedule", Edited = "19 Feb" },
            new { Title = "Reading: Steinbeck", Edited = "11 Feb" },
            new { Title = "Water temperature log", Edited = "04 Feb" },
        };

        Adjuntos.ItemsSource = new[]
        {
            new { File = "east_pool_0610.jpg", Kind = "Photograph", Size = "1.4 MB" },
            new { File = "urchin_counts.csv", Kind = "Spreadsheet", Size = "12 KB" },
            new { File = "tide_chart_march.pdf", Kind = "Document", Size = "88 KB" },
        };
    }
}
