@ECHO OFF
SET full_path=%~dp0
IF EXIST %full_path%lib\deploy.js (
        cscript "%full_path%lib\run.js" %* //nologo
) ELSE (
    ECHO.
    ECHO ERROR: Could not find 'run.js' in cordova/lib, aborting...>&2
    EXIT /B 1
)
