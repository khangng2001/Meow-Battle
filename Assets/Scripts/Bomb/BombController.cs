using Interfaces;
using Unity.Netcode;
using UnityEngine;

public class BombController : NetworkBehaviour, ICollide
{
    private GameObject owner;
    private float explosionLength;

    [SerializeField] private float timeExist = 4f;
    private float timeExistAfterBoom = 2f;
    private bool canBoom = true;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private LayerMask layerMaskIndestructible;
    [SerializeField] private LayerMask layerMaskDestructible;

    private NetworkVariable<int> explosionLengthNetWork = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GetComponent<BombColliderDetectMelee>()?.MovementWhenDetectedByMelee(this);
        if (!canBoom) AsyncPositionClientRpc(transform.position);

        timeExist -= Time.deltaTime;
        if (timeExist <= 0 && CanBoom)
        {
            timeExist = 10f;

            HideBomb();

            HandleSpawnExplosion(transform.position);

            owner?.GetComponent<PlayerBomb>().ReturnBomb(1);
            Destroy(this.gameObject, timeExistAfterBoom);
        }
    }

    [ClientRpc]
    private void AsyncPositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    private void HideBomb()
    {
        // Hide Visual
        if (GetComponentInChildren<MeshRenderer>())
        {
            GameObject visual = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;
            visual.SetActive(false);
        }

        // Hide Collider
        GetComponent<CapsuleCollider>().enabled = false;

        // Async Client
        HideBombClientRpc();
    }
    [ClientRpc]
    private void HideBombClientRpc()
    {
        if (GetComponentInChildren<MeshRenderer>())
        {
            GameObject visual = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;
            visual.SetActive(false);
        }

        GetComponent<CapsuleCollider>().enabled = false;
        
        // AUDIO
        GetComponent<BombAudio>().TranslateExplosionSound();
    }

    // Use recursive (Tieng Viet: De Quy)
    private void SpawnExplosion(GameObject owner, GameObject root, Vector3 location, Vector3 directionCheck, float loopTimes)
    {
        GameObject explosionGO = Instantiate(explosionPrefab, location, Quaternion.identity);
        NetworkObject explosionNO = explosionGO.GetComponent<NetworkObject>();
        explosionNO.Spawn();

        ExplosionController explosionController = explosionGO.GetComponent<ExplosionController>() ?? explosionGO.AddComponent<ExplosionController>();
        explosionController.Owner = owner;
        explosionController.Root = root;

        if (loopTimes <= 0) return;
        if (!CheckIndestructible(location + Vector3.up, directionCheck)) return;
        if (CheckDestructible(location + Vector3.up, directionCheck)) SpawnExplosion(owner, root, location + directionCheck * 2, directionCheck, 0);
        else SpawnExplosion(owner, root, location + directionCheck * 2, directionCheck, loopTimes - 1);
    }

    private void HandleSpawnExplosion(Vector3 location)
    {
        // Spawn In Place
        SpawnExplosion(owner, this.gameObject, location, Vector3.zero, 0);

        if (explosionLength <= 0) return;

        // Check The Right Before Spawn
        // If The Ray Hit Indestructible => We Stop. If Inverse => We Continue Read The Line Below
        // If The Ray Hit Destructible => We Spawn One Time, Then We Stop. If Inverse => We Spawn With Recursive
        Vector3 diretionCheck = Vector3.right;
        if (CheckIndestructible(location + Vector3.up, diretionCheck))
        {
            if (CheckDestructible(location + Vector3.up, diretionCheck))    SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, 0);
            else SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, explosionLength - 1);
        }

        // Check The Left
        diretionCheck = Vector3.left;
        if (CheckIndestructible(location + Vector3.up, diretionCheck))
        {
            if (CheckDestructible(location + Vector3.up, diretionCheck)) SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, 0);
            else SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, explosionLength - 1);
        }

        // Check The Forward
        diretionCheck = Vector3.forward;
        if (CheckIndestructible(location + Vector3.up, diretionCheck))
        {
            if (CheckDestructible(location + Vector3.up, diretionCheck)) SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, 0);
            else SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, explosionLength - 1);
        }

        // Check The Back
        diretionCheck = Vector3.back;
        if (CheckIndestructible(location + Vector3.up, diretionCheck))
        {
            if (CheckDestructible(location + Vector3.up, diretionCheck)) SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, 0);
            else SpawnExplosion(owner, this.gameObject, location + diretionCheck * 2, diretionCheck, explosionLength - 1);
        }
    }

    // Example: Wall
    private bool CheckIndestructible(Vector3 startLocation,Vector3 direction)
    {
        //Debug.DrawRay(startLocation, direction * 2, Color.yellow, 2f);
        Ray ray = new Ray(startLocation, direction);
        bool result = Physics.Raycast(ray, out RaycastHit hit, 2f, layerMaskIndestructible);
        if (result)
        {
            return false;
        }

        return true;
    }

    // Example: Wood Box
    private bool CheckDestructible(Vector3 startLocation, Vector3 direction)
    { 
        Ray ray = new Ray(startLocation, direction);
        bool result = Physics.Raycast(ray, out RaycastHit hit, 2f, layerMaskDestructible);
        if (result)
        {
            return true;
        }

        return false;
    }

    // COLLIDE
    public void OnDestroyByBomb(GameObject owner)
    {
    }

    public void OnCollideByBullet(float timeFreeze, float pushForce, Vector3 directionPush)
    {
        if (timeFreeze > 0) timeExist += 5f;
        if (pushForce > 0) timeExist = 0f;
    }

    public void OnCollideByMelee(float timeStun, Vector3 pointHit, int strength)
    {
        if (timeStun <= 0) return;

        GetComponent<BombAudio>().BeatenSoundClientRpc();
        GetComponent<BombColliderDetectMelee>()?.OnCollideMelle(timeStun, pointHit, strength);
    }

    // GET - SET
    public GameObject Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }
    public float ExplosionLength
    {
        get
        {
            return explosionLength;
        }
        set
        {
            explosionLength = value;
        }
    }
    public int ExplosionLengthNetWork
    {
        get
        {
            return explosionLengthNetWork.Value;
        }
        set
        {
            explosionLengthNetWork.Value = value;
        }
    }
    public float TimeExist
    {
        get
        {
            return timeExist;
        }
    }
    public bool CanBoom
    {
        get
        {
            return canBoom;
        }
        set
        {
            canBoom = value;
        }
    }
}
