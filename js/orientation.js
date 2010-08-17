
/****************************** ORIENTATION ******************************/

function Orientation() {
  this._succssCallback = null;
  this._errorCallback = null;
  this._watches = new Object;
  this._baseWatchId = 1;
  this.currentOrientation = null;
}

function getCurrentOrientation_Success(newOrientation) {
  navigator.orientation.currentOrientation = newOrientation;
  navigator.orientation._succssCallback(newOrientation);
}

function getCurrentOrientation_Error(errorMessage) {
  navigator.orientation._errorCallback(errorMessage);
}

Orientation.prototype.getCurrentOrientation = function (successCallback, errorCallback) {
  this._succssCallback = successCallback;
  this._errorCallback = errorCallback;

  PhoneGap.exec("Orientation.GetCurrentOrientation;getCurrentOrientation_Success;getCurrentOrientation_Error");
}

Orientation.prototype.watchOrientation = function (successCallback, errorCallback) {
  var newId = this._baseWatchId;
  this._baseWatchId = this._baseWatchId + 1;

  // Not currenlty doing anything with errorCallBack
  this._watches[newId] = successCallback;
  PhoneGap.exec("Orientation.WatchOrientation;watchOrientation_Changed;" + newId);

  return newId;
}

Orientation.prototype.clearWatch = function (watchId) {
  this._watches[watchId] = null;
  PhoneGap.exec("Orientation.ClearWatch;"+watchId);
}

function watchOrientation_Changed(watchId, newOrientation) {
  navigator.orientation.currentOrientation = newOrientation;

  if (navigator.orientation._watches[watchId] != null) {
    navigator.orientation._watches[watchId](newOrientation);
  }
}

if (typeof navigator.orientation == "undefined") {
  navigator.orientation = new Orientation();
}
