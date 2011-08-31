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
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using System.Diagnostics;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Device : BaseCommand
    {
        public void Get(string notused)
        {

            string res = String.Format("\"name\":\"{0}\",\"phonegap\":\"{1}\",\"platform\":\"{2}\",\"uuid\":\"{3}\",\"version\":\"{4}\"",
                                        this.name,
                                        this.phonegap,
                                        this.platform,
                                        this.uuid,
                                        this.version);


            res = "{" + res + "}";

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res));
        }

        public string name
        {
            get
            {
                return DeviceStatus.DeviceName;
            }
        }

        public string phonegap
        {
            get
            {
                // TODO: should be able to dynamically read the PhoneGap version from somewhere...
                return "1.1";
            }
        }

        public string platform
        {
            get
            {
                return Environment.OSVersion.Platform.ToString();
            }
        }

        public string uuid
        {
            get
            {
                string returnVal = "";
                object id;
                UserExtendedProperties.TryGetValue("ANID", out id);

                if (id != null)
                {
                    returnVal = id.ToString().Substring(2, 32);
                }
                else
                {
                    returnVal = "???unknown???";

                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        try
                        {
                            IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream("DeviceID.txt", FileMode.Open, FileAccess.Read, appStorage);

                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                returnVal = reader.ReadLine();
                            }
                        }
                        catch (Exception /*ex*/)
                        {

                        }
                    }
                }

                return returnVal;
            }
        }

        public string version
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }

    }
}
