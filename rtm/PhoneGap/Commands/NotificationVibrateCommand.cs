//-----------------------------------------------------------------------
// <copyright file="NotificationVibrateCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using Microsoft.Devices;

namespace PhoneGap.Commands
{
    internal class NotificationVibrateCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            this.HasCallback = false;

            // The emulator doesn't support vibration
            if (!NativeExecution.IsRunningOnEmulator())
            {
                var vibrator = VibrateController.Default;
                vibrator.Start(new TimeSpan(0, 0, 0, 0, int.Parse(args[0])));
            }
        }
    }
}