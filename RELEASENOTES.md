Release Notes for Cordova (WP8)

Update these notes using: git log --pretty=format:'* %s' --topo-order --no-merges origin/2.7.x...HEAD

Cordova is a static library that enables developers to include the Cordova API in their WP8 application projects easily, and also create new Cordova-based WP8 application projects through the command-line.

### 3.5.0 (201405XX) ###

* Apply app-hello-world update
* CB-6558: added package.json files for wp7 & wp8
* CB-6491 add CONTRIBUTING.md
* CB-6412 Include release notes in repo.
* CB-6450 added support for local XHR.responseXML getter
* CB-6341 don't rely on msbuild being in the path.
* bumping version in prep for upcoming 3.5.0
* applied Sergey's SpecificVersion flag fix to the wp7 template also CB-6103
* CB-6103 [wp8] CordovaDeploy potential build issue
* applied CB-6268 backgroundcolor to WP7 also
* CB-6268 WP8. Apply BackgroundColor from config.xml
* CB-5965 [wp7] support set responseType, get response
* CB-5965 support set responseType, get response
* CB-6299 [wp7] Strip protocol and leading slashes from XHRLOCAL URL
* CB-6299 [wp8] Strip protocol and leading slashes from XHRLOCAL URL
* CB-6091 [windows] Build fails if application path contains whitespaces
* CB-6041 createTemplates should install them for VS-2013 as well
* apply CB-5219 to WP7 also
* [CB-5219] weinre disconnects when history.replaceState is used
* CB-5951 Added namespace to config.xml
* Removed wp7 template ref to non-existent file
* Update to 3.4.0 proper
* CB-6041 createTemplates should install them for VS-2013 as well
* CB-5951 Added namespace to config.xml
* Update to 3.4.0-rc1


### 3.4.0 (201402XX) ###

* Update to 3.4.0 proper
* CB-6041 createTemplates should install them for VS-2013 as well
* CB-5951 Added namespace to config.xml
* Update to 3.4.0-rc1
* oops, wp7 still needed some of those using's
* Debug.WriteLine output, not Console.WriteLine to avoid (possible) ambiguous reference. Clean up 'usings'
* Apply the same changes to wp7
* Config handler has to treat the xml 'name' as a key but use the wp-package param value
* One more case : default classpath + command alias, ex Console=>WPCordovaClassLib.Cordova.Commands + 'DebugConsole'
* added Namespace string property, used by CommandFactory to create the command.
* CommandFactory.CreateByServiceName takes new optional string value which can be the fully qualified class name, or just an alias class name to prevent ambiguity
* Get namespace before passing off to NativeExec
* CB-4533 return exit code 2
* CB-5025 Don't escape/decode undefined|null values +WP7
* CB-5357 Multistage exit app fires events pause->exit->onunload before exiting completely. WP8 only
* cleaned up param parsing for 'help'
* rejiggered to avoid extra bat files.  RegReading is done in jscript
* CB-5359 Get the tools version number from the registry and not the tool because it has issues with internationalized output parsing.
* added readme to WP7 for deprecation notice
* added some formatting
* Deprecated WP7
* Update JS and VERSION to the latest, back on the dev branch
* CB-802 WP8 ConsoleHelper outputs all console.log calls to isolated storage file for watching with CordovaDeploy tool
* CB-802 output console.log to cordova-cli run command
* updated master js
* CB-5360 fix compiler whinings
* CB-5360 fix compiler warnings
* Update 3.3.0 JS + VERSION
* CB-5544 update versions, js for 3.3.0-rc1
* CB-5360 fix compiler whinings
* CB-5360 fix compiler warnings