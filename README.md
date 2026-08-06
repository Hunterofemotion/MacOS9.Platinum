# MacOS9.Platinum

Biblioteca de controles WPF con el aspecto de la interfaz Platinum de Mac OS 9.

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
- **ScrollBar** — vertical y horizontal, con thumb arrastrable y estado deshabilitado

Hay un catálogo visual en [docs/componentes.html](docs/componentes.html).

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
