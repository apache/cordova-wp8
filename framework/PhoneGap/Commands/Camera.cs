using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using Microsoft.Phone.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;

namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Camera : BaseCommand
    {
    
        /// <summary>
        /// Return base64 encoded string
        /// </summary>
        private const int DATA_URL = 0;
	
        /// <summary>
        /// Return file uri
        /// </summary>
        private const int FILE_URI = 1;				
	
        /// <summary>
        /// Choose image from picture library
        /// </summary>
	    private const int PHOTOLIBRARY = 0;		 
	
        /// <summary>
        /// Take picture from camera
        /// </summary>
    
        private const int CAMERA = 1;

        /// <summary>
        /// Choose image from picture library
        /// </summary>
	    private const int SAVEDPHOTOALBUM = 2;	
	
        /// <summary>
        /// Take a picture of type JPEG
        /// </summary>
	    private const int JPEG = 0;   
    
        /// <summary>
        /// Take a picture of type PNG
        /// </summary>
	    private const int PNG = 1;                    
	
	    /// <summary>
        /// Desired width of the image
	    /// </summary>
        private int targetWidth;
    
        /// <summary>
        /// desired height of the image
        /// </summary>    
	    private int targetHeight;                


        /// <summary>
        /// Folder to store captured images
        /// </summary>
        private string isoFolder = "CapturedImagesCache";

        /// <summary>
        /// Represents captureImage action options.
        /// </summary>
        [DataContract]
        public class CameraOptions
        {
            /// <summary>
            /// Source to getPicture from.
            /// </summary>
            [DataMember]
            public int PictureSourceType { get; set; }
        
            /// <summary>
            /// Format of image that returned from getPicture.
            /// </summary>
            [DataMember]
            public int DestinationType { get; set; }

            /// <summary>
            /// Encoding of image returned from getPicture.
            /// </summary>
            [DataMember]                
            public int EncodingType { get; set; }

            public static CameraOptions Default 
            {
                get { return new CameraOptions() { PictureSourceType = CAMERA, DestinationType = DATA_URL, EncodingType = JPEG}; }
            }

        }
    
        /// <summary>
        /// Used to open photo library
        /// </summary>
        PhotoChooserTask photoChooserTask;

        /// <summary>
        /// Used to open camera application
        /// </summary>
        CameraCaptureTask cameraTask;
        
        /// <summary>
        /// Camera options
        /// </summary>
        CameraOptions cameraOptions;
        
        public void getPicture(string options)
        {
            try
            {
                this.cameraOptions = String.IsNullOrEmpty(options) ?
                        CameraOptions.Default : JSON.JsonHelper.Deserialize<CameraOptions>(options);

            }
            catch (Exception ex)
            {
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION, ex.Message));
                return;
            }

            //TODO Check if all the options are acceptable


            if (cameraOptions.PictureSourceType == CAMERA)
            {
                cameraTask = new CameraCaptureTask();
                cameraTask.Completed += onComplited;
                cameraTask.Show();
            }
            else
            {
                if ((cameraOptions.PictureSourceType == PHOTOLIBRARY) || (cameraOptions.PictureSourceType == SAVEDPHOTOALBUM))
                {
                    photoChooserTask = new PhotoChooserTask();
                    photoChooserTask.Completed += onComplited;
                    photoChooserTask.Show();
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.NO_RESULT));
                }
            }
            
        }

        public void onComplited(object sender,PhotoResult e) {
            if (e.Error != null)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            switch (e.TaskResult)
            {
                case TaskResult.OK:
                    try
                    {
                        string imageData = string.Empty;
                        
                        if(cameraOptions.PictureSourceType == CAMERA)
                        {
                            if (cameraOptions.DestinationType == DATA_URL)
                            {
                                imageData = getBase64(e.ChosenPhoto);
                            }
                            else
                            {
                                byte[] imageBytes = new byte[e.ChosenPhoto.Length];
                                e.ChosenPhoto.Read(imageBytes, 0, imageBytes.Length);
                                imageData = this.SaveImageToLocalStorage(Path.GetFileName(e.OriginalFileName), isoFolder, imageBytes);
                            }
                        }
                        else 
                        {
                            if (cameraOptions.DestinationType == DATA_URL)
                            {
                                imageData = getBase64(e.ChosenPhoto);
                            }
                            else
                            {
                                //TODO Set default value or return base64 or something else
                            }
                        }
                                                                       
                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK,imageData));

                    }
                    catch (Exception ex)
                    {
                        DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Error retrieving image."));
                    }
                    break;

                case TaskResult.Cancel:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR,"Selection cancelled."));
                    break;

                default:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR,"Selection did not complete!"));
                    break;
            }         
        }


        /// <summary>
        /// Creates base64 string from binary file
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string getBase64(Stream stream) {
            int streamLength = (int)stream.Length;
            byte[] fileData = new byte[streamLength + 1];
            stream.Read(fileData, 0, streamLength);
            stream.Close();
            return Convert.ToBase64String(fileData);
        }


        /// <summary>
        /// Saves captured image in isolated storage
        /// </summary>
        /// <param name="imageFileName">image file name</param>
        /// <param name="imageFolder">folder to store images</param>
        /// <returns>Image path</returns>
        private string SaveImageToLocalStorage(string imageFileName, string imageFolder, byte[] imageBytes)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException("imageBytes");
            }
            try
            {
                var isoFile = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isoFile.DirectoryExists(imageFolder))
                {
                    isoFile.CreateDirectory(imageFolder);
                }
                string filePath = System.IO.Path.Combine(imageFolder, imageFileName);

                using (var stream = isoFile.CreateFile(filePath))
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                }

                return filePath;
            }
            catch (Exception e)
            {
                //TODO: log or do something else
                throw;
            }
        }

    }
}
