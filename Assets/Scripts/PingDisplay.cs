using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PingDisplay : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    void Update()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            // Get the local player's RTT in milliseconds
            float ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.LocalClientId);
            Debug.Log($"Current Ping: {ping}ms");
            counterText.text = $"Ping: {ping} ms";
        }
    }
}
