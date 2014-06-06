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
var procEnv = wscript_shell.Environment("Process");
// possible values and additional details: http://msdn.microsoft.com/en-us/library/aa384274.aspx
var procArchitecture = procEnv("PROCESSOR_ARCHITECTURE").toLowerCase();
var is64Mode = procArchitecture && procArchitecture != 'x86';

// build type. Possible values: "debug", "release"
var buildType = null,
// list of build architectures. list of strings
buildArchs = null;

var args = WScript.Arguments;

// working dir
var ROOT = WScript.ScriptFullName.split('\\cordova\\lib\\build.js').join('');

// help/usage function
function Usage() {
    Log("");
    Log("Usage: build [ --debug | --release ] [--archs=\"<list of architectures...>\"]");
    Log("    --help    : Displays this dialog.");
    Log("    --debug   : Cleans and builds project in debug mode.");
    Log("    --release : Cleans and builds project in release mode.");
    Log("    --release : Cleans and builds project in release mode.");
    Log("    --archs   : Builds project binaries for specific chip architectures. `arm` and `x86` are supported for wp8");
    Log("examples:");
    Log("    build ");
    Log("    build --debug");
    Log("    build --release");
    Log("    build --release --archs=\"arm x86\"");
    Log("");
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

// executes a commmand in the shell
function exec_verbose(command) {
    //Log("Command: " + command);
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status == 0) {
        //Wait a little bit so we're not super looping
        WScript.sleep(100);
        //Print any stdout output from the script
        if (!oShell.StdOut.AtEndOfStream) {
            var line = oShell.StdOut.ReadLine();
            Log(line);
        }
    }
    //Check to make sure our scripts did not encounter an error
    if (!oShell.StdErr.AtEndOfStream) {
        var line = oShell.StdErr.ReadAll();
        Log("ERROR: command failed in build.js : " + command);
        Log(line, true);
        WScript.Quit(2);
    }
    return oShell.ExitCode;
}

// escapes a path so that it can be passed to shell command. 
function escapePath(path) {
    return '"' + path + '"';
}

// checks to see if a .csproj file exists in the project root
function is_cordova_project(path) {
    if (fso.FolderExists(path)) {
        var proj_folder = fso.GetFolder(path);
        var proj_files = new Enumerator(proj_folder.Files);
        for (;!proj_files.atEnd(); proj_files.moveNext()) {
            if (fso.GetExtensionName(proj_files.item()) == 'csproj') {
                return true;
            }
        }
    }
    return false;
}

function get_solution_name(path) {
    if (fso.FolderExists(path)) {
        var proj_folder = fso.GetFolder(path);
        var proj_files = new Enumerator(proj_folder.Files);
        for (;!proj_files.atEnd(); proj_files.moveNext()) {
            if (fso.GetExtensionName(proj_files.item()) == 'sln') {
                return proj_files.item();
            }
        }
    }
    return null;
}

// returns full path to msbuild tools required to build the project
function getMSBuildToolsPath() {
    // WP8 requires x86 version of MSBuild, CB-6732
    var regRoot = is64Mode ? 'HKLM\\SOFTWARE\\Wow6432Node' : 'HKLM\\SOFTWARE';

    // use the latest version of the msbuild tools available on this machine
    var toolsVersions = ['12.0','4.0'];          // for WP8 we REQUIRE 4.0 !!!
    for (idx in toolsVersions) {
        try {
            return wscript_shell.RegRead(regRoot + '\\Microsoft\\MSBuild\\ToolsVersions\\' + toolsVersions[idx] + '\\MSBuildToolsPath');
        } catch (err) {
            Log("toolsVersion " + idx + " is not supported");
        }
    }
    Log('MSBuild tools have not been found. Please install Microsoft Visual Studio 2012 or later', true);
    WScript.Quit(2);
}

// builds the project and .xap with buildtype and architectures specified
function build_xap(path, buildtype, buildarchs) {

    // check if file xap was created for specified buldtype and architecture
    // raises error if xap not found
    function checkForXap (path, mode, arch) {
        var buildFolder = arch.toLowerCase() == 'any cpu' ? path + '\\Bin\\' + mode : path + '\\Bin\\' + arch + '\\' + mode;
        Log('Checking for .xap in: ' + buildFolder);
        if (fso.FolderExists(buildFolder)) {
            var out_folder = fso.GetFolder(buildFolder);
            var out_files = new Enumerator(out_folder.Files);
            for (;!out_files.atEnd(); out_files.moveNext()) {
                if (fso.GetExtensionName(out_files.item()) == 'xap') {
                    Log("BUILD SUCCESS.\n");
                    return;
                }
            }
        }
        Log('ERROR: MSBuild failed to create .xap when building cordova-wp8 for release.', true);
        WScript.Quit(2);
    }

    if (!buildtype) {
        Log("WARNING: [ --debug | --release ] not specified, defaulting to debug...");
        buildtype = "debug";
    }

    if (!buildarchs) {
        Log("WARNING: target architecture not specified, defaulting to AnyCPU...");
        buildarchs = ["Any CPU"];
    }

    exec_verbose('%comspec% /c "' + path + '\\cordova\\clean"');
    
    var MSBuildToolsPath = getMSBuildToolsPath();
    Log("\tMSBuildToolsPath: " + MSBuildToolsPath);

    for (var i = 0; i < buildarchs.length; i++) {
        var buildarch = buildarchs[i];

        Log("Building Cordova-WP8 Project:");
        Log("\tConfiguration : " + buildtype);
        Log("\tPlatform      : " + buildarch);
        Log("\tDirectory     : " + path);

        wscript_shell.CurrentDirectory = path;

        var buildCommand = escapePath(MSBuildToolsPath + 'msbuild') +
                ' ' + escapePath(get_solution_name(path)) +
                ' /clp:NoSummary;NoItemAndPropertyList;Verbosity=minimal' +
                ' /nologo' +
                ' /p:Configuration=' + buildtype +
                ' /p:Platform="' + buildarch + '"';

        // hack to get rid of 'Access is denied.' error when running the shell w/ access to C:\path..
        buildCommand = '%comspec% /c "' + buildCommand + '"';

        Log("buildCommand = " + buildCommand);

        if (exec_verbose(buildCommand) !== 0) {
            // msbuild failed
            WScript.Quit(2);
        }

        checkForXap(path, buildtype, buildarch);
    }

    Log("WP8 builds successfully completed.");
}

// parses script args and set global variables for build
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

    for (var i = 0; i < args.Length; i++) {
        if (getBuildType(args(i))) {
            buildType = getBuildType(args(i));
        } else if (getBuildArchs(args(i))) {
            buildArchs = getBuildArchs(args(i));
        } else {
            Log("Error: \"" + args(i) + "\" is not recognized as a build option", true);
            Usage();
            WScript.Quit(2);
        }
    }
}


Log("");

if (args.Count() > 0) {
    // support help flags
    if (args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help") {
        Usage();
        WScript.Quit(2);
    }
    // Handle project errors
    else if (!fso.FolderExists(ROOT)) {
        Log("Error: Project directory not found,", true);
        Usage();
        WScript.Quit(2);
    }
    else if (!is_cordova_project(ROOT)) {
        Log('Error: .csproj file not found in ' + ROOT, true);
        Log('could not build project.', true);
        WScript.Quit(2);
    }
    // Parse arguments
    else {
        parseArgs();
    }
}

build_xap(ROOT, buildType, buildArchs);