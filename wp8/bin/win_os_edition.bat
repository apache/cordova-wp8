@ECHO OFF
SET script_path="%~dp0win_os_edition.js"
IF EXIST %script_path% (
    cscript %script_path% %* //nologo
) ELSE (
    ECHO.
    ECHO ERROR: Could not find 'win_os_edition.js' in 'bin' folder, aborting...>&2
    EXIT /B 1
)