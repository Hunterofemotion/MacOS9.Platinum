# Compara lo que dibuja Probe contra las imágenes aprobadas y falla si algún píxel
# cambió.
#
#   pwsh -File tools/check-render.ps1            comprueba
#   pwsh -File tools/check-render.ps1 -Aprobar   acepta lo dibujado como nuevo patrón
#
# Por qué existe. Los defectos de este tema son de un píxel: un filete que se pinta
# dos veces, un borde que se desborda, una sombra de más. Nada de eso rompe la
# compilación ni lanza excepción, así que el único detector era una persona mirando
# capturas. Con una imagen aprobada, la que mira es la prueba.
#
# Aprobar es un acto deliberado: al hacerlo se revisa la imagen y se acepta. Lo que
# no se debe hacer es aprobar sin mirar, porque entonces la prueba solo certifica
# que hoy se ve igual que ayer.
#
# Un patrón por escala. El tamaño de lo dibujado depende de la escala de la
# pantalla: al 200 % todo sale del doble de píxeles que al 100 %, así que un patrón
# hecho en una máquina no le sirve a otra y la comprobación reportaba las dieciocho
# imágenes como distintas sin que nada hubiera cambiado. Por eso cada escala tiene
# su carpeta —`aprobadas/125`, `aprobadas/200`— y se compara contra la que toca.
#
# La alternativa era fijar la sonda al 100 % en todas partes, y sale más cara de lo
# que parece: casi todos los defectos que este tema persigue solo aparecen cuando la
# escala no es entera, porque son líneas de un píxel que caen entre dos. Comprobar
# nada más al 100 % es dejar de mirar justo donde está el problema.

param([switch]$Aprobar)

Add-Type -AssemblyName System.Drawing

$raiz = Split-Path -Parent $PSScriptRoot
$probe = Join-Path $PSScriptRoot "Probe"

Write-Output "Dibujando los escenarios..."
$salida = & dotnet run --project (Join-Path $probe "Probe.csproj") -c Release 2>&1
if ($LASTEXITCODE -ne 0) {
    $salida | Select-Object -Last 10
    throw "Probe no pudo correr."
}

# La escala la reporta la propia sonda al arrancar; se lee de ahí y no del sistema
# para que sea la misma con la que dibujó.
$m = [regex]::Match(($salida -join "`n"), 'DPI del proceso: x([\d.,]+)')
if (-not $m.Success) { throw "La sonda no reportó su escala." }
$escala = [double]::Parse($m.Groups[1].Value.Replace(',', '.'),
    [System.Globalization.CultureInfo]::InvariantCulture)
$etiqueta = [string][int][math]::Round($escala * 100)
Write-Output "Escala de esta corrida: $etiqueta %"

$aprobadas = Join-Path (Join-Path $PSScriptRoot "aprobadas") $etiqueta

$render = Get-ChildItem (Join-Path $probe "bin\Release") -Recurse -Directory -Filter "render" |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($null -eq $render) { throw "Probe no dejó carpeta render." }

if (-not (Test-Path $aprobadas)) {
    if (-not $Aprobar) {
        $otras = (Get-ChildItem (Join-Path $PSScriptRoot "aprobadas") -Directory |
            ForEach-Object { $_.Name + ' %' }) -join ', '
        Write-Output ""
        Write-Output "No hay patrón para $etiqueta %. Los que existen son: $otras."
        Write-Output "No se compara contra otra escala: saldrían las dieciocho como distintas."
        Write-Output "Revisa las imágenes de $($render.FullName) y, si están bien,"
        Write-Output "córrelo con -Aprobar para crear el patrón de $etiqueta %."
        exit 1
    }
    New-Item -ItemType Directory -Force $aprobadas | Out-Null
}

# Un filo en diagonal se suaviza en varios tonos, y esos tonos se corren una
# fracción de píxel de una corrida a otra sin que nada haya cambiado: medido, tres
# píxeles escalonados en el triángulo de una flecha. Tolerarlos es una concesión,
# no un descuido, y los números están puestos para que lo que sí perseguimos siga
# fallando: un filete pintado dos veces es un salto duro a lo largo de una línea
# entera, no tres tonos vecinos.
$SaltoDuro = 64      # diferencia por canal que ya no es suavizado
$SuavesTolerados = 8 # cuántos píxeles suaves pueden bailar

function Diferencia($a, $b) {
    $ia = [System.Drawing.Bitmap]::FromFile($a)
    $ib = [System.Drawing.Bitmap]::FromFile($b)
    try {
        if ($ia.Width -ne $ib.Width -or $ia.Height -ne $ib.Height) {
            return @{ duros = -1; suaves = 0 }
        }
        $duros = 0
        $suaves = 0
        for ($y = 0; $y -lt $ia.Height; $y++) {
            for ($x = 0; $x -lt $ia.Width; $x++) {
                $ca = $ia.GetPixel($x, $y)
                $cb = $ib.GetPixel($x, $y)
                if ($ca.ToArgb() -eq $cb.ToArgb()) { continue }
                $salto = [Math]::Max([Math]::Abs($ca.R - $cb.R),
                         [Math]::Max([Math]::Abs($ca.G - $cb.G), [Math]::Abs($ca.B - $cb.B)))
                if ($salto -ge $SaltoDuro) { $duros++ } else { $suaves++ }
            }
        }
        return @{ duros = $duros; suaves = $suaves }
    }
    finally { $ia.Dispose(); $ib.Dispose() }
}

$nuevas = 0
$cambiadas = 0
$iguales = 0

foreach ($imagen in (Get-ChildItem $render.FullName -Filter "*.png")) {
    $patron = Join-Path $aprobadas $imagen.Name

    if ($Aprobar) {
        Copy-Item $imagen.FullName $patron -Force
        Write-Output "aprobada  $($imagen.Name)"
        continue
    }

    if (-not (Test-Path $patron)) {
        Write-Output "SIN PATRÓN  $($imagen.Name)  (córrelo con -Aprobar después de revisarla)"
        $nuevas++
        continue
    }

    $d = Diferencia $patron $imagen.FullName
    if ($d.duros -lt 0) {
        Write-Output "CAMBIÓ DE TAMAÑO  $($imagen.Name)"
        $cambiadas++
    }
    elseif ($d.duros -gt 0) {
        Write-Output "CAMBIÓ  $($imagen.Name)  ($($d.duros) píxeles con salto duro)"
        $cambiadas++
    }
    elseif ($d.suaves -gt $SuavesTolerados) {
        Write-Output "CAMBIÓ  $($imagen.Name)  ($($d.suaves) píxeles suaves, más de los $SuavesTolerados tolerados)"
        $cambiadas++
    }
    else { $iguales++ }
}

Write-Output ""
if ($Aprobar) { Write-Output "Patrones actualizados en tools/aprobadas."; exit 0 }

Write-Output "$iguales sin cambios, $cambiadas con cambios, $nuevas sin patrón."
if ($cambiadas -gt 0 -or $nuevas -gt 0) { exit 1 }
Write-Output "El render coincide con lo aprobado."
exit 0
