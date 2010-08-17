//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Phone.Controls;

namespace PhoneGap
{
    public partial class MainPage
    {
        private readonly Stack<Uri> historyList;
        private NativeExecution native;

        public MainPage()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            this.historyList = new Stack<Uri>();

            Loaded += this.MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.native = new NativeExecution(ref webBrowser1);

            // Load all content files into IsolatedStorage in a separate thread to avoid issues with it taking longer that the system will allow for startup
            BackgroundWorker isoLoader = new BackgroundWorker
                                             {
                                                 WorkerReportsProgress = false,
                                                 WorkerSupportsCancellation = false
                                             };
            isoLoader.DoWork += isoLoader_DoWork;
            isoLoader.RunWorkerCompleted += this.isoLoader_RunWorkerCompleted;
            isoLoader.RunWorkerAsync();
        }

        private void isoLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            webBrowser1.Navigate(new Uri("www/index.html", UriKind.Relative));
        }

        private static void isoLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveFilesInWwwFolderToIsoStore();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            // Navigate back through page history if there is any
            if (this.historyList.Count > 1)
            {
                this.historyList.Pop();
                this.webBrowser1.Navigate(this.historyList.Peek());
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        private static void SaveFilesInWwwFolderToIsoStore()
        {
#if DEBUG 
            // This deletes all existing files in IsolatedStorage - Useful in testing
            // In live should not do this, but only load files once - this speeds subsequent loading of the app
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.Remove();
            }
#endif
            string[] files = AllFilesInWwwFolder();

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.FileExists(files[0]))
                {
                    foreach (string f in files)
                    {
                        StreamResourceInfo sr = Application.GetResourceStream(new Uri(f, UriKind.Relative));

                        // T4 Template includes all files in source folder(s). This may include some which are not in the project
                        if (sr != null)
                        {
                            using (BinaryReader br = new BinaryReader(sr.Stream))
                            {
                                byte[] data = br.ReadBytes((int)sr.Stream.Length);
                                SaveFileToIsoStore(f, data);
                            }
                        }
                    }
                }
            }
        }

        private static void SaveFileToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            const string DelimStr = "/";
            char[] delimiter = DelimStr.ToCharArray();
            string[] dirsPath = fileName.Split(delimiter);

            // Get the IsoStore
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Recreate the directory structure
                for (int i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }

                // Create the file if not exists
                if (!isoStore.FileExists(fileName))
                {
                    // Write the file
                    using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(fileName)))
                    {
                        bw.Write(data);
                    }
                }
            }
        }

        private void webBrowser1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            this.native.ProcessJavascriptCommand(e.Value);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            this.native.OrientationChanged(e.Orientation.ToString());

            base.OnOrientationChanged(e);
        }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // The webbrowsercontrol doesn't allow us to navigate backeards through it's history stack
            // so we create our own to allow simulation of going backwards between pages

            // Don't track the page we go BACK to
            if ((this.historyList.Count == 0) || (e.Uri != this.historyList.Peek()))
            {
                this.historyList.Push(e.Uri);
            }

            // After first page loaded, hide the fake splash screen
            if (this.historyList.Count == 1)
            {
                // Using own loadingimage/splashscreen so don't see empty web browser while waiting for first page to load
                // Might be nice to animate removal/hiding to be more like default splash screen behaviour
                this.loadingImage.Visibility = Visibility.Collapsed;
            }
        }

        /*
         * If using VS2010 Express Edition:
         * - remove reference to MainPage.xaml.gen.tt
         * - uncomment the method below
         * - Update the list of files below so it includes ALL used files
         */
        ////private static string[] AllFilesInWwwFolder()
        ////{
        ////    return new [] {
        ////                    "www/index.html",
        ////                    "www/mobile-spec.html",
        ////                    "www/pg_logo.png",
        ////                    "www/css/master.css",
        ////                    "www/css/qunit.css",
        ////                    "www/images/ArrowImg.png",
        ////                    "www/js/phonegap.js",
        ////                    "www/js/qunit.js",
        ////                    "www/js/tests/accelerometer.tests.js",
        ////                    "www/js/tests/camera.tests.js",
        ////                    "www/js/tests/contacts.tests.js",
        ////                    "www/js/tests/device.tests.js",
        ////                    "www/js/tests/file.tests.js",
        ////                    "www/js/tests/geolocation.tests.js",
        ////                    "www/js/tests/map.tests.js",
        ////                    "www/js/tests/media.tests.js",
        ////                    "www/js/tests/network.tests.js",
        ////                    "www/js/tests/notification.tests.js",
        ////                    "www/js/tests/orientation.tests.js",
        ////                    "www/js/tests/sms.tests.js",
        ////                    "www/js/tests/storage.tests.js",
        ////                    "www/js/tests/telephony.tests.js",
        ////                };
        ////}
    }
}