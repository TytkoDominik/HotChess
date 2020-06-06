using System.Collections;

namespace GameDesire.Rest.Utility
{
    public class RequestWrapper
    {
        public RequestWrapper(RestRequest restRequest, CallbackWrapper onSuccess = null, CallbackWrapper onFailed = null)
        {
            RestRequest = restRequest;
            OnSuccess = onSuccess;
            OnFailed = onFailed;
        }

        public RestRequest RestRequest { get; private set; }
        public CallbackWrapper OnSuccess { get; set; }
        public CallbackWrapper OnFailed { get; set; }
    }
}