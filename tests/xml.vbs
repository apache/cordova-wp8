Const dirPath = "C:\Users\herm\Documents\hermwong\phonegap-wp7\tests\"
Dim strLine

Set objFS = CreateObject("Scripting.FileSystemObject")

' create the list of xml nodes and write them into the temp.txt file
Set objInputFile = objFS.OpenTextFile(dirPath & "filelist.txt")
Set objOutputFile = objFS.CreateTextFile(dirPath & "temp.txt", True)

Do Until objInputFile.AtEndOfStream
	strLine = objInputFile.ReadLine
	
	' break out of the loop if EOF character is found
	If asc(left(strLine,1))<32 Then
		objOutputFile.WriteLine(strLine)
		Exit Do
	End If

	objOutputFile.WriteLine("<FilePath Value=""" & strLine & """/>")

Loop

objOutputFile.Close
objInputFile.Close

Set objInputFile = NOTHING
Set objOutputFile = NOTHING

' read the list of xml nodes from temp.txt and create the GapSourceDictionary.xml
Set objXmlNodeFile = objFS.OpenTextFile(dirPath & "temp.txt")
Set objResourceFile = objFS.CreateTextFile(dirPath & "GapSourceDictionary.xml", True)

objResourceFile.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
objResourceFile.WriteLine("<GapSourceDictionary>")
objResourceFile.WriteLine("<FilePath Value=""www/index.html""/>")
objResourceFile.WriteLine("<FilePath Value=""www/master.css""/>")
objResourceFile.WriteLine("<FilePath Value=""www/main.js""/>")

Do Until objXmlNodeFile.AtEndOfStream
	strLine = objXmlNodeFile.ReadLine

	' exit the loop once we reach the EOF character
	If asc(left(strLine,1))<32 Then
		Exit Do
	End If

	If InStr(1, strLine, ".") Then
		objResourceFile.WriteLine(strLine)
	End If
Loop

objResourceFile.WriteLine("</GapSourceDictionary>")

objXmlNodeFile.Close
objResourceFile.Close

Set objXmlNodeFile = NOTHING
Set objResourceFile = NOTHING

Set objFS = NOTHING