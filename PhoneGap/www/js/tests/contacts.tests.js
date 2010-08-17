Tests.prototype.ContactsTests = function() {
	module("Contacts model");
	test("should be able to define a Contacts object with name, emails, and phones attributes.", function() {
		expect(5);
		var con = new Contact();
		ok(con != null, "new Contact() should not be null.");
		ok(con.name != null, "new Contact() should include a 'name' property.");
		ok(con.name.formatted != null, "new Contact()'s 'name' property should, at the very least, contain a 'formatted' property.");
		ok(con.emails != null, "new Contact() should include an 'emails' array.");
		ok(con.phones != null, "new Contact() should include a 'phones' array.");
	});
	module('Contacts (navigator.contacts)');
	test("should exist", function() {
  		expect(1);
  		ok(navigator.contacts != null, "navigator.contacts should not be null.");
	});
	test("should contain a find function", function() {
		expect(2);
		ok(typeof navigator.contacts.find != 'undefined' && navigator.contacts.find != null, "navigator.contacts.find should not be null.");
		ok(typeof navigator.contacts.find == 'function', "navigator.contacts.find should be a function.");
	});
	// TODO: Need to add tests for the find function. Need to be able to include test data, but how? How do we add a contact 
	// 		 to the phone before running the test?
	// TODO: Need to include tests that check error-handling, doubt there is any in the framework code.
	test("contacts.find success callback should be called with an array", function() {
		expect(2);
		stop(tests.TEST_TIMEOUT);
		var win = function(result) {
			ok(typeof result == 'object', "Object returned in contacts.find success callback is of type 'object' (actually array).");
			ok(typeof result.length == 'number', "Object returned in contacts.find success callback has a length property which is numerical.");
			start();
		};
		var fail = function() { start(); };
		var filter = new Contact();
		filter.name.formatted = '';
		navigator.contacts.find(filter,win, fail);
	});
};
