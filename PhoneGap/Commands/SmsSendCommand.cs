//-----------------------------------------------------------------------
// <copyright file="SmsSendCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using Microsoft.Phone.Tasks;

namespace PhoneGap.Commands
{
    // TODO: this needs to handle async callback after sending (probably, just be aware of tombstoning)
    public class SmsSendCommand : PhoneGapCommand
    {
        private string errorCallback;

        public override void Execute(params string[] args)
        {
            this.errorCallback = args[0];
            this.CallbackArgs = new[] { string.Empty };

            try
            {
                var sct = new SmsComposeTask
                              {
                                  To = args[1],
                                  Body = args[2]
                              };
                sct.Show();
            }
            catch (Exception exc)
            {
                this.HasCallback = true;
                this.CallbackName = this.errorCallback;
                this.CallbackArgs = new[] { exc.Message };
            }
        }
    }
}
