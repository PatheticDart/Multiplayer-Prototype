using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    //Network-synced health variable
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone, //The host/Client/Server can read this variable
        NetworkVariableWritePermission.Server //Only the server can change this value
    );
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth; //Initialize health on the server
        }

        currentHealth.OnValueChanged += OnHealthChanged; //Subscribe to health change events
    }

    public override void OnNetworkDespawn()
    {
        //Optional: Handle any cleanup when the object is despawned
        currentHealth.OnValueChanged -= OnHealthChanged; //Unsubscribe from health change events
    }

    public void OnHealthChanged(int previousValue, int newValue)
    {
        //This method will be called on all clients when the health changes
        Debug.Log($"Health changed from {previousValue} to {newValue}");
        if (newValue <= 0)
        {
            //Handle player death (e.g., respawn, disable controls, etc.)
            Debug.Log("Player has died!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            
            if (!IsServer) return; //Only the server should modify health
            currentHealth.Value -= damage; //Reduce health by damage amount
            currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);
            if (currentHealth.Value <= 0)
            {
                currentHealth.Value = 0; //Ensure health doesn't go below zero
                //Handle player death (e.g., respawn, disable controls, etc.)
                Debug.Log("Player has died!");
                Respawn(); //Respawn the player after death
            }
        }
    }

    public void Respawn()
    {
        currentHealth.Value = maxHealth; //Reset health to max
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int randomIndex = Random.Range(0, spawnPointObjects.Length);
        Transform selectedSpawnPoint = spawnPointObjects[randomIndex].transform;

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false; //Disable character controller before moving
        }

        transform.position = selectedSpawnPoint.position; //Move player to spawn point
        transform.rotation = selectedSpawnPoint.rotation; //Reset player rotation to spawn point

        if (characterController != null)
        {
            characterController.enabled = true; //Re-enable character controller after moving
        }
    }
}
