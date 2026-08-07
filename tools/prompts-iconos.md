# Textos con los que se generaron los iconos

Estos son los textos que produjeron las láminas de
`src/MacOS9.Platinum.Gallery/Recursos/IconosPixel`. Se conservan para que la
procedencia del material sea reproducible.

Van en inglés porque el generador obedece mejor en ese idioma, y de uno en uno
porque pedir diez imágenes en un solo texto hacía que la herramienta lo leyera
como una orden de edición sobre material existente.

**Al generar la segunda lámina y las siguientes, adjunta la primera como
referencia de estilo.** Es lo que mantiene parejas las diez.

## Bloque de estilo

Va idéntico al principio de cada lámina.

```
A single square image at maximum resolution. Flat solid magenta (#FF00FF)
background, no gradient or texture. Exactly 12 icons in a strict 4-column by
3-row grid, each centered in its own invisible cell with equal padding, none
touching each other or the border. No text, labels, numbers or watermarks.

Style: clean geometric icons with a quiet retro desktop character, drawn to a
modern standard of clarity — a contemporary designer paying homage to classic
desktop software, not software from 1998. All twelve built on the same grid with
equal optical weight. Simple confident geometry, small consistent corner radius.
Restrained dimensionality: a shallow lit top face where it helps, never a full
isometric box with three visible faces; flat subjects stay front-on. Every
silhouette closed by an even outline in desaturated near-black #2A2A33, never
pure black. Shading in 2 or 3 flat bands, crisp poster-like steps, no gradients,
glow, gloss or transparency. Light from the upper left: a lighter band on top and
left edges, darker on bottom and right. One small flat gray shadow under each
object, offset slightly down and right. Palette limited to neutral grays plus
muted periwinkle blue (#8B8BC3, #B6B6DC, #5B5B92); amber #E0A020 and brick red
#C42B21 only where meaning demands. Bold uncluttered silhouettes that stay
readable at 16 pixels — no hairlines, no small lettering, no texture. Not
skeuomorphic, not glossy, not 3D render, not line art, not material design, not
long shadows, not app tiles.

The 12 icons, in reading order left to right, top to bottom: [LISTA]
```

## Las listas

**Archivos** — closed folder, open folder, blank document, text document with
lines, spreadsheet, image file, locked folder, compressed archive, document
template, shortcut with a small curved arrow, empty wastebasket, full wastebasket

**Edición** — scissors, two offset sheets for copy, clipboard holding a sheet,
curved arrow pointing left for undo, curved arrow pointing right for redo,
duplicated sheets, magnifying glass, magnifying glass with a small pencil,
ascending sort arrows, funnel filter, dashed selection rectangle, eraser

**Comandos de archivo** — blank sheet for new, folder with an arrow coming out,
floppy disk for save, floppy disk with a pencil, printer, box with an arrow
leaving it, box with an arrow entering it, paper clip, revert arrow over a sheet,
window with a close mark, two circular arrows for refresh, sheet with a preview
eye

**Estado y avisos** — amber warning triangle with an exclamation mark, red circle
with a white cross, blue circle with a white letter i, circle with a question
mark, green check mark, prohibition sign, hourglass, closed padlock, open
padlock, five-pointed star, bookmark ribbon, small pennant flag

**Navegación y vista** — left arrow, right arrow, up arrow, down arrow, house,
magnifying glass with a plus, magnifying glass with a minus, rectangle with four
corner arrows, list view lines, grid of small squares, three vertical columns,
expand to full screen

**Dispositivos** — external hard drive, solid state drive, floppy disk, optical
disc, rack server, network of connected nodes, desktop computer with monitor,
printer, scanner, camera, keyboard, mouse

**Comunicación** — sealed envelope, envelope with a down arrow, envelope with an
up arrow, mailbox, address book, telephone handset, speech bubble, wall calendar,
analog clock, notification bell, single person silhouette bust, three person
silhouettes

**Herramientas** — gear wheel, adjustable wrench, screwdriver, hammer, ruler,
paint palette, paintbrush, eyedropper, gear with a small magnifier, three
horizontal sliders, toggle switch, hex nut

**Datos** — bar chart, pie chart, line chart with points, data table grid,
stacked database cylinders, calculator, balance scale, thermometer, round dial
gauge, stopwatch, tag label, barcode

**Multimedia** — play triangle, pause bars, stop square, record circle,
fast-forward double triangle, rewind double triangle, loudspeaker with sound
waves, microphone, framed photograph, film strip, musical note, sun for
brightness

## Lo que hay que corregir en la siguiente vuelta

**Los signos de reproducción salieron como teclas.** Play, pausa, detener,
grabar, adelantar y expulsar vinieron dibujados como cuadros grises con bisel, y
metidos dentro de una tecla de verdad quedan tecla dentro de tecla.

La causa es que el bloque de estilo pide dimensionalidad para todo. Para las
láminas de signos —edición, estado, navegación, y los controles de
reproducción— hay que sustituir el párrafo de dimensionalidad por:

```
Flat, front-facing symbols with no perspective and no volume. Each icon is a
single solid shape or a small group of shapes seen straight on, filled with one
flat color plus at most one darker band for weight. No top faces, no side faces,
no three-quarter view, no beveled key or button shape around the symbol — the
symbol itself is the icon. Keep the same outline weight, palette and light
direction as the rest of the set so the two families sit together.
```

La regla de fondo: **volumen para lo que podrías levantar con la mano —disco,
impresora, cámara, carpeta— y plano para lo que no existe físicamente —
reproducir, cortar, buscar, una flecha—.** Es lo que hacía Mac OS 9. Darle
volumen a un signo le quita legibilidad y no aporta nada, porque no hay objeto
real que recordar.

La última frase del párrafo es la que evita que el set se parta en dos: mismo
contorno, misma paleta, misma luz. Lo único que cambia es si hay volumen.
