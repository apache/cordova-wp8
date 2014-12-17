<!--
#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
#  KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
#
-->

Release Notes for Cordova (WP8)

Update these notes using: git log --pretty=format:'* %s' --topo-order --no-merges origin/2.7.x...HEAD

Cordova is a static library that enables developers to include the Cordova API in their WP8 application projects easily, and also create new Cordova-based WP8 application projects through the command-line.

### 3.8.0 (December 17, 2014) ###

* Set VERSION to 3.8.0 (via coho)
* Update JS snapshot to version 3.8.0 (via coho)
* remove node_version which is breaking appveyor
* CB-8139 WP8. Fix callback for plugins with native ui
* CB-7028 Memory leak in plugins when navigating from native page
* CB-7892 XHR to local files poly should only load/run once
* Fix AutoloadPlugins
* updated description in package.json
* CB-7923 updated release notes

### 3.7.0 (October 31, 2014) ###

* Set VERSION to 3.7.0 (via coho)
* Update JS snapshot to version 3.7.0 (via coho)
* CB-7843 Fixes angular routing on **WP8**
* CB-7616 partial match support for `--target`
* CB-7465 add missing license
* Let `CordovaView` respect `DisallowOverscroll` preference
* factored out common code
* Supress inertia scrolling optionally
* make scripts executable
* CB-7618 Fix **WP8** build due to missing `node_modules`
* CB-7616 Deploy on **WP8** fails to run specific target
* CB-7493 Adds `space-in-path` and `unicode in name` tests for `CI`
* cleanup ignores and add missing windows cmd files, if you want to add a `bundledDependency` `node_module` use `-f`

### 3.6.4 (September 30, 2014) ###

* Updated JS snapshot + set version to 3.6.4
* CB-7616 partial match support for --target
* CB-7616 Deploy on WP8 fails to run specific target
* Fixing paths for npm published versions
* cleanup ignores and add missing windows cmd files, if you want to add a bundledDependency node_module use -f
* move node_modules up same level as package.json
* CB-7455 add bundledDependencies
* Moves node_modules from package root to bin/ folder
* CB-7444 Fixes XHR callback failure when requested file doesn't exists

### 3.6.0 (September 2, 2014) ###

* updated package.json
* Update 3.6.x branch
* update package to 3.6.0-dev
* update to latest 3.6.x cordova.js file
* Fix template def missing files
* Remove unwanted slashes and handle Exceptions
* Add appveyor badge
* ignore node_modules
* CB-7368 --archs="x86" now produces correct binaries.
* CB-7341 Port tooling/platform scripts from WSH to NodeJS
* yml must be ansi encoded, okay, force push this then
* Lemme try addin the yml
* adding testing to the workflow
* ignore node_modules
* [CB-6763] Fixes issue when multiple simultaneous requests are sent.
* This closes #13, This closes #25, This closes #31
* CB-4655 WP8: Default native project template should be overridable at project creation time
* CB-7305 remove trailing slash from dest
* cleanup file, remove commented out stuff after testing it
* Update instructions minus createTemplates stuff
* Fix intermittent issue with invalid app manifest because of xml commented apache header.
* create needs to copy+rename the defaults.xml file for projects created outside the cordova-cli
* Removing outdated/unused tooling scripts
* move createTemplates scripts ( they will live undoc'd for awhile
* Move common items to their rightful home
* CB-7028 fixed memory leak in wp with plugins
* Use wildcard for contents of www/ folder
* small fix for spaces in path
* createTemplates script copies defaults.xml->config.xml
* CB-5049 Create defaults.xml that contains platform config.xml defaults for WP8
* CB-6788 WP8 - Fix header licenses (Apache RAT report)
* CB-7060 WP8. Fix large project build performance issue
* Adds support for chip architectures to run command
* CB-6924 Fixed memory leak in WP page navigation
* CB-6939 Replace dash chars in package name and validate it.
* Fixes handling of UTF-8 encoded project files.
* Added list of supported architectures in help text
* Adds support for target architectures to build command
* Escapes paths in target() function
* add license header to all bat files, and echo off so we don't see the goto output
* add license header
* add license header
* CB-6788 add license header to template file
* CB-6788 Add license headers to cs files
* CB-6788 Add license header to sln files
* CB-6788 add headers to bat files
* CB-6788 add header to .md files
* CB-6788 Add license to CONTRIBUTING.md
* CB-6775 added support for autoload, splashscreen uses this
* CB-5653 make visible cordova version. This closes #35
* wp8.1 and the IE11 WebBrowser control do not support execScript, moving to 'eval'
* CB-6732 [WP8] Fix "MSBuild 64 bit is not supported" build error
* CB-6341 Remove windows requirement to have MSBuild in the %PATH%
* CB-6676 allow extra params to build/run and ignore them
* CB-6685 [3.5.0rc][WP8] Build error: Command failed with exit code 2
* update master branch with next dev version number
* Adds'-wait' flag to CordovaDeploy. Removes unnecessary Program class.
* Remove WP7 from readme, and tooling
* [WP7] Goodbye, it has been fun. ;)
* Update releasenotes, and state that WP7 support is about to disappear
* Apply app-hello-world update


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
