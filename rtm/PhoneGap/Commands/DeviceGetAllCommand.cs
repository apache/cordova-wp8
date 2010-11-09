//-----------------------------------------------------------------------
// <copyright file="DeviceGetAllCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap.Commands
{
    public class DeviceGetAllCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            // Most of this info is not available in the Beta but should be there in the RTM
            this.HasCallback = true;
            this.CallbackName = args[0];
            this.CallbackArgs = new[]
                                    {
                                        "Windows Phone 7", // platform
                                        "7.0.6414", // version - Microsoft.Devices.Environment.OSVersion.Version.ToString()
                                        "???", // name
                                        "???", // uuid
                                        "0.9.X" // gap
                                    };
        }
    }
}