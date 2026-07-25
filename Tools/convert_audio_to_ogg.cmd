@echo off
setlocal EnableExtensions EnableDelayedExpansion

rem Usage:
rem   convert_audio_to_ogg.cmd [root_folder]
rem   convert_audio_to_ogg.cmd --auto [root_folder]
rem
rem Default mode is interactive with staged pauses so output can be reviewed.
rem Use --auto to run non-interactive.

set "AUTO_MODE=0"
if /I "%~1"=="--auto" (
    set "AUTO_MODE=1"
    shift
)

set "SCRIPT_DIR=%~dp0"
set "FFMPEG_EXE="

set "ROOT=%~1"
if "%ROOT%"=="" set "ROOT=%CD%"
if not exist "%ROOT%" (
    echo [ERROR] Root folder does not exist: "%ROOT%"
    exit /b 1
)
for %%I in ("%ROOT%") do set "ROOT=%%~fI"

set "RUNSTAMP=%DATE% %TIME%"
set "LOG_CSV=%ROOT%\ogg_conversion_log.csv"
set "RUN_TXT=%ROOT%\ogg_conversion_last_run.txt"

set /a TOTAL=0
set /a PROCESSED=0
set /a CONVERTED=0
set /a SKIPPED=0
set /a FAILED=0
set "END_REASON=completed"

> "%RUN_TXT%" echo ==================================================
>>"%RUN_TXT%" echo OGG Conversion Run
>>"%RUN_TXT%" echo Root: %ROOT%
>>"%RUN_TXT%" echo Started: %RUNSTAMP%
>>"%RUN_TXT%" echo ==================================================
>>"%RUN_TXT%" echo.

call :StageHeader "Stage 1/5 - Preflight"
echo Root folder: %ROOT%
echo Mode       : %AUTO_MODE% ^(0=interactive, 1=auto^)
echo.
echo Target output format: .ogg ^(same base filename^)
echo Source extensions   : .wav .mp3 .flac .aac .m4a .wma
call :StagePause

call :StageHeader "Stage 2/5 - ffmpeg Setup"
call :EnsureFfmpeg
if errorlevel 1 (
    set "END_REASON=ffmpeg_setup_failed"
    echo [ERROR] ffmpeg setup failed.
    echo [ERROR] Please install ffmpeg manually and rerun.
    set /a FAILED+=1
    call :WriteLogs
    call :PrintSummary
    call :FinalPause
    exit /b 2
)
echo [OK] Using ffmpeg: %FFMPEG_EXE%
call :StagePause

call :StageHeader "Stage 3/5 - Scan Audio Files"
call :CountPattern "*.wav"
call :CountPattern "*.mp3"
call :CountPattern "*.flac"
call :CountPattern "*.aac"
call :CountPattern "*.m4a"
call :CountPattern "*.wma"
echo Found %TOTAL% source audio files to evaluate.
if %TOTAL% LEQ 0 (
    set "END_REASON=no_source_files_found"
    echo [INFO] Nothing to convert.
    call :WriteLogs
    call :PrintSummary
    call :FinalPause
    exit /b 0
)
call :StagePause

call :StageHeader "Stage 4/5 - Convert Files"
if "%AUTO_MODE%"=="0" (
    choice /C YN /N /M "Start conversion now? [Y/N]: "
    if errorlevel 2 (
        set "END_REASON=user_cancelled_before_conversion"
        echo [INFO] Conversion cancelled by user.
        call :WriteLogs
        call :PrintSummary
        call :FinalPause
        exit /b 0
    )
)
echo Live progress will show as [current/total percent].
echo.
call :ProcessPattern "*.wav"
call :ProcessPattern "*.mp3"
call :ProcessPattern "*.flac"
call :ProcessPattern "*.aac"
call :ProcessPattern "*.m4a"
call :ProcessPattern "*.wma"

call :StageHeader "Stage 5/5 - Summary"
call :WriteLogs
call :PrintSummary
call :FinalPause

if %FAILED% GTR 0 exit /b 10
exit /b 0

:StageHeader
echo.
echo ==================================================
echo %~1
echo ==================================================
exit /b 0

:StagePause
if "%AUTO_MODE%"=="1" exit /b 0
echo.
pause
exit /b 0

:FinalPause
if "%AUTO_MODE%"=="1" exit /b 0
echo.
choice /C YN /N /M "Open run summary in Notepad before exit? [Y/N]: "
if errorlevel 2 (
    echo Press any key to close.
    pause >nul
    exit /b 0
)
start "" notepad "%RUN_TXT%" >nul 2>nul
echo Press any key to close.
pause >nul
exit /b 0

:CountPattern
set "PATTERN=%~1"
for /r "%ROOT%" %%F in (%PATTERN%) do (
    set /a TOTAL+=1
)
exit /b 0

:ProcessPattern
set "PATTERN=%~1"
for /r "%ROOT%" %%F in (%PATTERN%) do (
    set /a PROCESSED+=1
    if !TOTAL! LEQ 0 (
        set /a PCT=0
    ) else (
        set /a PCT=PROCESSED*100/TOTAL
    )
    set "SRC=%%~fF"
    set "DST=%%~dpnF.ogg"
    set "STATUS="
    set "MESSAGE="
    set "DO_CONVERT=1"

    echo [!PROCESSED!/!TOTAL! !PCT!%%] Processing: !SRC!

    if exist "!DST!" (
        for %%A in ("!SRC!") do set "SRC_TS=%%~tA"
        for %%A in ("!DST!") do set "DST_TS=%%~tA"
        if "!DST_TS!" GEQ "!SRC_TS!" (
            set /a SKIPPED+=1
            set "STATUS=SKIPPED"
            set "MESSAGE=up_to_date"
            call :Log "!STATUS!" "!SRC!" "!DST!" "!MESSAGE!"
            >>"%RUN_TXT%" echo [SKIPPED] !SRC! ^| up_to_date
            set "DO_CONVERT=0"
        )
    )

    if "!DO_CONVERT!"=="1" (
        "%FFMPEG_EXE%" -hide_banner -loglevel warning -y -i "!SRC!" -vn -c:a libvorbis -q:a 5 "!DST!"
        if errorlevel 1 (
            set /a FAILED+=1
            set "STATUS=FAILED"
            set "MESSAGE=ffmpeg_error"
            echo           [FAILED] !SRC!
        ) else (
            set /a CONVERTED+=1
            set "STATUS=CONVERTED"
            set "MESSAGE=ok"
            echo           [OK] !DST!
        )

        call :Log "!STATUS!" "!SRC!" "!DST!" "!MESSAGE!"
        >>"%RUN_TXT%" echo [!STATUS!] !SRC! ^> !DST! ^| !MESSAGE!
    )
)
exit /b 0

:WriteLogs
if not exist "%LOG_CSV%" (
    >"%LOG_CSV%" echo timestamp,status,source,target,message
)

>>"%RUN_TXT%" echo.
>>"%RUN_TXT%" echo ==================================================
>>"%RUN_TXT%" echo Finished: %DATE% %TIME%
>>"%RUN_TXT%" echo ==================================================
>>"%RUN_TXT%" echo End reason    : %END_REASON%
>>"%RUN_TXT%" echo Total scanned : %TOTAL%
>>"%RUN_TXT%" echo Converted     : %CONVERTED%
>>"%RUN_TXT%" echo Skipped       : %SKIPPED%
>>"%RUN_TXT%" echo Failed        : %FAILED%
exit /b 0

:PrintSummary
echo [DONE] Root: %ROOT%
echo [DONE] End reason    : %END_REASON%
echo [DONE] Total scanned : %TOTAL%
echo [DONE] Converted     : %CONVERTED%
echo [DONE] Skipped       : %SKIPPED%
echo [DONE] Failed        : %FAILED%
echo [DONE] Log CSV       : %LOG_CSV%
echo [DONE] Run summary   : %RUN_TXT%
exit /b 0

:Log
set "L_STATUS=%~1"
set "L_SRC=%~2"
set "L_DST=%~3"
set "L_MSG=%~4"
if not exist "%LOG_CSV%" (
    >"%LOG_CSV%" echo timestamp,status,source,target,message
)
>>"%LOG_CSV%" echo "%DATE% %TIME%","%L_STATUS%","%L_SRC%","%L_DST%","%L_MSG%"
exit /b 0

:EnsureFfmpeg
set "FFMPEG_EXE="
call :LocateFfmpeg
if not errorlevel 1 exit /b 0

where winget >nul 2>nul
if not errorlevel 1 (
    echo [INFO] Trying winget install: Gyan.FFmpeg
    winget install -e --id Gyan.FFmpeg --accept-package-agreements --accept-source-agreements --silent
    call :LocateFfmpeg
    if not errorlevel 1 (
        echo [INFO] ffmpeg installed/found after winget step.
        exit /b 0
    )
)

where choco >nul 2>nul
if not errorlevel 1 (
    echo [INFO] Trying chocolatey install: ffmpeg
    choco install ffmpeg -y
    call :LocateFfmpeg
    if not errorlevel 1 (
        echo [INFO] ffmpeg installed/found after chocolatey step.
        exit /b 0
    )
)

echo [INFO] Falling back to portable ffmpeg download...
call :AcquirePortableFfmpeg
if errorlevel 1 exit /b 1
exit /b 0

:LocateFfmpeg
for /f "delims=" %%P in ('where ffmpeg 2^>nul') do (
    set "FFMPEG_EXE=%%~fP"
    exit /b 0
)

if exist "%LOCALAPPDATA%\Microsoft\WinGet\Packages" (
    for /r "%LOCALAPPDATA%\Microsoft\WinGet\Packages" %%P in (ffmpeg.exe) do (
        set "FFMPEG_EXE=%%~fP"
        exit /b 0
    )
)

if exist "%ProgramData%\chocolatey\bin\ffmpeg.exe" (
    set "FFMPEG_EXE=%ProgramData%\chocolatey\bin\ffmpeg.exe"
    exit /b 0
)

if exist "%SCRIPT_DIR%.ffmpeg\ffmpeg-essentials\bin\ffmpeg.exe" (
    set "FFMPEG_EXE=%SCRIPT_DIR%.ffmpeg\ffmpeg-essentials\bin\ffmpeg.exe"
    exit /b 0
)

exit /b 1

:AcquirePortableFfmpeg
set "PORTABLE_ROOT=%SCRIPT_DIR%.ffmpeg"
set "PORTABLE_EXTRACT=%PORTABLE_ROOT%\ffmpeg-essentials"
set "ZIP_FILE=%PORTABLE_ROOT%\ffmpeg-release-essentials.zip"
set "DOWNLOAD_URL=https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip"

if not exist "%PORTABLE_ROOT%" mkdir "%PORTABLE_ROOT%"

echo [INFO] Downloading %DOWNLOAD_URL%
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; Invoke-WebRequest -Uri '%DOWNLOAD_URL%' -OutFile '%ZIP_FILE%';"
if errorlevel 1 (
    echo [ERROR] Failed downloading portable ffmpeg.
    exit /b 1
)

echo [INFO] Extracting ffmpeg package...
if exist "%PORTABLE_EXTRACT%" rmdir /s /q "%PORTABLE_EXTRACT%"
mkdir "%PORTABLE_EXTRACT%" >nul 2>nul

powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; Expand-Archive -Path '%ZIP_FILE%' -DestinationPath '%PORTABLE_EXTRACT%' -Force;"
if errorlevel 1 (
    echo [ERROR] Failed extracting portable ffmpeg package.
    exit /b 1
)

for /d %%D in ("%PORTABLE_EXTRACT%\*") do (
    if exist "%%~fD\bin\ffmpeg.exe" (
        set "FFMPEG_EXE=%%~fD\bin\ffmpeg.exe"
        exit /b 0
    )
)

if exist "%PORTABLE_EXTRACT%\bin\ffmpeg.exe" (
    set "FFMPEG_EXE=%PORTABLE_EXTRACT%\bin\ffmpeg.exe"
    exit /b 0
)

echo [ERROR] Could not find ffmpeg.exe after extraction.
exit /b 1
