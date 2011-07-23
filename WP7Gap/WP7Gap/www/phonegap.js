
/****************************** ACCELERATION ******************************/

function Acceleration(x, y, z, timestamp) {
  // The force applied by the device in the x-axis.
  this.x = x;

  // The force applied by the device in the y-axis.
  this.y = y;

  // The force applied by the device in the z-axis.
  this.z = z;

  // The time that the acceleration was obtained.
  this.timestamp = timestamp || new Date().getTime();
}

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


/****************************** DEBUGCONSOLE ******************************/

/**
 * This class provides access to the debugging console.
 * @constructor
 */
function DebugConsole() {
}

/**
 * Utility function for rendering and indenting strings, or serializing
 * objects to a string capable of being printed to the console.
 * @param {Object|String} message The string or object to convert to an indented string
 * @private
 */
DebugConsole.prototype.processMessage = function(message) {
    if (typeof(message) != 'object') {
        return message;
    } else {
        /**
         * @function
         * @ignore
         */
        function indent(str) {
            return str.replace(/^/mg, "    ");
        }
        /**
         * @function
         * @ignore
         */
        function makeStructured(obj) {
            var str = "";
            for (var i in obj) {
                try {
                    if (typeof(obj[i]) == 'object') {
                        str += i + ":\n" + indent(makeStructured(obj[i])) + "\n";
                    } else {
                        str += i + " = " + indent(String(obj[i])).replace(/^    /, "") + "\n";
                    }
                } catch(e) {
                    str += i + " = EXCEPTION: " + e.message + "\n";
                }
            }
            return str;
        }
        return "Object:\n" + makeStructured(message);
    }
};

/**
 * Print a normal log message to the console
 * @param {Object|String} message Message or object to print to the console
 */
DebugConsole.prototype.log = function(message) {
  if (PhoneGap.available) {
    PhoneGap.exec('DebugConsole;INFO;' + this.processMessage(message));
  }
};

/**
 * Print a warning message to the console
 * @param {Object|String} message Message or object to print to the console
 */
DebugConsole.prototype.warn = function(message) {
  if (PhoneGap.available) {
    PhoneGap.exec('DebugConsole;WARN;' + this.processMessage(message));
  }
};

/**
 * Print an error message to the console
 * @param {Object|String} message Message or object to print to the console
 */
DebugConsole.prototype.error = function(message) {
  if (PhoneGap.available) {
    PhoneGap.exec('DebugConsole;ERROR;' + this.processMessage(message));
  }
};

if (typeof window.debug == "undefined") {
  window.debug = new DebugConsole();
}

/****************************** DEVICE ******************************/

function Device() {
  this.platform = null;
  this.version = null;
  this.name = null;
  this.uuid = null;
  this.gap = null;
}

navigator.device = window.device = new Device();

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

/****************************** NOTIFICATION ******************************/

function Notification() {
}

//// @title is optional
//// @button is ignored as this can't be specified in WP7
Notification.prototype.alert = function (message, title, button) {
  PhoneGap.exec("Notification.Alert;" + message + ";" + title + ";" + button);
};

//// The docs specify this signature
//// but all other implementations use:  function(count, volume)
Notification.prototype.beep = function (times) {
  PhoneGap.exec("Notification.Beep;" + times);
};

Notification.prototype.vibrate = function (duration) {
  PhoneGap.exec("Notification.Vibrate;" + duration);
};

if (typeof navigator.notification == "undefined") {
  navigator.notification = new Notification();
}


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


/****************************** TELEPHONY ******************************/

function Telephony() {
}

Telephony.prototype.call = function (number) {
  PhoneGap.exec("Telephony.CallNumber;"+number);
};

// The spec say this, but hte above makes more sense (to me at least)
Telephony.prototype.send = function (number) {
  PhoneGap.exec("Telephony.CallNumber;"+number);
};

if (typeof navigator.telephony == "undefined") {
  navigator.telephony = new Telephony();
}


/****************************** PHONEGAP-BASE ******************************/

/**
 * This represents the PhoneGap API itself.
 * It is dramatically simplified from other implementations as I couldn't get the queued processing of constructors working
 */
PhoneGap = {
};

PhoneGap.exec = function(nativeMethodAndArgs) {
  window.external.Notify(nativeMethodAndArgs);
};

PhoneGap.available = false;

function SetDeviceProperties(platform, version, name, uuid, gap) {
  device.platform = platform;
  device.version = version;
  device.name = name;
  device.uuid = uuid;
  device.gap = gap;

  // in theory, everything is now set up and working so this is where we should fire the deviceready event
  // unfortunately IE7 mobile doesn't seem to support a way of dynaically raising events
  PhoneGap.available = true;
}

// Load device info
PhoneGap.exec("Device.GetAll;SetDeviceProperties");
