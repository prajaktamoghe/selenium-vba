@echo off

REM Debuger : Firebug + chromebug

set Path=C:\WINDOWS;C:\WINDOWS\system32;C:\Progra~1\7-Zip
set APP_NAME="vb-formatters-1.0.1.xpi"

REM controle de la presence des repertoires et fichiers
for /f "usebackq tokens=1,2 delims==" %%a in (`SET ^| find "_dir=" `) do IF NOT EXIST %%b echo Directory not found : %%a=%%b &pause&exit
for /f "usebackq tokens=1,2 delims==" %%a in (`SET ^| find "_path=" `) do IF NOT EXIST %%b echo File not found : %%a=%%b &pause&exit
for %%a in ("%PATH:;=";"%") DO IF NOT EXIST %%a echo Directory not found in PATH : %%a &pause&exit

echo "Generating %APP_NAME%..."

IF EXIST %APP_NAME%*  Del /Q %APP_NAME%*
	
7z a %APP_NAME% -r -tzip chrome\*.* chrome.manifest install.rdf


pause
exit