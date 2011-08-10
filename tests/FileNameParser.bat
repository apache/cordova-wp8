::set "dirpath=c:\Users\herm\Documents\hermwong\phonegap-wp7\tests\MobileSpecUnitTests\"

set dirpath=%CD%"\MobileSpecUnitTests\"

dir /s /b %dirpath%www\*.* > temp.txt

:: remove local path
BatchSubstitute.bat %dirpath% "" temp.txt > filelist.txt

:: temp.txt is re-used by the xml.js to store the xml nodes