﻿/*  
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at
	
	http://www.apache.org/licenses/LICENSE-2.0
	
	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace WPCordovaClassLib.Cordova.JSON
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
            string json = String.Empty;

            try
            {
                json = JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Failed to serialize " + obj);
            }

            return json;

        }

        /// <summary>
        /// Parses json string to object instance
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="json">json string representation of the object</param>
        /// <returns>Deserialized object instance</returns>
        public static T Deserialize<T>(string json)
        {
            object result = null;
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Failed to deserialize " + typeof(T) + " with JSON value :: " + json);
            }

            return (T)result;

        }
    }
}
