@echo off
setlocal enabledelayedexpansion
set unity="C:\Program Files\Unity\Hub\Editor\2019.2.3f1\Editor\Unity.exe"
set zip="C:\Program Files\7-Zip\7z.exe"

:: set up paths for building and copying files
set project=%cd%\unity_project
set artifacts=%cd%\artifacts
set log=%artifacts%\logs\build-lighting.txt
set scenesZip=%artifacts%\GeneratedScenes.zip

set scenesPath=%project%\Assets\Artwork\Scenes\GeneratedScenes
set buildScript=BuildSystem.BambooBuilder.BuildLighting

:: clean artifacts folder
echo. Cleaning artifacts folder. "%artifacts%"
if exist "%artifacts%" rmdir /s /q "%artifacts%"

:: force creation of artifacts, scenes and log folders
call :force_folder "%artifacts%"
call :force_folder "%scenesZip%"
call :force_folder "%log%"

echo.

:: run unity to generate scenes and build lighting
echo. Launching Unity...
echo. Installation:  %unity%
echo. Arguments:     -batchmode
echo.                -projectPath "%project%"
echo.                -logFile "%log%"
echo.                -executeMethod %buildScript%
::echo.                +quality 2
%unity% -projectPath "%project%" -logFile "%log%" -batchmode -executeMethod %buildScript%
echo. Exited with code %errorlevel%

if %errorlevel% NEQ 0 (
    set errorlevel=1
    goto :END
)

echo.

:: check for existence of files at scene path
set "sceneFileTypes=.exr .unity .png LightingData.asset NavMesh.asset"
for %%I in (%sceneFileTypes%) do (
    call :list_files %%I "%scenesPath%" && (
        echo. Found %%I files.
    ) || (
        echo. Could not find %%I files.
        set errorLevel=2
        goto :END
    )    
    echo.
)

:: zip up generated scenes for upload to bamboo
echo. Zipping GeneratedScenes at "%scenesPath%"
%zip% a -tzip "%scenesZip%" "%scenesPath%\*" > nul
if exist "%scenesZip%" (
    echo. Successfully zipped up GeneratedScenes to "%cd%%scenesZip%".
) else (
    echo. Failed to zip up GeneratedScenes.
    set errorlevel=3
    goto :END
)

echo. Light Build Complete.


:: EXIT (print error code)
:END
if %errorlevel% NEQ 0 (
    echo. Exiting with Error Code %errorlevel%
)
endlocal & set returnCode=%errorlevel%
exit /b %returnCode%


:: FUNCTIONS

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

:: func to look for file extension in path
:list_files (
    setlocal
    echo. Looking for %1 files in "\\?\%~2"
    for /r "\\?\%~2" %%I in ("*%1") do (
        set fullPath=%%~I
        set rootFolder=\\?\%~2\
        call set path=%%fullPath:!rootFolder!=..%%
        echo. !path!
    )
    if not defined fullPath (
        set errorlevel=1
    )
    endlocal & set errorlevel=%errorlevel%
    goto :eof
)