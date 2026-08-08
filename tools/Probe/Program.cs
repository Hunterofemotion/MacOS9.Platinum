using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MacOS9.Platinum.Controls;

namespace Probe;

// Sonda de auditoría. No abre ninguna ventana ni toca el escritorio: instancia las
// plantillas del tema en memoria y renderiza los escenarios con RenderTargetBitmap.
// Sirve para cubrir estados que una captura de pantalla no alcanza (deshabilitados,
// contenido extremo, anchos de ventana) sin interrumpir la sesión del usuario.
internal static class Program
{
    // Carpeta de salida: PROBE_OUT si viene por ambiente, o "render" junto al exe.
    private static readonly string OutDir =
        Environment.GetEnvironmentVariable("PROBE_OUT")
        ?? Path.Combine(AppContext.BaseDirectory, "render");

    private static int _failures;

    [STAThread]
    private static int Main()
    {
        Directory.CreateDirectory(OutDir);

        var app = new Application
        {
            Resources = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MacOS9.Platinum;component/Themes/Platinum.xaml"),
            },
        };

        var dpi = VisualTreeHelper.GetDpi(new Border());
        Console.WriteLine($"DPI del proceso: x{dpi.DpiScaleX} y{dpi.DpiScaleY}");

        SweepTemplates(app.Resources);
        RenderScenarios(dpi);

        Console.WriteLine(_failures == 0
            ? "RESULTADO: sin fallos."
            : $"RESULTADO: {_failures} fallo(s).");
        return _failures == 0 ? 0 : 1;
    }

    // ---- Parte A: cargar todas las plantillas y estilos del tema -------------

    // Instanciar una plantilla obliga a WPF a resolver sus StaticResource y sus
    // TargetName. Es la única forma de tocar ramas que sólo existen abiertas, como
    // el contenido de un Popup de submenú.
    private static void SweepTemplates(ResourceDictionary root)
    {
        var seen = new HashSet<ResourceDictionary>();
        var templates = 0;
        var styles = 0;

        foreach (var rd in Flatten(root, seen))
        {
            foreach (DictionaryEntry entry in rd)
            {
                switch (entry.Value)
                {
                    case ControlTemplate ct when ct.TargetType is not null:
                        templates++;
                        Try($"plantilla {Describe(entry.Key)} ({ct.TargetType.Name})", () =>
                        {
                            var c = (Control)Activator.CreateInstance(ct.TargetType)!;
                            Populate(c);
                            c.Template = ct;
                            c.ApplyTemplate();
                        });
                        break;

                    case Style st when st.TargetType is not null && typeof(Control).IsAssignableFrom(st.TargetType):
                        styles++;
                        Try($"estilo {Describe(entry.Key)} ({st.TargetType.Name})", () =>
                        {
                            var c = (Control)Activator.CreateInstance(st.TargetType)!;
                            Populate(c);
                            c.Style = st;
                            c.ApplyTemplate();
                        });
                        break;
                }
            }
        }

        Console.WriteLine($"Plantillas instanciadas: {templates}   Estilos instanciados: {styles}");
    }

    // Algunas plantillas sólo arman sus ramas si el control tiene contenido: un
    // MenuItem sin hijos nunca crea el Popup del submenú, por ejemplo.
    private static void Populate(Control c)
    {
        switch (c)
        {
            case MenuItem mi:
                mi.Header = "Elemento";
                mi.Items.Add(new MenuItem { Header = "Hijo" });
                break;
            case TabControl tc:
                tc.Items.Add(new TabItem { Header = "Uno", Content = "a" });
                break;
            case TreeViewItem tvi:
                tvi.Header = "Nodo";
                tvi.Items.Add(new TreeViewItem { Header = "Hijo" });
                break;
            case ComboBox cb:
                cb.Items.Add("Uno");
                break;
            case ContentControl cc:
                cc.Content = "Contenido";
                break;
            case ItemsControl ic:
                ic.Items.Add("Uno");
                break;
        }
    }

    private static IEnumerable<ResourceDictionary> Flatten(ResourceDictionary rd, HashSet<ResourceDictionary> seen)
    {
        if (!seen.Add(rd))
        {
            yield break;
        }

        yield return rd;

        foreach (var child in rd.MergedDictionaries)
        {
            foreach (var d in Flatten(child, seen))
            {
                yield return d;
            }
        }
    }

    private static string Describe(object key) =>
        key as string ?? key.ToString() ?? "(sin nombre)";

    private static void Try(string what, Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _failures++;
            Console.WriteLine($"FALLO  {what}");
            Console.WriteLine($"       {ex.GetType().Name}: {Flatten(ex)}");
        }
    }

    private static string Flatten(Exception ex)
    {
        var sb = new StringBuilder();
        for (var e = ex; e is not null; e = e.InnerException)
        {
            if (sb.Length > 0)
            {
                sb.Append(" <- ");
            }

            sb.Append(e.Message.Replace(Environment.NewLine, " "));
        }

        return sb.ToString();
    }

    // ---- Parte B: renderizar escenarios fuera de pantalla --------------------

    private static void RenderScenarios(DpiScale dpi)
    {
        Render("botones", dpi, Stack(
            Row(Btn("Aceptar", def: true), Btn("Cancelar"), Btn("Apagado", on: false)),
            Row(Btn("Un texto de botón desmedidamente largo para el ancho"))));

        // El anillo del botón por omisión se dibuja aparte porque en el botón solo
        // aparece con IsDefaulted o IsKeyboardFocused, y en un render fuera de
        // pantalla no hay foco: nunca había entrado a un patrón aprobado, así que
        // sus medidas no las vigilaba nadie.
        Render("anillo", dpi, Stack(
            Row(ConAnillo(96), ConAnillo(150)),
            Row(ConAnillo(60, 40))));

        Render("marcas", dpi, Stack(
            Row(new CheckBox { Content = "Marcado", IsChecked = true },
                new CheckBox { Content = "Vacío" },
                new CheckBox { Content = "Mixto", IsChecked = null, IsThreeState = true },
                new CheckBox { Content = "Apagado", IsChecked = true, IsEnabled = false }),
            Row(new RadioButton { Content = "Elegido", IsChecked = true },
                new RadioButton { Content = "Libre" },
                new RadioButton { Content = "Apagado", IsChecked = true, IsEnabled = false })));

        Render("campos", dpi, Stack(
            Row(new TextBox { Text = "Editable", Width = 120 },
                new TextBox { Text = "Sólo lectura", Width = 120, IsReadOnly = true },
                new TextBox { Text = "Apagado", Width = 120, IsEnabled = false }),
            Row(new TextBox { Width = 200, Text = "Un contenido mucho más largo que el ancho disponible del campo" })));

        // Un mes fijo y no el de hoy: un escenario que cambia con la fecha no sirve
        // para comparar contra un patrón aprobado.
        Render("calendario", dpi, Stack(
            Row(new PlatinumCalendar
            {
                DisplayMonth = new DateTime(2024, 5, 1),
                SelectedDate = new DateTime(2024, 5, 15),
            })));

        // Los dos modos y los dos sentidos, con el mismo valor: así se compara qué
        // dice cada lectura del mismo dato.
        Render("medidores", dpi, Stack(
            Row(Medidor(72, LevelBandMode.Fill),
                Medidor(38, LevelBandMode.Fill),
                Medidor(62, LevelBandMode.Zones),
                Medidor(95, LevelBandMode.Zones)),
            // El caso horizontal va aquí porque su medida es distinta: la escala
            // gasta alto y la cifra ancho. Sin este escenario, que la cifra se
            // dibujara fuera del control no lo detectaba nadie.
            Row(Medidor(78, LevelBandMode.Zones, Orientation.Horizontal),
                Medidor(45, LevelBandMode.Fill, Orientation.Horizontal)),
            // La gráfica de bala, con y sin meta: las franjas atenuadas al fondo y
            // la barra de medida encima.
            Row(Medidor(68, LevelBandMode.Bullet, Orientation.Horizontal, meta: 85),
                Medidor(92, LevelBandMode.Bullet, Orientation.Horizontal)),
            Row(Medidor(68, LevelBandMode.Bullet, meta: 85),
                Medidor(30, LevelBandMode.Bullet))));

        Render("riel", dpi, Stack(
            Row(Rail())));

        Render("testigos", dpi, Stack(
            Row(new PlatinumLed(),
                new PlatinumLed { Fill = (Brush)Application.Current.Resources["LedAmberBrush"] },
                new PlatinumLed { Fill = (Brush)Application.Current.Resources["LedRedBrush"] },
                new PlatinumLed { IsOn = false },
                new PlatinumLed { Width = 20, Height = 20 })));

        Render("fecha_hora", dpi, Stack(
            Row(new PlatinumDateTimeField { Mode = DateTimeFieldMode.Date, Value = new DateTime(2024, 5, 15, 9, 30, 45) },
                new PlatinumDateTimeField { Mode = DateTimeFieldMode.Time, Value = new DateTime(2024, 5, 15, 9, 30, 45) },
                new PlatinumDateTimeField { Mode = DateTimeFieldMode.TimeWithSeconds, Value = new DateTime(2024, 5, 15, 9, 30, 45) })));

        Render("progreso", dpi, Stack(
            Row(Bar(0), Bar(50), Bar(100)),
            Row(new ProgressBar { Width = 160, Height = 14, IsIndeterminate = true },
                new ProgressBar { Width = 160, Height = 14, Value = 60, IsEnabled = false },
                new ProgressBar { Orientation = Orientation.Vertical, Width = 14, Height = 60, Value = 40 })));

        Render("deslizadores", dpi, Stack(
            Row(Sl(o: Orientation.Horizontal, len: 180, ticks: true),
                Sl(o: Orientation.Horizontal, len: 180, on: false)),
            Row(Sl(o: Orientation.Vertical, len: 110, ticks: true),
                Sl(o: Orientation.Vertical, len: 110))));

        Render("agrupacion", dpi, Stack(
            Row(new GroupBox
            {
                Header = "Configuración",
                Width = 220,
                Content = new StackPanel
                {
                    Children = { new RadioButton { Content = "Manual" }, new RadioButton { Content = "DHCP", IsChecked = true } },
                },
            },
            new GroupBox
            {
                Header = "Un encabezado de grupo mucho más ancho que su propio recuadro",
                Width = 180,
                Content = new TextBlock { Text = "…" },
            })));

        Render("arbol", dpi, Tree());
        Render("lista", dpi, List());
        Render("lista_sin_columnas", dpi, PlainList());
        Render("barras", dpi, Bars());
        Render("pestanas", dpi, Tabs());
        Render("menu", dpi, Bar());
        Render("desplegable", dpi, Combo());
        Render("iconos", dpi, Icons());
        Render("hoja", dpi, Sheet());

        RenderWindow("ventana_angosta", dpi, 480);
        RenderWindow("ventana_ancha", dpi, 980);
        RenderWindow("ventana_enrollada", dpi, 480, collapsed: true);
    }

    private static FrameworkElement ConAnillo(double ancho, double alto = 26)
    {
        // Las medidas son las mismas que el estilo del botón le pasa al anillo. Si
        // alguna cambia allá y no aquí, el escenario deja de comprobar lo que se usa.
        var caja = new Grid
        {
            Width = ancho,
            Height = alto,
            Margin = new Thickness(12, 8, 24, 12),
        };

        caja.Children.Add(new Border
        {
            Background = (Brush)Application.Current.Resources["ControlFaceBrush"],
            BorderBrush = (Brush)Application.Current.Resources["TextBrush"],
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(2),
        });

        caja.Children.Add(new DefaultRing
        {
            Stroke = (Brush)Application.Current.Resources["TextBrush"],
            Thickness = 1,
            Gap = 2,
            CornerRadius = 2,
            Margin = new Thickness(0, 0, 1, 1),
        });

        return caja;
    }

    private static Button Btn(string text, bool def = false, bool on = true) =>
        new() { Content = text, IsDefault = def, IsEnabled = on, MinWidth = 80 };

    private static Slider Sl(Orientation o, double len, bool ticks = false, bool on = true)
    {
        var s = new Slider
        {
            Orientation = o,
            Minimum = 0,
            Maximum = 100,
            Value = 40,
            IsEnabled = on,
            TickFrequency = 20,
            TickPlacement = ticks ? TickPlacement.BottomRight : TickPlacement.None,
            VerticalAlignment = VerticalAlignment.Top,
        };
        if (o == Orientation.Vertical)
        {
            s.Height = len;
        }
        else
        {
            s.Width = len;
        }

        return s;
    }

    private static ProgressBar Bar(double v) =>
        new() { Width = 160, Height = 14, Value = v };

    private static FrameworkElement Tree()
    {
        var tv = new TreeView { Width = 220, Height = 130 };
        var disk = new TreeViewItem { Header = "WorkDisk", IsExpanded = true };
        var docs = new TreeViewItem { Header = "Documents", IsExpanded = true };
        docs.Items.Add(new TreeViewItem { Header = "Projects", IsSelected = true });
        docs.Items.Add(new TreeViewItem { Header = "Un nombre de carpeta bastante más largo que el ancho del árbol" });
        disk.Items.Add(docs);
        disk.Items.Add(new TreeViewItem { Header = "System Folder" });
        tv.Items.Add(disk);
        return tv;
    }

    private static FrameworkElement List()
    {
        // Columna de icono igual a la de la galería: 26 de ancho con una imagen de 16.
        var cell = new DataTemplate();
        var img = new FrameworkElementFactory(typeof(Image));
        img.SetValue(Image.SourceProperty, (ImageSource)Application.Current.Resources["IconEnvelope"]);
        img.SetValue(FrameworkElement.WidthProperty, 16d);
        img.SetValue(FrameworkElement.HeightProperty, 16d);
        img.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        cell.VisualTree = img;

        var view = new GridView();
        view.Columns.Add(new GridViewColumn { Header = string.Empty, Width = 26, CellTemplate = cell });
        view.Columns.Add(new GridViewColumn { Header = "From", Width = 130 });
        view.Columns.Add(new GridViewColumn { Header = "Size", Width = 60 });
        var lv = new ListView { View = view, Width = 260, Height = 120, SelectedIndex = 2 };
        for (var i = 0; i < 8; i++)
        {
            lv.Items.Add(new { });
        }

        return lv;
    }

    private static ScrollBar Sb(Orientation o, double len, bool on = true)
    {
        var sb = new ScrollBar
        {
            Orientation = o,
            Minimum = 0,
            Maximum = 100,
            ViewportSize = 40,
            Value = 30,
            IsEnabled = on,
            Margin = new Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Top,
        };
        if (o == Orientation.Vertical)
        {
            sb.Height = len;
        }
        else
        {
            sb.Width = len;
        }

        return sb;
    }

    // ListView sin View: no debe salir la franja de encabezados.
    private static FrameworkElement PlainList()
    {
        var lv = new ListView { Width = 200, Height = 110 };
        foreach (var s in new[] { "Geneva", "Charcoal", "Chicago", "Monaco", "Palatino" })
        {
            lv.Items.Add(s);
        }

        return lv;
    }

    private static FrameworkElement Bars()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        panel.Children.Add(Sb(Orientation.Vertical, 120));
        panel.Children.Add(Sb(Orientation.Vertical, 120, on: false));
        // Riel muy corto: el thumb ya no cabe entre las flechas.
        panel.Children.Add(Sb(Orientation.Vertical, 46));
        panel.Children.Add(Sb(Orientation.Horizontal, 140));
        return panel;
    }

    private static FrameworkElement Tabs()
    {
        var tc = new TabControl { Width = 300, Height = 110 };
        tc.Items.Add(new TabItem { Header = "General", Content = new TextBlock { Text = "Contenido", Margin = new Thickness(10) }, IsSelected = true });
        tc.Items.Add(new TabItem { Header = "Opciones", Content = "b" });
        tc.Items.Add(new TabItem { Header = "Un rótulo de pestaña largo", Content = "c" });
        return tc;
    }

    private static FrameworkElement Bar()
    {
        var menu = new Menu { Width = 300 };
        var file = new MenuItem { Header = "_File" };
        file.Items.Add(new MenuItem { Header = "_New", InputGestureText = "Ctrl+N" });
        var sort = new MenuItem { Header = "Sort by" };
        sort.Items.Add(new MenuItem { Header = "Name", IsCheckable = true, IsChecked = true });
        menu.Items.Add(file);
        menu.Items.Add(new MenuItem { Header = "_Edit", Items = { sort } });
        menu.Items.Add(new MenuItem { Header = "_View", IsEnabled = false });
        return menu;
    }

    private static FrameworkElement Combo()
    {
        var a = new ComboBox { Width = 150, Items = { "Nueve", "Diez" }, SelectedIndex = 0 };
        var b = new ComboBox { Width = 150, Items = { "Apagado" }, SelectedIndex = 0, IsEnabled = false, Margin = new Thickness(10, 0, 0, 0) };
        var c = new ComboBox { Width = 110, Items = { "Un elemento mucho más ancho que la caja" }, SelectedIndex = 0, Margin = new Thickness(10, 0, 0, 0) };
        return new StackPanel { Orientation = Orientation.Horizontal, Children = { a, b, c } };
    }

    private static FrameworkElement Medidor(double valor, LevelBandMode modo,
        Orientation sentido = Orientation.Vertical, double meta = double.NaN)
    {
        var medidor = new PlatinumLevelGauge
        {
            Value = valor,
            BandMode = modo,
            Orientation = sentido,
            Target = meta,
            Margin = new Thickness(0, 0, 18, 0),
        };

        if (sentido == Orientation.Vertical) { medidor.Height = 190; }
        else { medidor.Width = 300; medidor.TrackThickness = 18; medidor.ScaleStep = 50; }

        // Franjas al revés que el ejemplo del búfer: aquí más es peor, que es el
        // caso de una ocupación. El control no supone ninguno de los dos sentidos.
        medidor.Bands.Add(new LevelBand { To = 50, Fill = (Brush)Application.Current.Resources["LedGreenBrush"] });
        medidor.Bands.Add(new LevelBand { To = 80, Fill = (Brush)Application.Current.Resources["LedAmberBrush"] });
        medidor.Bands.Add(new LevelBand { To = 100, Fill = (Brush)Application.Current.Resources["LedRedBrush"] });

        return medidor;
    }

    private static FrameworkElement Rail()
    {
        var lista = new ListBox
        {
            Style = (Style)Application.Current.Resources["PlatinumNavRail"],
            Width = 116,
        };

        (string llave, string texto)[] entradas =
        [
            ("IconFolder", "Recovery Items"),
            ("IconHardDisk", "Recovery Sets"),
            ("IconDocument", "Summary"),
            ("IconComputer", "Preferences"),
        ];

        foreach (var (llave, texto) in entradas)
        {
            lista.Items.Add(new PlatinumNavItem
            {
                Icon = (ImageSource)Application.Current.Resources[llave],
                Text = texto,
            });
        }

        lista.SelectedIndex = 0;
        return lista;
    }

    private static FrameworkElement Icons()
    {
        var keys = new[]
        {
            "IconFolder", "IconDocument", "IconFloppy", "IconHardDisk", "IconTrash",
            "IconAlert", "IconInfo", "IconEnvelope", "IconComputer", "IconMagnifier",
        };
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        foreach (var k in keys)
        {
            panel.Children.Add(new Image
            {
                Source = (ImageSource)Application.Current.Resources[k],
                Width = 16,
                Height = 16,
                Margin = new Thickness(4, 0, 4, 0),
            });
        }

        return panel;
    }

    // El tooltip y las hojas de menú viven en su propio HWND, así que una captura de
    // ventana no los alcanza. Aquí se renderiza el mismo chrome que usan, aplicado a
    // un ContentControl, que es exactamente lo que el Popup contiene.
    private static FrameworkElement Sheet()
    {
        var chrome = (ControlTemplate)Application.Current.Resources["PlatinumPopupChrome"];
        var tip = new ContentControl
        {
            Template = chrome,
            Background = (Brush)Application.Current.Resources["TipFaceBrush"],
            Padding = new Thickness(4, 2, 4, 2),
            Content = new TextBlock { Text = "Muestra las opciones adicionales." },
            Margin = new Thickness(0, 0, 12, 0),
        };
        var menu = new ContentControl
        {
            Template = chrome,
            Background = (Brush)Application.Current.Resources["ContentFaceBrush"],
            Padding = new Thickness(1),
            MinWidth = 150,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = "  Nuevo", Margin = new Thickness(0, 3, 0, 3) },
                    new TextBlock { Text = "  Abrir…", Margin = new Thickness(0, 3, 0, 3) },
                },
            },
        };
        return new StackPanel { Orientation = Orientation.Horizontal, Children = { tip, menu } };
    }

    private static StackPanel Stack(params UIElement[] rows)
    {
        var p = new StackPanel();
        foreach (var r in rows)
        {
            p.Children.Add(r);
        }

        return p;
    }

    private static StackPanel Row(params UIElement[] items)
    {
        var p = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        foreach (var i in items)
        {
            if (i is FrameworkElement fe)
            {
                fe.Margin = new Thickness(fe.Margin.Left, fe.Margin.Top, fe.Margin.Right + 10, fe.Margin.Bottom);
            }

            p.Children.Add(i);
        }

        return p;
    }

    private static void Render(string name, DpiScale dpi, FrameworkElement content)
    {
        Try($"render {name}", () =>
        {
            var host = new Border
            {
                Background = (Brush)Application.Current.Resources["WindowFaceBrush"],
                Padding = new Thickness(12),
                Child = content,
                UseLayoutRounding = true,
                SnapsToDevicePixels = true,
            };
            TextOptions.SetTextFormattingMode(host, TextFormattingMode.Display);
            Save(name, host, dpi, new Size(1200, 900));
        });
    }

    // La ventana no se muestra: se le aplica la plantilla y se mide y acomoda su
    // raíz visual a mano, que es lo que hace el sistema de layout al presentarla.
    private static void RenderWindow(string name, DpiScale dpi, double width, bool collapsed = false)
    {
        Try($"render {name}", () =>
        {
            var w = new PlatinumWindow
            {
                Title = "Un título de ventana lo bastante largo como para no caber en 480 puntos",
                Width = width,
                Height = 220,
                ShowInTaskbar = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBox { Text = "Contenido", Width = 140, HorizontalAlignment = HorizontalAlignment.Left },
                        new Button { Content = "Aceptar", Width = 80, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 8, 0, 0) },
                    },
                },
            };
            w.ApplyTemplate();

            // La ventana nunca se muestra, así que IsActive es falso y el trigger de
            // inactividad esconde las cajas y el rayado. Un valor local gana sobre un
            // setter de trigger, así que esto reconstruye el aspecto de ventana activa
            // sin abrir nada.
            foreach (var part in new[] { "PART_CloseBox", "PART_ZoomBox", "PART_CollapseBox", "Stripes" })
            {
                if (w.Template.FindName(part, w) is UIElement e)
                {
                    e.Visibility = Visibility.Visible;
                }
            }

            if (w.Template.FindName("TitleText", w) is TextBlock tb)
            {
                tb.Foreground = (Brush)Application.Current.Resources["TextBrush"];
            }

            // El windowshade fija Height desde el callback de IsCollapsed; se lee de
            // vuelta para acomodar la raíz exactamente al alto que la ventana pediría.
            double height = 220d;
            if (collapsed)
            {
                w.IsCollapsed = true;
                height = w.Height;
                Console.WriteLine($"  (enrollada: Height = {height} unidades)");
            }

            var root = (FrameworkElement)VisualTreeHelper.GetChild(w, 0);
            Save(name, root, dpi, new Size(width, height), exact: true);
        });
    }

    private static void Save(string name, FrameworkElement el, DpiScale dpi, Size available, bool exact = false)
    {
        // Dos pases con la cola de despacho vaciada entre ellos. Hay plantillas cuya
        // apariencia depende de un ActualWidth que sólo existe después del primer
        // acomodo (el hueco del título del GroupBox usa BorderGapMaskConverter); con
        // un solo pase la máscara sale vacía y la línea desaparece del render.
        for (var pass = 0; pass < 2; pass++)
        {
            el.Measure(available);
            el.Arrange(new Rect(exact ? available : el.DesiredSize));
            el.UpdateLayout();
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.Loaded);
        }

        var w = (int)Math.Ceiling(el.ActualWidth * dpi.DpiScaleX);
        var h = (int)Math.Ceiling(el.ActualHeight * dpi.DpiScaleY);
        var rtb = new RenderTargetBitmap(w, h, 96 * dpi.DpiScaleX, 96 * dpi.DpiScaleY, PixelFormats.Pbgra32);
        rtb.Render(el);

        var enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(rtb));
        using var fs = File.Create(Path.Combine(OutDir, name + ".png"));
        enc.Save(fs);
        Console.WriteLine($"  {name}.png  {w}x{h}");
    }
}
