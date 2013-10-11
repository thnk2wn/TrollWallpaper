set rootDir=%PROGRAMFILES(X86)%
set baseDir=%rootDir%\WIO

if exist "%baseDir%\WIO.exe" goto uninstallFirst
if not exist "%baseDir%" mkdir "%baseDir%"

:install
xcopy /y /e bin\*.* "%baseDir%\"
pushd "%baseDir%"
WIO.exe /install
popd

pause
goto end

:uninstallFirst
call Uninstall.cmd
goto install

:end