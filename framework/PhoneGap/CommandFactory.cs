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
using System.Collections.Generic;
using WP7GapClassLib.PhoneGap.Commands;

namespace WP7GapClassLib.PhoneGap
{
    /// <summary>
    /// Provides functionality to create phone gap command by name,
    /// Each command is created only once and stored in commands pool.
    /// </summary>
    public static class CommandFactory
    {
        /// <summary>
        /// Performance optimization allowing more faster create already known commands.
        /// </summary>
        private static Dictionary<string, BaseCommand> commandMap = new Dictionary<string,BaseCommand>();
 
        /// <summary>
        /// Creates command using command class name. Returns null for unknown commands.
        /// </summary>
        /// <param name="service">Command class name, for example Device or Notification</param>
        /// <returns>Command class instance or null</returns>
        public static BaseCommand CreateUsingServiceName(string service)
        {

            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentNullException("service", "service to create can't be null");
            }

            if (!commandMap.ContainsKey(service))
            {
            
                Type t = Type.GetType("WP7GapClassLib.PhoneGap.Commands." + service);
                if (t != null)
                {
                    BaseCommand bc = (BaseCommand)Activator.CreateInstance(t);

                    if (bc != null)
                    {
                        commandMap[service] = bc;
                        return bc;
                    }
                }

                commandMap[service] = null;
            }

            if (commandMap[service] != null)
            {
                return (BaseCommand) Activator.CreateInstance (commandMap[service].GetType());
            }

            return null;
            
        }
    }
}
