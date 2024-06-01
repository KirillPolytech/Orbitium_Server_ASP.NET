using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Web
{
    public class WebManager : MonoBehaviour
    {
        private static WebManager _instance = null;

        public UnityEvent OnLogged, OnRegistered, OnGetPlayerData, OnError;

        private string LoginURL = "https://localhost:7245/api/User/Login";
        private string RegisterURL = "https://localhost:7245/RegisterUser";
        private string DeleteUserURL = "https://localhost:7245/DeleteUser";

        private string UpdateStatisticURL = "https://localhost:7245/UpdateStatistic";
        private string GetLeaderBoardsURL = "https://localhost:7245/GetLeaderBoards";
        private string GetPlayerDataURL = "https://localhost:7245/GetPlayerData";
        private string GetItemDataURL = "https://localhost:7245/GetItemsData";
        private string BuyItemURL = "https://localhost:7245/BuyItem";
        private string SetRecordURL = "https://localhost:7245/SetRecord";

        private JwtToken playerToken;
        private UnityWebRequest www;

        private PlayerService _playerService;
        private MainPlayer _mainPlayer;
        private InGameTime _timer;
        private CanvasManager _canvasManager;
        private WebUIController UI;
        private void Awake()
        {
            Initializate();
        }

        private void OnLevelWasLoaded(int level)
        {
            Initializate();

            OnLogged.AddListener( () => _canvasManager.OpenMenu("Statistic") );
            OnRegistered.AddListener( () => _canvasManager.OpenMenu("Statistic") );

            if (_mainPlayer != null)
            {
                _mainPlayer.EventAtDeath = SendStatistic;
            }
        }

        private void Initializate()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
                Destroy(gameObject);

            _mainPlayer = FindAnyObjectByType<MainPlayer>();
            _playerService = FindObjectOfType<PlayerService>();
            _timer = FindObjectOfType<InGameTime>();
            _canvasManager = FindObjectOfType<CanvasManager>();
            UI = FindObjectOfType<WebUIController>();
        }
        // check
        public void Login(LoginData data)
        {
            StopAllCoroutines();

            if (data == null)
                return;

            if (CheckString(data.login) && CheckString(data.password))
            {
                StartCoroutine(LoginRequest(data) );
                return;
            }

            OnError.Invoke();
        }
        // check
        public void Registration(string login, string password, string password2, string nickname)
        {
            StopAllCoroutines();

            if (CheckString(login) && CheckString(nickname) &&
                CheckString(password) && 
                CheckString(password2) &&
                password == password2)
            {
                StartCoroutine( RegistrationRequest(new RegisterData() 
                { Login = login, 
                    Password = password, Nick = 
                    nickname, 
                    Create_time = DateTime.Now} ) );
                return;
            }

            OnError.Invoke();
        }
        // check
        public void GetLeaderBoard()
        {
            StopAllCoroutines();

            StartCoroutine(GetLeaderBoardRequest());
        }
        // check
        public void GetItems()
        {
            StopAllCoroutines();

            StartCoroutine(GetItemDataRequest());
        }
        // check
        public void BuyItem(string name)
        {
            StopAllCoroutines();

            StartCoroutine(BuyItemRequest(name));
        }
        // check
        private void SendStatistic()
        {
            if (playerToken == null || _playerService.GetPlayerData == null)
            {
                Debug.LogWarning("Empty data");   
                return;
            }

            var data = new PlayerData()
            {
                Orbs = _mainPlayer.GetCollectables,
                Nick = _playerService.GetPlayerData.Nick,
                Time = _timer.GetTime
            };

            _playerService.UpdateData(data);

            StartCoroutine( UpdateStatisticRequest(playerToken, _playerService.GetPlayerData) );
        }
        // check
        private IEnumerator LoginRequest(LoginData data)
        {
            www = UnityWebRequest.Post(LoginURL, JsonUtility.ToJson(data), "application/json");
            yield return www.SendWebRequest();

            playerToken = JsonConvert.DeserializeObject<JwtToken>(www.downloadHandler.text);

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            Debug.Log(playerToken.Token);

            StartCoroutine(GetPlayerDataRequest(playerToken));
        }
        // check
        private IEnumerator RegistrationRequest(RegisterData data)
        {
            www = UnityWebRequest.Post(RegisterURL, JsonUtility.ToJson(data), "application/json");
            yield return www.SendWebRequest();

            playerToken = JsonConvert.DeserializeObject<JwtToken>(www.downloadHandler.text);

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }


            Debug.Log(playerToken.Token);

            StartCoroutine(GetPlayerDataRequest(playerToken));
        }
        // check
        private IEnumerator GetPlayerDataRequest(JwtToken jwtToken)
        {
            www = UnityWebRequest.Get(GetPlayerDataURL);
            www.SetRequestHeader("Authorization", "Bearer " + jwtToken.Token);
            yield return www.SendWebRequest();

            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(www.downloadHandler.text);

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            _playerService.UpdateData(data);

            OnLogged.Invoke();

            Debug.Log(data.Orbs + " " + data.Nick);
        }
        // check
        private IEnumerator UpdateStatisticRequest(JwtToken jwtToken, PlayerData data)
        {
            string JsonData = JsonConvert.SerializeObject(data);

            www = UnityWebRequest.Post(UpdateStatisticURL, JsonData, "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + jwtToken.Token);

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            yield return null;
        }
        // check
        private IEnumerator GetLeaderBoardRequest()
        {
            www = UnityWebRequest.Get(GetLeaderBoardsURL);

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            List< LeaderBoardData> data = JsonConvert.DeserializeObject<List<LeaderBoardData>>(www.downloadHandler.text);

            UI.DisplayRecords(data);

            yield return null;
        }
        // check
        private IEnumerator GetItemDataRequest()
        {
            www = UnityWebRequest.Get(GetItemDataURL);

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            List<ItemData> d = JsonConvert.DeserializeObject<List<ItemData>>(www.downloadHandler.text);

            UI.DisplayShop(d);

            yield return null;
        }
        // Check
        private IEnumerator BuyItemRequest(string name)
        {
            string JsonData = JsonConvert.SerializeObject(name);

            www = UnityWebRequest.Post(GetItemDataURL, JsonData, "application/json");
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                OnError.Invoke();
                yield break;
            }

            Debug.Log(www.downloadHandler.text);

            yield return null;
        }

        private bool CheckString(string toCheck)
        {
            toCheck = toCheck.Trim();
            if (toCheck.Length > 4 && toCheck.Length < 16)
            {
                return true;
            }
            return false;
        }
    }
}