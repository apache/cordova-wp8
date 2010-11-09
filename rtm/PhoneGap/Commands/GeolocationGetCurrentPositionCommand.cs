//-----------------------------------------------------------------------
// <copyright file="GeolocationGetCurrentPositionCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Device.Location;

namespace PhoneGap.Commands
{
    public class GeolocationGetCurrentPositionCommand : PhoneGapCommand, IAsyncCommand
    {
        private GeoCoordinateWatcher watcher;

        public event EventHandler<PhoneGapCommand> OnCommandCompleted;

        private string successCallback;
        private string errorCallback;

        public override void Execute(params string[] args)
        {
            this.HasCallback = false; // This uses an AsyncCallback

            this.successCallback = args[0];
            this.errorCallback = args[1];

            if (NativeExecution.IsRunningOnEmulator())
            {
                this.CallbackName = this.successCallback;
                this.CallbackArgs = new[]
                                    {
                                        "51.318919",
                                        "-0.600214",
                                        "0",
                                        "0", 
                                        "0",
                                        "0",
                                        "0",
                                        DateTime.Now.ToString()
                                    };
                this.SendAsyncResponse();
            }
            else
            {
                var options = args[2].ToLowerInvariant();

                var accuracy = options.Equals("true") ? GeoPositionAccuracy.High : GeoPositionAccuracy.Default;

                this.watcher = new GeoCoordinateWatcher(accuracy)
                                   {
                                       MovementThreshold = 20
                                   };

                this.watcher.PositionChanged += this.watcher_PositionChanged;
                this.watcher.StatusChanged += this.watcher_StatusChanged;
                this.watcher.Start();
            }
        }

        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    this.CallbackName = this.errorCallback;
                    this.CallbackArgs = new[] { "location is unsupported on this device" };
                    this.SendAsyncResponse();
                    break;
                case GeoPositionStatus.NoData:
                    this.CallbackName = this.errorCallback;
                    this.CallbackArgs = new[] { "data unavailable" };
                    this.SendAsyncResponse();
                    break;
            }
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var epl = e.Position.Location;

            this.CallbackName = this.successCallback;
            this.CallbackArgs = new[]
                                    {
                                        epl.Latitude.ToString("0.000"),
                                        epl.Longitude.ToString("0.000"),
                                        epl.Altitude.ToString(),
                                        epl.HorizontalAccuracy.ToString(), 
                                        epl.VerticalAccuracy.ToString(),
                                        epl.Course.ToString(),
                                        epl.Speed.ToString(),
                                        e.Position.Timestamp.LocalDateTime.ToString()
                                    };
            this.SendAsyncResponse();
        }

        public void SendAsyncResponse()
        {
            if (this.OnCommandCompleted != null)
            {
                this.OnCommandCompleted(null, this);
                this.OnCommandCompleted = null; // Make sure that only return the result once
            }
        }
    }
}
