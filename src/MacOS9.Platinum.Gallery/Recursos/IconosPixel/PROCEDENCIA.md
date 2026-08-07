# Procedencia de los iconos

Estas piezas se generaron con el modelo de imagen de ChatGPT y se recortaron de
las láminas con [`tools/recortar-iconos.ps1`](../../../../tools/recortar-iconos.ps1).

Los textos con que se pidieron no se conservan. Hubo un archivo que decía
guardarlos, pero no eran los que de verdad se usaron, y un registro de
procedencia que no coincide con lo ocurrido es peor que no tenerlo: da por
comprobable algo que nadie puede comprobar. Se quitó en vez de corregirlo porque
los originales no se recuperaron.

De modo que el material no es reproducible a partir de este repositorio. Lo que
sí queda documentado es lo verificable: de dónde salió, con qué se recortó, qué
se revisó y qué se descartó.

## Revisión contra obra de terceros

Los términos de OpenAI ceden los derechos de la salida a quien la genera, así que
estas piezas pueden llevar la licencia MIT del repositorio. Lo que esos términos
**no** cubren es material ajeno que el modelo haya reproducido, así que cada
lámina se revisó pieza por pieza antes de aceptarla.

Lo que se descartó y por qué:

| Descartado | Motivo |
|---|---|
| Lámina de sistema, varias piezas | Traían la cara del Finder de Apple en el monitor, el disco, la carpeta y el maletín |
| Lámina de piezas de interfaz, completa | Una barra de menús con el logotipo de Apple. El resto eran retratos de ventanas, deslizadores y barras de desplazamiento, controles que esta biblioteca dibuja de verdad |
| Láminas de dispositivos y medios, completas | Quedaban cubiertas por las nueve que sí entraron, y el recortador les pegó cuatro pares de piezas cuyas sombras se tocaban |

Al añadir una lámina nueva, la revisión se repite. Una pieza con marca ajena se
descarta aunque el resto de su lámina sirva.

## Segunda entrega: treinta láminas

Se revisaron treinta láminas más —686 piezas— y entraron 329. La revisión pieza
por pieza está en `Downloads/Iconos/revision/CORTE.md`, con la referencia de cada
una y el motivo de su salida; aquí queda el resumen de lo descartado.

**Ninguna pieza salió por marca ajena.** Es la primera vez que ocurre, y vale la
pena anotar por qué: estas treinta láminas son de temas —laboratorio, banca,
vigilancia, forense— donde el modelo no tiene una interfaz famosa que copiar. Las
láminas problemáticas de la primera entrega eran justamente las de sistema
operativo.

Los 357 descartes se reparten en cuatro motivos:

| Motivo | Piezas | Qué significa |
|---|---|---|
| Repetida | 242 | El catálogo ya responde a esa búsqueda. No importa que el dibujo nuevo sea mejor: dos piezas para la misma palabra parten los resultados |
| No aguanta 16 px | 79 | Comprobado ampliando el recorte de 16, no supuesto. Siluetas de trazo fino —ADN, neurona, cromosoma, calibrador, proteína— que a ese tamaño dejan de significar algo |
| Letrero | 36 | La pieza solo se entiende si se lee la palabra impresa encima (`PAID`, `PAY STUB`, `W-4`). A 16 px el texto es una mancha, a 32 es ruido, y va en inglés cuando el catálogo se busca en dos idiomas |

Dos láminas completas quedaron fuera, las dos por el mismo motivo: eran la lámina
de carpetas, documentos, impresora, calendario y papelera otra vez, con un átomo
o una gráfica encima. El emblema no cambia la palabra por la que alguien busca.

Y cinco nombres chocaban con piezas que ya existían. Tres se renombraron porque
el dibujo sí era distinto —`LabMouse` contra el ratón de computadora, `DryCell`
contra la batería de aparato, `Walkie` contra el receptor de radio— y dos se
descartaron porque ahí no chocaba solo el nombre: la caja fuerte y el portafolios
ya estaban.

## Cómo se leen los nombres

`catalogo.tsv` liga cada recorte con su nombre en inglés y con las palabras por
las que se puede encontrar, en español e inglés:

```
base-r3c2	Printer	impresora printer imprimir papel
```

Las palabras van **de lo más propio a lo más lejano**. No es cosmética: el
catálogo las pesa por posición, así que buscar "impresora" trae primero la
impresora y después el fax, que la menciona de pasada.

El nombre del archivo guarda de dónde salió la pieza —lámina, renglón y
columna—, para poder volver a la lámina original si hay que recortarla de nuevo.

## Tamaños

De cada pieza hay tres archivos: `-128`, `-32` y `-16`. Los tres salen del mismo
recorte grande por reducción.

Eso es una limitación conocida, no un descuido: en el original cada tamaño era su
propio dibujo, con el detalle que aguanta a esa escala. Reducir el de 128 al de
16 deja manchas en las piezas de silueta muy horizontal —el escáner y el
enrutador son los casos claros—. Esas necesitan retoque a mano sobre la forma que
el generador propuso.
