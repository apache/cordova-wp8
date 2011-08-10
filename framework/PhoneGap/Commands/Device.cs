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
using Microsoft.Phone.Info;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Device : BaseCommand
    {
        public string Get()
        {
            object id;
            string uuid = "???";

            UserExtendedProperties.TryGetValue("ANID", out id);

            if(id != null)
            {
                uuid = id.ToString().Substring(2, 32);
            }

            string platform = Environment.OSVersion.Platform.ToString();
            string version = Environment.OSVersion.Version.ToString();
            string name = DeviceStatus.DeviceName;
            string phonegap = "1.0";

            return "";   
        }


        
    }
}
