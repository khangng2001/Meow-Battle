using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject cameraPrefab;
    private GameObject cameraExisted;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivityHorizontalMouse;
    [SerializeField] private float sensitivityVerticalMouse;

    public void Initialize(GameObject targetFollow)
    {
        if (cameraExisted == null)
        {
            if (cameraPrefab == null) return;

            cameraExisted = Instantiate(cameraPrefab, targetFollow.transform.position, targetFollow.transform.rotation);

            CinemachineFreeLook cinemachineFreeLook = cameraExisted.GetComponent<CinemachineFreeLook>() ?? cameraExisted.AddComponent<CinemachineFreeLook>();
            cinemachineFreeLook.Follow = targetFollow.transform;
            cinemachineFreeLook.LookAt = targetFollow.transform;
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "";

            CameraController cameraController = cameraExisted.GetComponent<CameraController>() ?? cameraExisted.AddComponent<CameraController>();
            cameraController.Owner = this.gameObject;
        }
    }

    public void Rotate(Vector2 rotationCameraInput, GameObject targetFollow)
    {
        if (cameraExisted == null)
        {
            Initialize(targetFollow);
            return;
        }

        CinemachineFreeLook cinemachineFreeLook = cameraExisted.GetComponent<CinemachineFreeLook>();

        cinemachineFreeLook.m_XAxis.Value += rotationCameraInput.x * sensitivityHorizontalMouse * Time.deltaTime;
        cinemachineFreeLook.m_YAxis.Value -= rotationCameraInput.y * sensitivityVerticalMouse * Time.deltaTime;
    }

    public Vector3 GetPosition()
    {
        return cameraExisted.transform.position;
    }
}
