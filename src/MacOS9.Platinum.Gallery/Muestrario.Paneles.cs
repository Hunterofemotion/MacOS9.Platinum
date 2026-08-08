using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MacOS9.Platinum.Controls;

namespace MacOS9.Platinum.Gallery;

/// <summary>
/// Los paneles del muestrario, uno por familia. Van en su propio archivo para que
/// la ventana no crezca cada vez que se agrega una variante.
/// </summary>
public partial class Muestrario
{
    // ---- Medidores ---------------------------------------------------------

    private static PlatinumLevelGauge Medidor(double valor, LevelBandMode modo,
        Orientation sentido = Orientation.Vertical, double meta = double.NaN,
        bool escala = true, bool cifra = true, int franjas = 3)
    {
        var medidor = new PlatinumLevelGauge
        {
            Value = valor,
            BandMode = modo,
            Orientation = sentido,
            Target = meta,
            ShowScale = escala,
            ShowReadout = cifra,
        };

        if (sentido == Orientation.Vertical) { medidor.Height = 170; }
        else { medidor.Width = 260; medidor.TrackThickness = 18; medidor.ScaleStep = 50; }

        var verde = (Brush)Application.Current.Resources["LedGreenBrush"];
        var ambar = (Brush)Application.Current.Resources["LedAmberBrush"];
        var rojo = (Brush)Application.Current.Resources["LedRedBrush"];

        if (franjas == 2)
        {
            medidor.Bands.Add(new LevelBand { To = 60, Fill = verde });
            medidor.Bands.Add(new LevelBand { To = 100, Fill = rojo });
        }
        else
        {
            medidor.Bands.Add(new LevelBand { To = 50, Fill = verde });
            medidor.Bands.Add(new LevelBand { To = 80, Fill = ambar });
            medidor.Bands.Add(new LevelBand { To = 100, Fill = rojo });
        }

        return medidor;
    }

    private static UIElement PanelMedidores()
    {
        var todo = Columna();

        todo.Children.Add(Nota(
            "El mismo 68 % leído de tres maneras. Fill lo pone en el área llena, "
            + "Zones pinta los umbrales y lo marca con una línea, y Bullet —la gráfica "
            + "de bala— hace las dos cosas: las franjas al fondo y una barra de medida "
            + "encima."));

        todo.Children.Add(Bloque("Las tres lecturas, de pie", Fila(
            Rotulo("Fill", Medidor(68, LevelBandMode.Fill)),
            Rotulo("Zones", Medidor(68, LevelBandMode.Zones)),
            Rotulo("Bullet", Medidor(68, LevelBandMode.Bullet)),
            Rotulo("Bullet con meta", Medidor(68, LevelBandMode.Bullet, meta: 85)))));

        todo.Children.Add(Bloque("Las tres lecturas, acostadas", Columna(
            Rotulo("Fill", Medidor(68, LevelBandMode.Fill, Orientation.Horizontal)),
            Rotulo("Zones", Medidor(68, LevelBandMode.Zones, Orientation.Horizontal)),
            Rotulo("Bullet con meta", Medidor(68, LevelBandMode.Bullet, Orientation.Horizontal, meta: 85)))));

        todo.Children.Add(Bloque("Qué se puede apagar", Fila(
            Rotulo("Completo", Medidor(45, LevelBandMode.Fill)),
            Rotulo("Sin escala", Medidor(45, LevelBandMode.Fill, escala: false)),
            Rotulo("Sin cifra", Medidor(45, LevelBandMode.Fill, cifra: false)),
            Rotulo("Solo carril", Medidor(45, LevelBandMode.Fill, escala: false, cifra: false)))));

        todo.Children.Add(Bloque("Franjas y rango", Fila(
            Rotulo("Dos franjas", Medidor(72, LevelBandMode.Fill, franjas: 2)),
            Rotulo("Sin franjas", SinFranjas()),
            Rotulo("Rango 0–8, un decimal", Escalado()),
            Rotulo("Otra unidad", ConUnidad()))));

        return todo;
    }

    private static PlatinumLevelGauge SinFranjas()
    {
        // Sin franjas declaradas el relleno toma la tinta del texto. Es el caso de
        // un medidor que solo informa cuánto hay y no si está bien.
        var medidor = new PlatinumLevelGauge { Value = 55, Height = 170 };
        return medidor;
    }

    private static PlatinumLevelGauge Escalado()
    {
        var medidor = Medidor(5.4, LevelBandMode.Fill);
        medidor.Minimum = 0;
        medidor.Maximum = 8;
        medidor.ScaleStep = 2;
        medidor.Decimals = 1;
        medidor.Unit = string.Empty;
        medidor.Bands.Clear();
        medidor.Bands.Add(new LevelBand { To = 6, Fill = (Brush)Application.Current.Resources["LedGreenBrush"] });
        medidor.Bands.Add(new LevelBand { To = 8, Fill = (Brush)Application.Current.Resources["LedRedBrush"] });
        return medidor;
    }

    private static PlatinumLevelGauge ConUnidad()
    {
        var medidor = Medidor(62, LevelBandMode.Bullet);
        medidor.Unit = " °C";
        medidor.ScaleStep = 50;
        return medidor;
    }

    // ---- Teclas ------------------------------------------------------------

    private static UIElement PanelTeclas()
    {
        var todo = Columna();

        todo.Children.Add(Bloque("Estados", Fila(
            new Button { Content = "Normal", Width = 100 },
            new Button { Content = "Por omisión", Width = 110, IsDefault = true },
            new Button { Content = "Apagada", Width = 100, IsEnabled = false })));

        todo.Children.Add(Bloque("Con icono", Fila(
            ConIcono("base-r2c1", "Imprimir..."),
            ConIcono("base-r2c5", "Buscar"),
            IconoGrande("arch-r1c3", "Subir"))));

        var barra = new ToolBar();
        barra.Items.Add(new PlatinumToolButton { Icon = Pieza("base-r1c3", 32), Text = "Nuevo" });
        barra.Items.Add(new PlatinumToolButton { Icon = Pieza("base-r1c1", 32), Text = "Abrir" });
        barra.Items.Add(new Separator());
        barra.Items.Add(new PlatinumToolButton { Icon = Pieza("base-r2c1", 32), Text = "Imprimir" });
        barra.Items.Add(new PlatinumToolButton { Icon = Pieza("seg-r2c3", 32), Text = "Avisos", IsEnabled = false });

        todo.Children.Add(Bloque("De barra de herramientas", barra));

        var abrir = new Button { Content = "Abrir un aviso...", Width = 150, HorizontalAlignment = HorizontalAlignment.Left };
        abrir.Click += (_, _) =>
        {
            var aviso = new PlatinumAlert
            {
                Owner = Application.Current.MainWindow,
                Title = "Sin espacio",
                Message = "El disco de destino se quedó sin espacio.",
                Detail = "Libera al menos 240 MB y vuelve a intentarlo.",
                AlertIcon = Pieza("base-r2c2", 32),
                Buttons = AlertButtons.OkCancel,
                OkText = "Reintentar",
                CancelText = "Cancelar",
            };
            aviso.Preparar();
            aviso.ShowDialog();
        };

        todo.Children.Add(Bloque("Que abren algo", abrir));

        return todo;
    }

    private static Button ConIcono(string icono, string texto)
    {
        var fila = new StackPanel { Orientation = Orientation.Horizontal };
        fila.Children.Add(new Image { Source = Pieza(icono, 16), Width = 16, Height = 16, Margin = new Thickness(0, 0, 6, 0) });
        fila.Children.Add(new TextBlock { Text = texto, VerticalAlignment = VerticalAlignment.Center });

        return new Button { Content = fila, Padding = new Thickness(10, 3, 10, 3) };
    }

    private static UIElement IconoGrande(string icono, string texto)
    {
        var caja = new StackPanel();
        caja.Children.Add(new Button
        {
            Width = 76,
            Height = 54,
            Padding = new Thickness(0),
            Content = new Image { Source = Pieza(icono, 32), Width = 32, Height = 32 },
        });
        caja.Children.Add(new TextBlock
        {
            Text = texto,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 0),
        });

        return caja;
    }

    // ---- Campos ------------------------------------------------------------

    private static UIElement PanelCampos()
    {
        var todo = Columna();

        todo.Children.Add(Bloque("Estados", Fila(
            Rotulo("Editable", new TextBox { Text = "Editable", Width = 140 }),
            Rotulo("Solo lectura", new TextBox { Text = "Solo lectura", Width = 140, IsReadOnly = true }),
            Rotulo("Apagado", new TextBox { Text = "Apagado", Width = 140, IsEnabled = false }))));

        todo.Children.Add(Bloque("Contraseña", Fila(
            Rotulo("Con contenido", new PasswordBox { Password = "secreto", Width = 140 }),
            Rotulo("Vacía", new PasswordBox { Width = 140 }))));

        var largo = new TextBox
        {
            Width = 380,
            Height = 90,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Text = "Un campo de varias líneas. El texto arranca arriba y no centrado, "
                 + "y la barra de desplazamiento aparece sola cuando el contenido no cabe. "
                 + "El relleno va dentro del visor para que la barra llegue al marco.",
        };

        todo.Children.Add(Bloque("De varias líneas", largo));

        var cuerpo = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Style = (Style)Application.Current.Resources["PlatinumTextBoxPlain"],
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Text = "Con franja de números al lado.\nCada renglón dibujado gasta un número.\n\n"
                 + "Los dos comparten un solo pozo, así que se leen como una pieza\ny no como dos pegadas.",
        };

        var pozo = new ContentControl
        {
            Template = (ControlTemplate)Application.Current.Resources["PlatinumWellChrome"],
            Width = 380,
            Height = 90,
        };

        var dentro = new DockPanel();
        var franja = new PlatinumLineGutter { Target = cuerpo };
        DockPanel.SetDock(franja, Dock.Left);
        dentro.Children.Add(franja);
        dentro.Children.Add(cuerpo);
        pozo.Content = dentro;

        todo.Children.Add(Bloque("Con franja de números", pozo));

        return todo;
    }

    // ---- Elección ----------------------------------------------------------

    private static UIElement PanelEleccion()
    {
        var todo = Columna();

        todo.Children.Add(Bloque("Casillas", Fila(
            new CheckBox { Content = "Marcada", IsChecked = true },
            new CheckBox { Content = "Sin marcar" },
            new CheckBox { Content = "Indeterminada", IsChecked = null, IsThreeState = true },
            new CheckBox { Content = "Apagada", IsChecked = true, IsEnabled = false })));

        todo.Children.Add(Bloque("Opciones excluyentes", Fila(
            new RadioButton { Content = "Elegida", IsChecked = true, GroupName = "m" },
            new RadioButton { Content = "Libre", GroupName = "m" },
            new RadioButton { Content = "Apagada", IsEnabled = false, GroupName = "m" })));

        var uno = new ComboBox { Width = 170, SelectedIndex = 0 };
        uno.Items.Add("Sobrescribir el del servidor");
        uno.Items.Add("Omitir el archivo");

        var dos = new ComboBox { Width = 170, SelectedIndex = 0, IsEnabled = false };
        dos.Items.Add("Apagado");

        var tres = new ComboBox { Width = 120, SelectedIndex = 0 };
        tres.Items.Add("Un elemento mucho más ancho que su caja");

        todo.Children.Add(Bloque("Menús", Fila(
            Rotulo("Normal", uno),
            Rotulo("Apagado", dos),
            Rotulo("Contenido de más", tres))));

        var deslizadores = Fila(
            Rotulo("Con marcas", new Slider { Width = 170, Value = 62, TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight, TickFrequency = 10 }),
            Rotulo("Sin marcas", new Slider { Width = 170, Value = 30 }),
            Rotulo("De pie", new Slider { Orientation = Orientation.Vertical, Height = 110, Value = 45 }));

        todo.Children.Add(Bloque("Deslizadores", deslizadores));

        return todo;
    }

    // ---- Listas ------------------------------------------------------------

    private sealed record Renglon(string Nombre, string Clase, string Tamano);

    private static UIElement PanelListas()
    {
        var todo = Columna();

        var conColumnas = new ListView { Width = 420, Height = 130 };
        var vista = new GridView();
        vista.Columns.Add(new GridViewColumn { Header = "Nombre", Width = 180, DisplayMemberBinding = new System.Windows.Data.Binding("Nombre") });
        vista.Columns.Add(new GridViewColumn { Header = "Clase", Width = 130, DisplayMemberBinding = new System.Windows.Data.Binding("Clase") });
        vista.Columns.Add(new GridViewColumn { Header = "Tamaño", Width = 80, DisplayMemberBinding = new System.Windows.Data.Binding("Tamano") });
        conColumnas.View = vista;
        conColumnas.ItemsSource = new[]
        {
            new Renglon("Presentación anual", "Documento", "2.4 MB"),
            new Renglon("Respaldo nocturno", "Unidad externa", "148 GB"),
            new Renglon("Grabación 12", "Audio", "38 MB"),
            new Renglon("Fotos del muelle", "Carpeta", "512 MB"),
        };
        conColumnas.SelectedIndex = 1;

        todo.Children.Add(Bloque("Con columnas", conColumnas));

        var lisa = new ListBox { Width = 200, Height = 110 };
        foreach (string x in new[] { "Primero", "Segundo", "Tercero", "Cuarto", "Quinto" }) { lisa.Items.Add(x); }
        lisa.SelectedIndex = 2;

        var arbol = new TreeView { Width = 220, Height = 110 };
        var raiz = new TreeViewItem { Header = "Aleaciones", IsExpanded = true };
        raiz.Items.Add(new TreeViewItem { Header = "AISI 1045" });
        raiz.Items.Add(new TreeViewItem { Header = "AISI 4340" });
        arbol.Items.Add(raiz);
        arbol.Items.Add(new TreeViewItem { Header = "Patrones" });

        todo.Children.Add(Bloque("Sin columnas y en árbol", Fila(
            Rotulo("Lista lisa", lisa),
            Rotulo("Árbol", arbol))));

        var riel = new ListBox
        {
            Style = (Style)Application.Current.Resources["PlatinumNavRail"],
            Width = 110,
            Height = 220,
            SelectedIndex = 0,
        };
        riel.Items.Add(new PlatinumNavItem { Text = "Equipo", Icon = Pieza("base-r3c3", 32) });
        riel.Items.Add(new PlatinumNavItem { Text = "Discos", Icon = Pieza("base-r2c2", 32) });
        riel.Items.Add(new PlatinumNavItem { Text = "Red", Icon = Pieza("base-r3c1", 32) });

        todo.Children.Add(Bloque("Riel de navegación", riel));

        return todo;
    }

    // ---- Estado ------------------------------------------------------------

    private static UIElement PanelEstado()
    {
        var todo = Columna();

        todo.Children.Add(Bloque("Avance", Columna(
            Rotulo("Vacía, a la mitad y llena", Fila(
                new ProgressBar { Width = 150, Height = 16, Value = 0 },
                new ProgressBar { Width = 150, Height = 16, Value = 50 },
                new ProgressBar { Width = 150, Height = 16, Value = 100 })),
            Rotulo("Indeterminada, apagada y de pie", Fila(
                new ProgressBar { Width = 150, Height = 16, IsIndeterminate = true },
                new ProgressBar { Width = 150, Height = 16, Value = 60, IsEnabled = false },
                new ProgressBar { Orientation = Orientation.Vertical, Width = 16, Height = 60, Value = 40 })))));

        var verde = (Brush)Application.Current.Resources["LedGreenBrush"];
        var ambar = (Brush)Application.Current.Resources["LedAmberBrush"];
        var rojo = (Brush)Application.Current.Resources["LedRedBrush"];

        todo.Children.Add(Bloque("Testigos", Fila(
            Rotulo("Listo", new PlatinumLed { Fill = verde }),
            Rotulo("Trabajando", new PlatinumLed { Fill = ambar }),
            Rotulo("Error", new PlatinumLed { Fill = rojo }),
            Rotulo("Apagado", new PlatinumLed { IsOn = false }),
            Rotulo("Grande", new PlatinumLed { Fill = verde, Width = 20, Height = 20 }))));

        var barra = new StatusBar { Width = 420 };
        var izq = new StatusBarItem { MinWidth = 180 };
        var conTestigo = new StackPanel { Orientation = Orientation.Horizontal };
        conTestigo.Children.Add(new PlatinumLed { Margin = new Thickness(0, 0, 6, 0), VerticalAlignment = VerticalAlignment.Center });
        conTestigo.Children.Add(new TextBlock { Text = "Listo." });
        izq.Content = conTestigo;
        DockPanel.SetDock(izq, Dock.Left);
        barra.Items.Add(izq);
        barra.Items.Add(new Separator());
        barra.Items.Add(new StatusBarItem { Content = new TextBlock { Text = "4 elementos, 1 marcado" } });

        todo.Children.Add(Bloque("Barra de estado", barra));

        return todo;
    }

    // ---- Tiempo ------------------------------------------------------------

    private static UIElement PanelTiempo()
    {
        var todo = Columna();

        var flechitas = new PlatinumStepper();

        todo.Children.Add(Bloque("Flechitas sueltas", Fila(
            Rotulo("Normal", flechitas),
            Rotulo("Apagadas", new PlatinumStepper { IsEnabled = false }))));

        todo.Children.Add(Bloque("Campo de fecha y hora", Fila(
            Rotulo("Fecha", new PlatinumDateTimeField { Mode = DateTimeFieldMode.Date, Value = new DateTime(2026, 5, 15, 9, 30, 45) }),
            Rotulo("Hora", new PlatinumDateTimeField { Mode = DateTimeFieldMode.Time, Value = new DateTime(2026, 5, 15, 9, 30, 45) }),
            Rotulo("Con segundos", new PlatinumDateTimeField { Mode = DateTimeFieldMode.TimeWithSeconds, Value = new DateTime(2026, 5, 15, 9, 30, 45) }))));

        todo.Children.Add(Bloque("Calendario", new PlatinumCalendar
        {
            DisplayMonth = new DateTime(2026, 5, 1),
            SelectedDate = new DateTime(2026, 5, 15),
            HorizontalAlignment = HorizontalAlignment.Left,
        }));

        return todo;
    }

    // ---- Cajas -------------------------------------------------------------

    private static UIElement PanelCajas()
    {
        var todo = Columna();

        var grupo = new GroupBox { Header = "Recuadro con título", Width = 240 };
        var dentro = new StackPanel();
        dentro.Children.Add(new RadioButton { Content = "Manual", GroupName = "g" });
        dentro.Children.Add(new RadioButton { Content = "Automático", IsChecked = true, GroupName = "g" });
        grupo.Content = dentro;

        var grupoLargo = new GroupBox
        {
            Header = "Un título mucho más ancho que su propio recuadro",
            Width = 180,
            Content = new TextBlock { Text = "…" },
        };

        todo.Children.Add(Bloque("Recuadros", Fila(grupo, grupoLargo)));

        var abierto = new Expander { Header = "Abierto", IsExpanded = true, Width = 240 };
        abierto.Content = new TextBlock { Text = "El triángulo apunta abajo y el contenido se ve.", TextWrapping = TextWrapping.Wrap };

        var cerrado = new Expander { Header = "Cerrado", Width = 240 };
        cerrado.Content = new TextBlock { Text = "Nunca se ve." };

        todo.Children.Add(Bloque("Plegables", Columna(abierto, cerrado)));

        var pestanas = new TabControl { Width = 420, Height = 120 };
        pestanas.Items.Add(new TabItem { Header = "Primera", Content = new TextBlock { Text = "Contenido de la primera.", Margin = new Thickness(10) } });
        pestanas.Items.Add(new TabItem { Header = "Segunda", Content = new TextBlock { Text = "…", Margin = new Thickness(10) } });
        pestanas.Items.Add(new TabItem { Header = "Un rótulo largo", Content = new TextBlock { Text = "…", Margin = new Thickness(10) } });

        todo.Children.Add(Bloque("Pestañas", pestanas));

        todo.Children.Add(Bloque("Separadores", Columna(
            new TextBlock { Text = "Arriba de la línea" },
            new Separator { Margin = new Thickness(0, 8, 0, 8) },
            new TextBlock { Text = "Abajo de la línea" })));

        return todo;
    }
}
