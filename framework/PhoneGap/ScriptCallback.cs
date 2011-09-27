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

namespace WP7GapClassLib.PhoneGap
{
    /// <summary>
    /// TODO: Description
    /// </summary>
    public class ScriptCallback : EventArgs
    {
        public string ScriptName { get; private set; }

        public string[] Args { get; private set; }

        public ScriptCallback(string function, string id, string message, string value)
        {
            this.ScriptName = function;
            this.Args = new string[] { id, message, value };
        }

        public ScriptCallback(string function, string id, string[] args)
        {
            this.ScriptName = function;
            this.Args = args;
        }
    }
}
