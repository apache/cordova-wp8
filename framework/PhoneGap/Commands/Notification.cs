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

using Microsoft.Devices;
using Microsoft.Phone.Scheduler;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Notification : BaseCommand
    {
        const int DEFAULT_DURATION = 5;

        // alert, confirm, blink, vibrate, beep
        // blink api - doesn't look like there is an equivalent api we can use...
        // vibrate api - http://msdn.microsoft.com/en-us/library/microsoft.devices.vibratecontroller(v=VS.92).aspx
        // beep api - can probably use: http://msdn.microsoft.com/en-us/library/microsoft.phone.scheduler.alarm(v=VS.92).aspx
        //          - examples of alarm class http://mkdot.net/blogs/filip/archive/2011/06/06/windows-phone-multitasking-part-2-2.aspx

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

        public void beep(string beepDuration, string msg, string soundFilePath)
        {
            Alarm alarm = new Alarm("notificationBeep");
            int seconds;

            try
            {
                seconds = int.Parse(beepDuration);
            }
            catch (FormatException)
            {
                seconds = DEFAULT_DURATION;
            }
            
            alarm.BeginTime = DateTime.Now;
            alarm.ExpirationTime = DateTime.Now.AddSeconds(seconds);
            alarm.Content = msg;
            alarm.RecurrenceType = RecurrenceInterval.None;
            alarm.Sound = new Uri(soundFilePath, UriKind.RelativeOrAbsolute);
            ScheduledActionService.Add(alarm);

            // TODO: may need a listener to trigger DispatchCommandResult after the alarm has finished executing...
            DispatchCommandResult();
        }

        public void vibrate(string vibrateDuration)
        {
            VibrateController vc = VibrateController.Default;
            TimeSpan ts;
            int seconds;

            try
            {
                seconds = int.Parse(vibrateDuration);
            }
            catch(FormatException)
            {
                seconds = DEFAULT_DURATION;
            }
           
            ts = new TimeSpan(0, 0, seconds);
            vc.Start(ts);

            // TODO: may need to add listener to trigger DispatchCommandResult when the vibration ends...
            DispatchCommandResult();
        }
    }
}
