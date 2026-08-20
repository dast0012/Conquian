using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviour
{
    private int targetPlayerCount;
    private bool gameStarting;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }
    private void OnClientConnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        int connectedPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log($"Players connected: {connectedPlayers}/{targetPlayerCount}");

        if (connectedPlayers >= targetPlayerCount && !gameStarting)
            StartGame();
    }
    private void OnClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        int connectedPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log($"Players connected: {connectedPlayers}/{targetPlayerCount}");
    }

    public async Task<string> StartHostWithRelay(int maxConnections)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        targetPlayerCount = maxConnections + 1;
        gameStarting = false;

        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
        NetworkManager.Singleton.StartHost();
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return joinCode;
    }
    public async Task StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));
        NetworkManager.Singleton.StartClient();
    }

    private void StartGame()
    {
        if (gameStarting)
            return;

        gameStarting = true;

        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene",LoadSceneMode.Single);
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Disconnecting from network");
            NetworkManager.Singleton.Shutdown();
        }

        targetPlayerCount = 0;
        gameStarting = false;
    }
}