/*
       Licensed to the Apache Software Foundation (ASF) under one
       or more contributor license agreements.  See the NOTICE file
       distributed with this work for additional information
       regarding copyright ownership.  The ASF licenses this file
       to you under the Apache License, Version 2.0 (the
       "License"); you may not use this file except in compliance
       with the License.  You may obtain a copy of the License at

         http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing,
       software distributed under the License is distributed on an
       "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
       KIND, either express or implied.  See the License for the
       specific language governing permissions and limitations
       under the License.
*/

var fso = WScript.CreateObject('Scripting.FileSystemObject'),
    shell = WScript.CreateObject("shell.application"),
    wscript_shell = WScript.CreateObject("WScript.Shell");

//Set up directory structure of current release
//arguments passed in
var args = WScript.Arguments;

var platformId = "wp7";
var platformName = "WP7";

//Root folder of cordova-windowsphone (i.e C:\Cordova\cordova-windowsphone)
var repoRoot = WScript.ScriptFullName.split('\\wp7\\tooling\\', 1);

//Sub folder containing templates
var templatePath = '\\template';

//Get version number
var versionNum ='0.0.0';

var platformRoot = WScript.ScriptFullName.split('\\tooling\\', 1);\

//  set with the -install switch, default false 
var addToVS = false; 

// help function
function Usage() {
    Log("\nUsage: createTemplates [-install]");
    Log("Build/zips up templates from the local repo")
    Log("    -install : also installs templates to user directory on success\n");
}

// logs messaged to stdout and stderr
function Log(msg, error) {
    if (error) {
        WScript.StdErr.WriteLine(msg);
    }
    else {
        WScript.StdOut.WriteLine(msg);
    }
}

// deletes file if it exists
function deleteFileIfExists(path) {
    if(fso.FileExists(path)) {
        fso.DeleteFile(path);
   }
}

function deleteFolderIfExists(path) {
    if(fso.FolderExists(path)) {
        fso.DeleteFolder(path);
    }
}

var ForReading = 1, ForWriting = 2, ForAppending = 8;
var TristateUseDefault = -2, TristateTrue = -1, TristateFalse = 0;

// returns the contents of a file
function read(filename) {
    WScript.Echo('Reading in ' + filename);
    if(fso.FileExists(filename))
    {
        var f=fso.OpenTextFile(filename, 1,2);
        var s=f.ReadAll();
        f.Close();
        return s;
    }
    else
    {
        WScript.StdErr.WriteLine('Cannot read non-existant file : ' + filename);
        WScript.Quit(1);
    }
    return null;
}

// writes the contents to the specified file
function write(filename, contents) {
    var f=fso.OpenTextFile(filename, ForWriting, TristateTrue);
    f.Write(contents);
    f.Close();
}

// replaces the matches of regexp with replacement
function replaceInFile(filename, regexp, replacement) {
    //WScript.Echo("Replaceing with "+replacement+ " in:");
    var text = read(filename).replace(regexp,replacement);
    //WScript.Echo(text);
    write(filename,text);
}

// executes a commmand in the shell
function exec(command) {
    Log("exec:" + command);
    var oShell=wscript_shell.Exec(command);
    var maxTime = 5000;
    while (oShell.Status === 0) {
        if(maxTime > 0) {
            maxTime -= 100;
            WScript.sleep(100);
        }
        else {
            Log("Exec timed out with command :: " + command);
        }
    }
}

// executes a commmand in the shell
function exec_verbose(command) {
    //Log("Command: " + command);
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status === 0) {
        //Wait a little bit so we're not super looping
        WScript.sleep(100);
        //Print any stdout output from the script
        if(!oShell.StdOut.AtEndOfStream) {
            var line = oShell.StdOut.ReadLine();
            Log(line);
        }
    }
    //Check to make sure our scripts did not encounter an error
    if(!oShell.StdErr.AtEndOfStream)
    {
        var err_line = oShell.StdErr.ReadAll();
        WScript.StdErr.WriteLine(err_line);
        WScript.Quit(1);
    }
}

function copyFile(src,dest) {
    exec('%comspec% /c copy /Y ' + src + ' ' + dest);
}


// packages templates into .zip
function package_templates()
{
    Log("Creating template .zip files ...");
    var templateOutFilename = repoRoot + '\\CordovaWP7_' + versionNum.replace(/\./g, '_') + '.zip';
    
    // clear the destination
    deleteFileIfExists(templateOutFilename);

    // clean the template folder, if the project was opened in VS, it creates some useless cruft.
    deleteFolderIfExists(platformRoot + templatePath + '\\Bin');
    deleteFolderIfExists(platformRoot + templatePath + '\\obj');
    deleteFolderIfExists(platformRoot + templatePath + '\\Service\ References');

    deleteFileIfExists(platformRoot + templatePath + "\\CordovaWP7Solution.v11.suo");

    exec('%comspec% /c xcopy /Y /E /I ' + repoRoot + '\\Plugins ' + platformRoot + templatePath + '\\Plugins');
    copyFile(repoRoot + '\\VERSION',platformRoot + templatePath);

    // update .vstemplate files for the template zips.
    var name_regex = /CordovaWP7[_](\d+)[_](\d+)[_](\d+)(rc\d)?/g;
    var discript_regex = /Cordova\s*(\d+)[.](\d+)[.](\d+)(rc\d)?/;

    replaceInFile(platformRoot + templatePath + '\\MyTemplate.vstemplate', name_regex,  'CordovaWP7_' + versionNum.replace(/\./g, '_'));
    replaceInFile(platformRoot + templatePath + '\\MyTemplate.vstemplate', discript_regex,  "Cordova " + versionNum);


    zip_project(templateOutFilename, platformRoot + templatePath);

    if(addToVS)
    {
        var template_dir = wscript_shell.ExpandEnvironmentStrings("%USERPROFILE%") + '\\Documents\\Visual Studio 2012\\Templates\\ProjectTemplates\\Testing';
        if(fso.FolderExists(template_dir ))
        {
            Log("template_dir = " + template_dir);
            dest = shell.NameSpace(template_dir);
            dest.CopyHere(templateOutFilename, 4|20);
        }
        else
        {
            Log("WARNING: Could not find template directory in Visual Studio,\n you can manually copy over the template .zip files.");
        }
  }
}

function zip_project(zip_path, project_path) {
    // create empty ZIP file and open for adding
    var file = fso.CreateTextFile(zip_path, true);

    // create twenty-two byte "fingerprint" for .zip
    file.write("PK");
    file.write(String.fromCharCode(5));
    file.write(String.fromCharCode(6));
    file.write('\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0');
    file.Close();

    // open .zip folder and copy contents of project_path to zip_path
    var zipFolder = shell.NameSpace(zip_path);
    Log("zip_path = " + zip_path);
    Log("zipFolder = " + zipFolder);
    Log("project_path = " + project_path);

    var sourceItems = shell.NameSpace(project_path).items();
    if (zipFolder !== null) {
        zipFolder.CopyHere(sourceItems, 4|16|512|1024);
        var maxTime = 5000;
        Log("sourceItems.Count = " + sourceItems.Count);
        while(zipFolder.items().Count < sourceItems.Count)
        {
            maxTime -= 100;
            if(maxTime > 0 ) {
                WScript.Sleep(100);
            }
            else {
                Log('Timeout. Failed to create .zip file.', true);
                break;
            }
        }
        Log("zipFolder.items().Count = " + zipFolder.items().Count);
    }
    else {
        Log('Failed to create .zip file.', true);
    }
}

// delete any unneeded files when finished
function cleanUp() {

}

function parseArgs() {
    if(args.Count() > 0) {

        //Support help flags -help, --help, /? 
        if(args(0).indexOf("-help") > -1 || 
           args(0).indexOf("/?") > -1 ) {
            Usage();
            WScript.Quit(1);
        }
        else if(args(0).indexOf("-install") > -1) {
            addToVS = true;
        }
    }
}

// MAIN
parseArgs();
// build/package the templates
versionNum = read(repoRoot + "\\VERSION");
package_templates(repoRoot);
cleanUp();