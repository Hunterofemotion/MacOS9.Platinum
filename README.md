# MacOS9.Platinum

A WPF control library with the look of the Mac OS 9 Platinum interface.

![Control gallery](docs/galeria.png)

*An actual screenshot of the gallery application, not a mock-up.*

There is also a browsable catalogue in [docs/componentes.html](docs/componentes.html),
with every control broken down by state and the file it lives in. That one *is* a
CSS re-creation: treat it as an inventory, not as proof of how things render.

## Layout

| Project | What it is |
|---|---|
| `src/MacOS9.Platinum` | The library: resource dictionaries and custom controls |
| `src/MacOS9.Platinum.Gallery` | A visual catalogue for reviewing each control while developing |

## Usage

The consuming application merges a single dictionary:

```xml
<Application.Resources>
    <ResourceDictionary Source="/MacOS9.Platinum;component/Themes/Platinum.xaml" />
</Application.Resources>
```

That is enough for every `Button`, `CheckBox`, `RadioButton`, `TextBox`, `ComboBox`
and `ScrollBar` to take on the Platinum look without touching existing markup.

For the window chrome, use the `PlatinumWindow` control instead of `Window`:

```xml
<platinum:PlatinumWindow
    xmlns:platinum="clr-namespace:MacOS9.Platinum.Controls;assembly=MacOS9.Platinum">
```

### One integration requirement

WPF ships two engines for painting text selection. The older one draws it as an
adornment on top of the text, so an opaque highlight hides the glyphs. Turn it off
during application start-up, before the first window is shown:

```csharp
AppContext.SetSwitch(
    "Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering",
    false);
```

This cannot be handled from a `ResourceDictionary`: it is a process-wide switch.

## Controls

- **PlatinumWindow** — pinstriped title bar, close/zoom/collapse box, grow box,
  three-layer frame, windowshade, maximise bounded to the work area
- **Button** — normal, default (with its ring), pressed, disabled
- **CheckBox** and **RadioButton** — checked, unchecked, indeterminate, disabled
- **TextBox** — editable, read-only, disabled, with opaque selection
- **ComboBox** — popup menu with arrows, and drop-down menu
- **ScrollBar** — vertical and horizontal, arrows grouped at the end of the track,
  channel with a checkerboard texture snapped to physical pixels, disabled state.
  The frame is bound to `BorderThickness`, so a host that already closes an edge
  can switch off that side and avoid two rules stacking into one thick line
- **TabControl** — trapezoidal tabs, the active one fused with the panel
- **ListView** — list with columns: embossed headers, row dividers anchored to the
  bottom of each row so the last one is closed too, full-row selection, and a
  dedicated template for the GridView `ScrollViewer`
- **Menu** and **ContextMenu** — menu bar with submenus, checkable items, keyboard
  gestures and separators; inverted blue highlight, the only one in the theme
- **Slider** — horizontal and vertical, with tick marks; the thumb pentagon is
  rasterised to physical pixels
- **GroupBox** and **Separator** — engraved two-tone line with the title breaking
  it; separator aliases for `ToolBar` and `StatusBar`
- **TreeView** — tree with disclosure triangles and Finder indentation
- **ProgressBar** — determinate, indeterminate (diagonal stripes), vertical and
  disabled
- **ToolTip** — the yellow help note from Mac OS 9
- **Icons** — ten 16×16 vector icons (folder, document, floppy, disk, trash, alert,
  info, envelope, computer, magnifier) in `Themes/Icons.xaml`

### Details that layout alone cannot solve

Several pieces are drawn by measuring the physical pixel instead of leaving them to
the layout system, because WPF rounds each edge independently and on a scaled
display the result comes out asymmetric or blurry:

| Piece | File |
|---|---|
| Title bar pinstripe | `Controls/Pinstripe.cs` |
| Default button ring | `Controls/DefaultRing.cs` |
| Arrow and disclosure triangles | `Controls/ArrowGlyph.cs` |
| Scrollbar channel checkerboard | `Controls/CheckerTexture.cs` |
| Slider thumb pentagon | `Controls/SliderThumbShape.cs` |

Tabs (`Controls/TabShape.cs`) are the deliberate exception: their diagonals and
curves need anti-aliasing, so they are drawn as vectors and only the straight runs
are snapped to the grid, the same way `Border` does for buttons.

The theme also sets `TextOptions.TextFormattingMode="Display"`, because WPF's
`Ideal` mode positions stems on fractional pixels, and at 11 px a one-pixel stroke
gets spread across three columns.

## Typography

Platinum uses Charcoal for chrome and Geneva for content. Neither exists on Windows,
so the theme substitutes Tahoma for Charcoal and Franklin Gothic Medium for Geneva.
The split is a rule of the theme: every control that displays user data uses
`ViewFontFamily`, and everything that is chrome uses `SystemFontFamily`.

## Building

```
dotnet build MacOS9.Platinum.slnx
dotnet run --project src/MacOS9.Platinum.Gallery
```

Requires .NET 9 or later with the Windows desktop workload.

## Verification

Two tools live in [tools/](tools/), outside the solution because they are not part
of the library:

```
pwsh -File tools/check-resources.ps1
dotnet run --project tools/Probe
```

`check-resources.ps1` checks that every `{StaticResource}` resolves against the keys
its own dictionary can reach, that no `TargetName` points at a missing `x:Name`, and
that no key is defined twice. None of that fails at compile time: a `StaticResource`
inside a `ControlTemplate` is resolved when the template is instantiated, so a
misspelled key in the submenu branch brings the application down the first time
somebody opens that submenu, not at start-up.

`Probe` instantiates every template and style in the theme — which forces WPF to
resolve them — and renders each control with `RenderTargetBitmap` into
`tools/Probe/bin/.../render`. It opens no window and never touches the desktop, so
it is useful for states a screenshot cannot reach: disabled, overflowing content,
a rolled-up window, extreme widths. Switching `dpiAware` to `false` in
`app.manifest` and rebuilding produces the 100% case.

### Measuring instead of eyeballing

Visual defects in this library are settled by sampling pixels, not by opinion. The
recurring cause is two elements painting the same place: if an edge looks twice as
thick, look for the second owner rather than adjusting the neighbour. Capture the
window, profile the row or column across the defect, name the colours, fix the
element that paints too much, then profile the same cut again.

When a defect is structural, dumping WPF's own stock template answers "how does the
framework solve this by default?" in seconds. That is how the doubled scrollbar edge
was traced: the stock `GridView` scroll viewer draws no frame around the scrollbar
at all, because the frame belongs to the list and there is exactly one owner.

## Licence

MIT. You may clone, modify, use and redistribute the library, including in
commercial projects; the only requirement is that you keep the copyright notice.
The full text is in [LICENSE](LICENSE).
