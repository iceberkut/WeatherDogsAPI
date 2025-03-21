using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Core.RequestSystem
{
    public class RequestQueue : IInitializable, IDisposable
    {
        private Queue<RequestData> _requestQueue = new Queue<RequestData>();
        private bool _isProcessing;
        private SignalBus _signalBus;

        [Serializable]
        private class ArrayWrapper
        {
            public DogBreed[] items;
        }

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            Debug.Log("RequestQueue: Constructor called");
            _signalBus = signalBus;
            Debug.Log("RequestQueue: Subscribing to ProcessNextRequestSignal");
            _signalBus.Subscribe<ProcessNextRequestSignal>(ProcessNextRequest);
        }

        public void Initialize()
        {
            Debug.Log("RequestQueue: Initialize called");
        }

        public void Dispose()
        {
            Debug.Log("RequestQueue: Dispose called");
            _signalBus.Unsubscribe<ProcessNextRequestSignal>(ProcessNextRequest);
        }

        public async Task<T> AddRequest<T>(string url, string requestType = "GET", string body = null)
        {
            Debug.Log($"RequestQueue: Adding request for {url}");
            var requestData = new RequestData<T>
            {
                Url = url,
                RequestType = requestType,
                Body = body,
                Tcs = new TaskCompletionSource<T>()
            };

            _requestQueue.Enqueue(requestData);
            Debug.Log($"RequestQueue: Request added to queue. Queue count: {_requestQueue.Count}");
            _signalBus.Fire<ProcessNextRequestSignal>();
            Debug.Log("RequestQueue: ProcessNextRequestSignal fired");

            return await requestData.Tcs.Task;
        }

        private async void ProcessNextRequest(ProcessNextRequestSignal signal)
        {
            if (_isProcessing || _requestQueue.Count == 0)
            {
                Debug.Log($"RequestQueue: Skipping request processing - isProcessing: {_isProcessing}, queue count: {_requestQueue.Count}");
                return;
            }

            _isProcessing = true;
            var requestData = _requestQueue.Dequeue();
            
            try
            {
                Debug.Log($"RequestQueue: Processing request for {requestData.Url}");
                using (var request = CreateRequest(requestData))
                {
                    Debug.Log($"RequestQueue: Sending request to {requestData.Url}");
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        Debug.Log($"RequestQueue: Request progress: {request.downloadProgress:P2}");
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"RequestQueue: Response received - {request.downloadHandler.text}");
                        var responseType = requestData.GetType().GetGenericArguments()[0];
                        Debug.Log($"RequestQueue: Deserializing response to type {responseType}");
                        
                        if (responseType.IsArray)
                        {
                            Debug.Log("RequestQueue: Processing array response");
                            var wrapper = JsonUtility.FromJson<ArrayWrapper>("{\"items\":" + request.downloadHandler.text + "}");
                            Debug.Log($"RequestQueue: Array wrapper created with {wrapper.items?.Length ?? 0} items");
                            requestData.SetResult(wrapper.items);
                        }
                        else
                        {
                            Debug.Log("RequestQueue: Processing single object response");
                            var response = JsonUtility.FromJson(request.downloadHandler.text, responseType);
                            requestData.SetResult(response);
                        }
                    }
                    else
                    {
                        Debug.LogError($"RequestQueue: Request failed - {request.error}");
                        requestData.SetException(new Exception($"Request failed: {request.error}"));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"RequestQueue: Exception during request - {e.Message}");
                Debug.LogError($"RequestQueue: Stack trace - {e.StackTrace}");
                requestData.SetException(e);
            }
            finally
            {
                _isProcessing = false;
                Debug.Log("RequestQueue: Request processing completed");
                _signalBus.Fire<ProcessNextRequestSignal>();
            }
        }

        private UnityWebRequest CreateRequest(RequestData requestData)
        {
            var request = new UnityWebRequest(requestData.Url, requestData.RequestType);
            
            if (!string.IsNullOrEmpty(requestData.Body))
            {
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData.Body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            return request;
        }
    }

    public abstract class RequestData
    {
        public string Url { get; set; }
        public string RequestType { get; set; }
        public string Body { get; set; }
        public abstract void SetResult(object result);
        public abstract void SetException(Exception exception);
    }

    public class RequestData<T> : RequestData
    {
        public TaskCompletionSource<T> Tcs { get; set; }

        public override void SetResult(object result)
        {
            Tcs.SetResult((T)result);
        }

        public override void SetException(Exception exception)
        {
            Tcs.SetException(exception);
        }
    }

    public class ProcessNextRequestSignal { }
} 