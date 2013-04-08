@ECHO OFF
SET full_path=%~dp0
IF EXIST %full_path%lib\log.js (
    cscript "%full_path%lib\log.js" %* //nologo
) ELSE (
    ECHO.
    ECHO ERROR: Could not find 'log.js' in cordova/lib, aborting...>&2
    EXIT /B 1
)