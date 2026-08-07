# Comprueba que cada PART_ declarado por las plantillas de fábrica de WPF exista
# también en las nuestras.
#
#   pwsh -File tools/check-parts.ps1
#
# Por qué existe. Una plantilla propia reemplaza a la de fábrica entera, y el
# control busca sus piezas por nombre: si falta un PART_, la pieza simplemente no
# aparece y con ella se van las conductas que el control ya traía resueltas. Nada
# de eso falla al compilar ni lanza excepción; se descubre cuando alguien nota que
# algo dejó de funcionar.
#
# Pasó de verdad: sin PART_HeaderGripper se perdieron el arrastre para
# redimensionar columnas y el doble clic para ajustarlas al contenido, y nadie se
# enteró hasta que un usuario lo pidió como si fuera una función nueva.
#
# Las plantillas de fábrica de tools/StockTemplates/stock se regeneran con:
#   dotnet run --project tools/StockTemplates

$raiz = Split-Path -Parent $PSScriptRoot
$stock = Join-Path $PSScriptRoot "stock"
if (-not (Test-Path $stock)) { $stock = Join-Path $PSScriptRoot "StockTemplates\stock" }
$temas = Join-Path $raiz "src\MacOS9.Platinum\Themes"

if (-not (Test-Path $stock)) { throw "No están las plantillas de fábrica en $stock." }
if (-not (Test-Path $temas)) { throw "No está la carpeta de temas en $temas." }

# Qué archivo del tema cubre a cada plantilla de fábrica. Un PART_ puede vivir en
# cualquiera de los archivos listados.
$mapa = @{
    "ScrollBar.vertical.plantilla.xaml"    = @("ScrollBar.xaml")
    "ScrollViewer.plantilla.xaml"          = @("ListView.xaml", "TextBox.xaml", "TreeView.xaml")
    "GridViewScrollViewer.estilo.xaml"     = @("ListView.xaml")
    "GridViewColumnHeader.plantilla.xaml"  = @("ListView.xaml")
    "ListView.plantilla.xaml"              = @("ListView.xaml")
    "ListViewItem.plantilla.xaml"          = @("ListView.xaml")
}

function PartesDe($ruta) {
    $texto = Get-Content $ruta -Raw
    $encontradas = [regex]::Matches($texto, 'Name="(PART_[A-Za-z0-9_]+)"')
    return $encontradas | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
}

$faltantes = 0
$revisadas = 0

foreach ($archivo in (Get-ChildItem $stock -Filter "*.xaml")) {
    if (-not $mapa.ContainsKey($archivo.Name)) {
        Write-Output "sin correspondencia declarada: $($archivo.Name)"
        continue
    }

    $esperadas = PartesDe $archivo.FullName
    if (-not $esperadas) { continue }

    $nuestras = @()
    foreach ($t in $mapa[$archivo.Name]) {
        $ruta = Join-Path $temas $t
        if (Test-Path $ruta) { $nuestras += PartesDe $ruta }
    }

    foreach ($parte in $esperadas) {
        $revisadas++
        if ($nuestras -notcontains $parte) {
            $donde = $mapa[$archivo.Name] -join ", "
            Write-Output "FALTA  $parte  (la declara $($archivo.Name); debería estar en $donde)"
            $faltantes++
        }
    }
}

Write-Output ""
if ($faltantes -eq 0) {
    Write-Output "Conformidad correcta: las $revisadas piezas de fábrica están presentes."
    exit 0
}

Write-Output "Faltan $faltantes de $revisadas piezas."
exit 1
