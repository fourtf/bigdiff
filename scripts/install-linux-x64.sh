zipurl="https://github.com/fourtf/bigdiff/releases/download/v1.0/linux-x64.zip"
installpath="$HOME/.local/bin/bigdiff"
zipname="dl.zip"
zipdlpath="$installpath/$zipname"

echo "This script requires curl and unzip to be in PATH."
echo "This script will first delete '$installpath' (if it exists), then download '$zipurl' and install it to '$installpath'."

promptyn () {
    while true; do
        read -p "$1 (y/n)" yn
        case $yn in
            [Yy]* ) return 0;;
            [Nn]* ) return 1;;
            * ) echo "Please enter y or n.";;
        esac
    done
}

if promptyn "Do you want to proceed?"; then
    rm -rf "$installpath"
    mkdir -p "$installpath"
    wget -O "$zipdlpath" "$zipurl"
    cd "$installpath"
    unzip "$zipname"
    rm "$zipdlpath"
    chmod +x bigdiff

    echo "You may want to add '$installpath' to PATH"
fi
