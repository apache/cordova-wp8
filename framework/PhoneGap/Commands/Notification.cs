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
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;

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

        [DataContract]
        public class AlertOptions
        {

            public AlertOptions()
            {

            }

            [OnDeserializing]
            public void OnDeserializing(StreamingContext context)
            {
                // set defaults
                this.message = "message";
                this.title = "Alert";
                this.buttonLabel = "ok";
            }

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string message;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string title;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string buttonLabel;
        }

        public void alert(string options)
        {
            AlertOptions alertOpts = JSON.JsonHelper.Deserialize<AlertOptions>(options);
            MessageBoxResult res = MessageBox.Show(alertOpts.message, alertOpts.title,MessageBoxButton.OK);

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK,(int)res));
        }

        public void confirm(string options)
        {
            AlertOptions alertOpts = JSON.JsonHelper.Deserialize<AlertOptions>(options);

            MessageBoxResult res = MessageBox.Show(alertOpts.message, alertOpts.title, MessageBoxButton.OKCancel);
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, (int)res));
        }

        public void beep(string count)
        {

            int times = int.Parse(count);

            StreamResourceInfo sri = Application.GetResourceStream(new Uri("/WP7GapClassLib;component/resources/notification-beep.wav", UriKind.Relative));
            if (sri != null)
            {
                SoundEffect effect = SoundEffect.FromStream(sri.Stream);
                SoundEffectInstance inst = effect.CreateInstance();
                while (times-- > 0)
                {
                    
                    inst.Play();
                    // This will pause while the beep plays but as it also blocks!
                    // Could do with finding a better solution if users want lots of beeps (or sound effect is changed to something longer)
                    Thread.Sleep(effect.Duration);
                }
            }
          

            // TODO: may need a listener to trigger DispatchCommandResult after the alarm has finished executing...
            DispatchCommandResult();
        }

        public void vibrate(string vibrateDuration)
        {
            int msecs = 200; // set default

            try
            {
                msecs = int.Parse(vibrateDuration);
                if (msecs < 1)
                {
                    msecs = 1;
                }
            }
            catch (FormatException)
            {

            }

            VibrateController.Default.Start(TimeSpan.FromMilliseconds(msecs));

            // TODO: may need to add listener to trigger DispatchCommandResult when the vibration ends...
            DispatchCommandResult();
        }
    }
}
