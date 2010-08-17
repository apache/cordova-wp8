//-----------------------------------------------------------------------
// <copyright file="PhoneGapCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;

namespace PhoneGap.Commands
{
    public abstract class PhoneGapCommand : EventArgs
    {
        public abstract void Execute(params string[] args);

        public bool HasCallback { get; set; }

        public string CallbackName { get; set; }

        public string[] CallbackArgs { get; set; }
    }
}
