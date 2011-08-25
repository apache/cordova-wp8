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

namespace WP7GapClassLib
{
    public partial class PGView : UserControl
    {
        

        public PGView()
        {
            InitializeComponent();

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
                //Thread.Sleep(1000);

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
#if DEBUG
                    IsolatedStorageFile.GetUserStoreForApplication().Remove();
#endif 

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
                //string res = (string)GapBrowser.InvokeScript("JavaScriptFunctionWithParameters", "1");
                //System.Diagnostics.Debug.WriteLine("Called JS with result :: " + res);

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
            //{"action":"log","service":"Debug","params":"This is a message","callbackId":"Debug0"}

            string commandStr = e.Value;

            PhoneGapCommandCall commandCallParams = PhoneGapCommandCall.Parse(commandStr);

            if (commandCallParams == null)
            {
                // ERROR

                Debug.WriteLine(commandStr); // this is the case of window.error messages

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
            catch(Exception ex)
            {
                bc.OnCommandResult -= this.OnCommandResult;
                // TODO log somehow
                this.InvokeJSSCallback(commandCallParams.CallbackId, new PluginResult(PluginResult.Status.INVALID_ACTION));
                return;
            }

            // Javascript can only work in a single thread
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

        private void OnCommandResult(object sender, PluginResult result)
        {
            BaseCommand command  = sender as BaseCommand;
            
            if (command == null)
            {
                Debug.WriteLine("OnCommandResult missing argument");
                return;
            }

            if (result == null)
            {
                Debug.WriteLine("OnCommandResult missing argument");
                return;
            }

            // no callback requied
            if (!command.IsJSCallbackAttached) return;

            this.InvokeJSSCallback(command.JSCallackId, result);

        }

        private void InvokeJSSCallback(String callbackId, PluginResult result)
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
                if (result.IsSuccess)
                {
                    //
                    var res = this.GapBrowser.InvokeScript("PhoneGapCallbackSuccess", new string[] { callbackId, result.ToJSONString() });
                    Debug.WriteLine("InvokeScript returned :: " + res.ToString());
                }
                else
                {
                    this.GapBrowser.InvokeScript("PhoneGapCallbackError", new string[] { callbackId, result.ToJSONString() });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in InvokeJSSCallback :: " + ex.Message);
            }
        }
    }
}
