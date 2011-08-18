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

namespace WP7GapClassLib.PhoneGap
{
    /// <summary>
    /// Represents command execution result
    /// </summary>
    public class PluginResult : EventArgs
    {
        /// <summary>
        /// Predefined resultant messages
        /// </summary>
        public static string[] StatusMessages = new string[] 
        {
		    "No result",
		    "OK",
		    "Class not found",
		    "Illegal access",
		    "Instantiation error",
		    "Malformed url",
		    "IO error",
		    "Invalid action",
		    "JSON error",
		    "Error"
	    };

        /// <summary>
        /// Possible command results status codes
        /// </summary>
        public enum Status :int
        {
            NO_RESULT = 0,
            OK,
            CLASS_NOT_FOUND_EXCEPTION,
            ILLEGAL_ACCESS_EXCEPTION,
            INSTANTIATION_EXCEPTION,
            MALFORMED_URL_EXCEPTION,
            IO_EXCEPTION,
            INVALID_ACTION,
            JSON_EXCEPTION,
            ERROR
        };

	    public Status Result {get; private set;}
	    public string Message {get; private set;}
        public object CallBackArgs {get; private set;}

        /// <summary>
        /// Whether command succeded or not
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return this.Result == Status.OK || this.Result == Status.NO_RESULT;
            }
        }

        /// <summary>
        /// Creates new instance of the PluginResult class.
        /// </summary>
        /// <param name="status">Execution result</param>
        public PluginResult(Status status) 
        {
		    this.Result = status;
            this.Message = PluginResult.StatusMessages[(int)this.Result];
	    }

        /// <summary>
        /// Creates new instance of the PluginResult class.
        /// </summary>
        /// <param name="status">Execution result</param>
        /// <param name="callBackArgs">Callback parameters</param>
        public PluginResult(Status status, object callBackArgs) 
        {
            this.Result = status;
            this.Message = PluginResult.StatusMessages[(int)this.Result];
            this.CallBackArgs = callBackArgs;
	    }

    }
    
}
