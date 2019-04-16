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

---
📌 **Deprecation Notice**

This repository is deprecated and no more work will be done on this by Apache Cordova. You can continue to use this and it should work as-is but any future issues will not be fixed by the Cordova community.

Feel free to fork this repository and improve your fork. Existing forks are listed in [Network](../../network) and [Forks](../../network/members).

- Learn more: https://github.com/apache/cordova/blob/master/deprecated.md#deprecated-platforms
---

[![Build status](https://ci.appveyor.com/api/projects/status/apoby7i5j5xnmhy2/branch/master)](https://ci.appveyor.com/project/Humbedooh/cordova-wp8/branch/master)


This repo includes code to build Apache Cordova applications that target Windows Phone 8 SDK.

An Apache Cordova based applications is, at the core, an application written with web technology: HTML, CSS and JavaScript.

[Apache Cordova][] is a project at The Apache Software Foundation (ASF).

Requires
---

- [Windows Phone SDK 8][]
-- Windows Phone 8 development requires Windows 8 Professional, and Visual Studio 2012 ( express works )


Getting Started 
---


## Create a new project

Getting Started from the command line
---

    >.\wp8\bin\create PathToNewProject [ PackageName ] [ AppName ]

    >PathToNewProject : The path to where you wish to create the project
    >PackageName      : The namespace for the project (default is Cordova.Example)
    >AppName          : The name of the application (default is CordovaWP8AppProj)

    >example:
    >.\wp8\bin\create C:\Users\anonymous\Desktop\MyWP8Proj io.cordova.example CordovaWP8App

    From here you can open it in Visual Studio:
    - Launch Visual Studio and open Solution file (.sln) in (C:\Users\anonymous\Desktop\MyWP8Proj)
    - Built and Run it

    Or, you can continue with the command line:
    - >cd C:\Users\anonymous\Desktop\MyWP8Proj
      >cordova\run


BUGS?
-----

- File them at the [Apache Cordova Issue Tracker][]


Further Reading
---

- [Apache Cordova Documentation][]
- [Apache Cordova Wiki][]

[Windows Phone SDK 8]: http://www.microsoft.com/en-us/download/details.aspx?id=35471 "Download Windows Phone SDK 8"

[Apache Cordova]: http://cordova.io "Apache Cordova"
[Apache Cordova Issue Tracker]: https://issues.apache.org/jira/browse/CB "Apache Cordova Issue Tracker"
[Apache Cordova Documentation]: http://cordova.io/docs "Apache Cordova Documentation"
[Apache Cordova Wiki]: http://wiki.apache.org/cordova "Apache Cordova Wiki"

