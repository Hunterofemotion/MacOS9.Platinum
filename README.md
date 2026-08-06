# MacOS9.Platinum

Biblioteca de controles WPF con el aspecto de la interfaz Platinum de Mac OS 9.

![Galería de controles](docs/galeria.png)

*Captura real de la aplicación de galería, no una recreación.*

Hay además un catálogo navegable en [docs/componentes.html](docs/componentes.html),
con cada control desglosado por estados y el archivo donde vive. Ese sí es una
recreación en CSS: sirve como inventario, no como prueba del render.

## Estructura

| Proyecto | Qué es |
|---|---|
| `src/MacOS9.Platinum` | La biblioteca: diccionarios de recursos y controles propios |
| `src/MacOS9.Platinum.Gallery` | Catálogo visual para revisar cada control durante el desarrollo |

## Uso

La aplicación consumidora fusiona un solo diccionario:

```xml
<Application.Resources>
    <ResourceDictionary Source="/MacOS9.Platinum;component/Themes/Platinum.xaml" />
</Application.Resources>
```

Con eso, todos los `Button`, `CheckBox`, `RadioButton`, `TextBox`, `ComboBox` y
`ScrollBar` adoptan el aspecto Platinum sin tocar el marcado existente.

Para el chrome de ventana se usa el control `PlatinumWindow` en lugar de `Window`:

```xml
<platinum:PlatinumWindow
    xmlns:platinum="clr-namespace:MacOS9.Platinum.Controls;assembly=MacOS9.Platinum">
```

### Requisito de integración

WPF trae dos motores para dibujar la selección de texto. El viejo la pinta como un
adorno encima del texto, así que un resalte opaco lo tapa. Hay que apagarlo en el
arranque de la aplicación, antes de mostrar la primera ventana:

```csharp
AppContext.SetSwitch(
    "Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering",
    false);
```

No puede resolverse desde un `ResourceDictionary`: es un ajuste de proceso.

## Controles

- **PlatinumWindow** — barra de título rayada, close/zoom/collapse box, grow box,
  marco de tres capas, windowshade, maximizado acotado al área de trabajo
- **Button** — normal, por omisión (con contorno), presionado, deshabilitado
- **CheckBox** y **RadioButton** — marcado, vacío, indeterminado, deshabilitado
- **TextBox** — editable, solo lectura, deshabilitado, con selección opaca
- **ComboBox** — popup menu con flechas y menú desplegable
- **ScrollBar** — vertical y horizontal, flechas agrupadas al final del riel, canal
  con textura y estado deshabilitado
- **TabControl** — pestañas trapezoidales con la activa fundida con el panel
- **ListView** — lista con columnas: encabezados con relieve, divisores y selección
  de fila completa

### Detalles que no se resuelven con layout

Tres piezas se dibujan midiendo el píxel físico de la pantalla en vez de dejarlas al
sistema de layout, porque WPF redondea cada borde por separado y con la pantalla
escalada el resultado sale asimétrico o borroso:

| Pieza | Archivo |
|---|---|
| Rayado de la barra de título | `Controls/Pinstripe.cs` |
| Contorno del botón por omisión | `Controls/DefaultRing.cs` |

## Tipografía

Platinum usa Charcoal para el chrome y Geneva para el contenido. Ninguna existe en
Windows, así que el tema sustituye Charcoal por Tahoma y Geneva por Franklin Gothic
Medium. El reparto es una regla del tema: todo control que muestre datos del usuario
usa `ViewFontFamily`, y todo lo que sea chrome usa `SystemFontFamily`.

## Compilar

```
dotnet build MacOS9.Platinum.slnx
dotnet run --project src/MacOS9.Platinum.Gallery
```

Requiere .NET 9 o superior con la carga de escritorio de Windows.
