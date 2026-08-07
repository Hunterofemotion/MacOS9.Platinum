# Procedencia de los iconos

Estas piezas se generaron con el modelo de imagen de ChatGPT a partir de los
textos que están en [`tools/prompts-iconos.md`](../../../../tools/prompts-iconos.md),
y se recortaron de las láminas con
[`tools/recortar-iconos.ps1`](../../../../tools/recortar-iconos.ps1).

Los textos se conservan versionados para que la procedencia sea reproducible y no
una afirmación: cualquiera puede volver a generar el material y comparar.

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
