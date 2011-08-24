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
using System.IO;

namespace WP7GapClassLib.PhoneGap.Commands
{
    /// <summary>
    /// Represents map to getting mime type by file extention
    /// </summary>
    public static class MimeTypeMapper
    {
        /// <summary>
        /// Stores mime type for all necessary extension
        /// </summary>
        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
                                                                             {                                                                                
                                                                                 {"avi", "video/x-msvideo"},
                                                                                 {"bmp", "image/bmp"},                                                                                                                                                                
                                                                                 {"gif", "image/gif"},                                                                                                                                                               
                                                                                 {"jpe", "image/jpeg"},
                                                                                 {"jpeg", "image/jpeg"},
                                                                                 {"jpg", "image/jpeg"},                                                                                                                                                             
                                                                                 {"mov", "video/quicktime"},
                                                                                 {"mp2", "audio/mpeg"},
                                                                                 {"mp3", "audio/mpeg"},
                                                                                 {"mp4", "video/mp4"},
                                                                                 {"mpe", "video/mpeg"},
                                                                                 {"mpeg", "video/mpeg"},
                                                                                 {"mpg", "video/mpeg"},
                                                                                 {"mpga", "audio/mpeg"},                                                                                
                                                                                 {"pbm", "image/x-portable-bitmap"},
                                                                                 {"pct", "image/pict"},
                                                                                 {"pgm", "image/x-portable-graymap"},
                                                                                 {"pic", "image/pict"},
                                                                                 {"pict", "image/pict"},
                                                                                 {"png", "image/png"},
                                                                                 {"pnm", "image/x-portable-anymap"},
                                                                                 {"pnt", "image/x-macpaint"},
                                                                                 {"pntg", "image/x-macpaint"},
                                                                                 {"ppm", "image/x-portable-pixmap"},
                                                                                 {"qt", "video/quicktime"},
                                                                                 {"ra", "audio/x-pn-realaudio"},
                                                                                 {"ram", "audio/x-pn-realaudio"},
                                                                                 {"ras", "image/x-cmu-raster"},
                                                                                 {"rgb", "image/x-rgb"},
                                                                                 {"snd", "audio/basic"},
                                                                                 {"tif", "image/tiff"},
                                                                                 {"tiff", "image/tiff"},
                                                                                 {"wav", "audio/x-wav"},
                                                                                 {"wbmp", "image/vnd.wap.wbmp"},

                                                                             };
        /// <summary>
        /// Gets mime type by file extension
        /// </summary>
        /// <param name="fileName">file name to extract extension</param>
        /// <returns>mime type</returns>
        public static string GetMimeType(string fileName)
        {
            if (MIMETypesDictionary.ContainsKey(Path.GetExtension(fileName).Remove(0, 1)))
            {
                return MIMETypesDictionary[Path.GetExtension(fileName).Remove(0, 1)];
            }
            // For unknown type it is allowed to use 'application/octet-stream' - http://stackoverflow.com/questions/1176022/unknown-file-type-mime
            return "application/octet-stream";
        }
    }
}
