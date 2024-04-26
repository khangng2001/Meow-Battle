using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class WarningZoneController : MonoBehaviour
{
    [SerializeField] private LayerMask WarningZoneLayer;
    [SerializeField] private LayerMask DestructiblesLayer;
    [SerializeField] private LayerMask BombLayer;
    private MeshRenderer visual;
    [SerializeField] private List<GameObject> bombRoots = new List<GameObject>();

    void Start()
    {
        visual = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if (bombRoots.Count > 0) TurnOn();
        else TurnOff();
    }

    private void FixedUpdate()
    {
        if (bombRoots.Count > 0)
        {
            // Check And Remove GameObject Missing
            List<GameObject> roots = new List<GameObject>();
            foreach (GameObject root in bombRoots)
            {
                if (root == null) roots.Add(root);
            }
            foreach (GameObject root in roots)
            {
                bombRoots.Remove(root);
            }
        }
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            BombRoots.Add(other.gameObject);

            // Ray
            float length = (other.gameObject.GetComponent<BombController>()) ? other.gameObject.GetComponent<BombController>().ExplosionLengthNetWork : 1f;
            while (length <= 0f)
            {
                await Task.Delay(100);
                length = (other.gameObject.GetComponent<BombController>()) ? other.gameObject.GetComponent<BombController>().ExplosionLengthNetWork : 1f;
            }

            RayWithDirectionToAdd(transform.position, Vector3.forward, length, other.gameObject);
            RayWithDirectionToAdd(transform.position, Vector3.back, length, other.gameObject);
            RayWithDirectionToAdd(transform.position, Vector3.right, length, other.gameObject);
            RayWithDirectionToAdd(transform.position, Vector3.left, length, other.gameObject);
        }
    }

    private async void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            BombRoots.Remove(other.gameObject);

            // Ray
            float length = (other.gameObject.GetComponent<BombController>()) ? other.gameObject.GetComponent<BombController>().ExplosionLengthNetWork : 1f;
            while (length <= 0f)
            {
                await Task.Delay(100);
                length = (other.gameObject.GetComponent<BombController>()) ? other.gameObject.GetComponent<BombController>().ExplosionLengthNetWork : 1f;
            }

            RayWithDirectionToRemove(transform.position, Vector3.forward, length, other.gameObject);
            RayWithDirectionToRemove(transform.position, Vector3.back, length, other.gameObject);
            RayWithDirectionToRemove(transform.position, Vector3.right, length, other.gameObject);
            RayWithDirectionToRemove(transform.position, Vector3.left, length, other.gameObject);
        }
    }

    public void RayWithDirectionToAdd(Vector3 location, Vector3 direction, float length, GameObject bombRoot)
    {
        if (length <= 0) return;

        if (Physics.Raycast(location + direction * 0.5f, direction, out RaycastHit hit, 1.5f, WarningZoneLayer))
        {
            WarningZoneController warningZoneController = hit.collider.gameObject.GetComponent<WarningZoneController>() ?? hit.collider.gameObject.AddComponent<WarningZoneController>();
            warningZoneController.BombRoots.Add(bombRoot);

            if (Physics.Raycast(location + direction * 0.5f, direction, 1.5f, DestructiblesLayer)) return;

            warningZoneController.RayWithDirectionToAdd(hit.collider.transform.position, direction, length - 1, bombRoot);
        }
        else return;
    }

    public void RayWithDirectionToRemove(Vector3 location, Vector3 direction, float length, GameObject bombRoot)
    {
        if (length <= 0) return;

        if (Physics.Raycast(location + direction * 0.5f, direction, out RaycastHit hit, 1.5f, WarningZoneLayer))
        {
            WarningZoneController warningZoneController = hit.collider.gameObject.GetComponent<WarningZoneController>() ?? hit.collider.gameObject.AddComponent<WarningZoneController>();
            warningZoneController.BombRoots.Remove(bombRoot);

            if (Physics.Raycast(location + direction * 0.5f, direction, 1.5f, DestructiblesLayer)) return;

            warningZoneController.RayWithDirectionToRemove(hit.collider.transform.position, direction, length - 1, bombRoot);
        }
        else return;
    }

    public void TurnOn()
    {
        visual.enabled = true;
    }
    public void TurnOff()
    {
        visual.enabled = false;
    }

    // GET - SET
    public List<GameObject> BombRoots 
    {
        get
        {
            return bombRoots;
        }
        set
        {
            bombRoots = value;
        }
    }
}
