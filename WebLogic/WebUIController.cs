using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Web
{
    public class WebUIController : MonoBehaviour
    {
        private CanvasManager canvasManager;

        [Header("Login")]
        [SerializeField] private TMP_InputField LoginText;
        [SerializeField] private TMP_InputField PasswordText;
        [SerializeField] private Button SendLoginButton;

        [SerializeField] private Button OnlineButton;

        [Header("Register")]
        [SerializeField] private TMP_InputField LoginTextRegister;
        [SerializeField] private TMP_InputField NickTextRegister;
        [SerializeField] private TMP_InputField PasswordTextRegister;
        [SerializeField] private TMP_InputField PasswordTextRegister2;
        [SerializeField] private Button SendButtonRegister;

        [Header("Statistic")]
        [SerializeField] private TextMeshProUGUI Nick;
        [SerializeField] private TextMeshProUGUI Orbs;
        [SerializeField] private TextMeshProUGUI BestTime;

        [Header("LeaderBoard")]
        [SerializeField] private Button LeaderBoardButton;
        [SerializeField] private GameObject columnPrefab;
        [SerializeField] private VerticalLayoutGroup content;

        [Header("Shop")]
        [SerializeField] private Button shopButton;
        [SerializeField] private GameObject item;
        [SerializeField] private HorizontalLayoutGroup shopContent;

        [Header("Error")]
        [SerializeField] private TextMeshProUGUI ErrorText;

        private WebManager _webManager;
        private LoginData _data;
        private PlayerService _playerService;

        private List<GameObject> records = new List<GameObject>();
        private List<GameObject> items = new List<GameObject>();
        private void Start()
        {
            _webManager = FindObjectOfType<WebManager>();
            canvasManager = FindObjectOfType<CanvasManager>();
            _playerService = FindAnyObjectByType<PlayerService>();

            SendButtonRegister.onClick.AddListener(SendRegisterData);

            LeaderBoardButton.onClick.AddListener(_webManager.GetLeaderBoard);
            shopButton.onClick.AddListener(() => { _webManager.GetItems(); canvasManager.OpenMenu("Shop"); }) ;

            _webManager.OnError.AddListener(() => StartCoroutine(StartErrorTimer()));

            _webManager.OnLogged.AddListener(() => { 
                OnlineButton.onClick.RemoveAllListeners();
                OnlineButton.onClick.AddListener(() => canvasManager.OpenMenu("Online(Logged)"));
                canvasManager.OpenMenu("Statistic");
                });

            ErrorText.text = "";

            OnlineButton.onClick.RemoveAllListeners();
            if (_playerService.GetStatus == PlayerStatus.logged)
            {
                OnlineButton.onClick.AddListener(() => { canvasManager.OpenMenu("Online(Logged)"); SetPlayerData(_playerService.GetPlayerData); });
            }
            else
            {
                SendLoginButton.onClick.AddListener(() => { SendLoginData(); });
                OnlineButton.onClick.AddListener(() => canvasManager.OpenMenu("Online(Anonyme)"));
            }
        }

        private void SendLoginData()
        {
            _data = new LoginData() { login = LoginText.text.Trim(), password = PasswordText.text.Trim() };

            _webManager.Login(_data);            
        }

        private void SendRegisterData()
        {
            _webManager.Registration(
                LoginTextRegister.text.Trim(),
                  PasswordTextRegister.text.Trim(),
                  PasswordTextRegister2.text.Trim(),
                  NickTextRegister.text.Trim()
                  );
        }

        public void SetPlayerData(PlayerData data)
        {
            Nick.text = $"Nick: {data.Nick}" ;
            Orbs.text = $"Orbs: {data.Orbs}";
            BestTime.text = $"BestTime: {data.Time}";
        }

        public void DisplayRecords( List<LeaderBoardData> data)
        {
            if (records != null)
            {
                foreach (var rec in records)
                {
                    Destroy(rec.gameObject);
                }
                records.Clear();
            }

            foreach (var record in data)
            {
                var temp = Instantiate(columnPrefab, content.transform);
                temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Nick: {record.nick_name}";
                temp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"BestTime: {record.time}";
                temp.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Creation_time: {record.create_time}";
                records.Add(temp);
            }
        }

        public void DisplayShop(List<ItemData> data)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Destroy(item.gameObject);
                }
                items.Clear();
            }

            foreach (var record in data)
            {
                var temp = Instantiate(item, shopContent.transform);
                temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{record.Name} ({record.Price}) ";

                UnityEvent _event = new UnityEvent();
                _event.AddListener(() => _webManager.BuyItem(record.Name));

                temp.GetComponentInChildren<Button>().onClick.AddListener(() => _event.Invoke());


                records.Add(temp);
            }
        }

        private IEnumerator StartErrorTimer()
        {
            ErrorText.text = "error";

            yield return new WaitForSeconds(5);
            ErrorText.text = "";

            yield break;
        }
    }
}
