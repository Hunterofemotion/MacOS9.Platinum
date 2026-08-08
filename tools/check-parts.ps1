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
    "ScrollBar.Horizontal.plantilla.xaml"  = @("ScrollBar.xaml")
    "ScrollViewer.plantilla.xaml"          = @("ListView.xaml", "TextBox.xaml", "TreeView.xaml", "NavRail.xaml")
    "GridViewScrollViewer.estilo.xaml"     = @("ListView.xaml")
    "GridViewColumnHeader.plantilla.xaml"  = @("ListView.xaml")
    "ListView.plantilla.xaml"              = @("ListView.xaml")
    "ListViewItem.plantilla.xaml"          = @("ListView.xaml")

    "Button.plantilla.xaml"                = @("Button.xaml")
    "RepeatButton.plantilla.xaml"          = @("Button.xaml", "ScrollBar.xaml", "DateTimeField.xaml")
    "ToggleButton.plantilla.xaml"          = @("Button.xaml", "ComboBox.xaml", "TreeView.xaml", "Expander.xaml")
    "CheckBox.plantilla.xaml"              = @("Selection.xaml")
    "RadioButton.plantilla.xaml"           = @("Selection.xaml")
    "TextBox.plantilla.xaml"               = @("TextBox.xaml")
    "PasswordBox.plantilla.xaml"           = @("PasswordBox.xaml")
    "ComboBox.plantilla.xaml"              = @("ComboBox.xaml")
    "Slider.Horizontal.plantilla.xaml"     = @("Slider.xaml")
    "Slider.Vertical.plantilla.xaml"       = @("Slider.xaml")
    "Thumb.plantilla.xaml"                 = @("Slider.xaml", "ScrollBar.xaml", "ListView.xaml")
    "ProgressBar.plantilla.xaml"           = @("ProgressBar.xaml")
    "TabControl.plantilla.xaml"            = @("TabControl.xaml")
    "TabItem.plantilla.xaml"               = @("TabControl.xaml")
    "TreeView.plantilla.xaml"              = @("TreeView.xaml")
    "TreeViewItem.plantilla.xaml"          = @("TreeView.xaml")
    "ListBox.plantilla.xaml"               = @("ListView.xaml", "NavRail.xaml")
    "ListBoxItem.plantilla.xaml"           = @("ListView.xaml", "NavRail.xaml")
    "GroupBox.plantilla.xaml"              = @("GroupBox.xaml")
    "Expander.plantilla.xaml"              = @("Expander.xaml")
    "Menu.plantilla.xaml"                  = @("Menu.xaml")
    "MenuItem.plantilla.xaml"              = @("Menu.xaml")
    "ToolBar.plantilla.xaml"               = @("ToolBar.xaml")
    "StatusBar.plantilla.xaml"             = @("StatusBar.xaml")
}

# Piezas que se dejan fuera a propósito, con el motivo. Van aquí y no se resuelven
# poniendo una pieza vacía en la plantilla: una pieza falsa callaría al comprobador
# y escondería la decisión. Se imprimen en cada corrida para que la omisión siga a
# la vista de quien lea la salida.
$excepciones = @{
    "PART_ToolBarPanel"         = "La barra de Mac OS 9 no se reacomoda: lo que no cabe se recorta, como en la barra de menús. El panel de fábrica existe para repartir entre banda y desbordamiento."
    "PART_ToolBarOverflowPanel" = "Sin tecla de desbordamiento por diseño. Una barra que esconde teclas detrás de una flecha no es de este tema."
}

function PartesDe($ruta) {
    $texto = Get-Content $ruta -Raw
    $encontradas = [regex]::Matches($texto, 'Name="(PART_[A-Za-z0-9_]+)"')
    return $encontradas | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
}

$faltantes = 0
$revisadas = 0
$omitidas = @()

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
        if ($nuestras -contains $parte) { continue }

        if ($excepciones.ContainsKey($parte)) {
            $omitidas += $parte
            continue
        }

        $donde = $mapa[$archivo.Name] -join ", "
        Write-Output "FALTA  $parte  (la declara $($archivo.Name); debería estar en $donde)"
        $faltantes++
    }
}

if ($omitidas.Count -gt 0) {
    Write-Output ""
    Write-Output "Omitidas a propósito:"
    foreach ($parte in ($omitidas | Sort-Object -Unique)) {
        Write-Output "  $parte"
        Write-Output "    $($excepciones[$parte])"
    }
}

Write-Output ""
if ($faltantes -eq 0) {
    Write-Output "Conformidad correcta: las $revisadas piezas de fábrica están presentes o justificadas."
    exit 0
}

Write-Output "Faltan $faltantes de $revisadas piezas."
exit 1
