//-----------------------------------------------------------------------
// <copyright file="GetCurrentAccelerationCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using Microsoft.Devices.Sensors;

namespace PhoneGap.Plugins.Accelerometer
{
    public class GetCurrentAccelerationCommand : PhoneGapCommand, IAsyncCommand
    {
        private Microsoft.Devices.Sensors.Accelerometer accelerometer;

        private string successCallback;
        private string errorCallback;

        public event EventHandler<PhoneGapCommand> OnCommandCompleted;

        public override void Execute(params string[] args)
        {
            this.HasCallback = false; // This uses an AsyncCallback

            this.successCallback = args[0];
            this.errorCallback = args[1];

            try
            {
                if (this.accelerometer == null)
                {
                    this.accelerometer = new Microsoft.Devices.Sensors.Accelerometer();
                }

                // TODO: remove deprecated event
                this.accelerometer.ReadingChanged += this.AccelerometerReadingChanged;
                this.accelerometer.Start();
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                this.CallbackName = this.errorCallback;
                this.CallbackArgs = new[] { unauthorizedAccessException.Message };
                this.SendAsyncResponse();
            }
            catch (AccelerometerFailedException accelerometerFailedException)
            {
                this.CallbackName = this.errorCallback;
                this.CallbackArgs = new[] { accelerometerFailedException.Message };
                this.SendAsyncResponse();
            }
        }

        private void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => this.ThreadedReadingChanged(e));
        }

        private void ThreadedReadingChanged(AccelerometerReadingEventArgs e)
        {
            if (this.accelerometer != null)
            {
                var state = this.accelerometer.State;

                if (state == SensorState.Ready)
                {
                    this.CallbackName = this.successCallback;

                    this.CallbackArgs = new[]
                                            {
                                                e.X.ToString("0.00"),
                                                e.Y.ToString("0.00"),
                                                e.Z.ToString("0.00"),
                                                e.Timestamp.ToString()
                                            };
                    this.SendAsyncResponse();
                }
                else
                {
                    this.CallbackName = this.errorCallback;
                    this.CallbackArgs = new[] { state.ToString() };
                    this.SendAsyncResponse();
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

        public void SendAsyncResponse()
        {
            if (this.OnCommandCompleted != null)
            {
                this.OnCommandCompleted(null, this);
                this.OnCommandCompleted = null;
            }
        }
    }
}