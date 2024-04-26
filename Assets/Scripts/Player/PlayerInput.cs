using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private PlayerInputAction playerInputAction;

    public PlayerInput(Role role)
    {
        if (role == Role.Client)
        {
            this.playerInputAction = new PlayerInputAction();
            this.playerInputAction.Enable();
        }
        else if (role == Role.Server)
        {
            this.playerInputAction = new PlayerInputAction();
        }
    }

    public Vector2 Move2DInput {  get; set; }
    public Vector3 Move3DInput {  get; set; }
    public bool SpeedInput {  get; set; }
    public Vector2 RotationCameraInput { get; set; }
    public bool AimRaiseInput { get; set; }
    public bool ShootPonkInput { get; set; }
    public bool UnequipInput { get; set; }
    public bool PutBombInput { get; set; }

    public void GetValueInput()
    {
        // MOVEMENT INPUT
        Move2DInput = playerInputAction.Player.Move.ReadValue<Vector2>();
        Move3DInput = new Vector3(Move2DInput.x, 0f, Move2DInput.y);

        // IS FAST INPUT
        SpeedInput = playerInputAction.Player.Speed.IsPressed();

        // MOUSE INPUT 
        RotationCameraInput = playerInputAction.Player.RotationCamera.ReadValue<Vector2>();

        // WEAPON INPUT
        AimRaiseInput = playerInputAction.Player.AimRaise.IsPressed(); // Explain: IsPress   == GetKey      (Allway press)
        ShootPonkInput = playerInputAction.Player.ShootPonk.triggered; //          triggered == GetKeyDown  (Just press once)
        UnequipInput = playerInputAction.Player.Unequip.triggered;

        // BOMB INPUT
        PutBombInput = playerInputAction.Player.PutBomb.triggered;
    }
}
