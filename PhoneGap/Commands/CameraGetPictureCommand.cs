//-----------------------------------------------------------------------
// <copyright file="CameraGetPictureCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using Microsoft.Phone.Tasks;

namespace PhoneGap.Commands
{
    public class CameraGetPictureCommand : PhoneGapCommand, IAsyncCommand
    {
        private string successCallback;
        private string errorCallback;

        public override void Execute(params string[] args)
        {
            this.HasCallback = false; // This uses an AsyncCallback

            this.successCallback = args[0];
            this.errorCallback = args[1];

            // args[2] contains a quality option but we ignore it in WP7
            try
            {
                // Tombstoning causes real problems at the moment, not least with subscribed events (like below), but also with just reloading the page
                // May need to reconsider how/if we wish to support this
                var cct = new CameraCaptureTask();
                cct.Completed += this.cct_Completed;
                cct.Show();
            }
            catch (Exception exc)
            {
                this.CallbackName = this.errorCallback;
                this.CallbackArgs = new[] { exc.Message };
            }
        }

        private void cct_Completed(object sender, PhotoResult e)
        {
            if (e.Error == null)
            {
                this.CallbackName = this.successCallback;

                switch (e.TaskResult)
                {
                    case TaskResult.OK:
                        int streamLength = Convert.ToInt32(e.ChosenPhoto.Length);
                        byte[] fileData = new byte[streamLength + 1];

                        e.ChosenPhoto.Read(fileData, 0, streamLength);
                        e.ChosenPhoto.Close();

                        string base64 = Convert.ToBase64String(fileData);

                        this.CallbackArgs = new[] { base64 };
                        break;
                    default:
                        this.CallbackArgs = new[] { string.Empty };
                        break;
                }
            }
            else
            {
                this.CallbackName = this.errorCallback;
                this.CallbackArgs = new[] { e.Error.Message };
            }
        }

        public void SendAsyncResponse()
        {
            if (this.OnCommandCompleted != null)
            {
                this.OnCommandCompleted(null, this);
            }
        }

        public event EventHandler<PhoneGapCommand> OnCommandCompleted;
    }
}
