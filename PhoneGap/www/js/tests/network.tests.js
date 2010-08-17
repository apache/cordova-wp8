Tests.prototype.NetworkTests = function() {
	module('Network (navigator.network)');
	test("should exist", function() {
  		expect(1);
  		ok(navigator.network != null, "navigator.network should not be null.");
	});
	test("should contain an isReachable function", function() {
		expect(2);
		ok(typeof navigator.network.isReachable != 'undefined' && navigator.network.isReachable != null, "navigator.network.isReachable should not be null.");
		ok(typeof navigator.network.isReachable == 'function', "navigator.network.isReachable should be a function.");
	});
	test("should define constants for network status", function() {
		expect(4);
		ok(NetworkStatus != null, "NetworkStatus object exists in global scope.");
		equals(NetworkStatus.NOT_REACHABLE, 0, "NetworkStatus.NOT_REACHABLE is equal to 0.");
		equals(NetworkStatus.REACHABLE_VIA_CARRIER_DATA_NETWORK, 1, "NetworkStatus.REACHABLE_VIA_CARRIER_DATA_NETWORK is equal to 1.");
		equals(NetworkStatus.REACHABLE_VIA_WIFI_NETWORK, 2, "NetworkStatus.REACHABLE_VIA_WIFI_NETWORK is equal to 2.");
	});
	test("isReachable function should return an object with a 'code' member as a NetworkStatus constant in its success callback", function() {
		expect(1);
		stop(Tests.TEST_TIMEOUT);
		var hostname = "http://www.google.com";
		var win = function(p) {
			ok(p.code == NetworkStatus.NOT_REACHABLE || p.code == NetworkStatus.REACHABLE_VIA_CARRIER_DATA_NETWORK || p.code == NetworkStatus.REACHABLE_VIA_WIFI_NETWORK, "Success callback in isReachable returns a proper object with a 'code' member equal to a NetworkStatus constant.");
			start();
		};
		navigator.network.isReachable(hostname, win);
	});
};