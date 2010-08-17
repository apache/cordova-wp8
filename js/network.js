
/****************************** NETWORK ******************************/

function Network() {
  this._callback = null;
}

Network.prototype.isReachable = function (hostName, successCallback, options) {
  this._callback = successCallback;
  PhoneGap.exec("Network.IsReachable;"+hostName+";networkIsReachable_Success;"+options);
};

function networkIsReachable_Success(reachability) {
  navigator.network._callback(reachability);
}

if (typeof navigator.network == "undefined") {
  navigator.network = new Network();
}

function NetworkStatus() { }
NetworkStatus.NOT_REACHABLE = 0;
NetworkStatus.REACHABLE_VIA_CARRIER_DATA_NETWORK = 1;
NetworkStatus.REACHABLE_VIA_WIFI_NETWORK = 2;
