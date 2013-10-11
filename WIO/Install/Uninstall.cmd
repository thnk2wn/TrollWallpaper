set rootDir=%PROGRAMFILES(X86)%
set baseDir=%rootDir%\WIO

taskkill /im WIO.exe
TIMEOUT /T 2
rmdir /s /q "%baseDir%"