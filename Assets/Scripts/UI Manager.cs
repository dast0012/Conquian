using UnityEngine;
using TMPro;


public class UImanager : MonoBehaviour
{
    public NetworkGameManager networkGameManager;

    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;

    public async void OnCreateGameButton(int maxPlayers)
    {
        networkGameManager.Disconnect();
        if (networkGameManager == null)
        {
            Debug.Log("NetworkGameManager mangler i UIManager.");
            return;
        }

        try
        {
            int maxConnections = maxPlayers - 1;
            string joinCode = await networkGameManager.StartHostWithRelay(maxConnections);
            joinCodeDisplay.text = "Join Code: " + joinCode;
            Debug.Log("Host started. Join Code: " + joinCode);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Kunne ikke oprette game: " + ex);
            joinCodeDisplay.text = "Failed to create game";
        }
    }

    public async void OnJoinGameButton()
    {
        if (networkGameManager == null)
        {
            Debug.LogError("NetworkGameManager mangler i NetworkUIManager.");
            return;
        }

        string code = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Join code er tom.");
            return;
        }

        try
        {
            await networkGameManager.StartClientWithRelay(code);
            Debug.Log("Joining game with code: " + code);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Kunne ikke joine game: " + ex);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
