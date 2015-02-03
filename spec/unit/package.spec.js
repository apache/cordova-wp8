/**
    Licensed to the Apache Software Foundation (ASF) under one
    or more contributor license agreements.  See the NOTICE file
    distributed with this work for additional information
    regarding copyright ownership.  The ASF licenses this file
    to you under the Apache License, Version 2.0 (the
    'License'); you may not use this file except in compliance
    with the License.  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on an
    'AS IS' BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
    KIND, either express or implied.  See the License for the
    specific language governing permissions and limitations
    under the License.
*/
var path = require('path'),
    rewire = require('rewire'),
    shell = require('shelljs'),
    platformRoot = '../../template',
    pkgRoot = './template/',
    pkgPath = path.join(pkgRoot, 'Bin'),
    testPkgPath = './spec/unit/fixtures/EmptyProject/Bin',
    pkg = rewire(platformRoot + '/cordova/lib/package.js');

function isXapPackage(pack) {
    return pack.packagePath.indexOf('.xap') >= 0;
}

describe('getPackage method', function() {
    it('start', function () {
        shell.rm('-rf', pkgPath);
        shell.cp('-R', testPkgPath, pkgRoot);
    });

    it('spec.1 should find arm release package', function(done) {
        var rejected = jasmine.createSpy();

        pkg.getPackage('release', 'arm')
        .then(function(pack) {
            expect(isXapPackage(pack)).toBe(true);
        }, rejected)
        .finally(function() {
            expect(rejected).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.2 should find anycpu debug package', function(done) {
        var rejected = jasmine.createSpy();

        pkg.getPackage('debug', 'anycpu')        
        .then(function(pack) {
            expect(isXapPackage(pack)).toBe(true);
        }, rejected)
        .finally(function() {
            expect(rejected).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.3 should not find x86 release package and reject', function(done) {
        var resolved = jasmine.createSpy();

        pkg.getPackage('release', 'x86')
        .then(resolved)
        .finally(function() {
            expect(resolved).not.toHaveBeenCalled();
            done();
        });
    });

    it('spec.4 should not find arm debug package and reject', function(done) {
        var resolved = jasmine.createSpy();

        pkg.getPackage('debug', 'arm')
        .then(resolved)
        .finally(function() {
            expect(resolved).not.toHaveBeenCalled();
            done();
        });
    });

    it('end', function() {
        shell.rm('-rf', pkgPath);
    });
});
