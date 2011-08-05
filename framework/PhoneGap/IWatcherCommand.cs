//-----------------------------------------------------------------------
// <copyright file="IWatcherCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap
{
    internal interface IWatcherCommand
    {
        void WatcherExecute(ref Watchers native, params string[] args);
    }
}