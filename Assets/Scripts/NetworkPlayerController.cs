using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float groundedGravity = -2f;
    [SerializeField] float lookSensitivity = 100f;

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f; // Stores vertical look for the camera

    private Camera mainCam;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main; // Cache the main camera reference
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);

        if (IsServer)
        {
            MovePlayer(inputDirection);
        }
        else
        {
            MovePlayerRPC(inputDirection);
        }
    }
    [Rpc(SendTo.Server)] // Marks the next method as an RPC that runs on the server.
    private void MovePlayerRPC(Vector2 movementInput)
    {
        MovePlayer(movementInput);
    }

    private void MovePlayer(Vector2 movementInput)
    {
        // 1. Snaps the player's rotation to the Camera's horizontal look
        if (Camera.main != null)
        {
            float cameraRotationY = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, cameraRotationY, 0);
        }

        // 2. Gravity logic
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 3. Move relative to this new rotation
        Vector3 moveDirection = (transform.forward * movementInput.y) + (transform.right * movementInput.x);
        if (moveDirection.magnitude > 1) moveDirection.Normalize();

        controller.Move((moveDirection * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }
}
