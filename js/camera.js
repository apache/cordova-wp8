
/****************************** CAMERA ******************************/

function Camera() {
  this._succssCallback = null;
  this._errorCallback = null;
}

Camera.prototype.getPicture = function (successCallback, errorCallback, options) {
  this._succssCallback = successCallback;
  this._errorCallback = errorCallback;
  PhoneGap.exec("Camera.GetPicture;getPicture_Success;getPicture_Error;" + options);
}

function getPicture_Success(imageData) {
  navigator.camera._succssCallback(imageData);
}

function getPicture_Error(errorMessage) {
  navigator.camera._errorCallback(errorMessage);
}

if (typeof navigator.camera == "undefined") {
  navigator.camera = new Camera();
}

