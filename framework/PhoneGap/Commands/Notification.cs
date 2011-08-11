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

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Notification : BaseCommand
    {
        // alert, confirm, blink, vibrate, beep
        //MessageBoxResult res = MessageBox.Show("Could not call script: " + ex.Message, "caption", MessageBoxButton.OKCancel);
        public void alert(string msg)
        {
            MessageBoxResult res = MessageBox.Show(msg, "Alert", MessageBoxButton.OK);
            DispatchCommandResult();
        }

        public void confirm(string msg)
        {
            MessageBoxResult res = MessageBox.Show(msg, "Confirm", MessageBoxButton.OKCancel);
            DispatchCommandResult();
        }
    }
}
