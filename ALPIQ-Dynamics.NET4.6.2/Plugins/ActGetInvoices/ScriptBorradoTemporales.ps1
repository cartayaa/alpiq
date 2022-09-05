Param(
[Parameter(Mandatory=$True,Position=1)]
[string]$directorio
)
$mascara = "*.txt, *.rar"
$numMins = 60
$curDate = Get-Date
$fechaLimite = $curDate.AddMinutes(-$numMins)
$archivosABorrar = Get-ChildItem  $directorio -Include $mascara 

Get-ChildItem -Path $directorio -Recurse -Force | Where-Object { !$_.PSIsContainer -and $_.CreationTime -lt $fechaLimite } | Remove-Item -Force
