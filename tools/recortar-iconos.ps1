# Recorta las piezas de una lámina de iconos y las deja con fondo transparente.
#
# La rejilla se descubre sola: se cuentan los píxeles distintos del fondo por
# columna y por renglón, y las franjas donde no hay ninguno son las calles entre
# celdas. Así funciona igual con una lámina de 4x3 que con una de 5x5, que es lo
# que hace falta cuando el generador no obedece el acomodo pedido.
#
# El fondo se quita por relleno desde la orilla hacia adentro y no por color: los
# iconos traen grises casi iguales al fondo, y borrar por color les abriría
# agujeros. Lo que está encerrado por el contorno nunca se alcanza desde afuera.
#
#   powershell -File tools/recortar-iconos.ps1 -Hoja "ruta\lamina.png" -Prefijo medios

param(
    [Parameter(Mandatory = $true)][string]$Hoja,
    [Parameter(Mandatory = $true)][string]$Prefijo,
    # Se resuelve abajo y no aquí: en el bloque param todavía no existe
    # $PSScriptRoot cuando el guion corre con -File.
    [string]$Destino = '',
    # Cuánto se puede parecer un píxel al fondo y seguir contando como fondo. Con
    # 30 se lleva también la sombra suave, que es lo que se quiere: el tema pinta
    # sus propias sombras y una horneada en el mapa de bits se ve mal en una lista.
    [int]$Tolerancia = 30,
    # Para buscar las calles entre celdas se es más exigente: con la tolerancia
    # del recorte, la sombra suave de una pieza llega hasta la vecina y las dos
    # columnas se detectan como una sola.
    [int]$ToleranciaRejilla = 60
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

if ([string]::IsNullOrEmpty($Destino)) {
    $Destino = Join-Path $PSScriptRoot '..\src\MacOS9.Platinum.Gallery\Recursos\IconosPixel'
}
New-Item -ItemType Directory -Force $Destino | Out-Null

# ---- Lectura de la lámina a un arreglo de bytes -------------------------

$origen = [System.Drawing.Bitmap]::FromFile((Resolve-Path $Hoja))
$ancho = $origen.Width
$alto = $origen.Height

$lienzo = New-Object System.Drawing.Bitmap $ancho, $alto, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($lienzo)
$g.DrawImage($origen, 0, 0, $ancho, $alto)
$g.Dispose()
$origen.Dispose()

$rect = New-Object System.Drawing.Rectangle 0, 0, $ancho, $alto
$datos = $lienzo.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadWrite,
    [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$paso = $datos.Stride
$bytes = New-Object 'byte[]' ($paso * $alto)
[System.Runtime.InteropServices.Marshal]::Copy($datos.Scan0, $bytes, 0, $bytes.Length)

# El fondo se toma de la esquina, que es donde con seguridad no hay dibujo.
$fB = $bytes[0]; $fG = $bytes[1]; $fR = $bytes[2]

function EsFondo([int]$x, [int]$y, [int]$tol) {
    $i = ($y * $paso) + ($x * 4)
    $d = [math]::Max([math]::Abs($bytes[$i] - $fB),
         [math]::Max([math]::Abs($bytes[$i + 1] - $fG), [math]::Abs($bytes[$i + 2] - $fR)))
    return $d -le $tol
}

# ---- Descubrir la rejilla ----------------------------------------------

function Franjas([bool[]]$ocupado) {
    $tramos = @()
    $inicio = -1
    for ($i = 0; $i -lt $ocupado.Length; $i++) {
        if ($ocupado[$i] -and $inicio -lt 0) { $inicio = $i }
        elseif (-not $ocupado[$i] -and $inicio -ge 0) {
            $tramos += , @($inicio, ($i - 1))
            $inicio = -1
        }
    }
    if ($inicio -ge 0) { $tramos += , @($inicio, ($ocupado.Length - 1)) }
    return , $tramos
}

$colOcupada = New-Object 'bool[]' $ancho
$renOcupado = New-Object 'bool[]' $alto

for ($y = 0; $y -lt $alto; $y++) {
    for ($x = 0; $x -lt $ancho; $x++) {
        if (-not (EsFondo $x $y $ToleranciaRejilla)) {
            $colOcupada[$x] = $true
            $renOcupado[$y] = $true
        }
    }
}

$columnas = Franjas $colOcupada
$renglones = Franjas $renOcupado

# Los tramos muy angostos son ruido del suavizado, no una columna de iconos.
$columnas = @($columnas | Where-Object { ($_[1] - $_[0]) -gt 20 })
$renglones = @($renglones | Where-Object { ($_[1] - $_[0]) -gt 20 })

Write-Output "Rejilla detectada: $($columnas.Count) columnas x $($renglones.Count) renglones"

# ---- Recorte pieza por pieza -------------------------------------------

function Guardar([System.Drawing.Bitmap]$pieza, [string]$nombre, [int]$lado) {
    $chico = New-Object System.Drawing.Bitmap $lado, $lado, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $gg = [System.Drawing.Graphics]::FromImage($chico)
    $gg.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $gg.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $gg.DrawImage($pieza, 0, 0, $lado, $lado)
    $gg.Dispose()
    $chico.Save((Join-Path $Destino "$nombre-$lado.png"), [System.Drawing.Imaging.ImageFormat]::Png)
    $chico.Dispose()
}

$cuenta = 0
for ($f = 0; $f -lt $renglones.Count; $f++) {
    for ($c = 0; $c -lt $columnas.Count; $c++) {
        # La celda se ensancha un poco antes de rellenar: las calles se buscaron
        # con la tolerancia exigente, así que su borde puede caer dentro de la
        # sombra de la pieza y el relleno no tendría de dónde arrancar.
        $holgura = 14
        $x1 = [math]::Max(0, $columnas[$c][0] - $holgura)
        $x2 = [math]::Min($ancho - 1, $columnas[$c][1] + $holgura)
        $y1 = [math]::Max(0, $renglones[$f][0] - $holgura)
        $y2 = [math]::Min($alto - 1, $renglones[$f][1] + $holgura)
        $w = $x2 - $x1 + 1
        $h = $y2 - $y1 + 1

        # Relleno desde la orilla de la celda: lo que se alcanza desde afuera es
        # fondo, lo que queda encerrado por el contorno es el icono.
        $visto = New-Object 'bool[]' ($w * $h)
        $pila = New-Object System.Collections.Generic.Stack[int]

        for ($x = 0; $x -lt $w; $x++) {
            $pila.Push($x)
            $pila.Push((($h - 1) * $w) + $x)
        }
        for ($y = 0; $y -lt $h; $y++) {
            $pila.Push($y * $w)
            $pila.Push(($y * $w) + $w - 1)
        }

        while ($pila.Count -gt 0) {
            $p = $pila.Pop()
            if ($visto[$p]) { continue }
            $px = $p % $w
            $py = [math]::Floor($p / $w)
            if (-not (EsFondo ($x1 + $px) ($y1 + $py) $Tolerancia)) { continue }
            $visto[$p] = $true
            if ($px -gt 0) { $pila.Push($p - 1) }
            if ($px -lt ($w - 1)) { $pila.Push($p + 1) }
            if ($py -gt 0) { $pila.Push($p - $w) }
            if ($py -lt ($h - 1)) { $pila.Push($p + $w) }
        }

        # Caja del contenido que sobrevivió
        $minX = $w; $maxX = -1; $minY = $h; $maxY = -1
        for ($y = 0; $y -lt $h; $y++) {
            for ($x = 0; $x -lt $w; $x++) {
                if ($visto[($y * $w) + $x]) { continue }
                if ($x -lt $minX) { $minX = $x }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
        if ($maxX -lt 0) { continue }

        # Se cuadra: un icono no cuadrado se deforma al meterlo en una caja
        # cuadrada, así que se centra dentro del lado mayor.
        $anchoPieza = $maxX - $minX + 1
        $altoPieza = $maxY - $minY + 1
        $lado = [math]::Max($anchoPieza, $altoPieza)
        $desX = [int](($lado - $anchoPieza) / 2)
        $desY = [int](($lado - $altoPieza) / 2)

        $pieza = New-Object System.Drawing.Bitmap $lado, $lado, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        for ($y = 0; $y -lt $altoPieza; $y++) {
            for ($x = 0; $x -lt $anchoPieza; $x++) {
                if ($visto[(($minY + $y) * $w) + ($minX + $x)]) { continue }
                $i = (($y1 + $minY + $y) * $paso) + (($x1 + $minX + $x) * 4)
                $col = [System.Drawing.Color]::FromArgb(255, $bytes[$i + 2], $bytes[$i + 1], $bytes[$i])
                $pieza.SetPixel(($desX + $x), ($desY + $y), $col)
            }
        }

        $nombre = "{0}-r{1}c{2}" -f $Prefijo, ($f + 1), ($c + 1)
        Guardar $pieza $nombre 128
        Guardar $pieza $nombre 32
        Guardar $pieza $nombre 16
        $pieza.Dispose()
        $cuenta++
    }
}

$lienzo.UnlockBits($datos)
$lienzo.Dispose()
Write-Output "$cuenta piezas en $Destino"
