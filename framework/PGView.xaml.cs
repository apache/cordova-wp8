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
using Microsoft.Xna.Framework;
using WP7GapClassLib.PhoneGap;
using System.Threading;
using Microsoft.Phone.Shell;

namespace WP7GapClassLib
{
    public partial class PGView : UserControl
    {

        /// <summary>
        /// Indicates whether web control has been loaded and no additional initialization is needed.
        /// Prevents data clearing during page transitions.
        /// </summary>
        private bool IsBrowserInitialized = false;

        public PGView()
        {

            InitializeComponent();

            PhoneApplicationService.Current.Activated += new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(AppActivated);
            PhoneApplicationService.Current.Launching += new EventHandler<LaunchingEventArgs>(AppLaunching);
            PhoneApplicationService.Current.Deactivated += new EventHandler<DeactivatedEventArgs>(AppDeactivated);
            PhoneApplicationService.Current.Closing += new EventHandler<ClosingEventArgs>(AppClosing);

            StartupMode mode = PhoneApplicationService.Current.StartupMode;
            Debug.WriteLine("StartupMode mode =" + mode.ToString());

            if (mode == StartupMode.Activate)
            {

            }
            else
            {

            }
        }

        

        void AppClosing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("AppClosing");
        }

        void AppDeactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("AppDeactivated");
        }

        void AppLaunching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("AppLaunching");
        }

        void AppActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            Debug.WriteLine("AppActivated");
        }

        void GapBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                return;
            }

            // prevents refreshing web control to initial state during pages transitions
            if (this.IsBrowserInitialized) return;

            try
            {

                // Before we possibly clean the ISO-Store, we need to grab our generated UUID, so we can rewrite it after.
                string deviceUUID = "";

                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream("DeviceID.txt", FileMode.Open, FileAccess.Read, appStorage);

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            deviceUUID = reader.ReadLine();
                        }
                    }
                    catch (Exception /*ex*/)
                    {
                        deviceUUID = Guid.NewGuid().ToString();
                    }

                    Debug.WriteLine("Updating IsolatedStorage for APP:DeviceID :: " + deviceUUID);
                    // always overwrite user-iso-store if we are in debug mode.
#if DEBUG
                    appStorage.Remove();
#endif 

                    IsolatedStorageFileStream file = new IsolatedStorageFileStream("DeviceID.txt", FileMode.Create, FileAccess.Write, appStorage);
                    using (StreamWriter writeFile = new StreamWriter(file))
                    {
                        writeFile.WriteLine(deviceUUID);
                        writeFile.Close();
                    }
   
                }

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
                                        }
                                    }

                                }
                            }
                            else
                            {
                                Debug.WriteLine("Failed to write file :: " + file.path + " did you forget to add it to the project?");
                            }
                        }
                    }
                }

                // todo: this should be a start page param passed in via a getter/setter
                // aka StartPage

                Uri indexUri = new Uri("www/index.html", UriKind.Relative);
                this.GapBrowser.Navigate(indexUri);

                this.IsBrowserInitialized = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GapBrowser_Loaded :: {0}", ex.Message);
            }
        }

        void GapBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("GapBrowser_LoadCompleted");
        }


        void GapBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            Debug.WriteLine("GapBrowser_Navigating to :: " + e.Uri.ToString());
            // TODO: tell any running plugins to stop doing what they are doing.
            // TODO: check whitelist / blacklist
            // NOTE: Navigation can be cancelled by setting :        e.Cancel = true;
        }

        /*
         *  This method does the work of routing commands
         *  NotifyEventArgs.Value contains a string passed from JS 
         *  If the command already exists in our map, we will just attempt to call the method(action) specified, and pass the args along
         *  Otherwise, we create a new instance of the command, add it to the map, and call it ...
         *  This method may also receive JS error messages caught by window.onerror, in any case where the commandStr does not appear to be a valid command
         *  it is simply output to the debugger output, and the method returns.
         * 
         **/
        void GapBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {

            string commandStr = e.Value;
            Debug.WriteLine("GapBrowser_ScriptNotify :: " + commandStr);
            PhoneGapCommandCall commandCallParams = PhoneGapCommandCall.Parse(commandStr);

            if (commandCallParams == null)
            {
                // ERROR

                //Debug.WriteLine(commandStr); // this is the case of window.error messages

                return;
            }

            BaseCommand bc = CommandFactory.CreateUsingServiceName(commandCallParams.Service);
           
            if (bc == null)
            {
                this.InvokeJSSCallback(commandCallParams.CallbackId, new PluginResult(PluginResult.Status.CLASS_NOT_FOUND_EXCEPTION));
                return;
            }

            bc.OnCommandResult += new EventHandler<PluginResult>(OnCommandResult);
            bc.JSCallackId = commandCallParams.CallbackId;

            try
            {
                bc.InvokeMethodNamed(commandCallParams.Action, commandCallParams.Args);
            }
            catch(Exception)
            {
                bc.OnCommandResult -= this.OnCommandResult;
                // TODO log somehow
                this.InvokeJSSCallback(commandCallParams.CallbackId, new PluginResult(PluginResult.Status.INVALID_ACTION));
                return;
            }
        }

        private void GapBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GapBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Debug.WriteLine("GapBrowser_NavigationFailed :: " + e.Uri.ToString());
        }

        private void GapBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("GapBrowser_Navigated");
        }

        private void OnCommandResult(object sender, PluginResult result)
        {
            BaseCommand command = sender as BaseCommand;

            if (command == null)
            {
                Debug.WriteLine("OnCommandResult missing argument");
            }
            else if (result == null)
            {
                Debug.WriteLine("OnCommandResult missing argument");
            }
            else if (command.IsJSCallbackAttached)
            {
                this.InvokeJSSCallback(command.JSCallackId, result);
            }

            // else // no callback required

        }

        private void InvokeJSSCallback(String callbackId, PluginResult result)
        {
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {

                if (String.IsNullOrEmpty(callbackId))
                {
                    throw new ArgumentNullException("callbackId");
                }
            
                if (result == null)
                {
                    throw new ArgumentNullException("result");
                }

                //string callBackScript = result.ToCallbackString(callbackId, "commandResult", "commandError");

                // TODO: this is correct invokation method
                //this.GapBrowser.InvokeScript("eval", new string[] {callBackScript });

                /// But we temporary use this version because C#<->JS bridge is on fully ready
                /// 
                try
                {
                    string status = ((int)result.Result).ToString();
                    string jsonResult = result.ToJSONString();
                    this.GapBrowser.InvokeScript("PhoneGapCommandResult", new string[] { status, callbackId, jsonResult });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception in InvokeJSSCallback :: " + ex.Message);
                }
            });
        }

    }
}
