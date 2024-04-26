//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/Input System/PlayerInputAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputAction: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputAction"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""0d3f9b03-250f-4a8f-a9b1-1c1218ae8438"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""d439d295-318c-41c2-9d26-8e387015a053"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Speed"",
                    ""type"": ""Button"",
                    ""id"": ""00fac925-49f9-4afb-8077-b2c960351c5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotationCamera"",
                    ""type"": ""Value"",
                    ""id"": ""11286925-6258-40a4-8cd6-efe978e8d9e6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aim/Raise"",
                    ""type"": ""Button"",
                    ""id"": ""2428db2d-e2ce-474e-83d7-f5ef1a91f280"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot/Ponk"",
                    ""type"": ""Button"",
                    ""id"": ""8bf47980-7787-4b1d-898e-5b6b49e88076"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Unequip"",
                    ""type"": ""Button"",
                    ""id"": ""90f78485-8923-4007-8461-d49662a34134"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PutBomb"",
                    ""type"": ""Button"",
                    ""id"": ""4d58c3d6-2f51-49cf-bfd0-292635c50d24"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f6f6c52e-cc16-4234-a804-5697e987a340"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""0e5eb33c-3871-492e-9a5b-d698b733d3a8"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""20d83557-59cb-4ab5-a031-d5f1468ad2f6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""34bfaf39-3140-43b4-82ff-0d39a956d52e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""170153ab-006a-4008-b9ce-862c78310296"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""252636bc-62df-429c-b2d1-6363f91e19d2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""241671fe-b9a0-4788-8687-4e78468d14da"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""75abb1d7-c6e8-4d2a-8f21-4aa10c4da147"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Speed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d646b14b-691d-4bac-abba-421633567594"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotationCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3f13fbc2-fa5f-4e9e-835f-93d5c777ddbb"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false),ScaleVector2(x=7,y=3)"",
                    ""groups"": """",
                    ""action"": ""RotationCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8829371-b5ca-4d55-9658-2b2e38e5b938"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim/Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f90f7463-116c-4cb9-86ae-ecc583b0b712"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim/Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""36f024c7-f815-4191-8aa7-60a48164ffce"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot/Ponk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef95848f-1a18-4bd3-aff2-4fdd98a08b1e"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot/Ponk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28fd3638-c061-4438-b2f7-aba18fe5f3df"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unequip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""801f2e5f-e20a-4089-9315-f4ac3570c9b5"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unequip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49cd174a-a555-465d-8519-00b093d6f1f1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PutBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b28b9e8a-f592-448b-98f0-2a7623c31246"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PutBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Speed = m_Player.FindAction("Speed", throwIfNotFound: true);
        m_Player_RotationCamera = m_Player.FindAction("RotationCamera", throwIfNotFound: true);
        m_Player_AimRaise = m_Player.FindAction("Aim/Raise", throwIfNotFound: true);
        m_Player_ShootPonk = m_Player.FindAction("Shoot/Ponk", throwIfNotFound: true);
        m_Player_Unequip = m_Player.FindAction("Unequip", throwIfNotFound: true);
        m_Player_PutBomb = m_Player.FindAction("PutBomb", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Speed;
    private readonly InputAction m_Player_RotationCamera;
    private readonly InputAction m_Player_AimRaise;
    private readonly InputAction m_Player_ShootPonk;
    private readonly InputAction m_Player_Unequip;
    private readonly InputAction m_Player_PutBomb;
    public struct PlayerActions
    {
        private @PlayerInputAction m_Wrapper;
        public PlayerActions(@PlayerInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Speed => m_Wrapper.m_Player_Speed;
        public InputAction @RotationCamera => m_Wrapper.m_Player_RotationCamera;
        public InputAction @AimRaise => m_Wrapper.m_Player_AimRaise;
        public InputAction @ShootPonk => m_Wrapper.m_Player_ShootPonk;
        public InputAction @Unequip => m_Wrapper.m_Player_Unequip;
        public InputAction @PutBomb => m_Wrapper.m_Player_PutBomb;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Speed.started += instance.OnSpeed;
            @Speed.performed += instance.OnSpeed;
            @Speed.canceled += instance.OnSpeed;
            @RotationCamera.started += instance.OnRotationCamera;
            @RotationCamera.performed += instance.OnRotationCamera;
            @RotationCamera.canceled += instance.OnRotationCamera;
            @AimRaise.started += instance.OnAimRaise;
            @AimRaise.performed += instance.OnAimRaise;
            @AimRaise.canceled += instance.OnAimRaise;
            @ShootPonk.started += instance.OnShootPonk;
            @ShootPonk.performed += instance.OnShootPonk;
            @ShootPonk.canceled += instance.OnShootPonk;
            @Unequip.started += instance.OnUnequip;
            @Unequip.performed += instance.OnUnequip;
            @Unequip.canceled += instance.OnUnequip;
            @PutBomb.started += instance.OnPutBomb;
            @PutBomb.performed += instance.OnPutBomb;
            @PutBomb.canceled += instance.OnPutBomb;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Speed.started -= instance.OnSpeed;
            @Speed.performed -= instance.OnSpeed;
            @Speed.canceled -= instance.OnSpeed;
            @RotationCamera.started -= instance.OnRotationCamera;
            @RotationCamera.performed -= instance.OnRotationCamera;
            @RotationCamera.canceled -= instance.OnRotationCamera;
            @AimRaise.started -= instance.OnAimRaise;
            @AimRaise.performed -= instance.OnAimRaise;
            @AimRaise.canceled -= instance.OnAimRaise;
            @ShootPonk.started -= instance.OnShootPonk;
            @ShootPonk.performed -= instance.OnShootPonk;
            @ShootPonk.canceled -= instance.OnShootPonk;
            @Unequip.started -= instance.OnUnequip;
            @Unequip.performed -= instance.OnUnequip;
            @Unequip.canceled -= instance.OnUnequip;
            @PutBomb.started -= instance.OnPutBomb;
            @PutBomb.performed -= instance.OnPutBomb;
            @PutBomb.canceled -= instance.OnPutBomb;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSpeed(InputAction.CallbackContext context);
        void OnRotationCamera(InputAction.CallbackContext context);
        void OnAimRaise(InputAction.CallbackContext context);
        void OnShootPonk(InputAction.CallbackContext context);
        void OnUnequip(InputAction.CallbackContext context);
        void OnPutBomb(InputAction.CallbackContext context);
    }
}
