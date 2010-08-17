
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

