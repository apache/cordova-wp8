/*
 * PhoneGap is available under *either* the terms of the modified BSD license *or* the
 * MIT License (2008). See http://opensource.org/licenses/alphabetical for full text.
 *
 * Copyright (c) 2005-2011, Nitobi Software Inc.
 * Copyright (c) 2011, Microsoft Corporation
 */

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
    /// Provides functionality to create phone gap command by name.
    /// </summary>
    public static class CommandFactory
    {
        /// <summary>
        /// Performance optimization allowing more faster create already known commands.
        /// </summary>
        private static Dictionary<string, Type> commandMap = new Dictionary<string, Type>();
 
        /// <summary>
        /// Creates command using command class name. Returns null for unknown commands.
        /// </summary>
        /// <param name="service">Command class name, for example Device or Notification</param>
        /// <returns>Command class instance or null</returns>
        public static BaseCommand CreateByServiceName(string service)
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
                    commandMap[service] = t;

                }
            }

            return Activator.CreateInstance(commandMap[service]) as BaseCommand;
        }
    }
}
