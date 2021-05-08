$runtime_identifiers = "win-x64", "linux-x64", "osx-x64"

Write-Output "----- creating ./publish -----"
New-Item -Path ./publish -ItemType Directory -Force

foreach ($ident in $runtime_identifiers) {
    Write-Output "----- building $ident -----"
    dotnet publish -r $ident -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true

    Write-Output "----- compressing $ident -----"
    Compress-Archive "bin/Release/net5.0/$ident/publish/*" "publish/$ident.zip" -Force
}


Write-Output "Remember to update 'Program.fs' and 'scripts/install-win-x64.ps1'!"