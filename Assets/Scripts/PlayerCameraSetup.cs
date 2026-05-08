using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine; // Changed from 'Cinemachine'

public class PlayerCameraSetup : NetworkBehaviour
{
    // In Cinemachine 3, the class is now just 'CinemachineCamera'
    private CinemachineCamera playerVirtualCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerVirtualCamera = GameObject.FindAnyObjectByType<CinemachineCamera>();

            if (playerVirtualCamera != null)
            {
                // 1. Re-assign targets
                playerVirtualCamera.Follow = transform;
                playerVirtualCamera.LookAt = transform;

                // 2. Force the Cinemachine Input to refresh
                // Sometimes Cinemachine 3 stays 'asleep' if the target changes mid-session
                var inputController = playerVirtualCamera.GetComponent<CinemachineInputAxisController>();
                if (inputController != null)
                {
                    inputController.enabled = false;
                    inputController.enabled = true; 
                }

                playerVirtualCamera.Priority = 10;
            }
        }
    }
}