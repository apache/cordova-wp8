using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPCordovaClassLib.CordovaLib
{
    public class XHRProxy
    {

        private WebBrowser webBrowser;

        public XHRProxy(ref WebBrowser browser) 
        {
            this.webBrowser = browser;
            browser.ScriptNotify += browser_ScriptNotify;
            browser.Navigated += browser_Navigated;
        }

        void browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {


            string script = @"(function(win, doc) {

    var docDomain = null;
    try {
        docDomain = doc.domain;
    } catch (err) {}

    if (!docDomain || docDomain.length === 0) {

        var aliasXHR = win.XMLHttpRequest;
        
        var XHRShim = function() {};
        win.XMLHttpRequest = XHRShim;
        XHRShim.noConflict = aliasXHR;
        XHRShim.UNSENT = 0;
        XHRShim.OPENED = 1;
        XHRShim.HEADERS_RECEIVED = 2;
        XHRShim.LOADING = 3;
        XHRShim.DONE = 4;
        XHRShim.prototype = {
            isAsync: false,
            onreadystatechange: null,
            readyState: 0,
            _url: '',
            timeout: 0,
            withCredentials: false,
            _requestHeaders: null,
            open: function (reqType, uri, isAsync, user, password) {
                
                if (uri && uri.indexOf('http') === 0) {
                    if (!this.wrappedXHR) {
                        this.wrappedXHR = new aliasXHR();
                        var self = this;
                        if (this.timeout > 0) {
                            this.wrappedXHR.timeout = this.timeout;
                        }
                        Object.defineProperty(this, 'timeout', {
                            set: function(val) {
                                this.wrappedXHR.timeout = val;
                            },
                            get: function() {
                                return this.wrappedXHR.timeout;
                            }
                        });
                        if (this.withCredentials) {
                            this.wrappedXHR.withCredentials = this.withCredentials;
                        }
                        Object.defineProperty(this, 'withCredentials', {
                            set: function(val) {
                                this.wrappedXHR.withCredentials = val;
                            },
                            get: function() {
                                return this.wrappedXHR.withCredentials;
                            }
                        });
                        Object.defineProperty(this, 'status', {
                            get: function() {
                                return this.wrappedXHR.status;
                            }
                        });
                        Object.defineProperty(this, 'responseText', {
                            get: function() {
                                return this.wrappedXHR.responseText;
                            }
                        });
                        Object.defineProperty(this, 'statusText', {
                            get: function() {
                                return this.wrappedXHR.statusText;
                            }
                        });
                        Object.defineProperty(this, 'responseXML', {
                            get: function() {
                                return this.wrappedXHR.responseXML;
                            }
                        });
                        this.getResponseHeader = function(header) {
                            return this.wrappedXHR.getResponseHeader(header);
                        };
                        this.getAllResponseHeaders = function() {
                            return this.wrappedXHR.getAllResponseHeaders();
                        };
                        this.wrappedXHR.onreadystatechange = function() {
                            self.changeReadyState(self.wrappedXHR.readyState);
                        };
                    }
                    return this.wrappedXHR.open(reqType, uri, isAsync, user, password);
                }
                else
                {
                    this.isAsync = isAsync;
                    this.reqType = reqType;
                    var newUrl = uri;
                    this._url = newUrl;
                }
            },
            statusText: '',
            changeReadyState: function(newState) {
                this.readyState = newState;
                if (this.onreadystatechange) {
                    this.onreadystatechange();
                }
            },
            setRequestHeader: function(header, value) {
                if (this.wrappedXHR) {
                    this.wrappedXHR.setRequestHeader(header, value);
                }
            },
            getResponseHeader: function(header) {
                return this.wrappedXHR ? this.wrappedXHR.getResponseHeader(header) : '';
            },
            getAllResponseHeaders: function() {
                return this.wrappedXHR ? this.wrappedXHR.getAllResponseHeaders() : '';
            },
            responseText: '',
            responseXML: '',
            onResult: function(res) {
                this.status = 200;
                if (typeof res == 'object') {
                    res = JSON.stringify(res);
                }
                this.responseText = res;
                this.responseXML = res;
                this.changeReadyState(XHRShim.DONE);
            },
            onError: function(err) {
                this.status = 404;
                this.changeReadyState(XHRShim.DONE);
            },
            abort: function() {
                if (this.wrappedXHR) {
                    return this.wrappedXHR.abort();
                }
            },
            send: function(data) {
                if (this.wrappedXHR) {
                    return this.wrappedXHR.send(data);
                } 
                else {
                    this.changeReadyState(XHRShim.OPENED);
                    var alias = this;
                    var funk = function () {
                        window.__onXHRLocalCallback = function (responseCode, responseText) {
                            alias.status = responseCode;
                            if (responseCode == '200') {
                                alias.responseText = responseText;
                            }
                            else {
                                alias.onerror && alias.onerror(responseCode);
                            }

                            alias.changeReadyState(XHRShim.DONE);
                            alias.onload && alias.onload();
                            
                        }
                        alias.changeReadyState(XHRShim.LOADING);
                        window.external.Notify('XHRLOCAL/' + alias._url); 
                    }
                    if (this.isAsync) {
                        setTimeout(funk, 0);
                    }
                    else {
                        funk();
                    }
                }
            },
            getContentLocation: function() {
                if (window.contentLocation === undefined) {
                    window.contentLocation = navigator.userAgent.toUpperCase().indexOf('MSIE 10') > -1 ? this.contentLocation.RESOURCES : this.contentLocation.ISOLATED_STORAGE;
                }
                return window.contentLocation;
            },
            contentLocation: {
                ISOLATED_STORAGE: 0,
                RESOURCES: 1
            },
            status: 404
        };
    }
})(window, document); ";


            webBrowser.InvokeScript("execScript", new string[] { script });


        }

        void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Debug.WriteLine("ScriptNotify::" + e.Value);
            string commandStr = e.Value;

            if (commandStr.IndexOf("XHRLOCAL") == 0)
            {
                string url = commandStr.Replace("XHRLOCAL/", "");

                Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoFile.FileExists(uri.AbsolutePath))
                    {
                        using (TextReader reader = new StreamReader(isoFile.OpenFile(uri.AbsolutePath, FileMode.Open, FileAccess.Read)))
                        {
                            string text = reader.ReadToEnd();
                            webBrowser.InvokeScript("__onXHRLocalCallback", new string[] { "200", text });
                            return;
                        }
                    }       
                }

                Uri relUri = new Uri(uri.AbsolutePath,UriKind.Relative);
                
                var resource = Application.GetResourceStream(relUri);

                if (resource == null)
                {
                    // 404 ? 
                    webBrowser.InvokeScript("__onXHRLocalCallback", new string[] { "404" });
                    return;
                }
                else 
                {
                    using (StreamReader streamReader = new StreamReader(resource.Stream))
                    {
                        string text = streamReader.ReadToEnd();
                        webBrowser.InvokeScript("__onXHRLocalCallback", new string[] { "200", text });
                        return;
                    }
                }
            }
        }
    }
}
