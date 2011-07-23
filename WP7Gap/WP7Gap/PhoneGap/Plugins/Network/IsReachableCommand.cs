//-----------------------------------------------------------------------
// <copyright file="IsReachableCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Net;
using Microsoft.Phone.Net.NetworkInformation;

namespace PhoneGap.Plugins.Network
{
    /// <summary>
    /// Responses:
    ///   NetworkStatus.NOT_REACHABLE = 0;
    ///   NetworkStatus.REACHABLE_VIA_CARRIER_DATA_NETWORK = 1;
    ///   NetworkStatus.REACHABLE_VIA_WIFI_NETWORK = 2;
    /// </summary>
    public class IsReachableCommand : PhoneGapCommand, IAsyncCommand
    {
        public event EventHandler<PhoneGapCommand> OnCommandCompleted;

        public override void Execute(params string[] args)
        {
            this.HasCallback = false; // This uses an AsyncCallback

            this.CallbackName = args[1];

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += this.UpdateCheckCompleted;

                try
                {
                    wc.DownloadStringAsync(new Uri(args[0]));
                }
                catch (UriFormatException)
                {
                    this.CallbackArgs = new[] { "0" };
                    this.SendAsyncResponse();
                }
            }
            else
            {
                this.CallbackArgs = new[] { "0" };
                this.SendAsyncResponse();
            }
        }

        private void UpdateCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var result = 0; // Default to not reachable

            if (e.Error == null)
            {
                // May have to do extra chacks against the response (e.Result)
                // And/or may need to check other interface types
                result = NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ? 2 : 1;
            }

            this.CallbackArgs = new[] { result.ToString() };

            this.SendAsyncResponse();
        }

        public void SendAsyncResponse()
        {
            if (this.OnCommandCompleted != null)
            {
                this.OnCommandCompleted(null, this);
            } 
        }
    }
}
