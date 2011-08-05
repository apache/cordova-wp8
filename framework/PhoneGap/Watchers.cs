//-----------------------------------------------------------------------
// <copyright file="Watchers.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace PhoneGap
{
    public class Watchers
    {
        private readonly Dictionary<string, string> orientationChangedWatchers;

        public Watchers()
        {
            this.orientationChangedWatchers = new Dictionary<string, string>();
        }

        public Dictionary<string, string> OrientationChangedWatchers
        {
            get { return this.orientationChangedWatchers; }
        }

        public void AddOrientationWatcher(string watchId, string callbackFunctionName)
        {
            this.orientationChangedWatchers.Add(watchId, callbackFunctionName);
        }

        public void ClearOrientationWatch(string watchId)
        {
            this.orientationChangedWatchers.Remove(watchId);
        }
    }
}