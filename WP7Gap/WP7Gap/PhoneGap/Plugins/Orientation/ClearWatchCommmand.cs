//-----------------------------------------------------------------------
// <copyright file="ClearWatchCommmand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap.Plugins.Orientation
{
    public class ClearWatchCommmand : PhoneGapCommand, IWatcherCommand
    {
        public override void Execute(params string[] args)
        {
            // NOOP
        }

        public void WatcherExecute(ref Watchers native, params string[] args)
        {
            native.ClearOrientationWatch(args[0]);
        }
    }
}