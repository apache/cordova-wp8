//-----------------------------------------------------------------------
// <copyright file="AlertCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System.Windows;

namespace PhoneGap.Plugins.Notification
{
    public class AlertCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            //// arg[2] is supposed to be the button text but this isn't configurable in WP7
            //// Consider adding support for OK & Cancel buttons?
            var caption = string.Empty;

            if (args.Length >= 2)
            {
                caption = args[1];
            }

            MessageBox.Show(args[0], caption, MessageBoxButton.OK);
        }
    }
}
