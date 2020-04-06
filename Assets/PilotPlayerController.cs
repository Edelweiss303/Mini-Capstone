using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotPlayerController : MonoBehaviour
{
    public float Speed = 3.0f;
    public float RotationRate = 0.1f;

    public Transform TorsoTransform;
    public Transform CameraTransform;
    public float GunnerMoveUpdateSyncThreshold = 0.2f;

    [SerializeField]
    private int rotationTargetAngle = 0;

    private float rotationDirection = 0;
    private Rigidbody rb;

    private List<int> validRotations = new List<int>() { 0, 45, 90, 135, 180, 225, 270, 315 };
    private int currentTargetRotationIndex = 0;
    private float timeSinceLateGunnerMoveUpdate = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
        
    }

    void MovementUpdate()
    {
        CameraTransform.position = new Vector3(transform.position.x, CameraTransform.position.y, transform.position.z);

        if (InputManager.Instance.DirectionalPresses[InputManager.Direction.left])
        {
            currentTargetRotationIndex--;
            if (currentTargetRotationIndex < 0)
            {
                currentTargetRotationIndex = validRotations.Count - 1;
            }

            rotationTargetAngle = validRotations[currentTargetRotationIndex];
            rotationDirection = -1.0f;
        }
        if (InputManager.Instance.DirectionalPresses[InputManager.Direction.right])
        {
            currentTargetRotationIndex++;
            if (currentTargetRotationIndex >= validRotations.Count)
            {
                currentTargetRotationIndex = 0;
            }

            rotationTargetAngle = validRotations[currentTargetRotationIndex];

            rotationDirection = 1.0f;
        }

        Vector3 currentVelocity = new Vector3(transform.right.x * InputManager.Instance.DirectionalInput.x, 0, transform.forward.z * InputManager.Instance.DirectionalInput.y) * Time.deltaTime * Speed;
        transform.position += currentVelocity;

        TorsoTransform.Rotate(TorsoTransform.up, rotationDirection * RotationRate);

        if (Mathf.Abs(TorsoTransform.rotation.eulerAngles.y - rotationTargetAngle) < 2.0f)
        {
            TorsoTransform.rotation = Quaternion.Euler(new Vector3(TorsoTransform.rotation.eulerAngles.x, rotationTargetAngle, TorsoTransform.rotation.eulerAngles.z));
            rotationDirection = 0.0f;
        }

        timeSinceLateGunnerMoveUpdate += Time.deltaTime;

        if (timeSinceLateGunnerMoveUpdate > GunnerMoveUpdateSyncThreshold)
        {
            string message = "g:PilotTransformUpdate:" + transform.position.x + ":" + transform.position.y + ":" + transform.position.z + ":" + validRotations[currentTargetRotationIndex];
            Debug.Log(message);
            GameNetwork.Instance.ToPlayerQueue.Add(message);
            timeSinceLateGunnerMoveUpdate = 0.0f;
        }
    }
}
