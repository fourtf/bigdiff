$zipurl = "https://github.com/fourtf/bigdiff/releases/download/v1.0/win-x64.zip"
$installpath = "$env:LOCALAPPDATA/bigdiff"
$zipdlpath = "$installpath/dl.zip"

Write-Output "This script will first delete '$installpath' (if it exists), then download '$zipurl' and install it to '$installpath'."

$confirmation = Read-Host "Do you want to proceed? (y/N)"
if ($confirmation -match "[yY]") {
    if (Test-Path -Path $installpath) {
        Remove-Item -Path $installpath
    }
    New-Item -Path $installpath -ItemType Directory
    Invoke-WebRequest -Uri $zipurl -OutFile $zipdlpath
    Expand-Archive -Path $zipdlpath -DestinationPath $installpath
    Remove-Item -Path $zipdlpath


    $path = [System.Environment]::GetEnvironmentVariable('Path', [System.EnvironmentVariableTarget]::User)
    if (-not $path.ToUpperInvariant().Split(';').Contains($installpath.ToUpperInvariant())) {
        $confirmation = Read-Host "Do you want to add '$installpath' to your users PATH variable? (y/N)"

        if ($confirmation -match "[yY]") {
            [System.Environment]::SetEnvironmentVariable('Path', $path + ";" + $installpath, [System.EnvironmentVariableTarget]::User);
        }
    }
}