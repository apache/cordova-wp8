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

            // TODO: Throw MethodNotFound exception if mInfo is null
            if (mInfo != null)
            {
                return mInfo.Invoke(this, args);
            }
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
