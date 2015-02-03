/**
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
var Q = require('q'),
    path = require('path'),
    rewire = require('rewire'),
    platformRoot = '../../template',
    buildPath = path.join(platformRoot, 'cordova', 'build'),
    build = rewire(platformRoot + '/cordova/lib/build.js');

describe('run method', function() {
    var consoleLogOriginal,
        isCordovaProjectOriginal,
        findAvailableVersionOriginal;

    var isCordovaProjectFalse = function () {
        return false;
    };

    var isCordovaProjectTrue = function () {
        return true;
    };

    beforeEach(function () {
        // console output suppression
        consoleLogOriginal = build.__get__('console.log');
        build.__set__('console.log', function () {} );

        isCordovaProjectOriginal = build.__get__('utils.isCordovaProject');
        findAvailableVersionOriginal = build.__get__('MSBuildTools.findAvailableVersion');
    });

    afterEach(function() {
        build.__set__('console.log', consoleLogOriginal);
        build.__set__('utils.isCordovaProject', isCordovaProjectOriginal);
        build.__set__('MSBuildTools.findAvailableVersion', findAvailableVersionOriginal);
    });

    it('spec.1 should reject if not launched from project directory', function(done) {
        var rejectSpy = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectFalse);
        // mocking findAvailableVersion here to make sure that
        // no actual build will start even if it is called (when test is failing)
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function () {
                    return Q();
                }
            });
        });

        build.run([ 'node', buildPath, '--release' ])
        .fail(rejectSpy)
        .finally(function() {
            expect(rejectSpy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.2 should reject if both debug and release args specified', function(done) {
        var rejectSpy = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        // mocking findAvailableVersion here to make sure that
        // no actual build will start even if it is called (when test is failing)
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function () {
                    return Q();
                }
            });
        });

        build.run([ 'node', buildPath, '--release', '--debug' ])
        .fail(rejectSpy)
        .finally(function() {
            expect(rejectSpy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.3 should call buildProject of MSBuildTools with buildType = "release" if called with --release argument', function(done) {
        var buildSpy = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function (solutionFile, buildType, buildArch) {
                    expect(buildType).toBe('release');
                    buildSpy();
                    return Q.reject(); // rejecting here to stop build process
                }
            });
        });

        build.run([ 'node', buildPath, '--release' ])
        .finally(function() {
            expect(buildSpy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.4 should call buildProject of MSBuildTools with buildType = "debug" if called without arguments', function(done) {
        var buildSpy = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function (solutionFile, buildType, buildArch) {
                    expect(buildType).toBe('debug');
                    buildSpy();
                    return Q.reject(); // rejecting here to stop build process
                }
            });
        });

        build.run([ 'node', buildPath ])
        .finally(function() {
            expect(buildSpy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.5 should call buildProject of MSBuildTools with buildArch = "ARM" if called with --archs="arm" argument', function(done) {
        var buildSpy = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function (solutionFile, buildType, buildArch) {
                    expect(buildArch).toBe('arm');
                    buildSpy();
                    return Q.reject(); // rejecting here to stop build process
                }
            });
        });

        build.run([ 'node', buildPath, '--archs=arm' ])
        .finally(function() {
            expect(buildSpy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.6 should call buildProject of MSBuildTools for all architectures if called with --archs="x86 arm anycpu" argument', function(done) {
        var armBuild = jasmine.createSpy(),
            x86Build = jasmine.createSpy(),
            anyCpuBuild = jasmine.createSpy();

        build.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        build.__set__('MSBuildTools.findAvailableVersion', function() {
            return Q.resolve({
                version: '14.0',
                path: 'testpath',
                buildProject: function (solutionFile, buildType, buildArch) {
                    expect(buildArch).toMatch(/^arm$|^any\s?cpu$|^x86$/);
                    switch(buildArch) {
                        case 'arm':
                            armBuild();
                            return Q();
                        case 'x86':
                            x86Build();
                            return Q();
                        case 'any cpu':
                        case 'anycpu':
                            anyCpuBuild();
                            return Q();
                        default:
                            return Q.reject();
                    }
                }
            });
        });

        build.run([ 'node', buildPath, '--archs=arm x86 anycpu' ])
        .finally(function() {
            expect(armBuild).toHaveBeenCalled();
            expect(x86Build).toHaveBeenCalled();
            expect(anyCpuBuild).toHaveBeenCalled();
            done();
        });
    });
});
