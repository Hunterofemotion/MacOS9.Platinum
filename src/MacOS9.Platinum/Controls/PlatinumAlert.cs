using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MacOS9.Platinum.Controls;

/// <summary>Qué clase de aviso es, que decide el icono.</summary>
public enum AlertKind
{
    /// <summary>Nota: informa, no hay decisión que tomar.</summary>
    Note,
    /// <summary>Precaución: hay algo que ponderar antes de seguir.</summary>
    Caution,
    /// <summary>Alto: la acción no se pudo hacer.</summary>
    Stop
}

/// <summary>Juego de teclas del aviso.</summary>
public enum AlertButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel
}

/// <summary>Qué tecla cerró el aviso.</summary>
public enum AlertResult
{
    None,
    Ok,
    Cancel,
    Yes,
    No
}

/// <summary>
/// Aviso modal de Platinum: icono a la izquierda, mensaje a la derecha y teclas al
/// pie con el anillo en la de omisión. Es la ventana que toda aplicación necesita
/// y que, si no vive aquí, cada consumidor termina rehaciendo a su manera.
///
/// El contenido se arma en código y no como plantilla en el tema porque las teclas
/// cambian según el juego elegido: una plantilla fija tendría que declararlas
/// todas y esconder las que sobran.
///
/// Los rótulos se pueden cambiar: la biblioteca no impone idioma. Vienen en inglés
/// y el consumidor los traduce.
/// </summary>
public class PlatinumAlert : PlatinumWindow
{
    public PlatinumAlert()
    {
        // Un aviso no se redimensiona ni se enrolla, y su alto lo manda el texto:
        // un mensaje largo no debe quedar recortado ni dejar hueco abajo.
        ResizeMode = ResizeMode.NoResize;
        ShowGrowBox = false;
        SizeToContent = SizeToContent.Height;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Width = 396;
    }

    /// <summary>Lo que pasó, en una frase.</summary>
    public string Message { get; set; } = "";

    /// <summary>Segunda línea, más chica: el detalle o qué hacer al respecto.</summary>
    public string Detail { get; set; } = "";

    public AlertKind Kind { get; set; } = AlertKind.Note;

    /// <summary>
    /// Icono propio, en lugar del que corresponde al <see cref="Kind"/>.
    /// </summary>
    /// <remarks>
    /// Un aviso que habla de un objeto concreto —un disco, una impresora, un
    /// archivo— se entiende antes con el icono de ese objeto que con el triángulo
    /// genérico. Dejándolo sin asignar manda el Kind, que es lo correcto para un
    /// aviso de sistema.
    /// </remarks>
    public ImageSource? Icon { get; set; }

    public AlertButtons Buttons { get; set; } = AlertButtons.Ok;

    public string OkText { get; set; } = "OK";
    public string CancelText { get; set; } = "Cancel";
    public string YesText { get; set; } = "Yes";
    public string NoText { get; set; } = "No";

    /// <summary>Qué tecla lo cerró.</summary>
    public AlertResult Result { get; private set; } = AlertResult.None;

    /// <summary>
    /// Arma el contenido. Hay que llamarlo antes de mostrar la ventana, y por eso
    /// es público: quien construya el aviso a mano en vez de usar Show tiene que
    /// hacerlo.
    ///
    /// No se arma en el constructor porque las propiedades se asignan después, con
    /// un inicializador de objeto. Tampoco en OnSourceInitialized: ahí la ventana ya
    /// decidió su tamaño y aparecía como una tira vacía de 150 unidades, que es el
    /// ancho mínimo del marco.
    /// </summary>
    public void Preparar()
    {
        Content = Armar();
    }

    private UIElement Armar()
    {
        var raiz = new Grid { Margin = new Thickness(18, 16, 18, 16) };
        raiz.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        raiz.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        raiz.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        raiz.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        raiz.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Arriba y no centrado con el texto: con dos líneas de mensaje, un icono
        // centrado se descuelga y el bloque pierde su línea de arranque.
        var icono = new Image
        {
            Width = 32,
            Height = 32,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 0, 14, 0),
            Source = Icon ?? Recurso(Kind == AlertKind.Note ? "IconInfo" : "IconAlert") as ImageSource
        };
        Grid.SetRow(icono, 0);
        Grid.SetColumn(icono, 0);
        Grid.SetRowSpan(icono, 2);
        raiz.Children.Add(icono);

        var mensaje = new TextBlock
        {
            Text = Message,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = Recurso("SystemFontFamily") as FontFamily,
            FontWeight = Recurso("SystemFontWeight") is FontWeight peso ? peso : FontWeights.Normal,
            FontSize = Recurso("SystemFontSize") is double tam ? tam : 12d,
            Foreground = Recurso("TextBrush") as Brush
        };
        Grid.SetRow(mensaje, 0);
        Grid.SetColumn(mensaje, 1);
        raiz.Children.Add(mensaje);

        // El detalle solo ocupa lugar cuando lo hay: un renglón vacío permanente
        // separaría el mensaje de las teclas sin motivo.
        //
        // Va del mismo tamaño que el mensaje y se distingue por el peso, no por la
        // medida. En un aviso la segunda línea no es un pie de página: es la que
        // dice qué va a pasar si el usuario sigue adelante, y esta es la ventana
        // que se lee con prisa. Achicarla era esconder justo la consecuencia.
        if (!string.IsNullOrEmpty(Detail))
        {
            var detalle = new TextBlock
            {
                Text = Detail,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 0),
                FontFamily = Recurso("SystemFontFamily") as FontFamily,
                FontWeight = FontWeights.Normal,
                FontSize = Recurso("SystemFontSize") is double medida ? medida : 12d,
                Foreground = Recurso("TextBrush") as Brush
            };
            Grid.SetRow(detalle, 1);
            Grid.SetColumn(detalle, 1);
            raiz.Children.Add(detalle);
        }

        var teclas = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 18, 0, 0)
        };
        Grid.SetRow(teclas, 2);
        Grid.SetColumn(teclas, 1);
        raiz.Children.Add(teclas);

        // El orden es el de Mac OS: la acción que sigue adelante va a la derecha y
        // es la de omisión; la que se echa atrás queda a su izquierda.
        switch (Buttons)
        {
            case AlertButtons.OkCancel:
                teclas.Children.Add(Tecla(CancelText, AlertResult.Cancel, false, true));
                teclas.Children.Add(Tecla(OkText, AlertResult.Ok, true, false));
                break;

            case AlertButtons.YesNo:
                teclas.Children.Add(Tecla(NoText, AlertResult.No, false, true));
                teclas.Children.Add(Tecla(YesText, AlertResult.Yes, true, false));
                break;

            case AlertButtons.YesNoCancel:
                teclas.Children.Add(Tecla(CancelText, AlertResult.Cancel, false, true));
                teclas.Children.Add(Tecla(NoText, AlertResult.No, false, false));
                teclas.Children.Add(Tecla(YesText, AlertResult.Yes, true, false));
                break;

            default:
                teclas.Children.Add(Tecla(OkText, AlertResult.Ok, true, true));
                break;
        }

        return raiz;
    }

    private Button Tecla(string texto, AlertResult resultado, bool porOmision, bool esCancelar)
    {
        var boton = new Button
        {
            Content = texto,
            MinWidth = 90,
            IsDefault = porOmision,
            // Escape cierra por la tecla que se echa atrás; con una sola tecla es
            // esa misma. Un aviso siempre debe poder cerrarse con el teclado.
            IsCancel = esCancelar,
            Margin = new Thickness(12, 0, 0, 0)
        };

        boton.Click += (_, _) =>
        {
            Result = resultado;
            DialogResult = resultado is AlertResult.Ok or AlertResult.Yes;
        };

        return boton;
    }

    // El recurso puede no estar si el consumidor no fusionó el tema. Devolver nulo
    // deja el aviso feo pero abierto, que es mejor que tumbar la aplicación cuando
    // justo está intentando dar un mensaje.
    private object? Recurso(string clave) =>
        TryFindResource(clave) ?? Application.Current?.TryFindResource(clave);

    /// <summary>Muestra el aviso y devuelve qué tecla lo cerró.</summary>
    public static AlertResult Show(
        Window owner,
        string title,
        string message,
        string detail = "",
        AlertKind kind = AlertKind.Note,
        AlertButtons buttons = AlertButtons.Ok)
    {
        var aviso = new PlatinumAlert
        {
            Owner = owner,
            Title = title,
            Message = message,
            Detail = detail,
            Kind = kind,
            Buttons = buttons
        };

        aviso.Preparar();
        aviso.ShowDialog();
        return aviso.Result;
    }
}
