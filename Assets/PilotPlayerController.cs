using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotPlayerController : MonoBehaviour
{
    public float Speed = 3.0f;
    public float RotationRate = 0.1f;

    public Transform TorsoTransform;
    public Transform CameraTransform;
    public float MoveUpdateSyncThreshold = 0.5f;
    public string PlayerMoveSoundEffectName, PlayerRotateSoundEffectName;
    public float RotationHeatGeneration = 0.25f;
    public float MovementHeatGeneration = 0.1f;
    public bool IsOverheated = false;

    [SerializeField]
    private int rotationTargetAngle = 0;

    private float rotationDirection = 0;
    private Rigidbody rb;
    private List<int> validRotations = new List<int>() { 0, 45, 90, 135, 180, 225, 270, 315 };
    private int currentTargetRotationIndex = 0;
    private float timeSinceLateGunnerMoveUpdate = 0.0f;
    private bool isMoving = false;



    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOverheated)
        {
            MovementUpdate();
        }
        else
        {
            isMoving = false;
        }
    }

    void MovementUpdate()
    {
        CameraTransform.position = new Vector3(transform.position.x, CameraTransform.position.y, transform.position.z);

        if (InputManager.Instance.DirectionalPresses[InputManager.Direction.left])
        {
            AudioManager.Instance.PlaySound(PlayerRotateSoundEffectName);
            GameNetwork.Instance.ToPlayerQueue.Add("t:TechAddHeat:" + RotationHeatGeneration);
            currentTargetRotationIndex--;
            if (currentTargetRotationIndex < 0)
            {
                currentTargetRotationIndex = validRotations.Count - 1;
            }

            rotationTargetAngle = validRotations[currentTargetRotationIndex];
            rotationDirection = -1.0f;
        }
        else if (InputManager.Instance.DirectionalPresses[InputManager.Direction.right])
        {
            AudioManager.Instance.PlaySound(PlayerRotateSoundEffectName);
            GameNetwork.Instance.ToPlayerQueue.Add("t:TechAddHeat:" + RotationHeatGeneration);
            currentTargetRotationIndex++;
            if (currentTargetRotationIndex >= validRotations.Count)
            {
                currentTargetRotationIndex = 0;
            }

            rotationTargetAngle = validRotations[currentTargetRotationIndex];

            rotationDirection = 1.0f;
        }

        Vector3 currentVelocity = new Vector3(transform.right.x * InputManager.Instance.DirectionalInput.x, 0, transform.forward.z * InputManager.Instance.DirectionalInput.y) * Time.deltaTime * Speed;
        if(currentVelocity.magnitude > 0.05f && !isMoving)
        {
            AudioManager.Instance.PlaySound(PlayerMoveSoundEffectName);
            isMoving = true;
        }
        else if(isMoving && currentVelocity.magnitude == 0.0f)
        {
            AudioManager.Instance.StopSound(PlayerMoveSoundEffectName);
            isMoving = false;
        }
        transform.position += currentVelocity;

        TorsoTransform.Rotate(TorsoTransform.up, rotationDirection * RotationRate);

        if (Mathf.Abs(TorsoTransform.rotation.eulerAngles.y - rotationTargetAngle) < 2.0f)
        {
            TorsoTransform.rotation = Quaternion.Euler(new Vector3(TorsoTransform.rotation.eulerAngles.x, rotationTargetAngle, TorsoTransform.rotation.eulerAngles.z));
            rotationDirection = 0.0f;
        }

        timeSinceLateGunnerMoveUpdate += Time.deltaTime;

        if (timeSinceLateGunnerMoveUpdate > MoveUpdateSyncThreshold)
        {
            string message = "g:PilotTransformUpdate:" + transform.position.x + ":" + transform.position.y + ":" + transform.position.z + ":" + validRotations[currentTargetRotationIndex];
            GameNetwork.Instance.ToPlayerQueue.Add(message);

            if (isMoving)
            {
                GameNetwork.Instance.ToPlayerQueue.Add("t:TechAddHeat:" + MovementHeatGeneration);
            }

            timeSinceLateGunnerMoveUpdate = 0.0f;
        }
    }
}
