using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControllerClient : NetworkBehaviour
{
    private PlayerInput playerInput;
    private PlayerCamera playerCamera;
    private PlayerControllerServer playerControllerServer;

    // ====================== IMPLEMENT FUNCTION ==============================

    private void Awake()
    {
        playerControllerServer = GetComponent<PlayerControllerServer>();

        if (!NetworkManager.Singleton.IsClient) return;

        playerInput = new PlayerInput(Role.Client);
        playerCamera = GetComponent<PlayerCamera>();
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsClient) return;

    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        if (IsOwner)
        {
            playerInput.GetValueInput();

            playerCamera.Rotate(playerInput.RotationCameraInput, this.gameObject);

            SendMove3DInput(playerInput.Move3DInput, playerCamera.GetPosition());
            SendSpeedInput(playerInput.SpeedInput);
            SendPutBombInput(playerInput.PutBombInput);
            SendAimRaiseInput(playerInput.AimRaiseInput);
            SendShootPonkInput(playerInput.ShootPonkInput);
            SendUnequipInput(playerInput.UnequipInput);
        }
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.Singleton.IsClient) return;

    }

    // ==================================================================

    // ====================== DECLARE FUNCTION ==============================

    // Send Move3DInput And Position Camera
    private bool isSendLastMove3DInput = false;
    private void SendMove3DInput(Vector3 move3DInput, Vector3 positionCamera)
    {
        if (move3DInput.magnitude > 0)
        {
            isSendLastMove3DInput = false;
            SendMove3DInputServerRpc(move3DInput, positionCamera);
        }
        else if (move3DInput.magnitude <= 0 && !isSendLastMove3DInput)
        {
            isSendLastMove3DInput = true;
            SendMove3DInputServerRpc(move3DInput, positionCamera);
        }
    }
    [ServerRpc]
    private void SendMove3DInputServerRpc(Vector3 move3DInput, Vector3 positionCamera)
    {
        playerControllerServer.playerInput.Move3DInput = move3DInput;
        playerControllerServer.PositionOfCamera = positionCamera;
    }

    // Send SpeedInput
    private bool isSendLastSpeedInput = false;
    private void SendSpeedInput(bool speedInput)
    {
        if (speedInput)
        {
            isSendLastSpeedInput = false;
            SendSpeedInputServerRpc(speedInput);
        }
        else if (!speedInput && !isSendLastSpeedInput)
        {
            isSendLastSpeedInput = true;
            SendSpeedInputServerRpc(speedInput);
        }
    }
    [ServerRpc]
    private void SendSpeedInputServerRpc(bool speedInput)
    {
        playerControllerServer.playerInput.SpeedInput = speedInput;
    }

    // Send PutBombInput
    private void SendPutBombInput(bool putBombInput)
    {
        if (putBombInput)
        {
            SendPutBombInputServerRpc(putBombInput);
        }
    }
    [ServerRpc]
    private void SendPutBombInputServerRpc(bool putBombInput)
    {
        playerControllerServer.playerInput.PutBombInput = putBombInput;
    }

    // Send AimRaiseInput
    private bool isSendLastAimRaiseInput = false;
    private void SendAimRaiseInput(bool aimRaiseInput)
    {
        if (playerInput.AimRaiseInput)
        {
            isSendLastAimRaiseInput = false;
            SendAimRaiseInputServerRpc(aimRaiseInput);
        }
        else if (!playerInput.AimRaiseInput && !isSendLastAimRaiseInput)
        {
            isSendLastAimRaiseInput= true;
            SendAimRaiseInputServerRpc(aimRaiseInput);
        }
    }
    [ServerRpc]
    private void SendAimRaiseInputServerRpc(bool AimRaiseInput)
    {
        playerControllerServer.playerInput.AimRaiseInput = AimRaiseInput;
    }

    // Send ShootPonkInput
    private void SendShootPonkInput(bool shootPonkInput)
    {
        if (shootPonkInput)
        {
            SendShootPonkInputServerRpc(shootPonkInput);
        }
    }
    [ServerRpc]
    private void SendShootPonkInputServerRpc(bool shootPonkInput)
    {
        playerControllerServer.playerInput.ShootPonkInput = shootPonkInput;
    }

    // Send UnequipInput
    private void SendUnequipInput(bool unequipInput)
    {
        if (unequipInput)
        {
            SendUnequipInputServerRpc(unequipInput);
        }
    }
    [ServerRpc]
    private void SendUnequipInputServerRpc(bool unequipInput)
    {
        playerControllerServer.playerInput.UnequipInput = unequipInput;
    }

    
    
    // Debug Log
    private void DebugPlayerInput()
    {
        Debug.Log(playerControllerServer.playerInput.Move3DInput);
    }

    private void DebugLog()
    {
        Debug.Log("Server");
    }

    // ==================================================================
}
