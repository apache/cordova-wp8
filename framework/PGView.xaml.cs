using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.Windows.Interop;
using System.Runtime.Serialization.Json;
using System.IO;
using System.ComponentModel;
using System.Xml.Linq;
using WP7GapClassLib.PhoneGap.Commands;
using System.Diagnostics;
using System.Text;


namespace WP7GapClassLib
{
    public partial class PGView : UserControl
    {
        Dictionary<string, BaseCommand> commandMap;

        public PGView()
        {
            InitializeComponent();

            commandMap = new Dictionary<string, BaseCommand>();

            //Device device = new Device();
            //device.InvokeMethodNamed("Get");

            //Type t = Type.GetType("WP7GapClassLib.PhoneGap.Commands.Camera");

        }

        void GapBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                return;
            }
            try
            {

                StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("GapSourceDictionary.xml", UriKind.Relative));

                if (streamInfo != null)
                {
                    StreamReader sr = new StreamReader(streamInfo.Stream);
                    //This will Read Keys Collection for the xml file

                    XDocument document = XDocument.Parse(sr.ReadToEnd());

                    var files = from results in document.Descendants("FilePath")
                                 select new
                                 {
                                     path = (string)results.Attribute("Value")
                                 };
                    StreamResourceInfo fileResourceStreamInfo;

                    // always overwrite it if we are in debug mode.
                    //#if DEBUG
                    //IsolatedStorageFile.GetUserStoreForApplication().Remove();
                    //#endif 

                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {

                        foreach (var file in files)
                        {
                            fileResourceStreamInfo = Application.GetResourceStream(new Uri(file.path, UriKind.Relative));

                            if (fileResourceStreamInfo != null)
                            {
                                using (BinaryReader br = new BinaryReader(fileResourceStreamInfo.Stream))
                                {
                                    byte[] data = br.ReadBytes((int)fileResourceStreamInfo.Stream.Length);

                                    string strBaseDir = file.path.Substring(0, file.path.LastIndexOf('/'));
                                    appStorage.CreateDirectory(strBaseDir);

                                    // This will truncate/overwrite an existing file, or 
                                    using (IsolatedStorageFileStream outFile = appStorage.OpenFile(file.path, FileMode.Create))
                                    {
                                        Debug.WriteLine("Writing data for " + file.path + " and length = " + data.Length);
                                        using (var writer = new BinaryWriter(outFile))
                                        {
                                            writer.Write(data);
                                            writer.Flush();
                                            writer.Close();
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                // todo: this should be a start page param passed in via a getter/setter
                // aka StartPage
                //Uri indexUri = new Uri("http://www.google.com", UriKind.Absolute);
                Uri indexUri = new Uri("www/index.html", UriKind.Relative);
                this.GapBrowser.Navigate(indexUri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GapBrowser_Loaded :: {0}", ex.Message);
            }
        }

        void GapBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                //string res = (string)GapBrowser.InvokeScript("JavaScriptFunctionWithoutParameters");
                string res = (string)GapBrowser.InvokeScript("JavaScriptFunctionWithParameters", "1");
                System.Diagnostics.Debug.WriteLine("Called JS with result :: " + res);
            }
            catch (Exception ex)
            {

                MessageBoxResult res = MessageBox.Show("Could not call script: " + ex.Message, "caption", MessageBoxButton.OKCancel);

            }
        }

        void GapBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void GapBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //{"action":"log","service":"Debug","params":"This is a message","callbackId":"Debug0"}

            string commandStr = e.Value;

            string[] split = commandStr.Split('/');
            if (split.Length < 3)
            {
                // ERROR

                Debug.WriteLine(commandStr); // this is the case of window.error messages

                return;
            }
            string service = split[0];
            string action = split[1];
            string callbackId = split[2];
            string args = split[3];

            if (!commandMap.ContainsKey(service))
            {
                // TODO: if we do not find the command with that name, handle the error, somehow ...
                Type t = Type.GetType("WP7GapClassLib.PhoneGap.Commands." + service);
                if (t != null)
                {
                    BaseCommand bc = (BaseCommand)Activator.CreateInstance(t);
                    if (bc != null)
                    {
                        commandMap[service] = bc;
                        bc.InvokeMethodNamed(action, args);
                    }
                }
            }
            else
            {
                BaseCommand bc = commandMap[service];
                bc.InvokeMethodNamed(action, args);
            }
        }

        private void GapBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GapBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GapBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
