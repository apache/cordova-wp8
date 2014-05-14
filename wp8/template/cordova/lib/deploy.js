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


var fso = WScript.CreateObject('Scripting.FileSystemObject');
var wscript_shell = WScript.CreateObject("WScript.Shell");

var args = WScript.Arguments;
// working dir
var ROOT = WScript.ScriptFullName.split('\\cordova\\lib\\deploy.js').join('');
    // path to CordovaDeploy.exe
var CORDOVA_DEPLOY_EXE = '\\cordova\\lib\\CordovaDeploy\\CordovaDeploy\\bin\\Debug\\CordovaDeploy.exe';
    // path to CordovaDeploy
var CORDOVA_DEPLOY = '\\cordova\\lib\\CordovaDeploy';
//device_id for targeting specific device
var device_id;

//build types
var NONE = 0,
    DEBUG = 1,
    RELEASE = 2,
    NO_BUILD = 3;
var build_type = NONE;

//deploy tpyes
var NONE = 0,
    EMULATOR = 1,
    DEVICE = 2,
    TARGET = 3;
var deploy_type = NONE;


// help function
function Usage() {
    Log("");
    Log("Usage:");
    Log("  run [ --device || --emulator || --target=<id> ] ");
    Log("      [ --debug || --release || --nobuild ]");
    Log("    --device      : Deploys and runs the project on the connected device.");
    Log("    --emulator    : [DEFAULT] Deploys and runs the project on an emulator.");
    Log("    --target=<id> : Deploys and runs the project on the specified target.");
    Log("    --debug       : [DEFAULT] Builds project in debug mode.");
    Log("    --release     : Builds project in release mode.");
    Log("    --nobuild     : Ueses pre-built xap, or errors if project is not built.");
    Log("Examples:");
    Log("    run");
    Log("    run --emulator");
    Log("    run --device");
    Log("    run --target=7988B8C3-3ADE-488d-BA3E-D052AC9DC710");
    Log("    run --device --release");
    Log("    run --emulator --debug");
    Log("");
}

// log to stdout or stderr
function Log(msg, error) {
    if (error) {
        WScript.StdErr.WriteLine(msg);
    }
    else {
        WScript.StdOut.WriteLine(msg);
    }
} 

var ForReading = 1, ForWriting = 2, ForAppending = 8;
var TristateUseDefault = 2, TristateTrue = 1, TristateFalse = 0;


// executes a commmand in the shell
function exec(command) {
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status == 0) {
        WScript.sleep(100);
    }
}

// executes a commmand in the shell
function exec_verbose(command) {
    //Log("Command: " + command);
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status == 0) {
        //Wait a little bit so we're not super looping
        WScript.sleep(100);
        //Print any stdout output from the script
        while (!oShell.StdOut.AtEndOfStream) {
            var line = oShell.StdOut.ReadLine();
            Log(line);
        }
    }
    //Check to make sure our scripts did not encounter an error
    if (!oShell.StdErr.AtEndOfStream) {
        var line = oShell.StdErr.ReadAll();
        Log("ERROR: command failed in deploy.js : " + command);
        Log(line, true);
        WScript.Quit(2);
    }

    return oShell.ExitCode;
}

// returns the contents of a file
function read(filename) {
    if (fso.FileExists(filename)) {
        var f=fso.OpenTextFile(filename, 1,2);
        var s=f.ReadAll();
        f.Close();
        return s;
    }
    else {
        Log('Cannot read non-existant file : ' + filename, true);
        WScript.Quit(2);
    }
    return null;
}

// escapes a path so that it can be passed to shell command. 
function escapePath(path) {
    return '"' + path + '"';
}

// returns full path to msbuild tools required to build the project
function getMSBuildToolsPath() {
    // use the latest version of the msbuild tools available on this machine
    var toolsVersions = ['12.0','4.0'];          // for WP8 we REQUIRE 4.0 !!!
    for (idx in toolsVersions) {
        try {
            return wscript_shell.RegRead('HKLM\\SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions\\' + toolsVersions[idx] + '\\MSBuildToolsPath');
        } catch (err) {
            Log("toolsVersion " + idx + " is not supported");
        }
    }
    Log('MSBuild tools have not been found. Please install Microsoft Visual Studio 2012 or later', true);
    WScript.Quit(2);
}

// builds the CordovaDeploy.exe if it does not already exist 
function cordovaDeploy(path) {
    if (fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        return;
    }

    Log('CordovaDeploy.exe not found, attempting to build CordovaDeploy.exe...');

    // build CordovaDeploy.exe
    if (fso.FolderExists(path + '\\cordova') && fso.FolderExists(path + CORDOVA_DEPLOY) && 
        fso.FileExists(path + CORDOVA_DEPLOY + '\\CordovaDeploy.sln')) {
        // delete any previously generated files
        if (fso.FolderExists(path + CORDOVA_DEPLOY + '\\CordovaDeploy\\obj')) {
            fso.DeleteFolder(path + CORDOVA_DEPLOY + '\\CordovaDeploy\\obj');
        }
        if (fso.FolderExists(path + CORDOVA_DEPLOY + '\\CordovaDeploy\\Bin')) {
            fso.DeleteFolder(path + CORDOVA_DEPLOY + '\\CordovaDeploy\\Bin');
        }

        var MSBuildToolsPath = getMSBuildToolsPath();
        Log("\tMSBuildToolsPath: " + MSBuildToolsPath);

        var buildCommand = escapePath(MSBuildToolsPath + 'msbuild') + 
            ' ' + escapePath(path + CORDOVA_DEPLOY + '\\CordovaDeploy.sln') +
            ' /clp:NoSummary;NoItemAndPropertyList;Verbosity=minimal /nologo';

        // hack to get rid of 'Access is denied.' error when running the shell w/ access to C:\path..
        buildCommand = '%comspec% /c "' + buildCommand + '"';
        Log("buildCommand = " + buildCommand);

        if (exec_verbose(buildCommand) != 0 || !fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
            Log('ERROR: MSBUILD FAILED TO COMPILE CordovaDeploy.exe', true);
            WScript.Quit(2);
        }
        Log('CordovaDeploy.exe compiled, SUCCESS.');
    }
    else {
        Log('ERROR: CordovaDeploy.sln not found, unable to compile CordovaDeploy tool.', true);
        WScript.Quit(2);
    }
}

// launches project on device
function device(path)
{
    cordovaDeploy(path);
    if (fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        Log('Deploying to device ...');
        //TODO: get device ID from list-devices and deploy to first one
        exec_verbose('"' + path + CORDOVA_DEPLOY_EXE + '" "' + path + '" -d:0');
    }
    else
    {
        Log('Error: Failed to find CordovaDeploy.exe in ' + path, true);
        Log('DEPLOY FAILED.', true);
        WScript.Quit(2);
    }
}

// launches project on emulator
function emulator(path)
{
    cordovaDeploy(path);
    if (fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        Log('Deploying to emulator ...');
        //TODO: get emulator ID from list-emulators and deploy to first one
        exec_verbose('"' + path + CORDOVA_DEPLOY_EXE + '" "' + path + '" -d:1');
    }
    else
    {
        Log('Error: Failed to find CordovaDeploy.exe in ' + path, true);
        Log('DEPLOY FAILED.', true);
        WScript.Quit(2);
    }
}

// builds and launches the project on the specified target
function target(path) {
    if (!fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        cordovaDeploy(path);
    }
    wscript_shell.CurrentDirectory = path + CORDOVA_DEPLOY + '\\CordovaDeploy\\bin\\Debug';
    var cmd = 'CordovaDeploy -devices';
    var out = wscript_shell.Exec(cmd);
    while(out.Status == 0) {
        WScript.Sleep(100);
    }
    if (!out.StdErr.AtEndOfStream) {
        var line = out.StdErr.ReadAll();
        Log("Error calling CordovaDeploy : ", true);
        Log(line, true);
        WScript.Quit(2);
    }
    else {
        if (!out.StdOut.AtEndOfStream) {
            var line = out.StdOut.ReadAll();
            var targets = line.split('\r\n');
            var check_id = new RegExp(device_id);
            for (target in targets) {
                if (targets[target].match(check_id)) {
                    //TODO: this only gets single digit index, account for device index of 10+?
                    var index = targets[target].substr(0,1);
                    exec_verbose(path + CORDOVA_DEPLOY_EXE + ' ' + path + ' -d:' + index);
                    return;
                }
            }
            Log('Error : target ' + device_id + ' was not found.', true);
            Log('DEPLOY FAILED.', true);
            WScript.Quit(2);
        }
        else {
            Log('Error : CordovaDeploy Failed to find any devices', true);
            Log('DEPLOY FAILED.', true);
            WScript.Quit(2);
        }
    }
}

function build(path) {
    switch (build_type) {
        case DEBUG :
            exec_verbose('%comspec% /c "' + ROOT + '\\cordova\\build" --debug');
            break;
        case RELEASE :
            exec_verbose('%comspec% /c "' + ROOT + '\\cordova\\build" --release');
            break;
        case NO_BUILD :
            break;
        case NONE :
            Log("WARNING: [ --debug | --release | --nobuild ] not specified, defaulting to --debug.");
            exec_verbose('%comspec% /c "' + ROOT + '\\cordova\\build" --debug');
            break;
        default :
            Log("Build option not recognized: " + build_type, true);
            WScript.Quit(2);
            break;
    }
}

function run(path) {
    switch(deploy_type) {
        case EMULATOR :
            build(path);
            emulator(path);
            break;
        case DEVICE :
            build(path);
            device(path);
            break;
        case TARGET :
            build(path);
            target(path);
            break;
        case NONE :
            Log("WARNING: [ --target=<ID> | --emulator | --device ] not specified, defaulting to --emulator");
            build(path);
            emulator(path);
            break;
        default :
            Log("Deploy option not recognized: " + deploy_type, true);
            WScript.Quit(2);
            break;
    }
}


if (args.Count() > 0) {
    // support help flags
    if (args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help") {
        Usage();
        WScript.Quit(2);
    }
    else if (fso.FolderExists(ROOT)) {
        // parse arguments
        for(var i = 0; i < args.Count(); i++) {
            if (args(i) == "--release") {
                build_type = RELEASE;
            }
            else if (args(i) == "--debug") {
                build_type = DEBUG;
            }
            else if (args(i) == "--nobuild") {
                build_type = NO_BUILD;
            }
            else if (args(i) == "--emulator" || args(i) == "-e") {
                deploy_type = EMULATOR;
            }
            else if (args(i) == "--device" || args(i) == "-d") {
                deploy_type = DEVICE;
            }
            else if (args(i).substr(0,9) == "--target=") {
                device_id = args(i).split("--target=").join("");
                deploy_type = TARGET;
            }
            else {
                Log('Error: \"' + args(0) + '\" is not recognized as a deploy option', true);
                Usage();
                WScript.Quit(2);
            }
        }
    }
    else {
        Log('Error: Project directory not found,', true);
        Usage();
        WScript.Quit(2);
    }
}

// Finally run the project!
run(ROOT);