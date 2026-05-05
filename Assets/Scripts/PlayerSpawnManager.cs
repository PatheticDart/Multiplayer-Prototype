using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnManager : NetworkBehaviour
{
    private static int nextSpawnIndex;
    //Runs when the player obj is spawned by netcode
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            return;
        }
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPointObjects.Length == 0)
        {
            Debug.LogWarning("No spawn points found");
            return;
        }
        Transform selectedSpawnPoint = spawnPointObjects[nextSpawnIndex].transform;
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }
        transform.position = selectedSpawnPoint.position;
        transform.rotation = selectedSpawnPoint.rotation;
        if (characterController != null)
        {
            characterController.enabled = enabled;
        }
        nextSpawnIndex++;
        if (nextSpawnIndex >= spawnPointObjects.Length)
        {
            nextSpawnIndex = 0;
        }
    }
}