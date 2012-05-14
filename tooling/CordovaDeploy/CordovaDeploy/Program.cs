using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SmartDevice.Connectivity;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Linq;

namespace CordovaDeploy
{
    class Program
    {

        static void Log(string msg)
        {
            Debug.WriteLine(msg);
            Console.Error.WriteLine(msg);
        }

        static void Usage()
        {
            Log("Usage:");
        }



        static void ListDevices()
        {
            // Get CoreCon WP7 SDK
            DatastoreManager dsmgrObj = new DatastoreManager(1033);
            Platform WP7SDK = dsmgrObj.GetPlatforms().Single(p => p.Name == "Windows Phone 7");
            Collection<Device> devices = WP7SDK.GetDevices();
            for (int index = 0; index < devices.Count; index++)
            {
                Device d = devices[index];
                string info = string.Format("{0} : {1} : {2}", index.ToString(), d.Id, d.Name);
                Log(info);
            }

        }

        static Guid ReadAppId(string root)
        {
            Guid appID = Guid.Empty;
            string manifestFilePath = root + @"\WMAppManifest.xml";

            if (File.Exists(manifestFilePath))
            {
                XDocument xdoc = XDocument.Load(manifestFilePath);
                var appNode = xdoc.Root.Descendants("App").FirstOrDefault();
                if (appNode != null)
                {
                    string guidStr = appNode.Attribute("ProductID").Value;
                    appID = new Guid(guidStr);
                }
                else
                {
                    Log(string.Format("Unable to find appID, expected to find an App.ProductID property defined in the file {0}", manifestFilePath));
                }
            }
            else
            {
                Log(string.Format("Error: the file {0} does not exist", manifestFilePath));
            }


            return appID;
        }

        static void Main(string[] args)
        {
            string iconFilePath = "";
            string xapFilePath = "";
            Guid appID = Guid.Empty;

            string root = Directory.GetCurrentDirectory();

            if (args.Length < 1)
            {
                Usage();
                return;
            }
            else if (args[0] == "-devices")
            {
                ListDevices();
                return;
            }

            if (Directory.Exists(args[0]))
            {
                DirectoryInfo info = new DirectoryInfo(args[0]);
                Debug.WriteLine("expanded path is : " + info.FullName);
                root = info.FullName;
            }

            appID = ReadAppId(root);

            if (appID == Guid.Empty)
            {
                return;
            }

            if (File.Exists(root + @"\ApplicationIcon.png"))
            {
                iconFilePath = root + @"\ApplicationIcon.png";
            }
            else
            {
                Log(string.Format("Error: could not find application icon at {0}", root + @"\ApplicationIcon.png"));
                return;
            }


            xapFilePath = Directory.GetFiles(root, "*.xap").FirstOrDefault();
            if (string.IsNullOrEmpty(xapFilePath))
            {
                Log(string.Format("Error: could not find application .xap in folder {0}", root));
                return;
            }


            // Get CoreCon WP7 SDK
            DatastoreManager dsmgrObj = new DatastoreManager(1033);
            Collection<Platform> WP7SDKs = dsmgrObj.GetPlatforms();//.Single(p => p.Name == "New Windows Mobile 7 SDK");

            Platform WP7SDK = dsmgrObj.GetPlatforms().Single(p => p.Name == "Windows Phone 7");

            Collection<Device> devices = null;

            devices = WP7SDK.GetDevices();

            //// Get Emulator / Device
            bool useEmulator = false;
            Device WP7Device = null;

            if (useEmulator)
                WP7Device = WP7SDK.GetDevices().Single(d => d.Name == "Windows Phone Emulator - 512 MB"); // || 256 ...
            else
                WP7Device = WP7SDK.GetDevices().Single(d => d.Name == "Windows Phone Device");

            if (WP7Device != null)
            {
                RemoteApplication app;
                bool isConnected = WP7Device.IsConnected();

                Debug.WriteLine(WP7Device.ToString());

                if (!isConnected)
                {
                    try
                    {
                        WP7Device.Connect();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }

                if (args.Contains("-r") && WP7Device.IsApplicationInstalled(appID))
                {
                    Log("Uninstalling sample XAP to Windows Phone 7 Emulator/Device...");

                    app = WP7Device.GetApplication(appID);
                    app.Uninstall();

                    Log("Sample XAP Uninstalled from Windows Phone 7 Emulator/Device...");
                }

                app = WP7Device.InstallApplication(appID, appID, "NormalApp", iconFilePath, xapFilePath);

                Log("XAP installed to Windows Phone 7 Emulator...");

                // Launch Application
                Log("Launching app on Windows Phone 7 Emulator...");
                app.Launch();

                Log("Launched app on Windows Phone 7 Emulator..." + app.InstanceID.ToString());

            }
        }
    }
}
