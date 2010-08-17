//-----------------------------------------------------------------------
// <copyright file="DebugConsoleProcessMessageCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------

namespace PhoneGap.Commands
{
    public class DebugConsoleDebugCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            this.HasCallback = false;
            System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", args[0], args[1]));
        }
    }
}