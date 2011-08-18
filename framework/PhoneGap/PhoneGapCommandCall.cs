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
    public class PhoneGapCommandCall
    {
        public String Service {get; private set;}
        public String Action {get; private set;}
        public String CallbackId {get; private set;}
        public String Args {get; private set;}
        
        public static PhoneGapCommandCall Parse(string commandStr)
        {
            if (string.IsNullOrEmpty(commandStr))
            {
                throw new ArgumentNullException("commandStr");
            }

            string[] split = commandStr.Split('/');
            if (split.Length < 3)
            {
                return null;
            }

            PhoneGapCommandCall commandCallParameters = new PhoneGapCommandCall();

            commandCallParameters.Service = split[0];
            commandCallParameters.Action = split[1];
            commandCallParameters.CallbackId = split[2];
            commandCallParameters.Args = split.Length > 3 ? split[3] : String.Empty;

            return commandCallParameters;
        }


        private PhoneGapCommandCall() { }
            

    }
}
