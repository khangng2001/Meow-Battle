using Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

public class ItemLongRangeWeapon : NetworkBehaviour, IItem
{
    // Objects
    private ParticleSystem particle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    
    // Detail
    private GameObject owner;
    [SerializeField] private LongRangeWeaponSO longRangeWeaponDetail;
    [SerializeField] private float numberOfUse;

    Transform handPose;
    Transform worldPose;

    ItemFloating itemFloating;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Initialize();
        InitializedClientRpc();
    }

    public void Initialize()
    {
        // Call
        particle = GetComponentInChildren<ParticleSystem>();
        handPose = GetComponentInChildren<FindHand>().transform;
        worldPose = GetComponentInChildren<FindHand>().transform.GetChild(0).transform;
        itemFloating = GetComponentInChildren<ItemFloating>();

        // First Implement
        ParticleSystemTrigger(true);
        transform.position = new Vector3(transform.position.x, worldPose.localPosition.y, transform.position.z);
        itemFloating.Floating(true);
    }
    [ClientRpc]
    private void InitializedClientRpc()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        handPose = GetComponentInChildren<FindHand>().transform;
        worldPose = GetComponentInChildren<FindHand>().transform.GetChild(0).transform;
        itemFloating = GetComponentInChildren<ItemFloating>();

        ParticleSystemTrigger(true);
        itemFloating.Floating(true);
        transform.position = new Vector3(transform.position.x, worldPose.localPosition.y, transform.position.z);
    }

    private void ParticleSystemTrigger(bool turn)
    {
        if (turn)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
        else particle.gameObject.SetActive(false);
    }

    public void Bang(Vector3 target)
    {
        BangClientRpc(target);

        // Find Point Spawn In Server
        Transform playerTranform = transform.parent.transform;
        Vector3 shootPointPosition = playerTranform.position + Vector3.up * 0.743f + playerTranform.forward * 1f;

        GameObject GO = Instantiate(bulletPrefab, shootPointPosition, playerTranform.rotation);
        GO.transform.forward = target - shootPointPosition;

        BulletControllerServer bulletController = GO.GetComponent<BulletControllerServer>() ?? GO.AddComponent<BulletControllerServer>();
        bulletController.LongRangeWeaponDetail = longRangeWeaponDetail;

        if (GO.GetComponent<BulletControllerClient>())
        {
            GO.GetComponent<BulletControllerClient>().enabled = false;
        }

    }
    [ClientRpc]
    private void BangClientRpc(Vector3 target)
    {
        // AUDIO
        GetComponent<LongRangeWeaponAudio>().Shoot();
        
        GameObject GO = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        BulletControllerClient bulletController = GO.GetComponent<BulletControllerClient>() ?? GO.AddComponent<BulletControllerClient>();
        GO.transform.forward = target - shootPoint.position;

        if (GO.GetComponent<BulletControllerServer>())
        {
            GO.GetComponent<BulletControllerServer>().enabled = false;
        }
    }


    private void Equip(PlayerItem ownerPlayerItem)
    {
        // Myself
        owner = ownerPlayerItem.gameObject;
        ParticleSystemTrigger(false);
        itemFloating.Floating(false);
        GetComponent<SphereCollider>().enabled = false;

        // Send Number Of Use To Owner And Kind Of Weapon
        ownerPlayerItem.UseLongRangeWeapon(numberOfUse);

        // Find Hand In Player And Set Rotation
        Transform hand = ownerPlayerItem.GetComponentInChildren<FindHand>().transform;
        hand.localRotation = handPose.localRotation;

        // Set ParentConstraint Follow
        GetComponent<ParentConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = hand, weight = 1 });
        GetComponent<ParentConstraint>().constraintActive = true;

        // Set Parent Of Item
        transform.parent = ownerPlayerItem.transform;
    }
    [ClientRpc]
    private void BehaviourEquipClientRpc()
    {
        // Myself
        owner = GetComponentInParent<PlayerItem>().gameObject;
        ParticleSystemTrigger(false);
        itemFloating.Floating(false);
        GetComponent<SphereCollider>().enabled = false;

        // Find Hand In Player And Set Rotation
        GameObject gameObjectowner = transform.parent.gameObject;
        Transform hand = gameObjectowner.GetComponentInChildren<FindHand>().transform;
        hand.localRotation = handPose.localRotation;

        // Set ParentConstraint Follow
        GetComponent<ParentConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = hand, weight = 1 });
        GetComponent<ParentConstraint>().constraintActive = true;
    }

    public void Unequip(Vector3 location, float numberOfUse)
    {
        if (numberOfUse <= 0)
        {
            Destroy(this.gameObject);
            return;
        }
        this.numberOfUse = numberOfUse;

        // Set Up Weapon To World
        GetComponent<ParentConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = null, weight = 1 });
        GetComponent<ParentConstraint>().constraintActive = false;
        transform.parent = null;
        transform.position = new Vector3(location.x, worldPose.localPosition.y, location.z);
        transform.rotation = Quaternion.identity;

        // Myself
        owner = null;
        ParticleSystemTrigger(true);
        itemFloating.Floating(true);
        GetComponent<SphereCollider>().enabled = true;

        BehaviourUnequipClientRpc(location);
    }
    [ClientRpc]
    public void BehaviourUnequipClientRpc(Vector3 location)
    {
        GetComponent<ParentConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = null, weight = 1 });
        GetComponent<ParentConstraint>().constraintActive = false;
        transform.parent = null;
        transform.position = new Vector3(location.x, worldPose.localPosition.y, location.z);
        transform.rotation = Quaternion.identity;

        ParticleSystemTrigger(true);
        itemFloating.Floating(true);
        GetComponent<SphereCollider>().enabled = true;
    }

    // When player touch item
    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.GetComponent<PlayerItem>() && other.GetComponent<PlayerItem>().CanReceiveWeapon())
        {
            Equip(other.GetComponent<PlayerItem>());
            BehaviourEquipClientRpc();
        }
    }   

    private void OnTriggerStay(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.GetComponent<PlayerItem>() && other.GetComponent<PlayerItem>().CanReceiveWeapon())
        {
            Equip(other.GetComponent<PlayerItem>());
            BehaviourEquipClientRpc();
        }
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
    public Transform PointShoot { get { return transform; } }
}
