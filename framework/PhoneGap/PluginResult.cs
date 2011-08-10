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
    public class PluginResult
    {
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

	    private Status status;
	    private string message;
	    private bool keepCallback = false;
	    private string cast = null;
	
	    public PluginResult(Status status) 
        {
		    this.status = status;
		    this.message = PluginResult.StatusMessages[(int)this.status];
	    }
	
	    public PluginResult(Status status, Object message, string cast=null) 
        {
		    this.status = status;
            this.message = message.ToString();
            this.cast = cast;
	    }

	    public void setKeepCallback(bool b) 
        {
		    this.keepCallback = b;
	    }
	
	    public Status getStatus() 
        {
		    return status;
	    }

	    public string getMessage() 
        {
		    return message;
	    }
	
	    public bool getKeepCallback() 
        {
		    return this.keepCallback;
	    }
	
	    public string getJSONString() 
        {
		    return "{status:" + this.status + ",message:" + this.message + ",keepCallback:" + this.keepCallback + "}";
	    }
	
	    public string toSuccessCallbackString(string callbackId) 
        {
            return "";
            //StringBuffer buf = new StringBuffer("");
            //if (cast != null) 
            //{
            //    buf.append("var temp = "+cast+"("+this.getJSONString() + ");\n");
            //    buf.append("PhoneGap.callbackSuccess('"+callbackId+"',temp);");
            //}
            //else 
            //{
            //    buf.append("PhoneGap.callbackSuccess('"+callbackId+"',"+this.getJSONString()+");");			
            //}
            //return buf.toString();
	    }
	
	    public string toErrorCallbackString(string callbackId) 
        {
            return "";
		    //return "PhoneGap.callbackError('"+callbackId+"', " + this.getJSONString()+ ");";
	    }
    }
    
}
