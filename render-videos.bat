@echo off
setlocal enabledelayedexpansion
set unity="C:\Program Files\Unity\Hub\Editor\2019.2.3f1\Editor\Unity.exe"

:: set up paths for building and copying files
set project=%cd%\unity_project
set artifacts=%cd%\artifacts
set log=%artifacts%\logs\video_production.txt
set methodToExecute=VideoRenderer.RenderCatalogueVideos

echo.
:: clean artifacts folder
echo. Cleaning artifacts folder. "%artifacts%"
if exist "%artifacts%" rmdir /s /q "%artifacts%"

:: force creation of artifacts
call :force_folder "%artifacts%"
call :force_folder "%log%"

:: run unity to generate scenes and build lighting
echo. Launching Unity...
echo. Installation:  %unity%
echo. Arguments:	 -batchmode
echo.                -projectPath "%project%"
echo.                -logFile "%log%"
echo.                -executeMethod %methodToExecute%
::echo.                +quality 2
%unity% -projectPath "%project%" -logFile "%log%" -batchmode -executeMethod %methodToExecute%
echo. Exited with code %errorlevel%

:: function to force folder creation
:force_folder (
    setlocal
    set folder=%~dp1

    if not exist "%folder%" (
        echo. Creating folder: "%folder%"
        mkdir "%folder%"
    )

    endlocal
    goto :eof
)

:: EXIT (print error code)
:END
if %errorlevel% NEQ 0 (
    echo. Exiting with Error Code %errorlevel%
)
endlocal & set returnCode=%errorlevel%
exit /b %returnCode%