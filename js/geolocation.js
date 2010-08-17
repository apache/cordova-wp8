
/****************************** GEOLOCATION ******************************/

function Geolocation() {
  this._succssCallback = null;
  this._errorCallback = null;
  this.lastPosition = null;

  this.watchPosition = function(successCallback, errorCallback, options) {
    // TODO
    // return watchId
  }

  this.clearWatch = function(watchId) {
    // TODO
  }
}

Geolocation.prototype.getCurrentPosition = function(successCallback, errorCallback, options) {
  this._succssCallback = successCallback;
  this._errorCallback = errorCallback;

  PhoneGap.exec("Geolocation.GetCurrentPosition;getCurrentPosition_Success;getCurrentPosition_Error;" + options);
};

function getCurrentPosition_Success(lat, lng, altitude, horizontalaccuracy, verticalaccuracy, heading, velocity, time) {
  accuracy = new Accuracy(horizontalaccuracy, verticalaccuracy);
  coords = new Coordinates(lat, lng, altitude, accuracy, heading, velocity);
  position = new Position(coords, time);

  navigator.lastPosition = position;
  navigator.geolocation._succssCallback(position);
}

function getCurrentPosition_Error(errorMessage) {
  navigator.geolocation._errorCallback(errorMessage);
}

if (typeof navigator.geolocation == "undefined") {
  navigator.geolocation = new Geolocation();
}
