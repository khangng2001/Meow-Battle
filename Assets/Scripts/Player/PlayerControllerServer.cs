using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControllerServer : NetworkBehaviour, ICollide
{
    private CharacterController characterController;
    private Animator animator;
    public PlayerInput playerInput;
    private PlayerControllerClient playerControllerClient;
    private PlayerMovement playerMovement;
    private PlayerBomb playerBomb;
    private PlayerWeapon playerWeapon;
    private PlayerItem playerItem;
    private PlayerEffect playerEffect;
    private PlayerArmor playerArmor;
    private PlayerDataServer playerDataServer;
    private PlayerAudio playerAudio;
    private PlayerNotify playerNotify;

    // =================== Infomation Of Player =============================

    private PlayerState playerState = PlayerState.ReadyGame;
    private bool isFreeze = false;
    private bool isImmortal = false;

    private Vector3 locationBorn = Vector3.zero;

    //=========================================================================

    // ======================== GET - SET =====================================

    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }
        set
        {
            playerState = value;
        }
    }

    public Vector3 LocationBorn
    {
        get
        {
            // Start Corotine
            if (corotineImmortalTime != null) StopCoroutine(corotineImmortalTime);
            corotineImmortalTime = ImmortalTime(2f);
            StartCoroutine(corotineImmortalTime);

            return locationBorn;
        }
        set
        {
            locationBorn = value;
        }
    }
        
    // ========================================================================

    // ====================== IMPLEMENT FUNCTION ==============================

    private void Awake()
    {
        playerControllerClient = GetComponent<PlayerControllerClient>();

        if (!NetworkManager.Singleton.IsServer) return;

        playerInput = new PlayerInput(Role.Server);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerBomb = GetComponent<PlayerBomb>();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerItem = GetComponent<PlayerItem>();
        playerEffect = GetComponent<PlayerEffect>();
        playerArmor = GetComponent<PlayerArmor>();
        playerDataServer = GetComponent<PlayerDataServer>();
        playerAudio = GetComponent<PlayerAudio>();
        playerNotify = GetComponent<PlayerNotify>();
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        playerBomb?.CountingBombQuantity(playerBomb.BombCount);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        switch (playerState)
        {
            case PlayerState.ReadyGame:

                ReadyGameActivites(playerInput);

                playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.None);

                break;
            case PlayerState.Normal:

                HandleWeapon(playerInput, playerWeapon, animator);

                if (playerBomb != null) HandleBomb(playerInput, playerBomb.PutBomb, playerAudio);

                break;
            case PlayerState.Stun:

                ConsequencesOfStun(playerInput);
                
                playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.None);

                break;
            case PlayerState.Death:

                ConsequencesOfDeath(playerInput);

                playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.None);

                break;
            case PlayerState.EndGame:

                EndGameActivities(playerInput);

                playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.None);

                break;
        }
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        switch (playerState)
        {
            case PlayerState.ReadyGame:

                break;
            case PlayerState.Normal:

                HandleMovement(playerInput, playerMovement, playerWeapon, animator, playerAudio);

                break;
            case PlayerState.Stun:

                break;
            case PlayerState.Death:

                break;
            case PlayerState.EndGame:

                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!other.GetComponent<BombController>()) return;

        Vector3 direction = transform.position - other.transform.position;
        float force = 7f;

        playerMovement.PushByCharacterController(characterController, direction, force);
    }

    // ==================================================================

    // ====================== DECLARE FUNCTION ==============================

    // Calculator Direction Camera To Player
    public Vector3 PositionOfCamera { get; set; }
    private Vector3 CalculatorDirectionMovementWithPositionCamera(Vector3 positionOfCamera)
    {
        Vector3 cameraDir = transform.position - positionOfCamera;

        return cameraDir;
    }

    // Handle Movement
    private void HandleMovement(PlayerInput playerInput, PlayerMovement playerMovement, PlayerWeapon playerWeapon, Animator animator, PlayerAudio playerAudio)
    {
        bool isFast = playerInput.SpeedInput;
        float normalSpeed = playerMovement.NormalSpeed;
        float fastSpeed = playerMovement.FastSpeed;

        // Every Case Of Movement
        if (!CheckCanRun(playerWeapon))
        {
            isFast = false;
        }

        if (isFreeze)
        {
            fastSpeed = 1.5f;
            normalSpeed = 1.0f;
        }
        //

        if (isFast)
        {
            playerMovement?.MovementByCharaterController(characterController, playerInput.Move3DInput,
                     CalculatorDirectionMovementWithPositionCamera(PositionOfCamera), fastSpeed, playerMovement.SpeedRotate);
        }
        else
        {
            playerMovement?.MovementByCharaterController(characterController, playerInput.Move3DInput,
                    CalculatorDirectionMovementWithPositionCamera(PositionOfCamera), normalSpeed, playerMovement.SpeedRotate);
        }

        animator.SetFloat("IsMove", playerInput.Move3DInput.magnitude);
        animator.SetBool("IsRun", isFast);

        // AUDIO
        if (playerInput.Move3DInput.magnitude > 0)
        {
            if (isFast)
            {
                // fast
                playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.Run);
                return;
            }

            // normal
            playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.Walk);
        }
        else
        {
            playerAudio.MainTainSoundClientRpc(PlayerAudio.MoveType.None);
        }
    }

    // Every Case Cannot Run => Just Write Here => PLayer Will Only Walk
    private bool CheckCanRun(PlayerWeapon playerWeapon)
    {
        if (playerWeapon.UsingWeaponState != UsingWeaponState.NotUsing) return false;

        return true;
    }

    // Handle Bomb
    private void HandleBomb(PlayerInput playerInput, Action<GameObject, Vector3, float, PlayerAudio> OnPutBomb, PlayerAudio playerAudio)
    {
        if (playerInput.PutBombInput)
        {
            playerInput.PutBombInput = false;
            OnPutBomb?.Invoke(this.gameObject, transform.position, playerBomb.BombLength, playerAudio);
        }
    }

    // Handle Weapon
    private void HandleWeapon(PlayerInput playerInput, PlayerWeapon playerWeapon, Animator animator)
    {
        switch (playerWeapon.HavingWeaponState)
        {
            case HavingWeaponState.NotHave:
                animator.SetBool("IsHaveWeapon", false);
                animator.SetBool("IsRaise", false);
                animator.SetBool("IsAim", false);
                playerInput.UnequipInput = false;
                playerInput.AimRaiseInput = false;
                playerInput.ShootPonkInput = false;
                break;

            case HavingWeaponState.HaveMeleeWeapon:
                // Use
                animator.SetBool("IsHaveWeapon", true);
                if (playerInput.AimRaiseInput && playerWeapon.UsingWeaponState != UsingWeaponState.Using)
                {
                    playerWeapon.UsingWeaponState = UsingWeaponState.ReadyUsing;

                    if (playerInput.ShootPonkInput)
                    {
                        animator.SetTrigger("IsMelee");
                    }
                }
                else if (!playerInput.AimRaiseInput && playerWeapon.UsingWeaponState != UsingWeaponState.Using)
                {
                    playerWeapon.UsingWeaponState = UsingWeaponState.NotUsing;
                    animator.ResetTrigger("IsMelee");
                }

                playerInput.ShootPonkInput = false;
                animator.SetBool("IsRaise", playerInput.AimRaiseInput);

                // Unequip
                if (playerInput.UnequipInput && playerWeapon.UsingWeaponState == UsingWeaponState.NotUsing)
                {
                    playerItem.Unequip(playerWeapon.NumberOfUse);
                }
                playerInput.UnequipInput = false;
                break;

            case HavingWeaponState.HaveLongRangeWeapon:
                // Use
                animator.SetBool("IsHaveWeapon", true);
                if (playerInput.AimRaiseInput && playerWeapon.UsingWeaponState != UsingWeaponState.Using)
                {
                    playerWeapon.UsingWeaponState = UsingWeaponState.ReadyUsing;

                    if (playerInput.ShootPonkInput)
                    {
                        animator.SetTrigger("IsLongRange");
                    }
                }
                else if (!playerInput.AimRaiseInput && playerWeapon.UsingWeaponState != UsingWeaponState.Using)
                {
                    playerWeapon.UsingWeaponState = UsingWeaponState.NotUsing;
                    animator.ResetTrigger("IsLongRange");
                }

                playerInput.ShootPonkInput = false;
                animator.SetBool("IsAim", playerInput.AimRaiseInput);

                // Unequip
                if (playerInput.UnequipInput && playerWeapon.UsingWeaponState == UsingWeaponState.NotUsing)
                {
                    playerItem.Unequip(playerWeapon.NumberOfUse);
                }
                playerInput.UnequipInput = false;
                break;
        }
    }
    
    // When Stun
    private void ConsequencesOfStun(PlayerInput playerInput)
    {
        playerInput.Move3DInput = Vector3.zero;
        playerInput.AimRaiseInput = false;
        playerInput.ShootPonkInput = false;
        playerInput.UnequipInput = false;
        playerInput.PutBombInput = false;
        playerInput.SpeedInput = false;
    }
    
    private void ConsequencesOfDeath(PlayerInput playerInput)
    {
        // Stop Detect Input
        playerInput.Move3DInput = Vector3.zero;
        playerInput.AimRaiseInput = false;
        playerInput.ShootPonkInput = false;
        playerInput.UnequipInput = false;
        playerInput.PutBombInput = false;
        playerInput.SpeedInput = false;
    }

    // When ReadyGame
    private void ReadyGameActivites(PlayerInput playerInput)
    {
        // Stop Detect Input
        playerInput.Move3DInput = Vector3.zero;
        playerInput.AimRaiseInput = false;
        playerInput.ShootPonkInput = false;
        playerInput.UnequipInput = false;
        playerInput.PutBombInput = false;
        playerInput.SpeedInput = false;
    }

    // When EndGame
    private void EndGameActivities(PlayerInput playerInput)
    {
        // Stop Detect Input
        playerInput.Move3DInput = Vector3.zero;
        playerInput.AimRaiseInput = false;
        playerInput.ShootPonkInput = false;
        playerInput.UnequipInput = false;
        playerInput.PutBombInput = false;
        playerInput.SpeedInput = false;

        // Immortal And Stop Countdown Immortal
        isImmortal = true;
        if (corotineImmortalTime != null) StopCoroutine(corotineImmortalTime);
    }

    // ==================================================================

    // ======================== IMPLEMENT INTERFACE =====================

    public void OnDestroyByBomb(GameObject owner)
    {
        if (isImmortal) return;

        isImmortal = true;
        playerEffect.TurnOnEFfectUI(EffectUI.Boom);

        if (playerArmor.IsHaveArmor)
        {
            // Start Corotine
            if (corotineImmortalTime != null) StopCoroutine(corotineImmortalTime);
            corotineImmortalTime = ImmortalTime(2f);
            StartCoroutine(corotineImmortalTime);
            
            // Destroy Armor
            StartCoroutine(playerArmor.ReadyDestroyArmor());

            // AUDIO DESTROY ARMOR
            playerAudio.ShieldBrokenSoundClientRpc();
            return;
        }

        // Score
        // FOR YOU CAUSE YOU CATCH BOMB
        playerDataServer.UpdateScorePlayer(playerDataServer.PlayerID, 0, 1, 0);
        // FOR KILLER
        if (owner != this.gameObject)
        {
            playerDataServer.UpdateScorePlayer(owner.GetComponent<PlayerDataServer>().PlayerID, 1, 0, 0);

            // NOTIFY FOR KILLER
            playerNotify.Notify(owner.GetComponent<PlayerDataServer>().LocalClientID);

            playerNotify.NotifyAllClientRpc(owner.GetComponent<PlayerDataServer>().Name, this.GetComponent<PlayerDataServer>().Name);
        }
        else
        {
            playerNotify.NotifyAllSuicidalClientRpc(this.GetComponent<PlayerDataServer>().Name);
        }

        // Start Corotine
        if (corotineImmortalTime != null) StopCoroutine(corotineImmortalTime);
        corotineImmortalTime = ImmortalTime(4f);
        StartCoroutine(corotineImmortalTime);
        
        // Player Die
        playerState = PlayerState.Death;
        animator.SetBool("IsDeath", true);

        // AUDIO DIE
        playerAudio.DieSoundClientRpc();
    }

    public void OnCollideByBullet(float timeFreeze, float pushForce, Vector3 directionPush)
    {
        if (timeFreeze > 0)
        {
            isFreeze = true;
            playerEffect.TurnOnEFfectUI(EffectUI.Freeze);

            // Start Corotine
            if (corotineFreezeTime != null) StopCoroutine(corotineFreezeTime);
            corotineFreezeTime = FreezeTime(timeFreeze);
            StartCoroutine(corotineFreezeTime);
        }

        if (pushForce > 0)
        {
            // Destroy Armor
            if (playerArmor.IsHaveArmor)
            {
                // Destroy Armor
                StartCoroutine(playerArmor.ReadyDestroyArmor());
                return;
            }
        }
    }

    public void OnCollideByMelee(float timeStun, Vector3 pointHit, int strength)
    {
        if (timeStun > 0)
        {
            playerState = PlayerState.Stun;
            animator.SetTrigger("IsBeated");
            animator.SetBool("IsStun", true);

            // AUDIO
            playerAudio.BeatenSoundClientRpc();
            playerAudio.DazeSoundClientRpc();

            // Start Corotine
            if (corotineStunTime != null) StopCoroutine(corotineStunTime);
            corotineStunTime = StunTime(timeStun);
            StartCoroutine(corotineStunTime);
        }
    }

    // ==================================================================

    // ======================== COROTINE ================================

    IEnumerator corotineStunTime;
    IEnumerator StunTime(float time)
    {
        yield return new WaitForSeconds(time);
        playerState = PlayerState.Normal;
        animator.SetBool("IsStun", false);
    }

    IEnumerator corotineFreezeTime;
    IEnumerator FreezeTime(float time)
    {
        yield return new WaitForSeconds(time);
        isFreeze = false;
        playerEffect.TurnOffEFfectUI(EffectUI.Freeze);
    }

    IEnumerator corotineImmortalTime;
    IEnumerator ImmortalTime(float time)
    {
        yield return new WaitForSeconds(time);
        isImmortal = false;
        playerEffect.TurnOffEFfectUI(EffectUI.Boom);
    }

    IEnumerator corotineDetectBoomTime;
    IEnumerator DetectBoomTime(float time)
    {
        yield return new WaitForSeconds(time);

    }
    // ==================================================================

}
