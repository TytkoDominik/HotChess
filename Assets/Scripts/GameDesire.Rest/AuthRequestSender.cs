using System;
using System.Collections.Generic;
using DIFramework;
using GameDesire.Rest.Enums;
using GameDesire.Rest.Utility;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;
using Zenject;

namespace GameDesire.Rest
{
    public class AuthRequestSender : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private RestClient _restClient;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private GameObject _error;
        [SerializeField] private Text _errorText;

        private Action<string> _onSuccess = m =>
        {
            Debug.Log(m);
        };
        
        private Action<WWWErrorException> _logError = ex => Debug.Log("Error! " + ex.RawErrorMessage);

        private void Start()
        {
            _onSuccess += s => _loadingPanel.SetActive(false);
            _logError += ex =>
            {
                _loadingPanel.SetActive(false);
                _error.SetActive(true);
                _errorText.text = ex.Text;
            };
        }
        
        public void SendLoginRequest(string login, string password, Action<string> callback)
        {
            _loadingPanel.SetActive(true);
            callback += s => _loadingPanel.SetActive(false);
            var restRequest = new RestRequest("auth/authorize", HttpMethod.Get);
            var wrapper = new RequestWrapper(restRequest, new CallbackWrapper(callback), new CallbackWrapper(_logError));
            var headers = new Dictionary<string, string>();
            headers.Add("login", login);
            headers.Add("password", password);
            
            _restClient.SendRequest(wrapper, headers);
        }

        public void SendStatsRequest(int id, Action<string> callback)
        {
            _loadingPanel.SetActive(true);
            callback += s =>
            {
                _signalBus.Fire<UpdatePlayerLabelsSignal>();
                _loadingPanel.SetActive(false);
            };
            var restRequest = new RestRequest("stats/get", HttpMethod.Get);
            var wrapper = new RequestWrapper(restRequest, new CallbackWrapper(callback), new CallbackWrapper(_logError));
            var headers = new Dictionary<string, string>();
            headers.Add("id", id.ToString());
            
            _restClient.SendRequest(wrapper, headers);
        }

        public void SendUpdateStatsRequest(int id, string serializedStats)
        {
            _loadingPanel.SetActive(true);
            var restRequest = new RestRequest("stats/update", HttpMethod.Get);
            var wrapper = new RequestWrapper(restRequest, new CallbackWrapper(_onSuccess), new CallbackWrapper(_logError));
            var headers = new Dictionary<string, string>();
            headers.Add("id", id.ToString());
            headers.Add("gamestate", serializedStats);
            
            _restClient.SendRequest(wrapper, headers);
        }

        public void SendAddPlayerRequest(string login, string password, Action<string> callback)
        {
            _loadingPanel.SetActive(true);
            
            Action<string> _addPlayerSuccess = m =>
            {
                int id = -1;
                if (Int32.TryParse(m, out id))
                {
                    AddPlayerStatsRequest(id);
                }
                _loadingPanel.SetActive(false);
                callback(m);
            };
            
            var restRequest = new RestRequest("auth/add", HttpMethod.Get);
            var wrapper = new RequestWrapper(restRequest, new CallbackWrapper(_addPlayerSuccess), new CallbackWrapper(_logError));
            var headers = new Dictionary<string, string>();
            headers.Add("login", login);
            headers.Add("password", password);
            
            _restClient.SendRequest(wrapper, headers);
        }

        private void AddPlayerStatsRequest(int id)
        {
            _loadingPanel.SetActive(true);
            var restRequest = new RestRequest("stats/add", HttpMethod.Get);
            var wrapper = new RequestWrapper(restRequest, new CallbackWrapper(_onSuccess), new CallbackWrapper(_logError));
            var headers = new Dictionary<string, string>();
            headers.Add("id", id.ToString());
            
            var playerStats = new PlayerStats();
            
            headers.Add("gamestate", JsonConvert.SerializeObject(playerStats));
            
            _restClient.SendRequest(wrapper, headers);
        }
    }
}