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

            ventana.Close();
            Application.Current.Shutdown();
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
