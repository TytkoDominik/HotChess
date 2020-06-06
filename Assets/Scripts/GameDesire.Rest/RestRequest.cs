using System;
using System.IO;
using GameDesire.Rest.Enums;

namespace GameDesire.Rest
{
    [Serializable]
    public class RestRequest
    {
        public RestRequest(string resource, HttpMethod httpMethod, bool retryIfRequestFails = false, int timeoutInSeconds = 3) : this(resource, httpMethod,null, retryIfRequestFails, timeoutInSeconds)
        {
        }

        public RestRequest(string resource, HttpMethod httpMethod, string body, bool retryIfRequestFails = false, int timeoutInSeconds = 3)
        {
            Resource = resource;

            if (!Resource.StartsWith("/"))
            {
                Resource = Resource.Insert(0, "/");
            }

            Body = body;
            HttpMethod = httpMethod;
            RetryIfRequestFails = retryIfRequestFails;
            TimeoutInSeconds = timeoutInSeconds;
        }

        public string Resource { get; private set; }
        public string Body { get; private set; }
        public int TimeoutInSeconds { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public bool RetryIfRequestFails { get; private set; }
    }
}