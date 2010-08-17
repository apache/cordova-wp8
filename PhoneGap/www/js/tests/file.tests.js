Tests.prototype.FileTests = function() {	
	module('File I/O (navigator.file)');
	test("should exist", function() {
  		expect(1);
  		ok(navigator.file != null, "navigator.file should not be null.");
	});
	test("should contain a read function", function() {
		expect(2);
		ok(typeof navigator.file.read != 'undefined' && navigator.file.read != null, "navigator.file.read should not be null.");
		ok(typeof navigator.file.read == 'function', "navigator.file.read should be a function.");
	});
	test("should contain a write function", function() {
		expect(2);
		ok(typeof navigator.file.write != 'undefined' && navigator.file.write != null, "navigator.file.write should not be null.");
		ok(typeof navigator.file.write == 'function', "navigator.file.write should be a function.");
	});
};