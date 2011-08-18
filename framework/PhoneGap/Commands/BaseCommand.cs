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
using System.Reflection;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class BaseCommand : EventArgs
    {
        /*
         *  All commands + plugins must extend BaseCommand, because they are dealt with as BaseCommands in PGView.xaml.cs
         *  
         **/

        public event EventHandler<BaseCommand> OnCommandResult;

        public BaseCommand()
        {
             
        }

        /*
         *  InvokeMethodNamed will call the named method of a BaseCommand subclass if it exists and pass the variable arguments list along.
         **/

        public object InvokeMethodNamed(string methodName, params object[] args)
        {
            MethodInfo mInfo = this.GetType().GetMethod(methodName);

            if (mInfo != null)
            {
                return mInfo.Invoke(this, args);
            }

            // actually methodName could refer to a property
            if (args == null || args.Length == 0 ||
               (args.Length == 1 && "undefined".Equals(args[0])))
            {
                PropertyInfo pInfo = this.GetType().GetProperty(methodName);
                if (pInfo != null)
                {
                    return pInfo.GetValue(this , null);
                }
            }

            // TODO: Throw MethodNotFound exception

            return null;
        }

        public void DispatchCommandResult()
        {
            if (this.OnCommandResult != null)
            {
                this.OnCommandResult(null, this);
                this.OnCommandResult = null;
            }
        }
    }
}
