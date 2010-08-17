
/****************************** SMS ******************************/

function Sms() {
    this._errorCallback = null;
}

Sms.prototype.send = function (number, message, successCallback, errorCallback, options) {
// successcallback and options are not supported on WP7
  this._errorCallback = errorCallback;
  PhoneGap.exec("Send.Sms;send_Error;" + number + ";" + message);
}

function send_Error(errorMessage) {
  navigator.sms._errorCallback(errorMessage);
}

if (typeof navigator.sms == "undefined") {
  navigator.sms = new Sms();
}

