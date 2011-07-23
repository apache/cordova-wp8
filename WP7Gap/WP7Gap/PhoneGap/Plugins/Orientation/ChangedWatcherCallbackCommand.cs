//-----------------------------------------------------------------------
// <copyright file="ChangedWatcherCallbackCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap.Plugins.Orientation
{
    public class ChangedWatcherCallbackCommand : PhoneGapCommand
    {
        public ChangedWatcherCallbackCommand()
        {
            this.HasCallback = true;
        }

        public override void Execute(params string[] args)
        {
            // NOOP
        }
    }
}