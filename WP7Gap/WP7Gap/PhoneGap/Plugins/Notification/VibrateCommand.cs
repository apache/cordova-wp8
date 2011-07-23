//-----------------------------------------------------------------------
// <copyright file="VibrateCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using Microsoft.Devices;

namespace PhoneGap.Plugins.Notification
{
    internal class VibrateCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            this.HasCallback = false;

            // The emulator doesn't support vibration
            if (!NativeExecution.IsRunningOnEmulator())
            {
                var vibrator = VibrateController.Default;
                vibrator.Start(TimeSpan.FromMilliseconds(int.Parse(args[0])));
            }
        }
    }
}