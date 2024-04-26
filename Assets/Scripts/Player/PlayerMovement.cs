using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Move")]
    [SerializeField] private float normalSpeed;
    [SerializeField] private float fastSpeed;

    [Header("Speed Rotate")]
    [SerializeField] private float speedRotate;

    public void MovementByCharaterController(CharacterController characterController, Vector3 move3DInput, Vector3 direction, float speed, float speedRotate)
    {
        switch (move3DInput.z)
        {
            case > 0f:
                characterController.Move(transform.TransformDirection(Vector3.forward * (move3DInput.z * speed * Time.deltaTime)));
                break;
            case < 0f:
                characterController.Move(transform.TransformDirection(-Vector3.forward * (move3DInput.z * speed * Time.deltaTime)));
                break;
        }

        switch (move3DInput.x)
        {
            case > 0f:
                characterController.Move(transform.TransformDirection(Vector3.forward * (move3DInput.x * speed * Time.deltaTime)));
                break;
            case < 0f:
                characterController.Move(transform.TransformDirection(-Vector3.forward * (move3DInput.x * speed * Time.deltaTime)));
                break;
        }

        if (move3DInput.magnitude > 0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move3DInput) * Quaternion.LookRotation(direction), speedRotate);
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        }
    }

    public void PushByCharacterController(CharacterController characterController, Vector3 direction, float force)
    {
        characterController.Move(direction * Time.deltaTime * force);
    }
    
    // GET - SET
    public float NormalSpeed
    {
        get
        {
            return normalSpeed;
        }
        set
        {
            normalSpeed = value;
        }
    }
    public float FastSpeed
    {
        get
        {
            return fastSpeed;
        }
        set
        {
            fastSpeed = value;
        }
    }
    public float SpeedRotate
    {
        get
        {
            return speedRotate;
        }
        set
        {
            speedRotate = value;
        }
    }
}
