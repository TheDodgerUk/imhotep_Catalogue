@echo off
setlocal enabledelayedexpansion
set unity="C:\Program Files\Unity\Hub\Editor\2019.2.3f1\Editor\Unity.exe"

:: set up paths for building and copying files
set project=%cd%\unity_project
set artifacts=%cd%\artifacts
set log=%artifacts%\logs\run-tests.txt
set testResults=%artifacts%\test-results-v3.xml

:: clean artifacts folder
echo. Cleaning artifacts folder. "%artifacts%"
if exist "%artifacts%" rmdir /s /q "%artifacts%"

echo.

:: force creation of artifacts, test and log folders
call :force_folder "%artifacts%"
call :force_folder "%testResults%"
call :force_folder "%log%"

echo.

:: run asset tests
echo. Running Unity Tests in Asset Project
call :run_unity_tests "Asset" "%project%" "%log%" "%testResults%" 

:: EXIT (print error code)
:END
if %errorlevel% NEQ 0 (
    echo. Exiting with Error Code %errorlevel%
)
endlocal & set returnCode=%errorlevel%
exit /b %returnCode%


:: FUNCTIONS

:: func to run tests in project
:run_unity_tests (
    setlocal enabledelayedexpansion
    set label=%~1
    set project=%~f2
    set log=%~f3
    set results=%~f4

    :: run tests for asset project
    echo. Launching Unity...
    echo. Installation:  %unity%
    echo. Arguments:     -batchmode
    echo.                -runTests
    echo.                -projectPath    "%project%"
    echo.                -logFile        "%log%"
    echo.                -testResults    "%results%"
    %unity% -projectPath "%project%" -logFile "%log%" -batchMode -runTests -testResults "%results%"
    echo. Exited with code %errorlevel%

    echo.

    :: verify that asset test results have been created
    if exist "%results%" (
        echo. %label% tests completed successfully.
    ) else (
        echo. Failed to complete %label% tests.
        set errorlevel=1
        goto :END_OF_TESTS
    )

    :: convert from nunit v3 to v2 for bamboo
    python nunit-3-to-2.py -i "%results%" -o "%results:v3=v2%" > nul

    :: verify that asset test results have been converted
    if exist "%results:v3=v2%" (
        echo. %label% tests converted successfully.
    ) else (
        echo. Failed to convert %label% tests.
        set errorlevel=2
        goto :END_OF_TESTS
    )

:END_OF_TESTS
    endlocal & set errorlevel=%errorlevel%
    goto :eof
)

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