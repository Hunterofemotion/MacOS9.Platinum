# Propuesta de corte

Una entrada por pieza, con la referencia que va escrita bajo cada icono en las
hojas `rev-*.png`: `lámina·renglón,columna`.

Tres motivos de salida, en este orden:

- **repetida** — el catálogo actual ya responde a esa búsqueda. No importa que el
  dibujo sea mejor: dos piezas para la misma palabra parten los resultados.
- **16 px** — comprobado ampliando el recorte de 16, no supuesto. La silueta se
  empasta y deja de significar algo.
- **marca ajena** — reproduce obra de terceros. Fuera sin discusión, como pide
  `PROCEDENCIA.md`.

Para marcar tu desacuerdo basta con anotar la referencia.

---

## Lámina 1 — laboratorio básico · prefijo `lab`

**Entran (10)**

| Ref | Nombre | Palabras |
|---|---|---|
| 01·1,5 | Microscope | microscopio microscope laboratorio aumento muestra |
| 01·2,1 | TestTubeRack | tubos ensayo gradilla test tube rack muestras |
| 01·2,2 | Beaker | vaso precipitados beaker recipiente líquido química |
| 01·2,3 | Flask | matraz erlenmeyer flask frasco química |
| 01·2,5 | PetriDish | placa petri dish cultivo caja muestra |
| 01·3,2 | Molecule | molécula molecule átomos enlace química estructura |
| 01·4,1 | ChartScatter | dispersión scatter nube puntos correlación gráfica |
| 01·4,2 | DataTable | tabla datos table hoja cálculo renglones columnas |
| 01·4,4 | Oscilloscope | osciloscopio oscilloscope onda señal pantalla |
| 01·5,3 | Thermometer | termómetro thermometer temperatura grados calor |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 01·1,1 | Carpeta con átomo | repetida — `Folder` |
| 01·1,2 | Documento con molécula | repetida — `DocumentText` |
| 01·1,3 | Disquete | repetida — `Floppy` |
| 01·1,4 | Carpeta abierta con átomo | repetida — `FolderOpen` |
| 01·2,4 | Pipeta | 16 px — trazo de un píxel, se pierde |
| 01·3,1 | Hélice de ADN | 16 px — queda una mancha |
| 01·3,3 | Átomo | 16 px — se lee como engrane |
| 01·3,4 | Gráfica de línea | repetida — `ChartLine` |
| 01·3,5 | Gráfica de barras | repetida — `ChartBar` |
| 01·4,3 | Calculadora | repetida — `Calculator` |
| 01·4,5 | Calibrador | 16 px — desaparece |
| 01·5,1 | Balanza analítica | 16 px — se empasta |
| 01·5,2 | Centrífuga | 16 px — se empasta |
| 01·5,4 | Engranes | repetida — `Gear` |
| 01·5,5 | Globo de información | repetida — `Info` |

## Lámina 2 — instrumentos de medición · prefijo `medir`

Tema sin ningún traslape con el catálogo: no hay un solo instrumento de medición
en las 180 actuales.

**Entran (15)**

| Ref | Nombre | Palabras |
|---|---|---|
| 02·1,1 | DataLogger | registrador datos logger medición portátil |
| 02·1,3 | PressureGauge | manómetro presión gauge carátula aguja |
| 02·1,4 | FlowMeter | flujómetro caudal flow medidor tubo |
| 02·1,5 | Voltmeter | voltímetro voltaje volts tensión medidor |
| 02·2,1 | Ammeter | amperímetro corriente amperes medidor |
| 02·2,2 | Multimeter | multímetro multimeter tester medidor eléctrico |
| 02·2,3 | SignalGenerator | generador señal osciloscopio banco instrumento |
| 02·3,1 | PhMeter | phmetro ph acidez medidor sonda |
| 02·3,2 | ConductivityMeter | conductímetro conductividad medidor sonda |
| 02·3,4 | BarcodeScanner | lector código barras scanner pistola |
| 02·3,5 | Stopwatch | cronómetro stopwatch tiempo medir vuelta |
| 02·4,1 | Compass | brújula compass norte orientación rumbo |
| 02·4,4 | RulerFlat | regla ruler medir centímetros escala |
| 02·5,1 | PrecisionScale | báscula balanza precisión peso gramos |
| 02·5,2 | ChartDocument | documento gráfica reporte informe datos |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 02·1,2 | Sonda de temperatura | 16 px — cable delgado, queda un punto |
| 02·2,4 | Fuente de banco | repetida — casi idéntica a 02·2,3 |
| 02·2,5 | Sonda suelta | 16 px — mismo caso que 02·1,2 |
| 02·3,3 | Instrumento de banco | genérico — no responde a ninguna búsqueda propia |
| 02·4,2 | Giroscopio | 16 px — anillos finos, se empasta |
| 02·4,3 | Calibrador | 16 px — igual que 01·4,5 |
| 02·4,5 | Báscula chica | repetida — `PrecisionScale`, se queda la de 02·5,1 |
| 02·5,3 | Cámara fotográfica | repetida — `Camera` |
| 02·5,4 | Cable USB | repetida — `NetworkCable` |
| 02·5,5 | Importar a base de datos | repetida — `Database` |

## Lámina 3 — química · prefijo `quim`

**Entran (16)**

| Ref | Nombre | Palabras |
|---|---|---|
| 03·1,1 | PeriodicTable | tabla periódica elementos química periodic |
| 03·1,4 | BenzeneRing | anillo bencénico hexágono benceno molécula orgánica |
| 03·2,1 | FlaskGreen | matraz erlenmeyer reactivo química |
| 03·2,2 | FlaskRound | matraz balón fondo redondo destilación |
| 03·3,1 | GraduatedCylinder | probeta cilindro graduado volumen medir |
| 03·3,3 | ReagentBottle | frasco reactivo bote tapa químico |
| 03·3,4 | Dropper | gotero frasco pipeta dosificar |
| 03·3,5 | SafetyGoggles | goggles gafas seguridad protección ojos |
| 03·4,1 | BunsenBurner | mechero bunsen flama quemador calor |
| 03·4,2 | HotPlate | parrilla plato caliente calentador laboratorio |
| 03·4,3 | MagneticStirrer | agitador magnético parrilla mezclar vaso |
| 03·4,4 | ReactionArrows | flechas reacción reversible equilibrio química |
| 03·4,5 | CrystalLattice | red cristalina cristal estructura sólido |
| 03·5,1 | PhStrip | tira ph indicador colores acidez papel |
| 03·5,3 | HazardFlammable | peligro inflamable rombo advertencia flama |
| 03·5,4 | Chromatography | cromatografía placa corrida manchas separación |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 03·1,2 | Átomo | 16 px — igual que 01·3,3 |
| 03·1,3 | Molécula | repetida — se queda la de 01·3,2 |
| 03·1,5 | Vaso de precipitados | repetida — se queda la de 01·2,2 |
| 03·2,3 | Tubos en gradilla | repetida — se queda la de 01·2,1 |
| 03·2,4 | Micropipeta | 16 px |
| 03·2,5 | Bureta | 16 px — columna de un píxel |
| 03·3,2 | Placa de Petri | repetida — se queda la de 01·2,5 |
| 03·5,2 | Tubo con tapa | repetida — cubierto por `TestTubeRack` |
| 03·5,5 | Cuaderno de laboratorio | repetida — `Notebook` |

## Lámina 4 — biología · prefijo `bio`

La más ajena al catálogo actual y la que más pierde por tamaño: casi todo son
siluetas orgánicas de trazo fino.

**Entran (11)**

| Ref | Nombre | Palabras |
|---|---|---|
| 04·1,4 | Cell | célula cell núcleo organelos biología |
| 04·2,1 | MicroscopeSlide | portaobjetos laminilla muestra microscopio |
| 04·2,2 | MicroscopeBench | microscopio óptico laboratorio aumento |
| 04·2,5 | SampleJar | frasco muestra bote espécimen tapa |
| 04·3,1 | Bacterium | bacteria microbio bacilo germen |
| 04·3,2 | Virus | virus microbio patógeno germen |
| 04·3,3 | Leaf | hoja planta leaf botánica follaje |
| 04·3,4 | InsectSpecimen | insecto escarabajo espécimen colección |
| 04·3,5 | Fish | pez fish pescado animal acuático |
| 04·4,1 | LabMouse | ratón laboratorio animal roedor |
| 04·4,4 | Heart | corazón heart órgano anatomía |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 04·1,1 | Hélice de ADN | 16 px |
| 04·1,2 | Cadena de ARN | 16 px |
| 04·1,3 | Cromosoma | 16 px — las cuatro puntas se juntan |
| 04·1,5 | Placa de Petri con cultivo | repetida — se queda la de 01·2,5 |
| 04·2,3 | Jeringa | 16 px |
| 04·2,4 | Tubo con espécimen | repetida — cubierto por `SampleJar` |
| 04·4,2 | Embrión en placa | 16 px |
| 04·4,3 | Neurona | 16 px — dendritas de un píxel |
| 04·4,5 | Ojo | 16 px — se lee como una mancha |
| 04·5,1 | Proteína | 16 px — cintas finas |
| 04·5,2 | Electroforesis | 16 px |
| 04·5,3 | Centrífuga | 16 px — igual que 01·5,2 |
| 04·5,4 | Caja de crioviales | 16 px |
| 04·5,5 | Expediente genético | repetida — `Archive` |

## Lámina 5 — física y electrónica · prefijo `fis`

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 05·1,3 | CircuitBoard | placa circuito tarjeta electrónica pcb componentes |
| 05·1,4 | Resistor | resistencia resistor componente electrónica bandas |
| 05·1,5 | Capacitor | capacitor condensador componente electrónica |
| 05·2,1 | Inductor | inductor bobina componente electrónica |
| 05·2,2 | DryCell | pila seca celda batería energía |
| 05·2,3 | Magnet | imán magnet herradura polos magnetismo |
| 05·2,4 | Laser | láser laser haz rayo luz |
| 05·2,5 | Prism | prisma prism refracción luz espectro |
| 05·3,1 | Waveform | onda señal waveform gráfica oscilación |
| 05·3,2 | Pendulum | péndulo pendulum oscilación gravedad |
| 05·3,3 | Spring | resorte muelle spring elasticidad |
| 05·3,4 | TuningFork | diapasón tuning fork frecuencia sonido |
| 05·4,2 | ParticleDetector | detector partículas colisión acelerador física |
| 05·5,3 | SolderingIron | cautín soldador soldar estaño electrónica |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 05·1,1 | Átomo | 16 px |
| 05·1,2 | Osciloscopio | repetida — se queda 01·4,4 |
| 05·3,5 | Giroscopio | 16 px |
| 05·4,1 | Telescopio | repetida — se queda 06·1,1 |
| 05·4,3 | Voltímetro de aguja | repetida — `Voltmeter` de 02·1,5 |
| 05·4,4 | Amperímetro de aguja | repetida — `Ammeter` de 02·2,1 |
| 05·4,5 | Osciloscopio | repetida |
| 05·5,1 | Generador de señal | repetida — se queda 02·2,3 |
| 05·5,2 | Chip | repetida — cubierto por `CircuitBoard` y `MemoryModule` |
| 05·5,4 | Calibrador | 16 px |
| 05·5,5 | Documento de física | repetida — `DocumentText` |

## Lámina 6 — astronomía y geología · prefijo `astro`

**Entran (16)**

| Ref | Nombre | Palabras |
|---|---|---|
| 06·1,1 | Telescope | telescopio telescope observar estrellas astronomía |
| 06·1,3 | Planet | planeta saturno anillos planet astronomía |
| 06·1,4 | Moon | luna moon satélite cráteres |
| 06·2,1 | Galaxy | galaxia galaxy espiral estrellas cosmos |
| 06·2,2 | Comet | cometa comet estela meteoro |
| 06·2,3 | Asteroid | asteroide roca meteorito espacio |
| 06·2,4 | Satellite | satélite satellite órbita paneles espacio |
| 06·2,5 | RadioDish | antena parabólica radiotelescopio plato señal |
| 06·3,1 | Observatory | observatorio cúpula telescopio edificio |
| 06·3,3 | Volcano | volcán volcano erupción lava montaña |
| 06·3,4 | RockStrata | estratos capas roca geología corte |
| 06·3,5 | RockHammer | martillo geólogo piqueta roca golpear |
| 06·4,1 | Fossil | fósil ammonite concha paleontología |
| 06·4,2 | Crystals | cristales cuarzo mineral gema roca |
| 06·4,3 | Seismograph | sismógrafo sismo temblor registro terremoto |
| 06·5,2 | CompassRose | rosa vientos brújula norte mapa orientación |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 06·1,2 | Carpeta de constelaciones | repetida — `Folder` |
| 06·1,5 | Sol | repetida — se queda 10·3,2 |
| 06·3,2 | Tierra | repetida — `Globe` |
| 06·4,4 | Anemómetro | repetida — se queda 10·2,2 |
| 06·4,5 | Pluviómetro | repetida — se queda 10·2,3 |
| 06·5,1 | Mapa topográfico | repetida — se queda 10·3,4 |
| 06·5,3 | Libreta de campo | repetida — `Notebook` |
| 06·5,4 | GPS | repetida — se queda 10·3,5 |
| 06·5,5 | Bolsa de muestra | repetida — se queda 10·1,4 |

## Lámina 7 — estadística y datos · prefijo `dato`

La mitad son gráficas y el catálogo ya tiene barras, pastel y línea. Entra la
familia que falta.

**Entran (12)**

| Ref | Nombre | Palabras |
|---|---|---|
| 07·1,4 | Histogram | histograma histogram distribución frecuencia |
| 07·2,1 | Heatmap | mapa calor heatmap matriz colores intensidad |
| 07·2,2 | ContourPlot | curvas nivel contorno superficie topográfico |
| 07·2,3 | BoxPlot | caja bigotes boxplot cuartiles distribución |
| 07·2,5 | Spectrum | espectro spectrum picos frecuencia señal |
| 07·3,4 | FunctionCurve | función curva ejes gráfica matemática |
| 07·3,5 | Matrix | matriz matrix cuadrícula celdas álgebra |
| 07·4,1 | Sigma | sumatoria sigma suma total matemática |
| 07·4,2 | Regression | regresión ajuste recta tendencia correlación |
| 07·4,3 | Funnel | embudo funnel filtrar depurar |
| 07·4,5 | CompareCharts | comparar gráficas series contraste |
| 07·5,4 | Analyze | analizar examinar lupa gráfica revisar |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 07·1,1 | Gráfica de línea | repetida — `ChartLine` |
| 07·1,2 | Gráfica de barras | repetida — `ChartBar` |
| 07·1,3 | Dispersión | repetida — se queda 01·4,1 |
| 07·1,5 | Gráfica de pastel | repetida — `ChartPie` |
| 07·2,4 | Osciloscopio | repetida |
| 07·3,1 | Tabla | repetida — se queda 01·4,2 |
| 07·3,2 | Base de datos | repetida — `Database` |
| 07·3,3 | Calculadora | repetida — `Calculator` |
| 07·4,4 | Flechas de orden | repetida — `SortAlpha`, `ArrowUp`, `ArrowDown` |
| 07·5,1 | Exportar gráfica | repetida — `Shortcut` con `ChartLine` |
| 07·5,2 | Imprimir reporte | repetida — `Printer` |
| 07·5,3 | Tablero | 16 px — cuatro gráficas en 16 px son ruido |
| 07·5,5 | Presentación | 16 px — el rotafolio queda un rectángulo |

## Lámina 8 — medicina · prefijo `med`

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 08·1,2 | BloodTube | tubo sangre muestra análisis hematología |
| 08·2,1 | VitalsMonitor | monitor signos vitales ecg pulso paciente |
| 08·2,2 | Syringe | jeringa syringe inyección vacuna aguja |
| 08·2,3 | Stethoscope | estetoscopio stethoscope auscultar médico |
| 08·2,5 | PillBottle | frasco pastillas medicina píldoras receta |
| 08·3,1 | PatientChart | expediente paciente historia clínica |
| 08·3,2 | MedicalFolder | carpeta médica expediente cruz salud |
| 08·3,3 | Xray | radiografía rayos x tórax placa costillas |
| 08·3,4 | Bone | hueso bone fémur esqueleto traumatología |
| 08·3,5 | HeadScan | tomografía cráneo cerebro scan cabeza |
| 08·4,2 | Tooth | diente muela dental odontología |
| 08·4,4 | Freezer | congelador ultracongelador frío muestras |
| 08·5,1 | Lungs | pulmones lungs respiración órgano |
| 08·5,4 | Hospital | hospital clínica edificio salud urgencias |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 08·1,1 | Microscopio | repetida — se queda 01·1,5 |
| 08·1,3 | Placa de Petri | repetida |
| 08·1,4 | Tubos en gradilla | repetida |
| 08·1,5 | ADN | 16 px |
| 08·2,4 | Termómetro | repetida — se queda 01·5,3 |
| 08·4,1 | Ojo | 16 px |
| 08·4,3 | Centrífuga | 16 px |
| 08·4,5 | Célula | repetida — se queda 04·1,4 |
| 08·5,2 | Hígado | 16 px — mancha sin contorno propio |
| 08·5,3 | Riñón | 16 px — mismo caso |
| 08·5,5 | Engranes | repetida — `Gear` |

## Lámina 9 — archivos con tema científico

**Lámina completa fuera.** Es la lámina de sistema: carpetas, documentos,
disquete, impresora, lista de tareas, calendario, reloj, sincronizar, subir,
bajar, carpeta compartida, carpeta con candado, llave, engranes, lupa, caja de
archivo, papelera, respaldo y cuaderno de reporte. Las veinticinco responden a
búsquedas que el catálogo ya atiende, y el emblema científico encima —un átomo,
un matraz— no cambia la palabra por la que alguien la busca.

## Lámina 10 — campo y ambiente · prefijo `campo`

**Entran (15)**

| Ref | Nombre | Palabras |
|---|---|---|
| 10·1,1 | Leaf | hoja planta leaf follaje botánica |
| 10·1,2 | Tree | árbol tree bosque planta |
| 10·1,3 | WaterDrop | gota agua water drop líquido |
| 10·1,4 | SoilSample | muestra suelo bolsa tierra sedimento |
| 10·1,5 | InsectTrap | trampa insectos captura campo cebo |
| 10·2,1 | Binoculars | binoculares prismáticos observar avistar |
| 10·2,2 | WeatherStation | estación meteorológica sensores clima torre |
| 10·2,3 | RainGauge | pluviómetro lluvia precipitación medir |
| 10·2,5 | WindVane | veleta anemómetro viento dirección |
| 10·3,1 | Cloud | nube cloud nublado clima |
| 10·3,2 | Sun | sol sun soleado despejado clima |
| 10·3,3 | Mountains | montañas montaña sierra cumbre relieve |
| 10·3,4 | TopoMap | mapa topográfico curvas nivel terreno |
| 10·3,5 | Gps | gps navegador posición satelital ubicación |
| 10·4,3 | TrailCamera | cámara trampa fototrampeo fauna campo |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 10·2,4 | Termómetro | repetida |
| 10·4,1 | Libreta de campo | repetida — `Notebook` |
| 10·4,2 | Tubo de muestra de agua | repetida — cubierto por `SampleJar` |
| 10·4,4 | Ave | 16 px — el pico y las patas se pierden |
| 10·4,5 | Pez | repetida — se queda 04·3,5 |
| 10·5,1 | Microscopio | repetida |
| 10·5,2 | Lupa | repetida — `Magnifier` |
| 10·5,3 | Medidor de agua | repetida — cubierto por `PhMeter` |
| 10·5,4 | Marcador en mapa | 16 px — el pin sobre el mapa se empasta |
| 10·5,5 | Reciclaje | 16 px — las tres flechas se juntan |

---

A partir de la lámina 11 aparece un cuarto motivo de salida:

- **letrero** — la pieza solo significa algo si se lee la palabra impresa encima
  (`PAID`, `OVERDUE`, `PAY STUB`, `W-4`, `DIRECT DEPOSIT`). No se puede: a 16 px
  el texto es una mancha y a 32 es ruido. Además está en inglés, y el catálogo se
  busca en los dos idiomas. Un sello con la palabra dentro no es un icono, es un
  letrero pequeño.

---

## Lámina 11 — contabilidad · prefijo `conta`

Tema entero ausente del catálogo: no hay una sola pieza de dinero en las 180.

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 11·1,1 | Ledger | libro contable mayor registro contabilidad |
| 11·1,3 | Invoice | factura invoice recibo cobro comprobante |
| 11·1,4 | Receipt | ticket recibo tira comprobante compra |
| 11·1,5 | Coins | monedas coins dinero efectivo cambio |
| 11·1,6 | Bank | banco bank institución columnas edificio |
| 11·2,1 | CashRegister | caja registradora punto venta cobrar |
| 11·2,2 | Wallet | cartera billetera wallet dinero |
| 11·2,3 | PiggyBank | alcancía cochinito ahorro piggy |
| 11·3,2 | Cheque | cheque check pago firma banco |
| 11·3,3 | CreditCard | tarjeta crédito card pago plástico |
| 11·3,4 | MoneyBag | bolsa dinero saco efectivo fondos |
| 11·3,5 | Scales | balanza equilibrio justicia pesar comparar |
| 11·3,6 | Abacus | ábaco abacus cuentas calcular |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 11·1,2 | Calculadora | repetida — `Calculator` |
| 11·4,3 | Caja fuerte | repetida — `Safe` de seg-r1c5 |
| 11·2,4 | Gráfica de barras | repetida — `ChartBar` |
| 11·2,5 | Gráfica de pastel | repetida — `ChartPie` |
| 11·2,6 | Gráfica de línea | repetida — `ChartLine` |
| 11·3,1 | Hoja contable verde | repetida — cubierto por `Ledger` |
| 11·4,1 | Carpeta con billetes | repetida — `Folder` con `MoneyBag` |
| 11·4,2 | Hoja de cálculo | repetida — se queda 01·4,2 |
| 11·4,4 | Lista de verificación | repetida — `Checklist` |
| 11·4,5 | Calendario | repetida — `Calendar` |
| 11·4,6 | Analizar reporte | repetida — se queda 07·5,4 |

## Lámina 12 — administración de documentos · prefijo `admin`

La más golpeada por el letrero: seis piezas son sellos con la palabra dentro.

**Entran (9)**

| Ref | Nombre | Palabras |
|---|---|---|
| 12·1,2 | RecurringInvoice | factura recurrente periódica suscripción ciclo |
| 12·2,1 | DocumentFeed | alimentador documentos bandeja salida impresión |
| 12·2,2 | PersonnelFolder | expediente personal carpeta empleado ficha |
| 12·2,6 | MailDocument | sobre documento correspondencia enviar carta |
| 12·3,1 | ScreenInvoice | facturación pantalla monitor sistema cobro |
| 12·3,2 | CardTerminal | terminal punto venta datáfono cobrar tarjeta |
| 12·3,4 | Cash | billetes efectivo dinero cash pago |
| 12·3,5 | DiscountTag | etiqueta descuento porcentaje rebaja precio |
| 12·4,3 | BoundReport | reporte empastado informe volumen encuadernado |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 12·1,1 | Factura | repetida — se queda 11·1,3 |
| 12·1,3 | Sello PAID | letrero |
| 12·1,4 | Sello OVERDUE | letrero |
| 12·1,5 | Documento firmado | repetida — `ClipboardCheck` |
| 12·1,6 | Formato fiscal | repetida — cubierto por `Invoice` |
| 12·2,3 | Sello APPROVED | letrero — y repetida, `StampApproved` |
| 12·2,4 | Sello PENDING | letrero |
| 12·2,5 | Calendario | repetida — `Calendar` |
| 12·3,3 | Tarjeta de crédito | repetida — se queda 11·3,3 |
| 12·3,6 | Cheque firmado | repetida — se queda 11·3,2 |
| 12·4,1 | Documento con aviso | repetida — `Bell` con documento |
| 12·4,2 | Lista de verificación | repetida — `Checklist` |
| 12·4,4 | Credencial | repetida — `ContactCard` |
| 12·4,5 | Calendario en tabla | repetida — `CalendarMonth` |
| 12·4,6 | Carpeta de argollas | repetida — cubierto por `BoundReport` |

## Lámina 13 — banca · prefijo `banca`

**Entran (12)**

| Ref | Nombre | Palabras |
|---|---|---|
| 13·1,3 | Chequebook | chequera talonario cheques banco |
| 13·1,4 | BankStatement | estado cuenta movimientos banco extracto |
| 13·2,2 | CardBank | tarjeta bancaria débito banco plástico |
| 13·2,3 | OnlineBanking | banca electrónica en línea portal cuenta |
| 13·2,4 | Transfer | transferencia traspaso bancos envío dinero |
| 13·3,4 | DepositBox | buzón depósito caja seguridad ranura |
| 13·3,5 | MoneyRoll | fajo rollo billetes efectivo dinero |
| 13·3,6 | Atm | cajero automático atm retirar banco |
| 13·4,2 | CoinDeposit | depositar moneda alcancía ranura ahorro |
| 13·4,3 | Reconcile | conciliar comparar cuadrar estados igual |
| 13·4,5 | ExchangeArrows | intercambio cambio divisas flechas circular |
| 13·4,6 | Audit | auditoría revisar lupa documento examinar |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 13·1,1 | Banco | repetida — se queda 11·1,6 |
| 13·1,2 | Libro contable | repetida — se queda 11·1,1 |
| 13·1,5 | Estado de cuenta | repetida — cubierto por `BankStatement` |
| 13·1,6 | Caja fuerte | repetida — `Safe` |
| 13·2,1 | Llave | repetida — `Key` |
| 13·2,5 | Factura con dólar | repetida — se queda 11·1,3 |
| 13·2,6 | Carpeta de banco | repetida — `Folder` |
| 13·3,1 | Cheque | repetida — se queda 11·3,2 |
| 13·3,2 | Sello RECONCILED | letrero |
| 13·3,3 | Estado de cuenta | repetida |
| 13·4,1 | Cartera | repetida — se queda 11·2,2 |
| 13·4,4 | Palomita de aprobado | repetida — `Check` |

## Lámina 14 — nómina · prefijo `nomina`

La más golpeada de las treinta: doce de veinticuatro piezas son un formato con su
nombre impreso encima.

**Entran (7)**

| Ref | Nombre | Palabras |
|---|---|---|
| 14·1,2 | IdBadge | gafete credencial empleado identificación |
| 14·1,4 | PayEnvelope | sobre nómina pago efectivo raya |
| 14·1,5 | Timesheet | tarjeta tiempo asistencia horas registro |
| 14·2,4 | EmployeeFiles | expedientes empleados personal archivero |
| 14·2,5 | Bonus | bono premio estrella incentivo gratificación |
| 14·3,4 | BenefitsFolder | prestaciones beneficios seguro carpeta salud |
| 14·3,5 | Pension | pensión retiro jubilación ahorro alcancía |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 14·1,1 | Recibo PAY STUB | letrero |
| 14·1,3 | Libro de raya | repetida — se queda 11·1,1 |
| 14·1,6 | Reloj con cheque | repetida — `Clock` con `Cheque` |
| 14·2,1 | Formato W-4 | letrero — y ajeno a México |
| 14·2,2 | DIRECT DEPOSIT | letrero |
| 14·2,3 | Transferencia entre bancos | repetida — se queda 13·2,4 |
| 14·2,6 | OVERTIME | letrero |
| 14·3,1 | PAYDAY | letrero |
| 14·3,2 | DEDUCTIONS | letrero |
| 14·3,3 | REIMBURSEMENT | letrero |
| 14·3,6 | PAYROLL CHECK | letrero |
| 14·4,1 | Calculadora | repetida — `Calculator` |
| 14·4,2 | Candado PAYROLL | letrero — y repetida, `Padlock` |
| 14·4,3 | Reportes | repetida — cubierto por `ChartDocument` |
| 14·4,4 | Lista por empleado | repetida — `TaskList` |
| 14·4,5 | Sello APPROVED | letrero |
| 14·4,6 | PAYROLL TOTALS | letrero |

## Lámina 15 — impuestos · prefijo `fiscal`

**Entran (8)**

| Ref | Nombre | Palabras |
|---|---|---|
| 15·1,5 | DeadlineCalendar | vencimiento fecha límite plazo calendario círculo |
| 15·1,6 | PercentCalculator | calculadora porcentaje tasa impuesto calcular |
| 15·2,1 | ReceiptBundle | fajo recibos comprobantes atado tickets |
| 15·2,3 | ShieldCheck | escudo verificado cumplimiento validado protegido |
| 15·3,3 | AlertDocument | documento alerta aviso advertencia atención |
| 15·3,4 | FileCabinet | archivero gaveta cajón expedientes mueble |
| 15·3,6 | SignedDocument | documento firmado firma autógrafo rúbrica |
| 15·4,5 | LockedDocument | documento bloqueado candado confidencial cerrado |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 15·1,1 | Formato TAX | letrero |
| 15·1,2 | Hacienda | repetida — se queda 11·1,6 |
| 15·1,3 | Sello APPROVED | letrero |
| 15·1,4 | Sello FILED | letrero |
| 15·2,2 | Lista AUDIT | letrero — y repetida, `Checklist` |
| 15·2,4 | Balanza | repetida — se queda 11·3,5 |
| 15·2,5 | INVOICE con sello TAX | letrero |
| 15·2,6 | REFUND | letrero |
| 15·3,1 | DEDUCTION | letrero |
| 15·3,2 | Carpetas | repetida — `Folder` |
| 15·3,5 | TAX con lupa | letrero — y repetida, se queda 13·4,6 |
| 15·4,1 | Sobre con billetes | repetida — se queda 14·1,4 |
| 15·4,2 | Libro con gráfica | repetida — se queda 11·1,1 |
| 15·4,3 | Gráfica de línea | repetida — `ChartLine` |
| 15·4,4 | Monedas | repetida — se queda 11·1,5 |
| 15·4,6 | Libro con escudo | repetida — cubierto por `ShieldCheck` |

## Lámina 16 — presupuesto y pronóstico · prefijo `presu`

**Entran (7)**

| Ref | Nombre | Palabras |
|---|---|---|
| 16·1,3 | TargetMoney | meta objetivo blanco tiro presupuesto |
| 16·2,3 | Forecast | pronóstico previsión proyección bola futuro |
| 16·2,4 | GaugeMedal | indicador desempeño medidor meta logro |
| 16·2,5 | CoinJar | frasco monedas ahorro fondo reserva |
| 16·2,6 | GaugeMoney | medidor gasto presupuesto aguja consumo |
| 16·3,6 | GrowthArrow | crecimiento alza flecha subida tendencia |
| 16·4,5 | MoneyCycle | ciclo dinero flujo circulante rotación |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 16·1,1 | Carpeta con documentos | repetida — `Folder` |
| 16·1,2 | Alcancía | repetida — se queda 11·2,3 |
| 16·1,4 | Calendario marcado | repetida — se queda 15·1,5 |
| 16·1,5 | Gráfica de barras | repetida — `ChartBar` |
| 16·1,6 | Gráfica de pastel | repetida — `ChartPie` |
| 16·2,1 | Línea al alza | repetida — cubierto por `GrowthArrow` |
| 16·2,2 | Línea a la baja | repetida — misma pieza invertida |
| 16·3,1 | Cartera | repetida — se queda 11·2,2 |
| 16·3,2 | Sobre con billetes | repetida |
| 16·3,3 | Presupuesto en tabla | repetida — `TaskList` |
| 16·3,4 | Hoja de cálculo | repetida — se queda 01·4,2 |
| 16·3,5 | Tabla contable | repetida — se queda 11·1,1 |
| 16·4,1 | Comparar columnas | repetida — se queda 07·4,5 |
| 16·4,2 | Calculadora | repetida — `Calculator` |
| 16·4,3 | Documento de advertencia | repetida — se queda 15·3,3 |
| 16·4,4 | Documento APPROVED | letrero |
| 16·4,6 | Caja fuerte | repetida — `Safe` |

## Lámina 17 — logística e inventario · prefijo `alma`

Otro tema virgen: el catálogo no tiene caja, camión, almacén ni código de barras
de producto.

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 17·1,1 | Box | caja paquete bulto carton embalaje |
| 17·1,2 | BoxStack | cajas apiladas lote bultos existencias |
| 17·1,3 | PriceTag | etiqueta precio colgante marbete |
| 17·1,4 | Barcode | código barras barcode etiqueta lectura |
| 17·1,6 | SupplierFolder | proveedor carpeta expediente contacto |
| 17·2,1 | DeliveryTruck | camión reparto entrega transporte flete |
| 17·2,2 | PackingList | lista empaque surtido verificación bultos |
| 17·2,3 | WarehouseRack | anaquel estante rack almacén estibas |
| 17·2,4 | StockTable | existencias inventario tabla conteo saldos |
| 17·3,4 | HandTruck | diablito carretilla mover carga estibar |
| 17·3,5 | BoxChecked | caja verificada recibido conforme entregado |
| 17·3,6 | StockAlert | alerta existencias faltante mínimo aviso |
| 17·4,2 | Warehouse | almacén bodega nave depósito |
| 17·4,3 | BoxInspect | inspección caja revisar rastrear buscar |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 17·1,5 | Documento PO | letrero |
| 17·2,5 | Monedas | repetida — se queda 11·1,5 |
| 17·2,6 | Etiqueta con dólar | repetida — se queda 17·1,3 |
| 17·3,1 | Calculadora | repetida — `Calculator` |
| 17·3,2 | Organigrama | repetida — cubierto por `Group` |
| 17·3,3 | Documento INV | letrero |
| 17·4,1 | Ábaco | repetida — se queda 11·3,6 |
| 17·4,4 | Reporte con gráficas | repetida — se queda 02·5,2 |
| 17·4,5 | Devolución | repetida — cubierto por `Undo` con caja |
| 17·4,6 | Balanza | repetida — se queda 11·3,5 |

## Lámina 18 — reportes y tableros · prefijo (ninguno)

**Lámina completa fuera.** Es la lámina 7 otra vez, más pobre: veinticuatro
piezas que son gráfica de barras, de pastel, de línea, tabla, reporte impreso,
reporte exportado, presentación, lupa sobre gráfica, engrane con gráfica, flecha
al alza, flecha a la baja, caja de archivo, calculadora y documento aprobado. Todo
eso ya está, o entró por la lámina 7 con mejor dibujo.

La única que valdría la pena, el indicador de aguja de 18·3,4, ya entra por
16·2,6.

## Lámina 19 — auditoría y resguardo documental · prefijo `audit`

**Entran (6)**

| Ref | Nombre | Palabras |
|---|---|---|
| 19·2,3 | Shredder | trituradora destructora papel destruir confidencial |
| 19·2,5 | CompareDocuments | comparar documentos cotejar versiones diferencias |
| 19·2,6 | RevisionCycle | revisión ciclo versiones actualizar documento |
| 19·3,1 | ReportAlert | reporte con alerta hallazgo observación anomalía |
| 19·3,3 | SearchFolder | buscar carpeta explorar expediente lupa |
| 19·4,3 | SealedEnvelope | sobre lacrado sello cera confidencial cerrado |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 19·1,1 | Libro contable | repetida — se queda 11·1,1 |
| 19·1,2 | Auditar hoja | repetida — se queda 13·4,6 |
| 19·1,3 | Caja fuerte | repetida — `Safe` |
| 19·1,4 | Documento con candado | repetida — se queda 15·4,5 |
| 19·1,5 | Lista de verificación | repetida — `Checklist` |
| 19·1,6 | Sello APPROVED | letrero |
| 19·2,1 | Sello REJECTED | letrero |
| 19·2,2 | Caja de archivo | repetida — `ArchiveBox` |
| 19·2,4 | Documento firmado | repetida — se queda 15·3,6 |
| 19·3,2 | Balanza | repetida — se queda 11·3,5 |
| 19·3,4 | Exportar a disquete | repetida — `Floppy` |
| 19·3,5 | Llave | repetida — `Key` |
| 19·3,6 | Escudo | repetida — `Shield` |
| 19·4,1 | Documento aprobado | repetida — `Check` |
| 19·4,2 | Archivero | repetida — se queda 15·3,4 |
| 19·4,4 | Cámara | repetida — `Camera` |
| 19·4,5 | Carpeta protegida | repetida — `FolderLocked` |
| 19·4,6 | Calculadora | repetida — `Calculator` |

## Lámina 20 — gastos y comprobantes · prefijo `gasto`

**Entran (9)**

| Ref | Nombre | Palabras |
|---|---|---|
| 20·1,2 | BoardingPass | pase abordar boleto avión viaje vuelo |
| 20·1,6 | HotelReceipt | hotel hospedaje factura estancia noche |
| 20·2,1 | MealReceipt | comida restaurante consumo alimentos ticket |
| 20·2,2 | TaxiReceipt | taxi transporte traslado ticket viaje |
| 20·2,3 | MileageLog | kilometraje bitácora recorrido auto viáticos |
| 20·2,4 | ExpenseReport | reporte gastos comprobación relación viáticos |
| 20·3,4 | ReceiptFolder | carpeta comprobantes expediente gastos |
| 20·4,1 | Refund | reembolso devolución reintegro dinero |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 20·1,1 | Ticket | repetida — se queda 11·1,4 |
| 20·1,3 | Portafolios | repetida — `Briefcase` de ofi-r2c4 |
| 20·1,4 | Cartera | repetida — se queda 11·2,2 |
| 20·1,5 | Tarjeta de crédito | repetida — se queda 11·3,3 |
| 20·2,5 | Lista de verificación | repetida — `Checklist` |
| 20·2,6 | Cámara | repetida — `Camera` |
| 20·3,1 | Sobre con documentos | repetida — se queda 12·2,6 |
| 20·3,2 | Fajo de billetes | repetida — se queda 13·3,5 |
| 20·3,3 | Monedas | repetida — se queda 11·1,5 |
| 20·3,5 | Calendario | repetida — `Calendar` |
| 20·3,6 | Calculadora | repetida — `Calculator` |
| 20·4,2 | Gafete | repetida — se queda 14·1,2 |
| 20·4,3 | Sello de aprobado | repetida — `StampApproved` |
| 20·4,4 | Documento de alerta | repetida — se queda 15·3,3 |
| 20·4,5 | Alcancía | repetida — se queda 11·2,3 |
| 20·4,6 | Comprobante con lupa | repetida — se queda 13·4,6 |

## Lámina 21 — videovigilancia · prefijo `vigi`

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 21·1,1 | SecurityCamera | cámara vigilancia cctv seguridad videocámara |
| 21·1,2 | DomeCamera | cámara domo techo vigilancia esfera |
| 21·1,3 | PtzCamera | cámara motorizada ptz giratoria seguimiento |
| 21·1,4 | MonitorWall | monitoreo pantallas mosaico central vigilancia |
| 21·1,5 | PanicButton | botón pánico emergencia alarma pulsador |
| 21·2,1 | Recorder | grabador dvr nvr video almacenamiento |
| 21·2,2 | MotionSensor | sensor movimiento detector presencia infrarrojo |
| 21·2,3 | NightMode | modo nocturno noche visión oscuridad |
| 21·2,5 | Beacon | torreta baliza luz giratoria emergencia |
| 21·3,1 | KeypadPanel | teclado acceso panel código tablero |
| 21·3,2 | Intercom | interfón portero intercomunicador timbre |
| 21·4,1 | BarrierArm | pluma barrera acceso vehicular caseta |
| 21·4,2 | WatchTower | torre vigilancia atalaya observación garita |
| 21·4,3 | Floodlight | reflector luminaria iluminación perimetral |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 21·2,4 | Escudo | repetida — `Shield` |
| 21·3,3 | Chapa de puerta | repetida — se queda 22·2,1 |
| 21·3,4 | Tarjeta de acceso | repetida — se queda 22·3,3 |
| 21·3,5 | Teclado numérico | repetida — se queda 21·3,1 |
| 21·4,4 | Triángulo de advertencia | repetida — `Warning` |
| 21·4,5 | Carpeta de video | repetida — `Folder` |

## Lámina 22 — control de acceso · prefijo `acceso`

**Entran (13)**

| Ref | Nombre | Palabras |
|---|---|---|
| 22·1,1 | Door | puerta door acceso entrada cerrada |
| 22·1,2 | DoorOpen | puerta abierta acceso permitido entrada |
| 22·1,3 | DoorLocked | puerta con candado acceso restringido cerrada |
| 22·2,1 | Deadbolt | cerradura chapa cerrojo puerta llave |
| 22·2,2 | Fingerprint | huella dactilar biométrico dedo identidad |
| 22·2,3 | FaceRecognition | reconocimiento facial rostro biométrico cara |
| 22·2,4 | IrisScan | iris ojo biométrico escaneo retina |
| 22·2,5 | Turnstile | torniquete molinete acceso paso control |
| 22·3,1 | AccessCard | credencial acceso tarjeta identificación |
| 22·3,2 | VisitorBadge | gafete visitante pase temporal identificación |
| 22·3,4 | Elevator | elevador ascensor piso subir bajar |
| 22·4,3 | GuardBooth | caseta vigilancia garita control entrada |
| 22·4,5 | MetalDetector | arco detector metales revisión seguridad |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 22·1,4 | Candado cerrado | repetida — `Padlock` |
| 22·1,5 | Candado abierto | repetida — `FolderUnlocked` cubre el sentido |
| 22·3,3 | Tarjeta con chip | repetida — se queda 22·3,1 |
| 22·3,5 | Llave | repetida — `Key` |
| 22·4,1 | Llavero | repetida — cubierto por `Key` |
| 22·4,2 | Lista de verificación | repetida — `Checklist` |
| 22·4,4 | Pluma de barrera | repetida — se queda 21·4,1 |

## Lámina 23 — alarmas y emergencia · prefijo `alarma`

**Entran (14)**

| Ref | Nombre | Palabras |
|---|---|---|
| 23·1,1 | AlarmBell | campana alarma timbre sonar aviso |
| 23·1,3 | EmergencyStop | paro emergencia botón hongo detener |
| 23·1,5 | Armed | armado activado sistema encendido protegido |
| 23·2,1 | Disarmed | desarmado desactivado sistema apagado |
| 23·2,2 | MotionAlarm | detector con ondas movimiento disparo alarma |
| 23·2,3 | Intruder | intruso ladrón allanamiento silueta ventana |
| 23·2,4 | BrokenGlass | vidrio roto rotura cristal sensor |
| 23·2,5 | SmokeDetector | detector humo sensor incendio techo |
| 23·3,3 | AlarmBoard | tablero alarma placa central circuito |
| 23·3,4 | BackupBattery | batería respaldo acumulador energía apoyo |
| 23·3,5 | PowerFailure | falla energía corte apagón sin luz |
| 23·4,1 | LocationAlert | ubicación alerta mapa incidente lugar |
| 23·4,3 | EmergencyPhone | teléfono emergencia auxilio llamada urgencia |
| 23·4,4 | Strobe | estrobo destello luz intermitente aviso |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 23·1,2 | Torreta | repetida — se queda 21·2,5 |
| 23·1,4 | Teclado de alarma | repetida — se queda 21·3,1 |
| 23·3,1 | Detector de humo | repetida — se queda 23·2,5 |
| 23·3,2 | Detector de humo | repetida — tercera copia |
| 23·4,2 | Diálogo de error | repetida — `Alert` |
| 23·4,5 | Carpeta de incidentes | repetida — `Folder` |

## Lámina 24 — seguridad informática · prefijo `ciber`

**Entran (16)**

| Ref | Nombre | Palabras |
|---|---|---|
| 24·1,1 | Firewall | cortafuegos firewall muro fuego barrera |
| 24·1,2 | ServerLocked | servidor protegido bloqueado candado equipo |
| 24·1,3 | GlobalShield | protección global red mundo escudo internet |
| 24·1,4 | ThreatTarget | amenaza objetivo blanco riesgo mira |
| 24·2,1 | CloudLocked | nube protegida cifrada candado respaldo |
| 24·2,2 | VpnTunnel | túnel vpn cifrado conexión privada |
| 24·2,3 | MalwareScan | análisis malware virus lupa detectar |
| 24·2,4 | Malware | virus malware amenaza calavera infección |
| 24·2,5 | Phishing | phishing anzuelo fraude engaño bloqueado |
| 24·3,2 | SecurityToken | token llave dinámica código autenticación |
| 24·3,3 | SecurityKey | llave usb autenticación física seguridad |
| 24·3,4 | VerifiedUser | usuario verificado identidad validado cuenta |
| 24·3,5 | SecureSite | sitio seguro https candado navegador |
| 24·4,1 | SignedMail | correo firmado cifrado seguro certificado |
| 24·4,2 | NetworkLocked | red protegida segmento cifrado nodos |
| 24·4,3 | DataLeak | fuga datos filtración goteo incidente |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 24·1,5 | Carpeta con candado | repetida — `FolderLocked` |
| 24·3,1 | Caja fuerte | repetida — `Safe` |
| 24·4,4 | Escudo de actualización | repetida — `Sync` con `Shield` |
| 24·4,5 | Bote de riesgo biológico | 16 px — el símbolo de tres aspas se empasta |

## Lámina 25 — centro de mando · prefijo `mando`

**Entran (11)**

| Ref | Nombre | Palabras |
|---|---|---|
| 25·1,1 | DispatchConsole | consola despacho mapa central operación |
| 25·1,2 | ControlDesk | mesa control tablero operador puesto |
| 25·1,3 | Headset | diadema audífonos operador comunicación |
| 25·1,4 | PoliceBadge | placa policía insignia corporación agente |
| 25·2,2 | RouteMap | ruta recorrido trayecto mapa camino |
| 25·2,3 | CommsVan | unidad móvil vehículo antena comunicaciones |
| 25·2,4 | Walkie | radio portátil handy comunicación frecuencia |
| 25·3,1 | IncidentPhoto | fotografía evidencia incidente escena |
| 25·3,2 | IncidentFolder | expediente incidente carpeta reporte caso |
| 25·3,4 | Escalation | escalamiento escalar nivel elevar prioridad |
| 25·4,3 | RuggedTablet | tableta rugerizada campo terminal portátil |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 25·1,5 | Bandeja de avisos | repetida — `Inbox` con `Alert` |
| 25·2,1 | Torreta | repetida — se queda 21·2,5 |
| 25·3,3 | Expediente de personal | repetida — se queda 12·2,2 |
| 25·3,5 | Lista y tablero | repetida — `Checklist` y `OrgChart` |
| 25·4,1 | Palomita verde | repetida — `Check` |
| 25·4,2 | Advertencia | repetida — `Warning` |
| 25·4,4 | Octágono de alto | repetida — `StopSign` |
| 25·4,5 | Portapapeles confidencial | repetida — se queda 15·4,5 |

## Lámina 26 — identidad y credenciales · prefijo `ident`

Choca de frente con la lámina 22: huella, rostro e iris ya entraron ahí.

**Entran (9)**

| Ref | Nombre | Palabras |
|---|---|---|
| 26·1,2 | BadgeBarcode | gafete código barras credencial lectura |
| 26·2,2 | Signature | firma autógrafo rúbrica firmar consentimiento |
| 26·2,3 | Approve | aprobar aceptar validar visto bueno |
| 26·2,4 | Reject | rechazar denegar negar cancelar |
| 26·2,5 | Passport | pasaporte documento viaje identidad |
| 26·3,3 | UserShield | usuario protegido cuenta segura identidad |
| 26·3,4 | UserLocked | usuario bloqueado cuenta suspendida acceso |
| 26·4,1 | BackgroundCheck | investigación antecedentes verificación persona |
| 26·4,4 | CardReader | lector credencial pistola escanear acceso |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 26·1,1 | Gafete | repetida — se queda 14·1,2 |
| 26·1,3 | Credencial | repetida — se queda 22·3,1 |
| 26·1,4 | Huella | repetida — se queda 22·2,2 |
| 26·1,5 | Reconocimiento facial | repetida — se queda 22·2,3 |
| 26·2,1 | Iris | repetida — se queda 22·2,4 |
| 26·3,1 | Carpeta de personal | repetida — se queda 12·2,2 |
| 26·3,2 | Escudo de usuario | repetida — se queda 26·3,3 |
| 26·3,5 | Lista de personal | repetida — `TaskList` |
| 26·4,2 | Sello de aprobado | repetida — `StampApproved` |
| 26·4,3 | Sello de rechazo | repetida — cubierto por `Reject` |
| 26·4,5 | Credencial segura | repetida — se queda 22·3,1 |

## Lámina 27 — seguridad perimetral · prefijo `perim`

**Entran (13)**

| Ref | Nombre | Palabras |
|---|---|---|
| 27·1,1 | Fence | reja malla ciclónica cerca perímetro |
| 27·1,2 | BarbedWire | alambre púas concertina cerca disuasión |
| 27·1,3 | Gate | portón reja entrada vehicular acceso |
| 27·1,4 | BeamSensor | sensor haz barrera fotoeléctrica cruce |
| 27·1,5 | GuardDogSign | señal perro guardián advertencia canino |
| 27·2,2 | PatrolRoute | ronda recorrido patrullaje puntos control |
| 27·2,3 | PatrolCar | patrulla vehículo policía rondín |
| 27·3,1 | CameraHousing | gabinete cámara carcasa intemperie montaje |
| 27·3,2 | LockedShed | caseta bodega asegurada candado resguardo |
| 27·3,3 | NoEntrySign | prohibido el paso restringido señal peatón |
| 27·3,4 | HazardTape | cinta peligro acordonar delimitar precaución |
| 27·3,5 | EmergencyExit | salida emergencia evacuación ruta escape |
| 27·4,5 | TrafficCone | cono tránsito señalamiento delimitar vía |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 27·2,1 | Reflector | repetida — se queda 21·4,3 |
| 27·2,4 | Caseta de vigilancia | repetida — se queda 22·4,3 |
| 27·2,5 | Pluma de barrera | repetida — se queda 21·4,1 |
| 27·4,1 | Plano del sitio | repetida — se queda 10·3,4 |
| 27·4,2 | Geocerca | repetida — se queda 23·4,1 |
| 27·4,3 | Cámara domo | repetida — se queda 21·1,2 |
| 27·4,4 | Sirena de trompeta | repetida — cubierto por `AlarmBell` y `Strobe` |

## Lámina 28 — cumplimiento y resguardo · prefijo `cumpl`

**Entran (7)**

| Ref | Nombre | Palabras |
|---|---|---|
| 28·1,2 | Policy | política reglamento norma pergamino lineamiento |
| 28·1,3 | LockedChest | cofre resguardo caja asegurada custodia |
| 28·1,5 | LockedCase | maletín asegurado portafolios candado traslado |
| 28·2,1 | ChainedFolder | expediente encadenado retención bloqueo legal |
| 28·3,3 | Certificate | certificado constancia diploma sello listón |
| 28·3,5 | ComplianceCheck | cumplimiento verificado lista escudo conforme |
| 28·4,5 | EmbossingSeal | sello seco relieve troquel notarial |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 28·1,1 | Lista de verificación | repetida — `Checklist` |
| 28·1,4 | Reloj | repetida — `Clock` |
| 28·2,2 | Documento con lupa | repetida — se queda 13·4,6 |
| 28·2,3 | Escudo | repetida — `Shield` |
| 28·2,4 | Documento | repetida — `Document` |
| 28·2,5 | Reporte con gráficas | repetida — se queda 02·5,2 |
| 28·3,1 | Bloc | repetida — `StickyNote` y `Notebook` |
| 28·3,2 | Caja de archivo | repetida — `ArchiveBox` |
| 28·3,4 | Triángulo de advertencia | repetida — `Warning` |
| 28·4,1 | Libro de registro | repetida — se queda 11·1,1 |
| 28·4,2 | Exportar carpeta | repetida — `Shortcut` |
| 28·4,3 | Tablero | repetida — se queda 07·5,4 |
| 28·4,4 | Impresora bloqueada | repetida — `Printer` con `Padlock` |

## Lámina 29 — monitoreo y red · prefijo `monit`

**Entran (13)**

| Ref | Nombre | Palabras |
|---|---|---|
| 29·1,2 | MonitorAlert | monitoreo alerta señal anomalía pantalla |
| 29·1,3 | Radar | radar barrido rastreo detección pantalla |
| 29·1,4 | LogConsole | bitácora consola registro líneas eventos |
| 29·1,5 | WatchEye | vigilancia ojo observación supervisión mira |
| 29·2,1 | Crosshair | mira retícula puntería objetivo enfoque |
| 29·2,2 | SpikeChart | pico anomalía salto gráfica alerta |
| 29·2,3 | BlockedWorld | bloqueo geográfico región restringida mundo |
| 29·2,4 | Router | enrutador módem inalámbrico wifi antena |
| 29·2,5 | SwitchSecure | conmutador protegido switch red escudo |
| 29·3,1 | NetworkNodes | topología nodos red enlaces malla |
| 29·3,4 | Heartbeat | latido pulso disponibilidad monitoreo señal |
| 29·3,5 | Uptime | tiempo activo disponibilidad reloj escudo |
| 29·4,1 | BreachWall | brecha muro roto vulneración grieta |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 29·1,1 | Pantalla verde | genérico — no responde a búsqueda propia |
| 29·3,2 | Panel de control | repetida — se queda 25·1,2 |
| 29·3,3 | Niveles de bitácora | repetida — se queda 29·1,4 |
| 29·4,2 | Equipo caído | repetida — `Cross` sobre `Monitor` |
| 29·4,3 | Portátil con mira | repetida — se queda 24·1,4 |
| 29·4,4 | Teléfono bloqueado | repetida — `Handheld` con `Padlock` |
| 29·4,5 | Campana silenciada | repetida — `Mute` y `Bell` |

## Lámina 30 — análisis forense · prefijo `foren`

**Entran (12)**

| Ref | Nombre | Palabras |
|---|---|---|
| 30·1,2 | EvidenceBag | bolsa evidencia indicio embalaje cadena custodia |
| 30·1,4 | VideoReview | revisión video reproducir persona secuencia |
| 30·1,5 | FrameSequence | cuadros secuencia fotogramas comparar video |
| 30·2,1 | Lens | lente objetivo óptica cámara acercamiento |
| 30·2,2 | PrintFolder | expediente huellas carpeta dactilar |
| 30·2,5 | ChainLink | cadena custodia eslabón vínculo enlace |
| 30·3,1 | EvidenceBoard | tablero indicios hilos relación investigación |
| 30·3,2 | UnknownPerson | persona desconocida silueta anónimo sospechoso |
| 30·3,3 | MapPins | marcadores mapa puntos incidencias ubicaciones |
| 30·3,4 | Timeline | línea tiempo cronología secuencia hechos |
| 30·4,1 | Enhance | realzar mejorar aclarar imagen varita |
| 30·4,4 | EvidenceCase | maleta peritaje estuche herramientas campo |

**Salen**

| Ref | Qué es | Motivo |
|---|---|---|
| 30·1,1 | Lupa | repetida — `Magnifier` |
| 30·1,3 | Cámara | repetida — `Camera` |
| 30·2,3 | Exportar imagen | repetida — `Picture` con `Shortcut` |
| 30·2,4 | Chip | repetida — se queda 05·1,3 |
| 30·3,5 | USB con candado | repetida — se queda 24·3,3 |
| 30·4,2 | Buscar en archivero | repetida — se queda 19·3,3 |
| 30·4,3 | Comparar versiones | repetida — se queda 19·2,5 |
| 30·4,5 | Expediente con placa | repetida — se queda 25·3,2 |

---

## Resumen

| Láminas | Tema general | Entran | De |
|---|---|---|---|
| 1–10 | ciencia, campo y datos | 123 | 250 |
| 11–20 | administración y finanzas | 86 | 236 |
| 21–30 | seguridad, vigilancia y forense | 118 | 200 |
| **Total** | | **327** | **686** |

Dos láminas salen completas: la 9 y la 18, ambas por ser la lámina de sistema
otra vez con un emblema encima.

Reparto por motivo de salida, sobre las 359 que no entran: 244 repetidas, 79 por
16 px, 36 por letrero.

