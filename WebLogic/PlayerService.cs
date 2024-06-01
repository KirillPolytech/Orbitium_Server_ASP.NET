using UnityEngine;

namespace Web
{
    public enum PlayerStatus { logged, unlogged }
    public class PlayerService : MonoBehaviour
    {
        private static PlayerService _instance = null;

        public PlayerData GetPlayerData => _playerData;

        public PlayerStatus GetStatus => _status;

        private PlayerData _playerData = new PlayerData() { Nick = null, Time = 0, Orbs = 0 };
        private WebUIController _controller;

        private PlayerStatus _status = PlayerStatus.unlogged;
        private void Awake()
        {
            Initializate();

            _controller = FindAnyObjectByType<WebUIController>();
        }

        private void OnLevelWasLoaded(int level)
        {
            Initializate();
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
        }

        public void UpdateData(PlayerData data)
        {
            if (data.Nick == null)
                return;
            else if (_playerData.Nick == null)
                _playerData.Nick = data.Nick;            

            _playerData.Orbs += data.Orbs;

            if (_playerData.Time < data.Time)
            {
                _playerData.Time = data.Time;
            }

            _controller.SetPlayerData(_playerData);

            _status = PlayerStatus.logged;
        }
    }
}