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
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Linq;
using WP7GapClassLib.PhoneGap.JSON;
using System.Runtime.Serialization;



namespace WP7GapClassLib
{
    public class XHRHelper
    {
        [DataContract]
        public class XHROptions
        {
            public string uri { get; set; }
            public bool isAsync { get; set; }
        }


        protected WebBrowser webBrowser1;



        public XHRHelper(WebBrowser gapBrowser)
        {
            this.webBrowser1 = gapBrowser;



            //Application.Current.Exit += new EventHandler(OnAppExit);
        }

        void OnAppExit(object sender, EventArgs e)
        {

        }

        public void HandleCommand(string options)
        {
            // TODO:
            //XHROptions xhrOptions = WP7GapClassLib.PhoneGap.JSON.JsonHelper.Deserialize<XHROptions>(options);

        }

    }
}
