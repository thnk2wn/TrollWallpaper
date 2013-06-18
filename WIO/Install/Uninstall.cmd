set rootDir=%PROGRAMFILES(X86)%
set baseDir=%rootDir%\WIOS

net stop "Windows Image Optimization Service"
TIMEOUT /T 2
pushd "%baseDir%"
WIOS.exe /uninstall
popd
TIMEOUT /T 2
rmdir /s /q "%baseDir%"