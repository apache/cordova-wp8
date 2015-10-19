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

var Q     = require('q'),
    fs    = require('fs'),
    path  = require('path'),
    shell = require('shelljs'),
    uuid  = require('node-uuid');

var defaultAppName = 'CordovaWP8AppProj';
var defaultSlnName = 'CordovaWP8Solution';

// Creates cordova-windows project at specified path with specified namespace, app name and GUID
module.exports.run = function (argv) {

    // Get script args
    var args = argv.slice(2);

    // Set and validate parameters/defaults for create
    
    var projectPath = args[0];
    if (fs.existsSync(projectPath)){
        return Q.reject('Project directory already exists:\n\t' + projectPath);
    }

    // Package name can't contain dashes, so replace it with underscores
    // if replacing the - with a _ does not work, give up
    var packageName = args[1] ? args[1].replace('-', '_') : 'Cordova.Example';
    if(!/^[a-zA-Z0-9._$]+$/g.test(packageName)) {
        return Q.reject('Invalid identifier. PackageName may only include letters, numbers, _ and $');
    }

    var appName = args[2] || defaultAppName,
        safeAppName = appName.replace(/(\.\s|\s\.|\s+|\.+)/g, '_'),
        platformRoot = path.join(__dirname, '..', '..'),
        templatePath = path.join(platformRoot, 'template'),
        customTemplate = args[3];

    console.log('Creating Cordova Windows Project:');
    console.log('\tApp Name  : ' + appName);
    console.log('\tNamespace : ' + packageName);
    console.log('\tPath      : ' + projectPath);
    if (customTemplate) {
        console.log('Custom template path: ' + customTemplate);
    }

    console.log('Copying necessary files to ' + projectPath);
    // Copy the template source files to the new destination
    shell.cp('-rf', path.join(templatePath, '*'), projectPath);
    // Copy cordova-js-src directory
    shell.cp('-rf', path.join(platformRoot, 'cordova-js-src'), path.join(projectPath, 'platform_www'));
    // Copy our unique VERSION file, so peeps can tell what version this project was created from.
    shell.cp('-rf', path.join(platformRoot, 'VERSION'), projectPath);
    // copy the defaults.xml into config.xml so this project can be built when create is called minus the cordova-cli
    shell.cp(path.join(projectPath, 'cordova', 'defaults.xml'), path.join(projectPath, 'config.xml'));
    // CB-7618 node_modules must be copied to project folder
    shell.cp('-r', path.join(platformRoot, 'node_modules'), path.join(projectPath, 'cordova'));

    // CB-8954 Copy check_reqs module, since it will be required by 'requirements' command
    shell.cp('-r', path.join(platformRoot, 'bin', 'check_reqs*'), path.join(projectPath, 'cordova'));
    shell.cp('-r', path.join(platformRoot, 'bin', 'lib', 'check_reqs*'), path.join(projectPath, 'cordova', 'lib'));

    // if any custom template is provided, just copy it over created project
    if (customTemplate && fs.existsSync(customTemplate)) {
        console.log('Copying template overrides from ' + customTemplate + ' to ' + projectPath);
        shell.cp('-rf', customTemplate, projectPath);
    }

    console.log('Updating project files');
    // replace values in the AppManifest
    var wmAppManifest = path.join(projectPath, 'Properties', 'WMAppManifest.xml'),
        guid = uuid.v1();

    shell.sed('-i', /\$guid1\$/g, guid, wmAppManifest);
    shell.sed('-i', /\$safeprojectname\$/g, appName, wmAppManifest);

    //replace projectname in project files
    ['App.xaml', 'App.xaml.cs', 'MainPage.xaml', 'MainPage.xaml.cs', defaultAppName + '.csproj'].forEach(function (file) {
        shell.sed('-i', /\$safeprojectname\$/g, packageName, path.join(projectPath, file));
    });
    
    if (appName != defaultAppName) {
        var slnFile = path.join(projectPath, defaultSlnName + '.sln'),
            csprojFile = path.join(projectPath, defaultAppName + '.csproj');

        shell.sed('-i', new RegExp(defaultAppName, 'g'), safeAppName, slnFile);
        // rename project and solution
        shell.mv('-f', slnFile, path.join(projectPath, safeAppName + '.sln'));
        shell.mv('-f', csprojFile, path.join(projectPath, safeAppName + '.csproj'));
    }

    // remove template cruft
    ['__PreviewImage.jpg', '__TemplateIcon.png', 'MyTemplate.vstemplate'].forEach(function (file) {
        shell.rm(path.join(projectPath, file));
    });

    // Delete bld forder and bin folder
    ['bld', 'bin', '*.user', '*.suo'].forEach(function (file) {
        shell.rm('-rf', path.join(projectPath, file));
    });
    
    return Q.resolve();
};

module.exports.help = function () {
    console.log('Usage: create PathToNewProject [ PackageName [ AppName [ CustomTemplate ] ] ]');
    console.log('    PathToNewProject : The path to where you wish to create the project');
    console.log('    PackageName      : The namespace for the project (default is Cordova.Example)');
    console.log('    AppName          : The name of the application (default is CordovaAppProj)');
    console.log('    CustomTemplate   : The path to project template overrides');
    console.log('                       (will be copied over default platform template files)');
    console.log('examples:');
    console.log('    create C:\\Users\\anonymous\\Desktop\\MyProject');
    console.log('    create C:\\Users\\anonymous\\Desktop\\MyProject io.Cordova.Example AnApp');
};
