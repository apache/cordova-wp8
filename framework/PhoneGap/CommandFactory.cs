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
    public static class CommandFactory
    {
        private static Dictionary<string, BaseCommand> commandMap = new Dictionary<string,BaseCommand>();
 
        public static BaseCommand CreateUsingServiceName(string service)
        {

            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentNullException("service", "service to create can't be null");
            }

            if (!commandMap.ContainsKey(service))
            {
                // TODO: if we do not find the command with that name, handle the error, somehow ...
                Type t = Type.GetType("WP7GapClassLib.PhoneGap.Commands." + service);
                if (t != null)
                {
                    BaseCommand bc = (BaseCommand)Activator.CreateInstance(t);

                    if (bc == null) return null;

                    commandMap[service] = bc;                        
                    
                }
            }

            //TODO: we should clone the class instance to allow async callbacks
            return commandMap[service];
            
        }
    }
}
