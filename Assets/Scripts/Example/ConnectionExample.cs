using System;
using System.Collections.Generic;
using GameDesire.Rest;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

class ConnectionExample : MonoBehaviour
{
    private RestClient _restClient;
    private ExampleResponse _exampleResponse;

    [SerializeField]
    public string Url;
    [SerializeField]
    public List<Header> Headers = new List<Header>();
    [SerializeField]
    public string GamestateResource;
    [SerializeField]
    public string HealthCheckResource;

    private void Start()
    {
        _restClient = new RestClient(Url, HealthCheckResource);
        foreach (var header in Headers)
        {
            _restClient.AddHeader(header.key, header.value);
        }
    }

    public void OnGetButtonPressed()
    {
        _restClient.Get<ExampleResponse>(GamestateResource, OnExampleResponseReceived);
    }

    public void OnPostButtonPressed()
    {
        _restClient.Post(GamestateResource, JsonConvert.SerializeObject(_exampleResponse));
    }


    public void OnPutButtonPressed()
    {
        _exampleResponse.Response.Nickname = "User" + Guid.NewGuid();
        _restClient.Put(GamestateResource, JsonConvert.SerializeObject(_exampleResponse));
    }

    public void OnDeleteButtonPressed()
    {
        _restClient.Delete(GamestateResource, JsonConvert.SerializeObject(_exampleResponse));
    }

    private void OnExampleResponseReceived(ExampleResponse response)
    {
        _exampleResponse = response;
        Debug.Log(string.Format("User ID: {0}, Nickname: {1}", response.UserId, response.Response.Nickname));
    }
}