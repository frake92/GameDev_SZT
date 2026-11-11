using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panelek")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject joinPanel;
    [SerializeField] GameObject clientWaitPanel;

    [Header("Main")]
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;

    [Header("Lobby (Host)")]
    [SerializeField] TMP_Text waitingText;
    [SerializeField] TMP_Text ipText;
    [SerializeField] Button startButton;
    [SerializeField] Button lobbyBackButton;

    [Header("Join")]
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] Button connectButton;
    [SerializeField] Button joinBackButton;

    [Header("Client várakozás")]
    [SerializeField] Button clientWaitBackButton;

    [Header("Játék")]
    [SerializeField] string gameSceneName = "";   // később: "Game"

    const ushort Port = 7777;
    UnityTransport transport;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ipInput.text = "127.0.0.1";

        hostButton.onClick.AddListener(OnHostClicked);
        joinButton.onClick.AddListener(() => Show(joinPanel));
        connectButton.onClick.AddListener(OnConnectClicked);
        startButton.onClick.AddListener(OnStartClicked);

        lobbyBackButton.onClick.AddListener(BackToMenu);
        joinBackButton.onClick.AddListener(BackToMenu);
        clientWaitBackButton.onClick.AddListener(BackToMenu);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        Show(mainPanel);
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    // ---------- Gombok ----------

    void OnHostClicked()
    {
        // 0.0.0.0: a hálózatról is fogadjon kapcsolatot
        transport.SetConnectionData("127.0.0.1", Port, "0.0.0.0");
        NetworkManager.Singleton.StartHost();

        ipText.text = "Host IP: " + GetLocalIP();
        UpdateWaitingText(null);
        Show(lobbyPanel);
    }

    void OnConnectClicked()
    {
        transport.SetConnectionData(ipInput.text.Trim(), Port);
        NetworkManager.Singleton.StartClient();
        // sikerről az OnClientConnected, kudarcról az OnClientDisconnected értesít
    }

    void OnStartClicked()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log("Start megnyomva, de még nincs játékjelenet megadva.");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    void BackToMenu()
    {
        if (NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();
        Show(mainPanel);
    }

    // ---------- Hálózati események ----------

    void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
            UpdateWaitingText(null);                       // valaki csatlakozott
        else if (clientId == NetworkManager.Singleton.LocalClientId)
            Show(clientWaitPanel);                         // mi csatlakoztunk
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
            UpdateWaitingText(clientId);                   // valaki lelépett
        else
            BackToMenu();                                  // sikertelen / megszakadt kapcsolat
    }

    // ---------- Segédek ----------

    void UpdateWaitingText(ulong? leavingClientId)
    {
        int others = 0;
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (id == NetworkManager.ServerClientId) continue;   // a Host saját magát nem számoljuk
            if (leavingClientId.HasValue && id == leavingClientId.Value) continue;
            others++;
        }

        waitingText.text = "Waiting for players: " + others;
        startButton.interactable = others >= 1;                  // PvP-hez legalább 2 játékos
    }

    void Show(GameObject panel)
    {
        mainPanel.SetActive(panel == mainPanel);
        lobbyPanel.SetActive(panel == lobbyPanel);
        joinPanel.SetActive(panel == joinPanel);
        clientWaitPanel.SetActive(panel == clientWaitPanel);
    }

    static string GetLocalIP()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        return "?";
    }
}