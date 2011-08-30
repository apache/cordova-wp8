using System;
using System.Net;
using System.Windows;
using Microsoft.Devices.Sensors;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Accelerometer : BaseCommand
    {
        private Microsoft.Devices.Sensors.Accelerometer accelerometer;

        public void getAcceleration(string options)
        {
            try
            {
                if (this.accelerometer == null)
                {
                    this.accelerometer = new Microsoft.Devices.Sensors.Accelerometer();
                }

                this.accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                this.accelerometer.Start();
            }
            catch (UnauthorizedAccessException ex1)
            {
                //this.CallbackName = this.errorCallback;
                //this.CallbackArgs = new[] { unauthorizedAccessException.Message };
                //this.SendAsyncResponse();
            }
            catch (AccelerometerFailedException ex2)
            {
                //this.CallbackName = this.errorCallback;
                //this.CallbackArgs = new[] { accelerometerFailedException.Message };
                //this.SendAsyncResponse();
            }
        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (this.accelerometer != null)
            {
                SensorState state = this.accelerometer.State;

                // TODO: switch on sensor state
                if (state == SensorState.Ready)
                {

                    string messageResult = String.Format("\"x\":{0},\"y\":{1},\"z\":{2}",
                                                e.SensorReading.Acceleration.X.ToString("0.00000"),
                                                e.SensorReading.Acceleration.Y.ToString("0.00000"),
                                                e.SensorReading.Acceleration.Z.ToString("0.00000"));

                    messageResult = "{" + messageResult + "}";

                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, messageResult));
                }
                else
                {
                    //this.CallbackName = this.errorCallback;
                    //this.CallbackArgs = new[] { state.ToString() };
                    //this.SendAsyncResponse();
                }

                try
                {
                    this.accelerometer.Stop();
                    this.accelerometer.Dispose();
                    this.accelerometer = null;
                }
                catch
                {
                    // NOOP - We don't, currently, care if we can't stop the accelerometer
                }
            }
        }
    }
}
