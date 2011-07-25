//-----------------------------------------------------------------------
// <copyright file="DeviceGetAllCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.Phone.Info;

namespace PhoneGap.Plugins.Device
{
    public class GetAllCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            object id;

            UserExtendedProperties.TryGetValue("ANID", out id);

            var uuid = id == null ? "???" : id.ToString().Substring(2, 32);

            this.HasCallback = true;
            this.CallbackName = args[0];
            this.CallbackArgs = new[]
                                    {
                                        Environment.OSVersion.Platform.ToString(),
                                        Environment.OSVersion.Version.ToString(), // or DeviceStatus.DeviceFirmwareVersion ???
                                        DeviceStatus.DeviceName, // name
                                        uuid, // no UUID but this is hopefully a suitable alternative
                                        "1.X" // gap version
                                    };
        }
    }
}