set "dirpath=c:\Users\herm\Documents\hermwong\phonegap-wp7\tests\MobileSpecUnitTests\"

:: create temp list of all files in www folder
dir /s /b %dirpath%www\*.html > html.txt
dir /s /b %dirpath%www\*.css > css.txt
dir /s /b %dirpath%www\*.js > js.txt
copy html.txt+css.txt+js.txt c:\Users\herm\Documents\hermwong\phonegap-wp7\tests\temp.txt

:: remove temp files
del html.txt
del css.txt
del js.txt

:: remove local path
BatchSubstitute.bat %dirpath% "" temp.txt > filelist.txt


