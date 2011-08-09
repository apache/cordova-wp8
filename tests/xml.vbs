Dim strLine

Set objFS = CreateObject("Scripting.FileSystemObject")

Set objInputFile = objFS.OpenTextFile("C:\Users\herm\Documents\hermwong\phonegap-wp7\tests\filelist.txt")
Set objOutputFile = objFS.CreateTextFile("C:\Users\herm\Documents\hermwong\phonegap-wp7\tests\temp.txt", True)

Do Until objFile.AtEndOfStream
	strLine = objInputFile.ReadLine
	
	' break out of the loop if EOF character is found; we do not want the EOF character written into the file
	If asc(left(strLine,1))<32 Then
		Exit Do
	End If

	objOutputFile.WriteLine("<FilePath Value=""" & strLine & """/>")

Loop

objOutputFile.Close
objInputFile.Close

Set objInputFile = NOTHING
Set objOutputFile = NOTHING
Set objFS = NOTHING