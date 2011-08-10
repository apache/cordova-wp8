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

            this.commands = new Dictionary<string, PhoneGapCommand>();
                                //{
                                //    // TODO: These need to be included based on plugin configuration
                                //    { "Accelerometer.GetCurrentAcceleration", new Plugins.Accelerometer.GetCurrentAccelerationCommand() },
                                //    { "Camera.GetPicture", new Plugins.Camera.GetPictureCommand() },
                                //    { "DebugConsole.debug", new Plugins.DebugConsole.DebugCommand() },
                                //    { "Device.GetAll", new Plugins.Device.GetAllCommand() },
                                //    { "Geolocation.GetCurrentPosition", new Plugins.Geolocation.GetCurrentPositionCommand() },
                                //    { "Network.IsReachable", new Plugins.Network.IsReachableCommand() },
                                //    { "Notification.Alert", new Plugins.Notification.AlertCommand() },
                                //    { "Notification.Beep", new Plugins.Notification.BeepCommand() },
                                //    { "Notification.Vibrate", new Plugins.Notification.VibrateCommand() },
                                //    { "Orientation.GetCurrentOrientation", new Plugins.Orientation.GetCurrentOrientationCommand() },
                                //    { "Orientation.ClearWatch", new Plugins.Orientation.ClearWatchCommmand() },
                                //    { "Orientation.WatchOrientation", new Plugins.Orientation.WatchOrientationCommmand() },
                                //    { "Send.Sms", new Plugins.Sms.SendCommand() },
                                //    { "Telephony.CallNumber", new Plugins.Telephony.CallCommand() }
                                //};

            this.watchers = new Watchers();
        }

        public void ProcessJavascriptCommand(string javascriptDetails)
        {
            // TODO: process JSON rep of command
            //
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

        // TODO: This needs to be included based on plugin configuration
        public void OrientationChanged(string newOrientation)
        {
            foreach (var watcher in this.watchers.OrientationChangedWatchers)
            {
                //this.ProcessAsyncCallback(
                //    null,
                //    new Plugins.Orientation.ChangedWatcherCallbackCommand
                //        {
                //            CallbackName = watcher.Value,
                //            CallbackArgs = new[] { watcher.Key, newOrientation }
                //        });
            }
        }
    }
}