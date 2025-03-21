using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace WeatherApp.Services
{
    public class NetworkRequest
    {
        public UnityWebRequest Request { get; set; }
        public TaskCompletionSource<string> CompletionSource { get; set; }
        public Action OnCancel { get; set; }
    }

    public class NetworkService : IInitializable, IDisposable
    {
        private readonly Queue<NetworkRequest> _requestQueue = new Queue<NetworkRequest>();
        private bool _isProcessing;
        private NetworkRequest _currentRequest;

        public async Task<string> EnqueueRequest(string url, Action onCancel = null)
        {
            var request = UnityWebRequest.Get(url);
            var completionSource = new TaskCompletionSource<string>();
            
            var networkRequest = new NetworkRequest
            {
                Request = request,
                CompletionSource = completionSource,
                OnCancel = onCancel
            };

            _requestQueue.Enqueue(networkRequest);
            ProcessQueue();

            return await completionSource.Task;
        }

        private async void ProcessQueue()
        {
            if (_isProcessing || _requestQueue.Count == 0)
                return;

            _isProcessing = true;
            _currentRequest = _requestQueue.Dequeue();

            try
            {
                var operation = _currentRequest.Request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (_currentRequest.Request.result == UnityWebRequest.Result.Success)
                {
                    _currentRequest.CompletionSource.SetResult(_currentRequest.Request.downloadHandler.text);
                }
                else
                {
                    _currentRequest.CompletionSource.SetException(new Exception(_currentRequest.Request.error));
                }
            }
            catch (Exception e)
            {
                _currentRequest.CompletionSource.SetException(e);
            }
            finally
            {
                _currentRequest.Request.Dispose();
                _currentRequest = null;
                _isProcessing = false;
                ProcessQueue();
            }
        }

        public void CancelCurrentRequest()
        {
            if (_currentRequest != null)
            {
                _currentRequest.Request.Abort();
                _currentRequest.OnCancel?.Invoke();
            }
        }

        public void RemoveRequestFromQueue(NetworkRequest request)
        {
            var queueList = new List<NetworkRequest>(_requestQueue);
            queueList.Remove(request);
            _requestQueue.Clear();
            foreach (var req in queueList)
            {
                _requestQueue.Enqueue(req);
            }
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
            CancelCurrentRequest();
            _requestQueue.Clear();
        }
    }
} 