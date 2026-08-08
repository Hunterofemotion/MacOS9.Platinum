using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Xml;

namespace Volcador
{
    // Vuelca las plantillas de fábrica de WPF a disco. Sin ningún tema mezclado,
    // así que lo que sale es lo que el marco de trabajo trae de origen.
    public static class Programa
    {
        private static string carpeta;

        [STAThread]
        public static void Main()
        {
            carpeta = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "salida");
            carpeta = Path.GetFullPath(carpeta);
            Directory.CreateDirectory(carpeta);

            new Application();

            ListView lista = new ListView();
            GridView vista = new GridView();
            vista.Columns.Add(new GridViewColumn { Header = "Nombre", Width = 90 });
            vista.Columns.Add(new GridViewColumn { Header = "Tamaño", Width = 60 });
            lista.View = vista;
            lista.Items.Add("uno");
            lista.Items.Add("dos");

            StackPanel caja = new StackPanel();
            caja.Children.Add(lista);
            ScrollBar barra = new ScrollBar { Orientation = Orientation.Vertical, Height = 120 };
            caja.Children.Add(barra);
            GridViewColumnHeader encabezado = new GridViewColumnHeader { Content = "X" };
            caja.Children.Add(encabezado);
            ScrollViewer visor = new ScrollViewer { Height = 60 };
            caja.Children.Add(visor);

            Window ventana = new Window
            {
                Content = caja,
                Width = 400,
                Height = 500,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Left = -20000,
                Top = -20000
            };
            ventana.Show();
            ventana.UpdateLayout();

            Guardar("ListView.plantilla.xaml", lista.Template);
            Guardar("ScrollBar.vertical.plantilla.xaml", barra.Template);
            Guardar("GridViewColumnHeader.plantilla.xaml", encabezado.Template);
            Guardar("ScrollViewer.plantilla.xaml", visor.Template);

            object estilo = lista.TryFindResource(GridView.GridViewScrollViewerStyleKey);
            Guardar("GridViewScrollViewer.estilo.xaml", estilo);

            object plantillaGrid = lista.TryFindResource(typeof(GridViewHeaderRowPresenter));
            Guardar("GridViewHeaderRowPresenter.estilo.xaml", plantillaGrid);

            ListViewItem renglon = new ListViewItem();
            caja.Children.Add(renglon);
            ventana.UpdateLayout();
            Guardar("ListViewItem.plantilla.xaml", renglon.Template);

            // El resto de los controles que el tema reemplaza. Antes solo se
            // volcaban los de lista y desplazamiento, así que las piezas de fábrica
            // de todos los demás nunca se compararon: el comprobador decía que
            // estaban las siete y eran siete de seis controles, no de todos.
            VolcarResto(caja, ventana);

            ventana.Close();
            Application.Current.Shutdown();
        }

        // Cada control se mete en la ventana antes de pedirle su plantilla: sin
        // estar en un árbol vivo, Template viene en nulo.
        private static void VolcarResto(Panel caja, Window ventana)
        {
            Control[] controles =
            {
                new Button(),
                new CheckBox(),
                new RadioButton(),
                new TextBox(),
                new PasswordBox(),
                new ComboBox(),
                new Slider { Orientation = Orientation.Horizontal, Width = 120 },
                new Slider { Orientation = Orientation.Vertical, Height = 120 },
                new ProgressBar { Width = 120 },
                new TabControl(),
                new TabItem(),
                new TreeView(),
                new TreeViewItem(),
                new GroupBox(),
                new Expander(),
                new Menu(),
                new MenuItem(),
                new ContextMenu(),
                new ToolBar(),
                new StatusBar(),
                new ListBox(),
                new ListBoxItem(),
                new RepeatButton(),
                new ToggleButton(),
                new Thumb(),
                new ScrollBar { Orientation = Orientation.Horizontal, Width = 120 },
            };

            foreach (Control control in controles)
            {
                // El menú contextual y el de la barra no van dentro del panel:
                // viven en su propia ventana emergente.
                if (control is ContextMenu) { continue; }
                caja.Children.Add(control);
            }

            ventana.UpdateLayout();

            int i = 0;
            foreach (Control control in controles)
            {
                string nombre = control.GetType().Name;

                // Los dos deslizadores y las dos barras se distinguen por sentido.
                if (control is Slider desliza) { nombre += "." + desliza.Orientation; }
                if (control is ScrollBar barra2 && barra2.Orientation == Orientation.Horizontal)
                {
                    nombre += ".Horizontal";
                }

                Guardar(nombre + ".plantilla.xaml", control.Template);
                i++;
            }

            Console.WriteLine(i + " plantillas más volcadas.");
        }

        private static void Guardar(string nombre, object valor)
        {
            string destino = Path.Combine(carpeta, nombre);
            if (valor == null)
            {
                File.WriteAllText(destino + ".NULO.txt", "No se resolvió el recurso.");
                return;
            }
            try
            {
                StringBuilder texto = new StringBuilder();
                XmlWriterSettings ajustes = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineOnAttributes = false,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter escritor = XmlWriter.Create(texto, ajustes))
                {
                    XamlWriter.Save(valor, escritor);
                }
                File.WriteAllText(destino, texto.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                File.WriteAllText(destino + ".ERROR.txt", ex.ToString());
            }
        }
    }
}
