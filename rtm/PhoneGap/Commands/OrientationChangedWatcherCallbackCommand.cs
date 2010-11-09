//-----------------------------------------------------------------------
// <copyright file="OrientationChangedWatcherCallbackCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap.Commands
{
    public class OrientationChangedWatcherCallbackCommand : PhoneGapCommand
    {
        public OrientationChangedWatcherCallbackCommand()
        {
            this.HasCallback = true;
        }

        public override void Execute(params string[] args)
        {
            // noop
        }
    }
}