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

// build type. Possible values: "debug", "release"
// required to determine which package should be deployed
var buildType = null,
    // nobuild flag
    noBuild = false,
    // list of build architectures. list of strings
    // required to determine which package should be deployed
    buildArchs = null,
    // build target. Possible values: "device", "emulator", "<target_name>"
    buildTarget = null;

// help function
function Usage() {
    Log("");
    Log("Usage:");
    Log("  run [ --device || --emulator || --target=<id> ] ");
    Log("      [ --debug || --release || --nobuild ]");
    Log("      [--archs=\"<list of architectures...>\"]");
    Log("    --device      : Deploys and runs the project on the connected device.");
    Log("    --emulator    : [DEFAULT] Deploys and runs the project on an emulator.");
    Log("    --target=<id> : Deploys and runs the project on the specified target.");
    Log("    --debug       : [DEFAULT] Builds project in debug mode.");
    Log("    --release     : Builds project in release mode.");
    Log("    --nobuild     : Ueses pre-built xap, or errors if project is not built.");
    Log("    --archs       : Builds project binaries for specific chip architectures.");
    Log("                    Deploys and runs package with first architecture specified.");
    Log("                    arm` and `x86` are supported for wp8");
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

// check if file xap was created for specified buldtype and architecture
// raises error if xap not found
function getXap (path, buildtype, buildarch) {
    var buildFolder = buildarch.toLowerCase() == 'any cpu' ?
        path + '\\Bin\\' + buildtype :
        path + '\\Bin\\' + buildarch + '\\' + buildtype;

    if (fso.FolderExists(buildFolder)) {
        var out_folder = fso.GetFolder(buildFolder);
        var out_files = new Enumerator(out_folder.Files);
        for (;!out_files.atEnd(); out_files.moveNext()) {
            // search for first .xap file in folder
            if (fso.GetExtensionName(out_files.item()) == 'xap') {
                Log('Found .xap: ' + out_files.item().Path);
                return out_files.item().Path;
            }
        }
    }
    Log('No .xap found for ' + buildtype + ' build type and ' + buildarch + ' architecture', true);
    WScript.Quit(2);
}

// launches project on device
function device(path, buildtype, buildarchs)
{
    // set default values
    var build = buildtype ? buildtype : "debug";
    var arch = buildarchs ? buildarchs[0] : "any cpu";

    cordovaDeploy(path);
    if (fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        Log('Deploying to device ...');
        //TODO: get device ID from list-devices and deploy to first one
        exec_verbose('"' + path + CORDOVA_DEPLOY_EXE + '" "' + getXap(path, build, arch) + '" -d:0');
    }
    else
    {
        Log('Error: Failed to find CordovaDeploy.exe in ' + path, true);
        Log('DEPLOY FAILED.', true);
        WScript.Quit(2);
    }
}

// launches project on emulator
function emulator(path, buildtype, buildarchs)
{
    var build = buildtype ? buildtype : "debug";
    var arch = buildarchs ? buildarchs[0] : "any cpu";

    cordovaDeploy(path);
    if (fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        Log('Deploying to emulator ...');
        //TODO: get emulator ID from list-emulators and deploy to first one
        exec_verbose('"' + path + CORDOVA_DEPLOY_EXE + '" "' + getXap(path, build, arch) + '" -d:1');
    }
    else
    {
        Log('Error: Failed to find CordovaDeploy.exe in ' + path, true);
        Log('DEPLOY FAILED.', true);
        WScript.Quit(2);
    }
}

// builds and launches the project on the specified target
function target(path, buildtarget, buildtype, buildarchs) {
    if (!fso.FileExists(path + CORDOVA_DEPLOY_EXE)) {
        cordovaDeploy(path);
    }
    var cmd = '"' + path + CORDOVA_DEPLOY_EXE + '" -devices';
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
                    var build = buildtype ? buildtype : "debug";
                    var arch = buildarchs ? buildarch[0] : "any cpu";
                    exec_verbose('"' + path + CORDOVA_DEPLOY_EXE + '" "' + getXap(path, build, arch) + '" -d:' + index);
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

function build(path, buildtype, buildarchs) {
    // if --nobuild flag is specified, no action required here
    if (noBuild) return;

    var cmd = '%comspec% /c ""' + path + '\\cordova\\build"';
    if (buildtype){
        cmd += " --" + buildtype;
    }
    if (buildarchs){
        cmd += ' --archs="' + buildarchs.join(",") + '"';
    }
    cmd += '"';

    exec_verbose(cmd);
}

function run(path, buildtarget, buildtype, buildarchs) {
    switch(buildtarget) {
        case "emulator" :
            emulator(path, buildtype, buildarchs);
            break;
        case "device" :
            device(path, buildtype, buildarchs);
            break;
        case null :
            Log("WARNING: [ --target=<ID> | --emulator | --device ] not specified, defaulting to --emulator");
            emulator(path, buildtype, buildarchs);
            break;
        default :
            // if buildTarget is neither "device", "emulator" or null
            // then it is a name of target
            target(path, buildtarget, buildtype, buildarchs);
            break;
    }
}

// parses script args and set global variables for build/deploy
// throws error if unknown argument specified.
function parseArgs () {

    // return build type, specified by input string, or null, if not build type parameter
    function getBuildType (arg) {
        arg = arg.toLowerCase();
        if (arg == "--debug" || arg == "-d") {
            return "debug";
        }
        else if (arg == "--release" || arg == "-r") {
            return "release";
        }
        else if (arg == "--nobuild") {
            noBuild = true;
            return true;
        }
        return null;
    }

    // returns build architectures list, specified by input string
    // or null if nothing specified, or not --archs parameter
    function getBuildArchs (arg) {
        arg = arg.toLowerCase();
        var archs = /--archs=(.+)/.exec(arg);
        if (archs) {
            // if architectures list contains commas, suppose that is comma delimited
            if (archs[1].indexOf(',') != -1){
                return archs[1].split(',');
            }
            // else space delimited
            return archs[1].split(/\s/);
        }
        return null;
    }

    // returns deploy target, specified by input string or null, if not deploy target parameter
    function getBuildTarget (arg) {
        arg = arg.toLowerCase();
        if (arg == "--device"){
            return "device";
        }
        else if (arg == "--emulator"){
            return "emulator";
        }
        else {
            var target = /--target=(.*)/.exec(arg);
            if (target){
                return target[1];
            }
        }
        return null;
    }

    for (var i = 0; i < args.Length; i++) {
        if (getBuildType(args(i))) {
            buildType = getBuildType(args(i));
        } else if (getBuildArchs(args(i))) {
            buildArchs = getBuildArchs(args(i));
        } else if (getBuildTarget(args(i))){
            buildTarget = getBuildTarget(args(i));
        } else {
            Log("Error: \"" + args(i) + "\" is not recognized as a build/deploy option", true);
            Usage();
            WScript.Quit(2);
        }
    }
}

if (args.Count() > 0) {
    // support help flags
    if (args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help") {
        Usage();
        WScript.Quit(2);
    }
    else if (!fso.FolderExists(ROOT)) {
        Log('Error: Project directory not found,', true);
        Usage();
        WScript.Quit(2);
    }
    parseArgs();
}

// build and run the project!
build(ROOT, buildType, buildArchs);
run(ROOT, buildTarget, buildType, buildArchs);