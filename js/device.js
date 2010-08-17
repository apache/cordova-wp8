
/****************************** DEVICE ******************************/

function Device() {
  this.platform = null;
  this.version = null;
  this.name = null;
  this.uuid = null;
  this.gap = null;
}

navigator.device = window.device = new Device();
