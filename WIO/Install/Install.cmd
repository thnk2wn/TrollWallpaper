REM @echo off
set rootDir=%PROGRAMFILES(X86)%
set baseDir=%rootDir%\WIOS

if exist "%baseDir%\WIOS.exe" goto uninstallFirst
if not exist "%baseDir%" mkdir "%baseDir%"

:install
xcopy /y /e bin\*.* "%baseDir%\"
pushd "%baseDir%"
WIOS.exe /install
REM Grant full control on baseDir and subfolders to NETWORK SERVICE for creating Logs, Images
icacls "%baseDir%" /grant "NETWORK SERVICE":(OI)(CI)F
popd

pause
goto end

:uninstallFirst
call Uninstall.cmd
goto install

:end