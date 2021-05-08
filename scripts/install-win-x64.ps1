$zipurl = "https://github.com/fourtf/bigdiff/releases/download/v1.0/win-x64.zip"
$installpath = "$env:LOCALAPPDATA/bigdiff"
$zipdlpath = "$installpath/dl.zip"

Write-Output "This script will first delete '$installpath', then download '$zipurl' and install it to '$installpath'."

$confirmation = Read-Host "Do you want to proceed? (y/N)"
if ($confirmation -match "[yY]") {
    Remove-Item -Path $installpath
    New-Item -Path $installpath
    Invoke-WebRequest -Uri $zipurl -OutFile $zipdlpath
    Expand-Archive -Path $zipdlpath -DestinationPath $installpath
}