using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Net.NetworkInformation;

namespace WP7GapClassLib.PhoneGap.Commands
{

    // http://msdn.microsoft.com/en-us/library/microsoft.phone.net.networkinformation(v=VS.92).aspx
    // http://msdn.microsoft.com/en-us/library/microsoft.phone.net.networkinformation.devicenetworkinformation(v=VS.92).aspx

    public class Connection : BaseCommand
    {
        const string UNKNOWN = "Unknown connection";
        const string ETHERNET = "Ethernet connection";
        const string WIFI = "WiFi connection";
        const string CELL_2G = "Cell 2G connection";
        const string CELL_3G = "Cell 3G connection";
        const string CELL_4G = "Cell 4G connection";
        const string NONE = "No network connection";
        const string CELL = "Cell connection";

        public string type
        {
            get
            {
                return checkConnectionType();
            }
        }

        private string checkConnectionType()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                if (DeviceNetworkInformation.IsWiFiEnabled)
                {
                    return WIFI;
                }
                else
                {
                    if (DeviceNetworkInformation.IsCellularDataEnabled)
                    {
                        // WP7 doesn't let us determine which type of cell data network
                        return CELL;
                    }
                    else
                    {
                        return UNKNOWN;
                    }
                }
            }
            else
            {
                return NONE;
            }
        }

        
    }
}
