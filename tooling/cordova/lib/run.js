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
var ROOT = WScript.ScriptFullName.split('\\cordova\\lib\\run.js').join('');


// help function
function Usage() {
    Log("");
    Log("Usage: run [ --target=<ID> ]");
    Log("examples:");
    Log("    run");
    Log("    run target=7988B8C3-3ADE-488d-BA3E-D052AC9DC710")
    Log("");
    Log("Flow for run command :");
    Log("\t0. Are there any actual devices available? (use list-devices to determine this). If so, target the first one. If no, continue.");
    Log("\t1. Are there any actual emulators available, i.e. started/running? (use list-started-emulators to determine this). If so, target the first one. If no, continue.");
    Log("\t2. Are there any emulator images available to start? (use list-emulator-images to determine this). If so, call start-emulator <id> of the first available image, wait for it to become ready, then target it. If no, continue.");
    Log("\t3. Fail horribly.");
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
        Log(line, true);
        WScript.Quit(1);
    }
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

// deploy to specified target
function target(path, target) {
    var cmd = 'cscript ' + path + '\\cordova\\lib\\deploy.js --target=' + target + ' //nologo';
    exec_verbose(cmd);
}

// deploy to availible device, if no device is availible, deploy to emulator.
// TODO: implement list-devices (currenly not possible) and list-started-emulators and use device first then started emulator
function deploy(path) {
  var cmd = 'cscript ' + path + '\\cordova\\lib\\deploy.js --emulator //nologo';
  exec_verbose(cmd);
}


Log("");

if (args.Count() > 0) {
    // support help flags
    if (args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help") {
        Usage();
        WScript.Quit(1);
    }
    else if (args.Count() > 1) {
        Log("Error: Too many arguments.", true);
        Usage();
        WScript.Quit(1);
    }
    else if (fso.FolderExists(ROOT)) {
        if (args(0).substr(0,9) == "--target=") {
            target(ROOT, args(0));
        }
        else {
            Log("Error: \"" + arg(0) + "\" is not recognized as a run option", true);
            Usage();
            WScript.Quit(1);
        }
    }
    else {
        Log("Error: Project directory not found,", true);
        Usage();
        WScript.Quit(1);
    }
}
else {
    Log("WARNING: [ --target=<ID> ] not specified, using defaults...");
    deploy(ROOT);
}
