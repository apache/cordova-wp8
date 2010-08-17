
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
