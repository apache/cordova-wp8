
@echo off

SET REG_QUERY="HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildRuntimeVersion

REG QUERY %REG_QUERY% > nul 2>&1
if ERRORLEVEL 1 goto MissingMSBuildRegistry

for /f "skip=2 tokens=2,*" %%A in ('REG QUERY %REG_QUERY%') do SET MSBUILDRTVERSION=%%B
echo %MSBUILDRTVERSION%
goto:eof

:MissingMSBuildRegistry
echo ERROR: Cannot obtain MSBuild version info from registry
goto:eof

