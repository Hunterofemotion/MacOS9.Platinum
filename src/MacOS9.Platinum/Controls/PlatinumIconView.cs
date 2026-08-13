using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Entrada de la vista de iconos: el dibujo con su rótulo debajo, como un archivo
/// en el Finder.
/// </summary>
/// <remarks>
/// El icono y el texto van como propiedades y no como contenido armado a mano por
/// la misma razón que en el riel: la cuadrícula entera tiene que salir alineada.
/// Deriva de <see cref="ListBoxItem"/> para no reinventar la selección ni el
/// recorrido con el teclado.
/// </remarks>
public class PlatinumIconViewItem : ListBoxItem
{
    static PlatinumIconViewItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumIconViewItem), new FrameworkPropertyMetadata(typeof(PlatinumIconViewItem)));
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(ImageSource),
            typeof(PlatinumIconViewItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(PlatinumIconViewItem),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged));

    // El rótulo vive en Text y no en Content, así que sin esto la automatización
    // vería la entrada sin nombre, igual que pasaba en el riel.
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Windows.Automation.AutomationProperties.SetName(d, (string)e.NewValue ?? string.Empty);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(PlatinumIconViewItem),
            new FrameworkPropertyMetadata(32d, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}

/// <summary>
/// Vista de iconos al estilo del Finder: cuadrícula de iconos con rótulo, en un
/// pozo blanco, con selección múltiple y marquesina punteada al arrastrar sobre
/// el fondo.
/// </summary>
/// <remarks>
/// La marquesina no existe en ListBox y se implementa aquí: al presionar sobre un
/// hueco se ancla el origen, al arrastrar se dibuja el rectángulo punteado y se
/// selecciona lo que toque. Con Ctrl o Shift lo tocado se suma a lo que ya estaba
/// elegido, como en el original; sin modificador, el clic en el fondo también
/// deselecciona, que es la otra mitad del gesto del Finder.
/// </remarks>
[TemplatePart(Name = PartMarqueeHost, Type = typeof(Canvas))]
[TemplatePart(Name = PartMarquee, Type = typeof(Rectangle))]
public class PlatinumIconView : ListBox
{
    public const string PartMarqueeHost = "PART_MarqueeHost";
    public const string PartMarquee = "PART_Marquee";

    private Canvas? marqueeHost;
    private Rectangle? marquee;
    private ScrollContentPresenter? viewport;
    private Point origin;
    private bool tracking;
    private bool marqueeShown;
    private PlatinumIconViewItem[] keepSelected = [];

    static PlatinumIconView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumIconView), new FrameworkPropertyMetadata(typeof(PlatinumIconView)));

        // El Finder elige varios por omisión; Single es la excepción, no la regla.
        SelectionModeProperty.OverrideMetadata(
            typeof(PlatinumIconView), new FrameworkPropertyMetadata(SelectionMode.Extended));
    }

    protected override DependencyObject GetContainerForItemOverride() => new PlatinumIconViewItem();

    protected override bool IsItemItsOwnContainerOverride(object item) => item is PlatinumIconViewItem;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        marqueeHost = GetTemplateChild(PartMarqueeHost) as Canvas;
        marquee = GetTemplateChild(PartMarquee) as Rectangle;
        viewport = null;
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);

        if (marqueeHost is null || marquee is null || e.OriginalSource is not DependencyObject source)
        {
            return;
        }

        // Solo el fondo arma la marquesina: un clic sobre un icono es selección
        // normal de la lista, y uno sobre la barra de desplazamiento es de la barra.
        if (!IsBackground(source))
        {
            return;
        }

        origin = e.GetPosition(marqueeHost);
        tracking = true;
        marqueeShown = false;

        // Lo conservado con Ctrl/Shift se recuerda por CONTENEDOR y no por valor:
        // dos elementos que comparan iguales (dos «Respaldo») encendían los dos
        // aunque solo uno estuviera elegido.
        bool additive = (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) != 0;
        keepSelected = additive ? [.. SelectedContainers()] : [];
        if (!additive)
        {
            UnselectAll();
        }

        // El clic al fondo de un ListBox no mueve el foco por sí solo (solo los
        // elementos lo toman): sin esto, la selección recién arrastrada salía
        // pintada con el gris de lista sin foco dentro de la ventana activa, y
        // las flechas seguían gobernando el control anterior.
        Focus();

        CaptureMouse();
        // Sin e.Handled: el resto del clic sigue su curso normal.
    }

    private IEnumerable<PlatinumIconViewItem> SelectedContainers()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is PlatinumIconViewItem container
                && container.IsSelected)
            {
                yield return container;
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!tracking || marqueeHost is null || marquee is null)
        {
            return;
        }

        Point position = e.GetPosition(marqueeHost);

        // Umbral de arranque: sin él, el temblor natural del clic pintaba un
        // rectángulo de un píxel en cada clic al fondo.
        if (!marqueeShown &&
            Math.Abs(position.X - origin.X) < 3 && Math.Abs(position.Y - origin.Y) < 3)
        {
            return;
        }

        marqueeShown = true;
        var rect = new Rect(origin, position);

        // Recortada al área de contenido: el lienzo cubre el visor completo, y sin
        // este recorte las hormigas se pintaban encima de la barra de
        // desplazamiento, cosa que el Finder no hacía.
        Rect clip = ViewportBounds();
        if (!clip.IsEmpty)
        {
            rect.Intersect(clip);
        }

        if (rect.IsEmpty)
        {
            marquee.Visibility = Visibility.Collapsed;
            return;
        }

        Canvas.SetLeft(marquee, rect.X);
        Canvas.SetTop(marquee, rect.Y);
        marquee.Width = rect.Width;
        marquee.Height = rect.Height;
        marquee.Visibility = Visibility.Visible;

        SelectIntersecting(rect);
    }

    private Rect ViewportBounds()
    {
        viewport ??= FindDescendant<ScrollContentPresenter>(this);
        if (viewport is null || marqueeHost is null)
        {
            return Rect.Empty;
        }

        try
        {
            return viewport
                .TransformToVisual(marqueeHost)
                .TransformBounds(new Rect(viewport.RenderSize));
        }
        catch (InvalidOperationException)
        {
            return Rect.Empty;
        }
    }

    private static T? FindDescendant<T>(DependencyObject root)
        where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, i);
            if (child is T found)
            {
                return found;
            }

            if (FindDescendant<T>(child) is T deeper)
            {
                return deeper;
            }
        }

        return null;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        EndMarquee();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        EndMarquee();
    }

    private void EndMarquee()
    {
        if (!tracking)
        {
            return;
        }

        tracking = false;
        keepSelected = [];
        if (marquee is not null)
        {
            marquee.Visibility = Visibility.Collapsed;
        }

        if (IsMouseCaptured)
        {
            ReleaseMouseCapture();
        }
    }

    private void SelectIntersecting(Rect rect)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is not PlatinumIconViewItem container)
            {
                continue;
            }

            bool hit;
            try
            {
                Rect bounds = container
                    .TransformToVisual(marqueeHost!)
                    .TransformBounds(new Rect(container.RenderSize));
                hit = rect.IntersectsWith(bounds);
            }
            catch (InvalidOperationException)
            {
                // El contenedor todavía no cuelga del árbol: pasa durante un
                // reciclado de plantilla. Sin geometría no hay intersección.
                hit = false;
            }

            container.IsSelected = hit || keepSelected.Contains(container);
        }
    }

    private bool IsBackground(DependencyObject source)
    {
        DependencyObject? node = source;
        while (node is not null && node != this)
        {
            if (node is PlatinumIconViewItem || node is ScrollBar)
            {
                return false;
            }

            // GetParent visual solo acepta visuales; el texto interno de un rótulo
            // (Run) es un ContentElement y se sube por el árbol lógico.
            node = node is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(node)
                : LogicalTreeHelper.GetParent(node);
        }

        return node == this;
    }
}
