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
using System.Runtime.Serialization.Json;
using System.IO;

namespace WP7GapClassLib.PhoneGap.JSON
{
    /// <summary>
    /// Provides JSON serialization/deserialization functionality.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Serializes object to JSON string representation
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>JSON representation of the object. Returns 'null' string for null passed as argument</returns>
        public static string Serialize(object obj)
        {
            if (obj == null)
            {
                return "null";
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, obj);

            ms.Position = 0;
            
            string json = String.Empty;

            using(StreamReader sr = new StreamReader(ms))
            {
                json = sr.ReadToEnd();
            }

            ms.Close();

            return json;

        }
    }
}
