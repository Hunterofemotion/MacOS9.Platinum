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
  con textura de tablero a píxel físico y estado deshabilitado
- **TabControl** — pestañas trapezoidales con la activa fundida con el panel
- **ListView** — lista con columnas: encabezados con relieve, divisores, selección
  de fila completa y plantilla propia del ScrollViewer de GridView
- **Menu** y **ContextMenu** — barra de menús con submenús, elementos marcables,
  gestos de teclado y separadores; resalte invertido azul, el único del tema
- **Slider** — horizontal y vertical, con marcas; el pentágono del cursor se
  rasteriza a píxel físico
- **GroupBox** y **Separator** — línea grabada de dos tonos con el título
  interrumpiéndola; alias de separador para ToolBar y StatusBar
- **TreeView** — árbol con triángulos de despliegue e indentación del Finder
- **ProgressBar** — determinada, indeterminada (franjas diagonales), vertical y
  deshabilitada
- **ToolTip** — la nota amarilla de la ayuda de Mac OS 9
- **Iconos** — diez íconos vectoriales de 16×16 (carpeta, documento, disquete,
  disco, papelera, alerta, info, sobre, computadora, lupa) en `Themes/Icons.xaml`

### Detalles que no se resuelven con layout

Varias piezas se dibujan midiendo el píxel físico de la pantalla en vez de dejarlas
al sistema de layout, porque WPF redondea cada borde por separado y con la pantalla
escalada el resultado sale asimétrico o borroso:

| Pieza | Archivo |
|---|---|
| Rayado de la barra de título | `Controls/Pinstripe.cs` |
| Contorno del botón por omisión | `Controls/DefaultRing.cs` |
| Triángulos de flechas y despliegues | `Controls/ArrowGlyph.cs` |
| Tablero del canal de las barras | `Controls/CheckerTexture.cs` |
| Pentágono del cursor del deslizador | `Controls/SliderThumbShape.cs` |

La excepción deliberada son las pestañas (`Controls/TabShape.cs`): sus diagonales y
curvas necesitan suavizado, así que se dibujan como vector y sólo se ajustan a la
retícula los tramos rectos, igual que hace `Border` con los botones.

El tema también fija `TextOptions.TextFormattingMode="Display"`, porque el modo
`Ideal` de WPF posiciona las astas en fracciones de píxel y a 11 px un trazo de uno
se reparte entre tres columnas.

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

## Verificación

Dos herramientas en [tools/](tools/), fuera de la solución porque no son parte de la
biblioteca:

```
pwsh -File tools/check-resources.ps1
dotnet run --project tools/Probe
```

`check-resources.ps1` comprueba que cada `{StaticResource}` se resuelva con las
llaves que su propio diccionario alcanza, que ningún `TargetName` apunte a un
`x:Name` inexistente y que ninguna llave se defina dos veces. Nada de eso falla al
compilar: un `StaticResource` dentro de un `ControlTemplate` se resuelve al
instanciar la plantilla, así que una llave mal escrita en la rama del submenú tira la
aplicación la primera vez que alguien abre ese submenú, no al arrancar.

`Probe` instancia todas las plantillas y estilos del tema —lo que obliga a WPF a
resolverlos— y renderiza cada control con `RenderTargetBitmap` a `tools/Probe/bin/.../render`.
No abre ninguna ventana ni toca el escritorio, así que sirve para revisar estados que
una captura de pantalla no alcanza: deshabilitados, contenido que desborda, ventana
enrollada, anchos extremos. Cambiar `dpiAware` a `false` en `app.manifest` y recompilar
produce el caso de 100 %.
