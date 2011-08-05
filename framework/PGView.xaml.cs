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


namespace WP7GapClassLib
{
    public partial class PGView : UserControl
    {
        public PGView()
        {
            InitializeComponent();
            GapBrowser.IsScriptEnabled = true;
            GapBrowser.ScriptNotify += new EventHandler<NotifyEventArgs>(GapBrowser_ScriptNotify);
            GapBrowser.Navigating += new EventHandler<NavigatingEventArgs>(GapBrowser_Navigating);
            GapBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(GapBrowser_LoadCompleted);
            GapBrowser.Loaded += new RoutedEventHandler(GapBrowser_Loaded);
            GapBrowser.NavigationFailed += new System.Windows.Navigation.NavigationFailedEventHandler(GapBrowser_NavigationFailed);

            GapBrowser.Unloaded += new RoutedEventHandler(GapBrowser_Unloaded);

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
                    StreamResourceInfo fileRespourceStreamInfo;

                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        // always overwrite it if we are in debug mode.
                        #if DEBUG
                            isoStore.Remove();
                        #endif 

                            foreach (var file in files)
                            {
                                fileRespourceStreamInfo = Application.GetResourceStream(new Uri(file.path, UriKind.Relative));

                                if (fileRespourceStreamInfo != null)
                                {
                                    using (BinaryReader br = new BinaryReader(fileRespourceStreamInfo.Stream))
                                    {
                                        byte[] data = br.ReadBytes((int)fileRespourceStreamInfo.Stream.Length);
                                        SaveFileToIsoStore(file.path, data);
                                    }
                                }
                            }
                    }
                }

                Uri indexUri = new Uri("www/index.html", UriKind.Relative);
                GapBrowser.Navigate(indexUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GapBrowser_Loaded :: {0}", ex.Message);
            }
        }

        private static void SaveFileToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            string[] dirsPath = fileName.Split(new[] { '/' });

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Recreate the directory structure
                for (int i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }

                if (!isoStore.FileExists(fileName))
                {
                    using (var bw = new BinaryWriter(isoStore.CreateFile(fileName)))
                    {
                        bw.Write(data);
                    }
                }
            }
        }

        void GapBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void GapBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void GapBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GapBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GapBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
