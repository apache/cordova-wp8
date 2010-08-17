//-----------------------------------------------------------------------
// <copyright file="OrientationGetCurrentOrientationCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using Microsoft.Phone.Controls;

namespace PhoneGap.Commands
{
    public class OrientationGetCurrentOrientationCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            this.HasCallback = args.Length >= 1;

            if (this.HasCallback)
            {
                this.CallbackName = args[0];
                this.CallbackArgs = new[] { (App.Current.RootVisual as PhoneApplicationFrame).Orientation.ToString() };
            }
        }
    }
}
