var args = WScript.Arguments;
var wscript_shell = WScript.CreateObject("WScript.Shell");

function Usage() {
    Log("Usage: [ check_reqs | cscript check_reqs.js ]");
    Log("examples:");
    Log("    cscript C:\\Users\\anonymous\\cordova-wp8\\bin\\check_reqs.js");
    Log("    CordovaWindowsPhone\\bin\\check_reqs");

}

// log to stdout or stderr
function Log(msg, error) {
    if(error) {
        WScript.StdErr.WriteLine(msg);
    }
    else{
        WScript.StdOut.WriteLine(msg);
    }
}

/* The tooling for cordova windows phone requires these commands
 *  in the environment PATH variable.
 * - msbuild (C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319)
 */
function SystemRequiermentsMet() {
    var cmd = 'msbuild -version'
    var out = wscript_shell.Exec(cmd);
    while(out.Status == 0)
    {
        WScript.Sleep(100);
    }

    //Check that command executed 
    if (!out.StdErr.AtEndOfStream) {
        var line = out.StdOut.ReadLine();
        Log('The command `msbuild` failed. Make sure you have the latest Windows Phone SDKs installed, and the `msbuild.exe` command (inside C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319) is added to your path.', true);
        Log('Output: ' + output, true);
        WScript.Quit(1);
    }

    //Get output
    var line;
    if (!out.StdOut.AtEndOfStream) {
        line = oShell.StdErr.ReadAll();
    }
    else {
        Log('Unable to get output from command "msbuild", check that you have it in your PATH');
    }
    var msversion = output.match(/\.NET\sFramework\,\sversion\s4\.0/);
    if (!msversion) {
        Log('Please install the .NET Framwork v4.0.30319 (in the latest windows phone SDK\'s).', true);
        Log('Make sure the "msbuild" command in your path is pointing to  v4.0.30319 of msbuild as well (inside C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319).', true);
        WScript.Quit(1);
    }
    //TODO: Check git for other tooling?
}


if(args.Count() > 0)
{
    // support help flags
    if(args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help" || args(0) == "-h")
    {
        Usage();
        WScript.Quit(1);
    }
    else
    {
        Log('Error : Did not recognize argument ' + args(0), true)
        Usage();
        WScript.Quit(1);
    }

    PROJECT_PATH = args(0);
    if(fso.FolderExists(PROJECT_PATH))
    {
        Log("Project directory already exists:");
        Log("\t" + PROJECT_PATH);
        Log("CREATE FAILED.");
        WScript.Quit(1);
    }

    if(args.Count() > 1)
    {
        PACKAGE = args(1);
    }
    else
    {
        PACKAGE = "Cordova.Example";
    }

    if(args.Count() > 2)
    {
        NAME = args(2);
    }
    else
    {
        NAME = "CordovaAppProj";
    }

    create(PROJECT_PATH, PACKAGE, NAME);
}