/*
 * PhoneGap is available under *either* the terms of the modified BSD license *or* the
 * MIT License (2008). See http://opensource.org/licenses/alphabetical for full text.
 *
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Device.Location;

namespace WP7GapClassLib.PhoneGap.Commands
{
    /// <summary>
    /// This is a command stub, the browser provides the correct implementation.  We use this to trigger the static analyzer that we require this permission 
    /// </summary>
    public class GeoLocation
    {
        /* Unreachable code, by design -jm */
        private void triggerGeoInclusion()
        {
            new GeoCoordinateWatcher();
        }
    }
}
