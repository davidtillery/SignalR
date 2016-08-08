// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.Client.Http
{
    public class HttpWebRequestWrapper : IRequest
    {
        private readonly HttpWebRequest _request;

#if !NET_STANDARD
        private IDictionary<string, Action<HttpWebRequest, string>> _restrictedHeadersSet = new Dictionary<string, Action<HttpWebRequest, string>>() {
                                                                        { HttpRequestHeader.Accept.ToString(), (request, value) => { request.Accept = value; } },
                                                                        { HttpRequestHeader.ContentType.ToString(), (request, value) => { request.ContentType = value; } },
                                                                        { HttpRequestHeader.ContentLength.ToString(), (request, value) => { request.ContentLength = Int32.Parse(value, CultureInfo.CurrentCulture); } },
                                                                        { HttpRequestHeader.UserAgent.ToString(), (request, value) => { request.UserAgent = value; } },
                                                                        { HttpRequestHeader.Connection.ToString(), (request, value) => { request.Connection = value; } },
                                                                        { HttpRequestHeader.Date.ToString(), (request, value) => {request.Date = DateTime.Parse(value, CultureInfo.CurrentCulture); } },
                                                                        { HttpRequestHeader.Expect.ToString(), (request, value) => {request.Expect = value;} },
                                                                        { HttpRequestHeader.Host.ToString(), (request, value) => {request.Host = value; }  },
                                                                        { HttpRequestHeader.IfModifiedSince.ToString(), (request, value) => {request.IfModifiedSince = DateTime.Parse(value, CultureInfo.CurrentCulture);} },
                                                                        { HttpRequestHeader.Referer.ToString(), (request, value) => { request.Referer = value; } },
                                                                        { HttpRequestHeader.TransferEncoding.ToString(), (request, value) => { request.TransferEncoding = value; } },
                                                                    };
#else

        private IDictionary<string, Action<HttpWebRequest, string>> _restrictedHeadersSet = new Dictionary<string, Action<HttpWebRequest, string>>() {
                                                                        { HttpRequestHeader.Accept.ToString(), (request, value) => { request.Accept = value; } },
                                                                        { HttpRequestHeader.ContentType.ToString(), (request, value) => { request.ContentType = value; } },
                                                                        { HttpRequestHeader.ContentLength.ToString(), (request, value) => { request.Headers[HttpRequestHeader.ContentLength] = value; } },
                                                                        { HttpRequestHeader.UserAgent.ToString(), (request, value) => { request.Headers[HttpRequestHeader.UserAgent] = value; } },
                                                                        { HttpRequestHeader.Connection.ToString(), (request, value) => { request.Headers[HttpRequestHeader.Connection] = value; } },
                                                                        { HttpRequestHeader.Date.ToString(), (request, value) => { request.Headers[HttpRequestHeader.Date] = value; } },
                                                                        { HttpRequestHeader.Expect.ToString(), (request, value) => { request.Headers[HttpRequestHeader.Expect] = value; } },
                                                                        { HttpRequestHeader.Host.ToString(), (request, value) => {request.Headers[HttpRequestHeader.Host] = value; } },
                                                                        { HttpRequestHeader.IfModifiedSince.ToString(), (request, value) => {request.Headers[HttpRequestHeader.IfModifiedSince] = value; } },
                                                                        { HttpRequestHeader.Referer.ToString(), (request, value) => { request.Headers[HttpRequestHeader.Referer] = value; } },
                                                                        { HttpRequestHeader.TransferEncoding.ToString(), (request, value) => { request.Headers[HttpRequestHeader.TransferEncoding] = value; } },
                                                                    };
#endif

        public HttpWebRequestWrapper(HttpWebRequest request)
        {
            _request = request;
        }

        public string UserAgent
        {
            get
            {
#if !NET_STANDARD
                return _request.UserAgent;
#else
                return _request.Headers[HttpRequestHeader.UserAgent];
#endif
            }
            set
            {
#if !NET_STANDARD
                _request.UserAgent = value;
#else
                _request.Headers[HttpRequestHeader.UserAgent] = value;
#endif
            }
        }

        public ICredentials Credentials
        {
            get
            {
                return _request.Credentials;
            }
            set
            {
                _request.Credentials = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get
            {
                return _request.CookieContainer;
            }
            set
            {
                _request.CookieContainer = value;
            }
        }

        public string Accept
        {
            get
            {
                return _request.Accept;
            }
            set
            {
                _request.Accept = value;
            }
        }

        public IWebProxy Proxy
        {
            get
            {
                return _request.Proxy;
            }
            set
            {
                _request.Proxy = value;
            }
        }

        public void Abort()
        {
            _request.Abort();
        }

        public void SetRequestHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            foreach (KeyValuePair<string, string> headerEntry in headers)
            {
                if (!_restrictedHeadersSet.Keys.Contains(headerEntry.Key))
                {
#if !NET_STANDARD
                    _request.Headers.Add(headerEntry.Key, headerEntry.Value);
#else
                    _request.Headers[headerEntry.Key] = headerEntry.Value;
#endif
                }
                else
                {
                    Action<HttpWebRequest, string> setHeaderAction;

                    if (_restrictedHeadersSet.TryGetValue(headerEntry.Key, out setHeaderAction))
                    {
                        setHeaderAction.Invoke(_request, headerEntry.Value);
                    }
                }
            }
        }

        public void AddClientCerts(X509CertificateCollection certificates)
        {
#if !NET_STANDARD
            if (certificates == null)
            {
                throw new ArgumentNullException("certificates");
            }

            // Mono hasn't implemented client certs
            if (!MonoUtility.IsRunningMono)
            {
                _request.ClientCertificates = certificates;
            }
#else
            throw new NotImplementedException();
#endif
        }
    }
}
