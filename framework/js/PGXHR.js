                              
          
(function(win){

/*
[NoInterfaceObject]
interface XMLHttpRequestEventTarget : EventTarget {
  // for future use
};

[Constructor]
interface XMLHttpRequest : XMLHttpRequestEventTarget {
  // event handler attributes
           attribute Function onreadystatechange;

  // states
  const unsigned short UNSENT = 0;
  const unsigned short OPENED = 1;
  const unsigned short HEADERS_RECEIVED = 2;
  const unsigned short LOADING = 3;
  const unsigned short DONE = 4;
  readonly attribute unsigned short readyState;

  // request
  void open(DOMString method, DOMString url);
  void open(DOMString method, DOMString url, boolean async);
  void open(DOMString method, DOMString url, boolean async, DOMString? user);
  void open(DOMString method, DOMString url, boolean async, DOMString? user, DOMString? password);
  void setRequestHeader(DOMString header, DOMString value);
  void send();
  void send(Document data);
  void send([AllowAny] DOMString? data);
  void abort();

  // response
  readonly attribute unsigned short status;
  readonly attribute DOMString statusText;
  DOMString getResponseHeader(DOMString header);
  DOMString getAllResponseHeaders();
  readonly attribute DOMString responseText;
  readonly attribute Document responseXML;
};
*/
	var aliasXHR = win.XMLHttpRequest;
	
	win.XMLHttpRequest = function(){console.log("created new XHR!")};
	
	var UNSENT = 0;
	var OPENED = 1;
	var HEADERS_RECEIVED = 2;
	var LOADING = 3;
	var DONE = 4;
          
	win.XMLHttpRequest.prototype =
	{
		isAsync:false,
		onreadystatechange:null,
		readyState:UNSENT,
		open:function(reqType,uri,isAsync,user,password)
		{
			console.log("XMLHttpRequest.open " + uri);
			if(uri && uri.indexOf("http") == 0)
			{
				if(!this.wrappedXHR)
				{
					console.log("using wrapped XHR");
					this.wrappedXHR = new aliasXHR();
					Object.defineProperty( this, "status", { get: function() {
						return this.wrappedXHR.status;										
					}});
					Object.defineProperty( this, "responseText", { get: function() {
						return this.wrappedXHR.responseText;										
					}});
					Object.defineProperty( this, "statusText", { get: function() {
						return this.wrappedXHR.statusText;										
					}});
					Object.defineProperty( this, "responseXML", { get: function() {
						return this.wrappedXHR.responseXML;										
					}});
					
					this.getResponseHeader = function() {
						return this.wrappedXHR.getResponseHeader.apply(this.wrappedXHR,arguments);
					};
					this.getAllResponseHeaders = function() {
						return this.wrappedXHR.getAllResponseHeaders.apply(this.wrappedXHR,arguments);
					};
					
					this.wrappedXHR.onreadystatechange = this.onreadystatechange;
				}
				return this.wrappedXHR.open(reqType,uri,isAsync,user,password);
			}
			else
			{
				console.log("gonna use the file api, mkay");
				this.changeReadyState(OPENED);
				navigator.fileMgr.readAsText(uri,"UTF-8", 
				this.onResult.bind(this),
				this.onError.bind(this));
			}
		},
		statusText:"",
		changeReadyState:function(newState)
		{
			this.readyState = newState;
			if(this.onreadystatechange)
			{
				this.onreadystatechange();	
			}
		},
		getResponseHeader:function()
		{
			return "";
		},
		getAllResponseHeaders:function()
		{
			return "";
		},
		responseText:"",
		responseXML:function()
		{
			return new Document(this.responseText);
		},
		onResult:function(res)
		{
			this.status = 200;
			this.responseText = res;
			this.changeReadyState(DONE);
		},
		onError:function(err)
		{
			this.status = 404;
			this.changeReadyState(DONE);s
		},
		
		send:function(data)
		{
			if(this.wrappedXHR)
			{
				return this.wrappedXHR.send.apply(this.wrappedXHR,arguments);
			}
			
		},
		status:404,
		responseText:"empty"
	};		  
		  
		  
		  
})(window);

          
