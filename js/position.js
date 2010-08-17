
/****************************** POSITION ******************************/

function Position(coords, timestamp) {
  this.coords = coords;
  this.timestamp = new Date().getTime();
}

function Accuracy(horizontal, vertical) {
  this.horizontal = horizontal;
  this.vertical = vertical;
}

function Coordinates(lat, lng, alt, acc, head, vel) {
  // The latitude of the position.
  this.latitude = lat;

  // The longitude of the position.
  this.longitude = lng;

  // The accuracy of the position.
  this.accuracy = acc;

  // The altitude of the position.
  this.altitude = alt;

  // The direction the device is moving at the position.
  this.heading = head;

  // The velocity with which the device is moving at the position.
  this.speed = vel;
}

function PositionOptions() {
  // Specifies the desired position accuracy.
  this.enableHighAccuracy = false;

  // The timeout after which if position data cannot be obtained the errorCallback is called.
  this.timeout = 10000;
}

function PositionError() {
  this.code = null;
  this.message = "";
}

PositionError.UNKNOWN_ERROR = 0;
PositionError.PERMISSION_DENIED = 1;
PositionError.POSITION_UNAVAILABLE = 2;
PositionError.TIMEOUT = 3;

