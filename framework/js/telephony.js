
/****************************** TELEPHONY ******************************/

function Telephony() {
}

Telephony.prototype.call = function (number) {
  PhoneGap.exec("Telephony.CallNumber;"+number);
};

// The spec say this, but the above makes more sense (to me at least)
Telephony.prototype.send = function (number) {
  PhoneGap.exec("Telephony.CallNumber;"+number);
};

if (typeof navigator.telephony == "undefined") {
  navigator.telephony = new Telephony();
}

