var strLine = "";

var objFS = new ActiveXObject("Scripting.FileSystemObject");

var DIR_PATH = objFS.GetAbsolutePathName("") + "\\";
var PATH = DIR_PATH + "MobileSpecUnitTests\\";

var objInputFile = objFS.OpenTextFile(DIR_PATH + "filelist.txt");
var objOutputFile = objFS.CreateTextFile(DIR_PATH + "temp.txt", true);

while (!objInputFile.AtEndOfStream)
{
	strLine = objInputFile.ReadLine();
	strLine = strLine.replace(PATH, "");
	strLine = strLine.replace(/\\/gi, "/");

	objOutputFile.WriteLine("<FilePath Value=\"" + strLine + "\"/>");
}

objOutputFile.Close();
objInputFile.Close();

objFS.DeleteFile(DIR_PATH + "filelist.txt");

objOutputFile = null;
objInputFile = null;

var objXmlNodeFile = objFS.OpenTextFile(DIR_PATH + "temp.txt");
var objResourceFile = objFS.CreateTextFile(PATH + "GapSourceDictionary.xml", true);

objResourceFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
objResourceFile.WriteLine("<GapSourceDictionary>");
objResourceFile.WriteLine("<FilePath Value=\"www/index.html\"/>");
objResourceFile.WriteLine("<FilePath Value=\"www/master.css\"/>");
objResourceFile.WriteLine("<FilePath Value=\"www/main.js\"/>");

while (!objXmlNodeFile.AtEndOfStream)
{
	strLine = objXmlNodeFile.ReadLine();

	if (strLine.indexOf(".") > 0)
	{
		objResourceFile.WriteLine(strLine);
	}
}

objResourceFile.WriteLine("</GapSourceDictionary>");

objXmlNodeFile.Close();
objResourceFile.Close();

objFS.DeleteFile(DIR_PATH + "temp.txt");


objXmlNodeFile = null;
objResourceFile = null;

objFS = null;

