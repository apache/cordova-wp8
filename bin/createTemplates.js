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

/* jshint wsh:true, node:false, bitwise:false */

var fso = WScript.CreateObject('Scripting.FileSystemObject'),
    shell = WScript.CreateObject('shell.application'),
    wscript_shell = WScript.CreateObject('WScript.Shell');

//Set up directory structure of current release
//arguments passed in
var args = WScript.Arguments;

//Root folder of cordova-windowsphone (i.e C:\Cordova\cordova-windowsphone)
var repoRoot = WScript.ScriptFullName.split('\\wp8\\bin\\', 1);

//Sub folder containing templates
var templatePath = '\\template';

//Get version number
var versionNum ='0.0.0';

var platformRoot = WScript.ScriptFullName.split('\\bin\\', 1);

//  set with the -install switch, default false
var addToVS = false;

// help function
function Usage() {
    Log('\nUsage: createTemplates [-install]');
    Log('Build/zips up templates from the local repo');
    Log('    -install : also installs templates to user directory on success\n');
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

// returns the contents of a file
function read(filename) {
    if(fso.FileExists(filename))
    {
        var f=fso.OpenTextFile(filename, 1,2);
        var s=f.ReadAll();
        f.Close();
        return s;
    }
    else
    {
        Log('Cannot read non-existant file : ' + filename,true);
        WScript.Quit(1);
    }
    return null;
}

// executes a commmand in the shell
function exec(command) {
    var oShell=wscript_shell.Exec(command);
    var maxTime = 5000;
    while (oShell.Status === 0) {
        if(maxTime > 0) {
            maxTime -= 100;
            WScript.sleep(100);
        }
        else {
            Log('Exec timed out with command :: ' + command);
        }
    }
}

function copyFile(src,dest) {
    exec('%comspec% /c copy /Y ' + src + ' ' + dest);
}

function copyCommonScripts() {
    var srcPath = platformRoot + '\\bin';
    var destPath = platformRoot + templatePath + '\\cordova';
    
    copyFile(srcPath + '\\check_reqs.bat', destPath + '\\check_reqs.bat');
    copyFile(srcPath + '\\check_reqs.js', destPath + '\\check_reqs.js');
}

function removeCommonScripts() {
    var destPath = platformRoot + templatePath + '\\cordova';

    deleteFileIfExists(destPath + '\\check_reqs.bat');
    deleteFileIfExists(destPath + '\\check_reqs.js');
}

// packages templates into .zip
function package_templates()
{
    Log('Creating template .zip files for wp8');
    var templateOutFilename = repoRoot + '\\CordovaWP8_' + versionNum.replace(/\./g, '_') + '.zip';

    // clear the destination
    deleteFileIfExists(templateOutFilename);

    // clean the template folder, if the project was opened in VS, it creates some useless cruft.
    deleteFolderIfExists(platformRoot + templatePath + '\\Bin');
    deleteFolderIfExists(platformRoot + templatePath + '\\obj');
    deleteFolderIfExists(platformRoot + templatePath + '\\Service\ References');

    deleteFileIfExists(platformRoot + templatePath + '\\CordovaWP8Solution.v11.suo');

    copyCommonScripts();

    copyFile(platformRoot + '\\VERSION',platformRoot + templatePath);

    copyFile(repoRoot + '\\wp8\\template\\cordova\\defaults.xml',platformRoot + templatePath + '\\config.xml');

    // update .vstemplate files for the template zips.

    var cleanVersionName = 'CordovaWP8_' + versionNum.replace(/\./g, '_');

    var projXml = WScript.CreateObject('Microsoft.XMLDOM');
    var xNode = null;
    projXml.async = false;
    var fullTemplatePath = platformRoot + templatePath + '\\MyTemplate.vstemplate';
    if (projXml.load(fullTemplatePath)) {

        // <Name>CordovaWP8_ + versionNum.replace(/\./g, '_')</Name>
        xNode = projXml.selectSingleNode('VSTemplate/TemplateData/Name');
        if(xNode !== null)
        {
            // Log("replacing version in Name");
            xNode.text = cleanVersionName;
        }

        // <DefaultName>CordovaWP8_ + versionNum</DefaultName>
        xNode = projXml.selectSingleNode('VSTemplate/TemplateData/DefaultName');
        if(xNode !== null)
        {
            // Log("replacing version in DefaultName");
            xNode.text = cleanVersionName  + '_';
        }

        xNode = projXml.selectSingleNode('VSTemplate/TemplateData/Description');
        if(xNode !== null)
        {
           xNode.text = xNode.text.replace('0.0.0', versionNum);
        }
        projXml.save(fullTemplatePath);

    }


    // Use proper XML-DOM named nodes and replace them with cordova current version
    projXml = WScript.CreateObject('Microsoft.XMLDOM');
    projXml.async = false;
    if (projXml.load(platformRoot + templatePath + '\\MyTemplate.vstemplate')) {

        // <Name>CordovaWP7_ + versionNum.replace(/\./g, '_')</Name>
        xNode = projXml.selectSingleNode('VSTemplate/TemplateData/Name');
        if(xNode !== null)
        {
           xNode.text = 'CordovaWP8_' + versionNum.replace(/\./g, '_');
        }

        // <DefaultName>CordovaWP7_ + versionNum</DefaultName>
        xNode = projXml.selectSingleNode('VSTemplate/TemplateData/DefaultName');
        if(xNode !== null)
        {
           xNode.text = 'CordovaWP8_' + versionNum;
        }

        projXml.save(platformRoot + templatePath + '\\MyTemplate.vstemplate');
    }

    zip_project(templateOutFilename, platformRoot + templatePath);


    if(addToVS)
    {
        var dest = null;
        var template_dir = wscript_shell.ExpandEnvironmentStrings('%USERPROFILE%') + '\\Documents\\Visual Studio 2012\\Templates\\ProjectTemplates';
        if(fso.FolderExists(template_dir ))
        {
            dest = shell.NameSpace(template_dir);
            dest.CopyHere(templateOutFilename, 4|20);
        }
        else
        {
            Log('WARNING: Could not find template directory in Visual Studio,\n you can manually copy over the template .zip files.');
        }
        // add to VS-2013 as well
        template_dir = wscript_shell.ExpandEnvironmentStrings('%USERPROFILE%') + '\\Documents\\Visual Studio 2013\\Templates\\ProjectTemplates';
        if(fso.FolderExists(template_dir ))
        {
            dest = shell.NameSpace(template_dir);
            dest.CopyHere(templateOutFilename, 4|20);
        }
    }

    removeCommonScripts();
    deleteFileIfExists(platformRoot + templatePath + '\\config.xml');
    deleteFileIfExists(platformRoot + templatePath + '\\VERSION');
}

function zip_project(zip_path, project_path) {
    // create empty ZIP file and open for adding
    var file = fso.CreateTextFile(zip_path, true);

    // create twenty-two byte "fingerprint" for .zip
    file.write('PK');
    file.write(String.fromCharCode(5));
    file.write(String.fromCharCode(6));
    file.write('\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0');
    file.Close();

    // open .zip folder and copy contents of project_path to zip_path
    var zipFolder = shell.NameSpace(zip_path);

    var sourceItems = shell.NameSpace(project_path).items();
    if (zipFolder !== null) {
        zipFolder.CopyHere(sourceItems, 4|16|512|1024);
        var maxTime = 5000;
        //Log("sourceItems.Count = " + sourceItems.Count);
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
        //Log("zipFolder.items().Count = " + zipFolder.items().Count);
    }
    else {
        Log('Failed to create .zip file.', true);
    }
}

function parseArgs() {
    if(args.Count() > 0) {

        //Support help flags -help, --help, /?
        if(args(0).indexOf('-help') > -1 ||
           args(0).indexOf('/?') > -1 ) {
            Usage();
            WScript.Quit(1);
        }
        else if(args(0).indexOf('-install') > -1) {
            addToVS = true;
        }
    }
}

// MAIN
parseArgs();
// build/package the templates
versionNum = read(platformRoot + '\\VERSION');
package_templates(repoRoot);
