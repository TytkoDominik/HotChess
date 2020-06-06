using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GameDesire.Rest.Enums;
using GameDesire.Rest.Utility;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace GameDesire.Rest
{
    public class RestClient
    {
        public RestClient(string baseUrl, string healthCheckResource) : this(baseUrl, healthCheckResource, new TimeSpan(0, 0, 3), null)
        {

        }

        public RestClient(string baseUrl, string healthCheckResource, Dictionary<string, string> headers) : this(baseUrl, healthCheckResource, new TimeSpan(0, 0, 3), headers)
        {

        }

        public RestClient(string baseUrl, string healthCheckResource, TimeSpan resendInterval, Dictionary<string, string> headers)
        {
            _baseUrl = baseUrl;

            _baseUrl = _baseUrl.TrimEnd('/');

            _headers = headers ?? new Dictionary<string, string>();

            if (!healthCheckResource.StartsWith("/"))
            {
               healthCheckResource = healthCheckResource.Insert(0, "/");
            }

            _helathCheckResource = healthCheckResource;

            foreach (var header in _headers)
            {
                _connectionCheckRequest.Headers.Add(header.Key, header.Value);
            }

            var path = Path.Combine(Application.persistentDataPath, CACHED_FILE_FOLDER);
            path = Path.Combine(path, _baseUrl.GetHashCode().ToString());
            _requestsCacheDirectory = new DirectoryInfo(path);

            if (!_requestsCacheDirectory.Exists)
            {
                _requestsCacheDirectory.Create();
            }

            ReadCachedRequestsToRequestQueue();

            CoroutineRunner.StartCoroutine(SendRequestsCoroutine(resendInterval));
        }

        private static CoroutineRunner _coroutineRunner;

        private static CoroutineRunner CoroutineRunner
        {
            get
            {
                if (_coroutineRunner == null)
                {
                    _coroutineRunner = new GameObject().AddComponent<CoroutineRunner>();
                    _coroutineRunner.gameObject.name = "CoroutineRunner";
                    Object.DontDestroyOnLoad(_coroutineRunner);
                }
                return _coroutineRunner;
            }
        }

        private RestRequest GetHealthCheckRequest()
        {
            return new RestRequest(_helathCheckResource, HttpMethod.Get, false, 3);
        }

        private readonly Queue<RequestWrapper> _requestsQueue = new Queue<RequestWrapper>();

        private const string CACHED_FILE_FOLDER = "cached";
        private const string CACHED_FILE_EXTENSION = "cm";
        private readonly string _baseUrl;
        private readonly string _helathCheckResource;
        private readonly Dictionary<string, string> _headers;
        private Dictionary<string, string> _temporaryHeaders;
        private readonly DirectoryInfo _requestsCacheDirectory;
        private readonly HttpWebRequest _connectionCheckRequest;
        private bool _isConnectionAvailable = false;

        #region HttpRequestMethods

        public void AddHeader(string key, string value)
        {
            _headers.Add(key, value);
            _connectionCheckRequest.Headers.Add(key, value);
        }

        public void Get(string resource, Action<string> onSuccess)
        {
            Get(resource, onSuccess, null);
        }

        public void Get<T>(string resource, Action<T> onSuccess, Action<WWWErrorException> onFailed = null)
        {
            var restRequest = new RestRequest(resource, HttpMethod.Get);
            EnqueueRequest(restRequest, onSuccess, onFailed);
        }

        public void Post(string resource, string payload, bool retryIfRequestFails = false)
        {
            Post<object>(resource, payload, null, null, retryIfRequestFails);
        }

        public void Post<T>(string resource, string payload, Action<T> onSuccess, bool retryIfRequestFails = false)
        {
            Post(resource, payload, onSuccess, null, retryIfRequestFails);
        }

        public void Post<T>(string resource, string payload, Action<T> onSuccess, Action<WWWErrorException> onFailed, bool retryIfRequestFails = false)
        {
            var restRequest = new RestRequest(resource, HttpMethod.Post, payload, retryIfRequestFails);
            EnqueueRequest(restRequest, onSuccess, onFailed);
        }

        public void Put(string resource, string payload, bool retryIfRequestFails = false)
        {
            Put<object>(resource, payload, null, null, retryIfRequestFails);
        }

        public void Put<T>(string resource, string payload, Action<T> onSuccess, bool retryIfRequestFails = false)
        {
            Put(resource, payload, onSuccess, null, retryIfRequestFails);
        }
        public void Put<T>(string resource, string payload, Action<T> onSuccess, Action<WWWErrorException> onFailed, bool retryIfRequestFails = false)
        {
            var restRequest = new RestRequest(resource, HttpMethod.Put, payload, retryIfRequestFails);
            EnqueueRequest(restRequest, onSuccess, onFailed);
        }

        public void Delete(string resource, string payload, bool retryIfRequestFails = false)
        {
            Delete<object>(resource, payload, null, null, retryIfRequestFails);
        }

        public void Delete<T>(string resource, string payload, Action<T> onSuccess, bool retryIfRequestFails = false)
        {
            Delete(resource, payload, onSuccess, null, retryIfRequestFails);
        }
        public void Delete<T>(string resource, string payload, Action<T> onSuccess, Action<WWWErrorException> onFailed, bool retryIfRequestFails = false)
        {
            var restRequest = new RestRequest(resource, HttpMethod.Delete, payload, retryIfRequestFails);
            EnqueueRequest(restRequest, onSuccess, onFailed);
        }

        #endregion

        public void SendRequest(RequestWrapper wrappedRequest, Dictionary<string, string> headers)
        {
            Execute(wrappedRequest, headers);
        }
        
        private void Execute(RequestWrapper wrappedRequest, Dictionary<string, string> headers = null)
        {
            if (headers == null)
            {
                headers = _headers;
            }
            
            var restRequest = wrappedRequest.RestRequest;
            var onSuccessCallback = wrappedRequest.OnSuccess;
            var onFailedCallback = wrappedRequest.OnFailed;

            CreateRequest(restRequest.HttpMethod, _baseUrl + restRequest.Resource, restRequest.Body, headers, restRequest.TimeoutInSeconds).Subscribe(
                onNext: result =>
                {
                    if (onSuccessCallback != null)
                    {
                        if (onSuccessCallback.ActionArgumentType == typeof(string))
                        {
                            onSuccessCallback.Invoke(result);
                        }
                        else
                        {
                            onSuccessCallback.Invoke(JsonConvert.DeserializeObject(result, onSuccessCallback.ActionArgumentType));
                        }
                    }
                },
                onError: ex =>
                {
                    if (onFailedCallback != null)
                    {
                        onFailedCallback.Invoke((WWWErrorException)ex);
                    }
                });
        }

        private static UniRx.IObservable<string> CreateRequest(HttpMethod httpMethod, string url, string postData, Dictionary<string, string> headers, int timeout)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => SendRequestCoroutine(httpMethod, url, postData, headers, timeout, observer, cancellation));
        }

        private static IEnumerator SendRequestCoroutine(HttpMethod httpMethod, string url, string postData,
            Dictionary<string, string> headers, int timeout, UniRx.IObserver<string> observer, CancellationToken cancel)
        {
            UnityWebRequest webRequest = null;

            switch (httpMethod)
            {
                case HttpMethod.Get:
                    webRequest = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    webRequest = UnityWebRequest.Post(url, postData);
                    break;
                case HttpMethod.Put:
                    webRequest = UnityWebRequest.Put(url, postData);
                    break;
                case HttpMethod.Delete:
                    webRequest = UnityWebRequest.Delete(url);
                    break;
            }

            webRequest.timeout = timeout;

            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            using (webRequest)
            {
                webRequest.SendWebRequest();

                yield return new WaitUntil(() => webRequest.isDone || webRequest.isHttpError || webRequest.isNetworkError || cancel.IsCancellationRequested);

                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }

                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    observer.OnError(new WWWErrorException(webRequest, webRequest.error));
                }
                else
                {
                    var spliter = new string[] {"$!$"};
                    var package =
                        webRequest.downloadHandler.text.Split(spliter, StringSplitOptions.RemoveEmptyEntries)[1];
                    observer.OnNext(package);
                    observer.OnCompleted();
                }
            }
        }

        private void SerializeRequest(RestRequest restRequest, string filePath)
        {

            BinarySerializer.WriteToBinaryFile(filePath, restRequest);
        }

        private void DeleteSerializedRequest(string filePath)
        {

            var file = new FileInfo(filePath);

            if (file.Exists)
            {
                file.Delete();
            }
        }

        private void EnqueueRequest<T>(RestRequest restRequest, Action<T> onSuccess, Action<WWWErrorException> onError, string serializedRequestPath = null)
        {
            if (restRequest.RetryIfRequestFails && (serializedRequestPath == null || !File.Exists(serializedRequestPath)))
            {
                var fileName = string.Format("{0}_{1}", DateTimeUtility.GetCurrentPosixMiliseconds(),
                    Guid.NewGuid());
                fileName = string.Format("{0}.{1}", fileName, CACHED_FILE_EXTENSION);
                serializedRequestPath = Path.Combine(_requestsCacheDirectory.ToString(), fileName);
                SerializeRequest(restRequest, serializedRequestPath);
            }

            var cachedRequest =
                new RequestWrapper(restRequest);

            cachedRequest.OnSuccess = new CallbackWrapper(
                new Action<T>(result =>
                {
                    if (onSuccess != null)
                    {
                        onSuccess.Invoke(result);
                    }

                    if (serializedRequestPath != null)
                    {
                        DeleteSerializedRequest(serializedRequestPath);
                    }
                }));

            cachedRequest.OnFailed = new CallbackWrapper(
                new Action<WWWErrorException>(ex =>
                {
                    OnErrorDefaultCallback(ex);

                    _requestsQueue.Enqueue(cachedRequest);

                    if (onError != null)
                    {
                        onError.Invoke(ex);
                    }
                }));

            _requestsQueue.Enqueue(cachedRequest);
        }

        private void OnErrorDefaultCallback(WWWErrorException ex)
        {
            _isConnectionAvailable = false;

            if (ex.StatusCode == 0)
            {
                Debug.LogError("Error: Unable to connect to server");
                return;
            }

            Debug.LogErrorFormat("Error {0}: {1}", (int)ex.StatusCode, ex.StatusCode);
        }

        private void ReadCachedRequestsToRequestQueue()
        {
            var files = _requestsCacheDirectory.GetFiles(string.Format("*.{0}", CACHED_FILE_EXTENSION)).OrderBy(f => f.Name);

            foreach (var file in files)
            {
                var restRequest = BinarySerializer.ReadFromBinaryFile<RestRequest>(file.FullName);
                EnqueueRequest<object>(restRequest, null, null, file.Name);
            }
        }

        private IEnumerator SendRequestsCoroutine(TimeSpan interval)
        {
            var intervalMultiplier = 1;
            bool healthCheckFinished = false;

            Action<string> healthCheckSuccess = o =>
            {
                healthCheckFinished = true;
                _isConnectionAvailable = true;
            };
            
            Action<WWWErrorException> healthCheckError = ex =>
            {
                OnErrorDefaultCallback(ex);
                healthCheckFinished = true;
                _isConnectionAvailable = false;
            };
                
            while (true)
            {
                yield return new WaitForSeconds((float)interval.TotalSeconds * intervalMultiplier);

                /*var healthCheckRequest = new RequestWrapper(GetHealthCheckRequest(),
                    new CallbackWrapper(healthCheckSuccess), new CallbackWrapper(healthCheckError));
                
                Execute(healthCheckRequest);
                
                yield return new WaitUntil(() => healthCheckFinished);
                Debug.Log("Connection finiished with result: " + _isConnectionAvailable);
                healthCheckFinished = false;

                if (!_isConnectionAvailable)
                {
                    continue;
                }

                while (_requestsQueue.Count > 0)
                {
                    var cachedRequest = _requestsQueue.Dequeue();
                    Execute(cachedRequest);
                }*/
            }
        }
    }
}