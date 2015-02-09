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
    run = rewire(platformRoot + '/cordova/lib/run.js');

describe('run method', function() {
    var consoleLogOriginal,
        isCordovaProjectOriginal,
        buildRunOriginal,
        getPackageOriginal;

    var isCordovaProjectFalse = function () {
        return false;
    };

    var isCordovaProjectTrue = function () {
        return true;
    };

    beforeEach(function () {
        // console output suppression
        consoleLogOriginal = run.__get__('console.log');
        run.__set__('console.log', function () {} );

        isCordovaProjectOriginal = run.__get__('utils.isCordovaProject');
        buildRunOriginal = run.__get__('build.run');
        getPackageOriginal = run.__get__('packages.getPackage');
    });

    afterEach(function() {
        run.__set__('utils.isCordovaProject', isCordovaProjectOriginal);
        run.__set__('build.run', buildRunOriginal);
        run.__set__('packages.getPackage', getPackageOriginal);
    });

    it('spec.1 should not run if not launched from project directory', function(done) {
        var buildRun = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectFalse);
        run.__set__('build.run', function () {
            buildRun();
            return Q.reject(); // rejecting to break run chain
        });

        run.run([ 'node', buildPath ])
        .finally(function() {
            expect(buildRun).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.2 should not run if both debug and release args are specified', function(done) {
        var buildRun = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        run.__set__('build.run', function () {
            buildRun();
            return Q.reject(); // rejecting to break run chain
        });

        run.run([ 'node', buildPath, '--release', '--debug' ])
        .finally(function() {
            expect(buildRun).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.3 should not run if device and emulator args are combined', function(done) {
        var buildRun = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        run.__set__('build.run', function () {
            buildRun();
            return Q.reject(); // rejecting to break run chain
        });
        run.run([ 'node', buildPath, '--device', '--emulator' ])
        .finally(function() {
            expect(buildRun).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.4 should not run if device and target args are combined', function(done) {
        var buildRun = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        run.__set__('build.run', function () {
            buildRun();
            return Q.reject(); // rejecting to break run chain
        });
        run.run([ 'node', buildPath, '--device', '--target=sometargethere' ])
        .finally(function() {
            expect(buildRun).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.5 should build and run if all args are ok', function(done) {
        var build = jasmine.createSpy(),
            deploy = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        run.__set__('build.run', function () {
            build();
            return Q();
        });
        run.__set__('packages.getPackage', function () {
            return Q({
                deployTo: function () {
                    deploy();
                    return Q();
                }
            });
        });

        run.run([ 'node', buildPath, '--emulator' ])
        .finally(function() {
            expect(build).toHaveBeenCalled();
            expect(deploy).toHaveBeenCalled();
            done();
        });
    });

    it('spec.6 should not call build if --nobuild specified', function(done) {
        var build = jasmine.createSpy(),
            deploy = jasmine.createSpy();

        run.__set__('utils.isCordovaProject', isCordovaProjectTrue);
        run.__set__('build.run', function () {
            build();
            return Q();
        });
        run.__set__('packages.getPackage', function () {
            return Q({
                deployTo: function () {
                    deploy();
                    return Q();
                }
            });
        });

        run.run([ 'node', buildPath, '--nobuild' ])
        .finally(function() {
            expect(build).not.toHaveBeenCalled();
            expect(deploy).toHaveBeenCalled();
            done();
        });
    });
});
