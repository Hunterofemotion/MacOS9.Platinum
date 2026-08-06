# Comprueba tres cosas que XAML no falla al compilar y solo revientan en ejecución,
# a veces mucho después de arrancar:
#
#   1. Que cada {StaticResource X} se resuelva con las llaves que su propio
#      diccionario define más las de sus MergedDictionaries. WPF no busca en
#      diccionarios hermanos, así que fusionar el tema completo en la aplicación no
#      salva a un archivo que olvidó fusionar sus dependencias.
#   2. Que ningún TargetName de un trigger apunte a un x:Name inexistente.
#   3. Que ninguna llave se defina en dos archivos, donde la última fusión gana en
#      silencio.
#
# El caso 1 es el que tumbó la galería: PlatinumSubmenuHeader referenciaba una llave
# que ya no existía y la aplicación moría al abrir un submenú, porque un
# StaticResource dentro de un ControlTemplate se resuelve al instanciar la plantilla,
# no al cargar el diccionario.
#
# Uso:  pwsh -File tools/check-resources.ps1     (código de salida 1 si algo falla)

$ErrorActionPreference = 'Stop'
$themes = Join-Path $PSScriptRoot '..\src\MacOS9.Platinum\Themes'
$files = Get-ChildItem $themes -Filter *.xaml

$keys = @{}
$merges = @{}
$refs = @{}
$names = @{}
$targets = @{}

foreach ($f in $files) {
    # Los comentarios se quitan primero: llevan prosa que menciona nombres de llaves.
    $t = [regex]::Replace([System.IO.File]::ReadAllText($f.FullName), '(?s)<!--.*?-->', '')

    $k = New-Object System.Collections.Generic.HashSet[string]
    foreach ($m in [regex]::Matches($t, 'x:Key="([^"]+)"')) { [void]$k.Add($m.Groups[1].Value) }
    $keys[$f.Name] = $k

    $md = New-Object System.Collections.Generic.HashSet[string]
    foreach ($m in [regex]::Matches($t, 'Themes/([A-Za-z0-9_]+)\.xaml')) { [void]$md.Add($m.Groups[1].Value + '.xaml') }
    $merges[$f.Name] = $md

    $r = New-Object System.Collections.Generic.HashSet[string]
    foreach ($m in [regex]::Matches($t, 'StaticResource\s+([^}"\s]+)')) {
        $v = $m.Groups[1].Value
        if ($v -notmatch '^\{') { [void]$r.Add($v) }
    }
    $refs[$f.Name] = $r

    $n = New-Object System.Collections.Generic.HashSet[string]
    foreach ($m in [regex]::Matches($t, 'x:Name="([^"]+)"')) { [void]$n.Add($m.Groups[1].Value) }
    $names[$f.Name] = $n

    $tg = @()
    foreach ($m in [regex]::Matches($t, 'TargetName="([^"]+)"')) { $tg += $m.Groups[1].Value }
    $targets[$f.Name] = $tg
}

function Get-Scope($name, $seen) {
    if ($seen.Contains($name)) { return @() }
    [void]$seen.Add($name)
    $acc = @($name)
    if ($merges.ContainsKey($name)) {
        foreach ($m in $merges[$name]) { $acc += Get-Scope $m $seen }
    }
    return $acc
}

$problems = 0

foreach ($f in $files) {
    $scope = Get-Scope $f.Name (New-Object System.Collections.Generic.HashSet[string])
    $avail = New-Object System.Collections.Generic.HashSet[string]
    foreach ($s in $scope) {
        if ($keys.ContainsKey($s)) { foreach ($k in $keys[$s]) { [void]$avail.Add($k) } }
    }
    foreach ($r in $refs[$f.Name]) {
        if (-not $avail.Contains($r)) {
            Write-Host "LLAVE SIN RESOLVER  $($f.Name)  ->  $r"
            $problems++
        }
    }
    foreach ($t in $targets[$f.Name]) {
        if (-not $names[$f.Name].Contains($t)) {
            Write-Host "TARGETNAME SIN x:Name  $($f.Name)  ->  $t"
            $problems++
        }
    }
}

$owner = @{}
foreach ($f in $files) {
    foreach ($k in $keys[$f.Name]) {
        if (-not $owner.ContainsKey($k)) { $owner[$k] = @() }
        $owner[$k] += $f.Name
    }
}
foreach ($k in ($owner.Keys | Sort-Object)) {
    if ($owner[$k].Count -gt 1) {
        Write-Host "LLAVE DUPLICADA  $k  en  $($owner[$k] -join ', ')"
        $problems++
    }
}

if ($problems -eq 0) {
    Write-Host "OK: $($files.Count) diccionarios, todas las referencias resuelven y no hay llaves duplicadas."
    exit 0
}

Write-Host "TOTAL DE PROBLEMAS: $problems"
exit 1
