//-----------------------------------------------------------------------
// <copyright file="GetCurrentOrientationCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using Microsoft.Phone.Controls;

namespace PhoneGap.Plugins.Orientation
{
    public class GetCurrentOrientationCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            this.HasCallback = args.Length >= 1;

            if (this.HasCallback)
            {
                this.CallbackName = args[0];
                this.CallbackArgs = new[]
                    {
                        // TODO: be sure to make sure this is templated correctly
                        (WP7Gap.App.Current.RootVisual as PhoneApplicationFrame).Orientation.ToString()
                    };
            }
        }
    }
}
