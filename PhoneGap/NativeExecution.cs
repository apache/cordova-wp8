//-----------------------------------------------------------------------
// <copyright file="NativeExecution.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using PhoneGap.Commands;

namespace PhoneGap
{
    public class NativeExecution
    {
        private readonly WebBrowser webBrowser;

        private readonly Dictionary<string, PhoneGapCommand> commands;

        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here as this needs to be public to get passed as a ref.")]
        public Watchers watchers;

        public NativeExecution(ref WebBrowser browser)
        {
            this.webBrowser = browser;

            this.commands = new Dictionary<string, PhoneGapCommand>
                                {
                                    { "Notification.Alert", new NotificationAlertCommand() },
                                    { "Notification.Beep", new NotificationBeepCommand() },
                                    { "Notification.Vibrate", new NotificationVibrateCommand() },
                                    { "Camera.GetPicture", new CameraGetPictureCommand() },
                                    { "Send.Sms", new SmsSendCommand() },
                                    { "Orientation.GetCurrentOrientation", new OrientationGetCurrentOrientationCommand() },
                                    { "Orientation.ClearWatch", new OrientationClearWatchCommmand() },
                                    { "Orientation.WatchOrientation", new OrientationWatchOrientationCommmand() },
                                    { "Telephony.CallNumber", new TelephonyCallCommand() },
                                    { "Device.GetAll", new DeviceGetAllCommand() },
                                    { "Network.IsReachable", new NetworkIsReachableCommand() },
                                    { "Geolocation.GetCurrentPosition", new GeolocationGetCurrentPositionCommand() },
                                    { "Accelerometer.GetCurrentAcceleration", new AccelerometergetCurrentAccelerationCommand() },
                                    { "DebugConsole.debug", new DebugConsoleDebugCommand() }
                                };

            this.watchers = new Watchers();
        }

        public void ProcessJavascriptCommand(string javascriptDetails)
        {
            var jsParams = javascriptDetails.Split(';');

            var commandName = jsParams[0];
            var commandArgs = jsParams.Skip(1).Take(jsParams.Count() - 1).ToArray();

            if (this.commands.ContainsKey(commandName))
            {
                var cmd = this.commands[commandName];

                if (cmd is IAsyncCommand)
                {
                    (cmd as IAsyncCommand).OnCommandCompleted += this.ProcessAsyncCallback;

                    cmd.Execute(commandArgs);
                }
                else if (cmd is IWatcherCommand)
                {
                    (cmd as IWatcherCommand).WatcherExecute(ref this.watchers, commandArgs);
                }
                else
                {
                    cmd.Execute(commandArgs);

                    if (cmd.HasCallback)
                    {
                        this.webBrowser.InvokeScript(cmd.CallbackName, cmd.CallbackArgs);
                    }
                }
            }
        }

        private void ProcessAsyncCallback(object sender, PhoneGapCommand command)
        {
            this.webBrowser.InvokeScript(command.CallbackName, command.CallbackArgs);
        }

        public static bool IsRunningOnEmulator()
        {
            return Environment.DeviceType == DeviceType.Emulator;
        }

        public void OrientationChanged(string newOrientation)
        {
            foreach (var watcher in this.watchers.OrientationChangedWatchers)
            {
                this.ProcessAsyncCallback(null, new OrientationChangedWatcherCallbackCommand
                                                    {
                                                        CallbackName = watcher.Value,
                                                        CallbackArgs = new[] { watcher.Key, newOrientation }
                                                    });
            }
        }
    }
}