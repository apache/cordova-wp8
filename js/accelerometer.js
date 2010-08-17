
/****************************** ACCELEROMETER ******************************/

function Accelerometer() {
  this._succssCallback = null;
  this._errorCallback = null;

  this.watchAcceleration = function (win, fail, opts) {
    // TODO
    // return watchId
  }

  this.clearWatch = function (watchId) {
    // TODO
  }
}

Accelerometer.prototype.getCurrentAcceleration = function (successCallback, errorCallback, options) {
  // We don't support any options so if anything is passed just ignore it
  this._succssCallback = successCallback;
  this._errorCallback = errorCallback;

  PhoneGap.exec("Accelerometer.GetCurrentAcceleration;getCurrentAcceleration_Success;getCurrentAcceleration_Error");
};

function getCurrentAcceleration_Success(x, y, z, changedTime) {
  acceleration = new Acceleration(x, y, z, changedTime);

  navigator.accelerometer._succssCallback(acceleration);
};

function getCurrentAcceleration_Error(errorMessage) {
  navigator.accelerometer._errorCallback(errorMessage);
};

if (typeof navigator.accelerometer == "undefined") {
  navigator.accelerometer = new Accelerometer();
}
