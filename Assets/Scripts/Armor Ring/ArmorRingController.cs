using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ArmorRingController : NetworkBehaviour
{
    // APPEAR
    [SerializeField] private float timeBeforeAppear = 8f; // 5 
    public float timecountDownBA = 0f;
    [SerializeField] private float posYBeforeAppear; // 0.9
    [SerializeField] private float posYAfterAppear; // 1.09

    // ARMOR
    [SerializeField] private GameObject armorPrefab;
    private GameObject armorGO;
    private NetworkObject armorNO;
    private ArmorController armorController;
    [SerializeField] private float posYArmorAppear;

    // EFFECT
    private ParticleSystem effectParticleSystem;

    // STATE
    public ArmorRingState armorRingState = ArmorRingState.UnderGround;

    // PLAYER
    public List<GameObject> playersInRing = new List<GameObject>();

    private void Awake()
    {
        effectParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        timecountDownBA = timeBeforeAppear;
        //transform.position = new Vector3(transform.position.x, posYBeforeAppear, transform.position.z);

    }

    private void Update()
    {
        UpdateInServer();

        UpdateInClient();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!other.GetComponent<PlayerArmor>()) return;

        playersInRing.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!other.GetComponent<PlayerArmor>()) return;

        playersInRing.Remove(other.gameObject);
    }

    // 

    private void UpdateInServer()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        switch (armorRingState)
        {
            case ArmorRingState.UnderGround:

                SwitchStateForClientRpc(ArmorRingState.UnderGround);

                if (transform.position.y > posYBeforeAppear) DigIntoGround();
                else
                {
                    WaitToGrowUp();
                }

                break;
            case ArmorRingState.OnGround:

                SwitchStateForClientRpc(ArmorRingState.OnGround);

                if (transform.position.y < posYAfterAppear) GrowUp();
                else
                {
                    SetUpArmor();
                }

                break;
            case ArmorRingState.WaitPlayerStayPlace:

                SwitchStateForClientRpc(ArmorRingState.WaitPlayerStayPlace);

                WaitPlayer();

                break;
            case ArmorRingState.CreateArmor:

                SwitchStateForClientRpc(ArmorRingState.CreateArmor);

                CreateArmor();

                break;
        }
    }

    private void UpdateInClient()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        switch (armorRingState)
        {
            case ArmorRingState.UnderGround:

                if (transform.position.y > posYBeforeAppear) DigIntoGroundClient();

                break;
            case ArmorRingState.OnGround:

                if (transform.position.y < posYAfterAppear) GrowUpClient();

                break;
            case ArmorRingState.WaitPlayerStayPlace:

                WaitPlayerClient();

                break;
            case ArmorRingState.CreateArmor:

                CreateArmorClient();

                break;
        }
    }

    [ClientRpc]
    private void SwitchStateForClientRpc(ArmorRingState armorRingState)
    {
        this.armorRingState = armorRingState;
    }

    //
    private void DigIntoGround()
    {
        transform.Translate(Vector3.down * Time.deltaTime * 0.2f);
        if (!effectParticleSystem.isStopped) effectParticleSystem.Stop();
        if(GetComponent<BoxCollider>().enabled) GetComponent<BoxCollider>().enabled = false;
    }
    private void DigIntoGroundClient()
    {
        transform.Translate(Vector3.down * Time.deltaTime * 0.2f);
        if (!effectParticleSystem.isStopped) effectParticleSystem.Stop();
    }

    private void WaitToGrowUp()
    {
        if (timecountDownBA > 0) timecountDownBA -= Time.deltaTime;
        else
        {
            timecountDownBA = timeBeforeAppear;
            armorRingState = ArmorRingState.OnGround;
        }
    }

    //
    private void GrowUp()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 0.2f);
    }
    private void GrowUpClient()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 0.2f);
        if (!effectParticleSystem.isStopped) effectParticleSystem.Stop();
    }

    private void SetUpArmor()
    {
        armorGO = Instantiate(armorPrefab, new Vector3(transform.position.x, posYArmorAppear, transform.position.z), Quaternion.identity);

        armorNO = armorGO.GetComponent<NetworkObject>();
        armorNO.Spawn();

        armorController = armorNO.GetComponent<ArmorController>();

        if (!GetComponent<BoxCollider>().enabled) GetComponent<BoxCollider>().enabled = true;

        armorRingState = ArmorRingState.WaitPlayerStayPlace;
    }

    //
    private void WaitPlayer()
    {
        if (!effectParticleSystem.isStopped) effectParticleSystem.Stop();

        if (playersInRing.Count > 0) armorRingState = ArmorRingState.CreateArmor;

        if (armorGO.transform.localScale.x > 0f
            && armorGO.transform.localScale.y> 0f
            && armorGO.transform.localScale.z > 0f)
        {
            armorGO.transform.localScale -= Vector3.one * Time.deltaTime * 0.5f;
        };
    }
    private void WaitPlayerClient()
    {
        if (!effectParticleSystem.isStopped) effectParticleSystem.Stop();
    }

    //
    private void CreateArmor()
    {
        if (!effectParticleSystem.isPlaying) effectParticleSystem.Play();

        if (playersInRing.Count <= 0) armorRingState = ArmorRingState.WaitPlayerStayPlace;

        if (armorGO.transform.localScale.x >= 1f
            && armorGO.transform.localScale.y >= 1f
            && armorGO.transform.localScale.z >= 1f)
        {
            TranslateToPlayer();

            armorRingState = ArmorRingState.UnderGround;
            return;
        }

        armorGO.transform.localScale += Vector3.one * Time.deltaTime * 0.2f;
    }
    private void CreateArmorClient()
    {
        if (!effectParticleSystem.isPlaying) effectParticleSystem.Play();
    }

    private void TranslateToPlayer()
    {
        if (playersInRing.Count <= 0)
        {
            Destroy(armorGO);
        }
        else
        {
            GameObject player;
            player = playersInRing[Random.Range(0, playersInRing.Count - 1)];

            if (player.GetComponent<PlayerArmor>().IsHaveArmor)
            {
                playersInRing.Remove(player);

                TranslateToPlayer();
                return;
            }

            if (player == null) Destroy(armorNO);
            else
            {
                player.GetComponent<PlayerArmor>().IsHaveArmor = true;
                player.GetComponent<PlayerArmor>().Armor = armorGO;
                armorController.Owner = player;
                armorNO.TrySetParent(player);
                armorNO.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
        }

        playersInRing.Clear();
        armorGO = null;
        armorNO = null;
        armorController = null;
    }

    //[SerializeField] private float timeBeforeSpawn = 8f;
    //public float timeContDown;
    //private bool isPicked = true;

    //[SerializeField] private GameObject armorPrefab;
    //private GameObject armorGO;
    //private NetworkObject armorNO;

    //private ParticleSystem effectSpawn;

    //private void Start()
    //{
    //    // FIRST SET UP 

    //    timeContDown = timeBeforeSpawn;

    //    effectSpawn = GetComponentInChildren<ParticleSystem>();
    //    effectSpawn?.Stop();

    //    GetComponent<BoxCollider>().enabled = false;

    //    //
    //}

    //private void Update()
    //{
    //    if (!NetworkManager.Singleton.IsServer) return;
    //    if (!isWorking) return;


    //    if (timeContDown > 0 && isPicked)
    //    {
    //        timeContDown -= Time.deltaTime;

    //    }
    //    else if (timeContDown <= 0)
    //    {
    //        timeContDown = timeBeforeSpawn;

    //        SetUpArmor();
    //    }

    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!NetworkManager.Singleton.IsServer) return;

    //    if (other.gameObject.GetComponent<PlayerControllerServer>())
    //    {
    //        PickedArmor(other.gameObject);
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!NetworkManager.Singleton.IsServer) return;

    //    if (other.gameObject.GetComponent<PlayerControllerServer>())
    //    {
    //        PickedArmor(other.gameObject);
    //    }
    //}

    ////

    //#region WHEN ARMOR APPEAR
    //private void SetUpArmor()
    //{
    //    // UI And Colider
    //    isPicked = false;
    //    effectSpawn?.Play();

    //    SetUpArmorClientRpc();

    //    // Spawn
    //    armorGO = Instantiate(armorPrefab, new Vector3(transform.position.x, 2.5f, transform.position.z), Quaternion.identity);

    //    armorNO = armorGO.GetComponent<NetworkObject>();
    //    armorNO.Spawn();

    //    // Wait Animation Of Armor (Appear)
    //    armorNO.GetComponent<ArmorController>().OnReadyArmor = ReadyArmor;
    //}
    //[ClientRpc]
    //private void SetUpArmorClientRpc()
    //{
    //    // UI
    //    effectSpawn?.Play();
    //}

    //private void ReadyArmor()
    //{
    //    armorNO.TrySetParent(this.gameObject);
    //    GetComponent<BoxCollider>().enabled = true;
    //}
    //#endregion

    //#region WHEN PICKED ARMOR
    //private void PickedArmor(GameObject player)
    //{
    //    // UI And Collider
    //    isPicked = true;
    //    GetComponent<BoxCollider>().enabled = false;
    //    effectSpawn?.Stop();

    //    PickedArmorrClientRpc();

    //    // Translate To Player
    //    armorNO.TrySetParent(player);

    //}
    //[ClientRpc]
    //private void PickedArmorrClientRpc()
    //{
    //    // UI
    //    effectSpawn?.Stop();
    //}
    //#endregion
}