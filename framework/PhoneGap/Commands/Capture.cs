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
using Microsoft.Phone.Tasks;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone;
using System.Windows.Media.Imaging;

namespace WP7GapClassLib.PhoneGap.Commands
{
/// <summary>
    /// Provides access to the audio, image, and video capture capabilities of the device
    /// </summary>
    public class Capture : BaseCommand
    {
        #region Internal classes (options and resultant objects)

        /// <summary>
        /// Represents captureImage action options.
        /// </summary>
        [DataContract]
        public class CaptureImageOptions
        {
            /// <summary>
            /// The maximum number of images the device user can capture in a single capture operation. The value must be greater than or equal to 1 (defaults to 1).
            /// </summary>
            [DataMember]
            public int limit { get; set; }

            public static CaptureImageOptions Default
            {
                get { return new CaptureImageOptions() { limit = 1 }; }
            }
        }

        /// <summary>
        /// Stores image info
        /// </summary>
        [DataContract]
        public class MediaFile
        {

            [DataMember]
            public string fileName;

            [DataMember]
            public string filePath;

            [DataMember]
            public string type;

            [DataMember]
            public string lastModifiedDate;

            [DataMember]
            public long size;

            public MediaFile(string filePath, Picture image)
            {
                this.filePath = filePath;
                this.fileName = System.IO.Path.GetFileName(this.filePath);
                // TODO find internal func
                this.type = "image/jpeg";
                this.size = image.GetImage().Length;
                this.lastModifiedDate = image.Date.ToString();

            }
        }

        /// <summary>
        /// Stores additional media file data
        /// </summary>
        [DataContract]
        public class MediaFileData
        {
            [DataMember]
            public int height;

            [DataMember]
            public int width;

            [DataMember]
            public int bitrate;

            [DataMember]
            public int duration;

            [DataMember]
            public string codecs;

            public MediaFileData(WriteableBitmap image)
            {
                this.height = image.PixelHeight;
                this.width = image.PixelWidth;
                this.bitrate = 0;
                this.duration = 0;
                this.codecs = "";
            }
        }

        #endregion

        /// <summary>
        /// Folder to store captured images
        /// </summary>
        private string isoFolder = "CapturedImagesCache";

        /// <summary>
        /// Capture Image options
        /// </summary>
        protected CaptureImageOptions captureImageOptions;

        /// <summary>
        /// Used to open camera application
        /// </summary>
        private CameraCaptureTask cameraTask;

        /// <summary>
        /// Stores informaton about captured files
        /// </summary>
        List<MediaFile> files = new List<MediaFile>();
        
        /// <summary>
        /// Launches default camera application to capture image
        /// </summary>
        /// <param name="options">may contains limit or mode parameters</param>
        public void captureImage(string options)
        {
            try
            {
                try
                {
                    this.captureImageOptions = String.IsNullOrEmpty(options) ?
                        CaptureImageOptions.Default : JSON.JsonHelper.Deserialize<CaptureImageOptions>(options);

                }
                catch (Exception ex)
                {
                    this.DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION, ex.Message));
                    return;
                }

    
                cameraTask = new CameraCaptureTask();
                cameraTask.Completed += this.cameraTask_Completed;
                cameraTask.Show();
            }
            catch (Exception e)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, e.Message));
            }
        }

        /// <summary>
        /// Retrieves the format information of the media file.
        /// </summary>
        /// <param name="options"></param>
        public void getFormatData(Dictionary<string, object> options)
        {

            try
            {
                string filePath = string.Empty;
                string mimeType = string.Empty;

                if (options.ContainsKey("filePath"))
                {
                    filePath = (string)options["filePath"];
                }

                if (options.ContainsKey("mimeType"))
                {
                    mimeType = (string)options["mimeType"];
                }

                if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(mimeType))
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                }

                if (mimeType.Equals("image/jpeg") || filePath.EndsWith(".jpeg"))
                {
                    WriteableBitmap image = ExtractImageFromLocalStorage(filePath);

                    if (image == null)
                    {
                        DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "File not found"));
                        return;
                    }

                    MediaFileData mediaData = new MediaFileData(image);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, mediaData));
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                }


            }
            catch (Exception e)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
            }
        }

        /// <summary>
        /// Handles result of capture to save image information 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">stores inforamation about currrent captured image</param>
        private void cameraTask_Completed(object sender, PhotoResult e)
        {
            
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
                        string fileName = System.IO.Path.GetFileName(e.OriginalFileName);

                        // Save image in media library
                        MediaLibrary library = new MediaLibrary();
                        Picture image = library.SavePicture(fileName, e.ChosenPhoto);

                        // Save image in isolated storage    
                    
                        // we should return stream position back after saving stream to media library
                        e.ChosenPhoto.Seek(0, SeekOrigin.Begin);
                        byte[] imageBytes = new byte[e.ChosenPhoto.Length];
                        e.ChosenPhoto.Read(imageBytes, 0, imageBytes.Length);
                        string pathLocalStorage = this.SaveImageToLocalStorage(fileName, isoFolder, imageBytes);                                              
                        
                        // Get image data
                        MediaFile data = new MediaFile(pathLocalStorage, image);

                        this.files.Add(data);

                        if (files.Count < this.captureImageOptions.limit)
                        {
                            cameraTask.Show();
                        }
                        else
                        {
                            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, files));
                            files.Clear();
                        }
                    }
                    catch(Exception ex) 
                    {
                        DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    }
                    break;

                case TaskResult.Cancel:
                    if (files.Count > 0)
                    {
                        // User canceled operation, but some images were made 
                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK, files));
                        files.Clear();
                    }
                    else
                    {
                        DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    }
                    break;
            }

            
            
        }

        /// <summary>
        /// Extract file from Isolated Storage as WriteableBitmap object
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private WriteableBitmap ExtractImageFromLocalStorage(string filePath)
        {
            try
            {

                var isoFile = IsolatedStorageFile.GetUserStoreForApplication();

                using (var imageStream = isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                {
                    var imageSource = PictureDecoder.DecodeJpeg(imageStream);
                    return imageSource;
                }
            }catch(Exception e){
                return null;
            }
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
            catch(Exception e)
            {
                //TODO: log or do something else
                throw;
            }
        }  
        

    }
}
