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
Apache Cordova for Windows Phone 8
===

- Apache Cordova WP8 is a .net application library that lets you create Apache Cordova applications targeting Windows Phone 8 devices.
- Apache Cordova based applications are, at the core, an application written with web technology: HTML, CSS and JavaScript.

[Apache Cordova][] is a project at The Apache Software Foundation (ASF).

Requires
---

- [Windows Phone SDK 8][]


Getting Started from Visual Studio Project Template
---

- Create the Visual Studio Cordova Starter Template
    - Open the file templates\standalone\CordovaSolution.sln in Visual Studio
    - From the file menu, select 'Export Template...' 
    - Choose template type 'Project template'
    - Give the exported template a name, ex. CordovaStarter-x.x.x will produce CordovaStarter-x.x.x.zip
- Visual Studio will put a copy of CordovaStarter-x.x.x.zip in \My Documents\Visual Studio 2012\Templates\ProjectTemplates\
    - if you prefer, you may add the project instead to the "Silverlight for Windows Phone" subfolder of "Visual C#".  This is up to you, and only affects where the project template is shown when creating a new project. Also, You may need to create this folder.
- Launch Visual Studio 2012 and select to create a new project
    - CordovaStarter should be listed under Templates->Other Languages->Visual C#
    - Give your new project a name
        - Note: The description will let you know the version of Cordova you are targetting, if you have multiple templates.
    - If you do not see it, you may have to select the top level 'Visual C#' to see it or use the search box and type "Cordova"
- Build and Run it!


Gettings Started from command line
---

    >.\bin\create PathTONewProject [ PackageName ] [ AppName ]

    >PathTONewProject : The path to where you wish to create the project
    >PackageName      : The namespace for the project (default is Cordova.Example)
    >AppName          : The name of the application (default is CordovaAppProj)

    >examples:
    >.\bin\create C:\Users\anonymous\Desktop\MyProject
    >.\bin\create C:\Users\anonymous\Desktop\MyProject io.cordova.example CordovaApp

    Launch Visual Studio and open Solution file (C:\Users\anonymous\Desktop\MyProject\MyProject.sln)

    Built and Run it



Known Problem Areas
---

- Many of the Media APIs will not function as expected when debugging while connect to the device with the Zune software.
- To get around this, you need to use the Windows Phone Connect tool. For details, please check out this [MSDN blog article][Tips for debugging WP7 media apps with WPConnect].


BUGS?
-----

- File them at the [Apache Cordova Issue Tracker][]


Further Reading
---

- [Apache Cordova Documentation][]
- [Apache Cordova Wiki][]


[Windows Phone SDK 8]: http://www.microsoft.com/en-us/download/details.aspx?id=35471 "Download Windows Phone SDK 8"
[Tips for debugging WP7 media apps with WPConnect]: http://blogs.msdn.com/b/jaimer/archive/2010/11/03/tips-for-debugging-wp7-media-apps-with-wpconnect.aspx "Tips for debugging WP7 media apps with WPConnect"

[Apache Cordova]: http://cordova.io "Apache Cordova"
[Apache Cordova Issue Tracker]: https://issues.apache.org/jira/browse/CB "Apache Cordova Issue Tracker"
[Apache Cordova Documentation]: http://cordova.io/docs "Apache Cordova Documentation"
[Apache Cordova Wiki]: http://wiki.apache.org/cordova "Apache Cordova Wiki"

