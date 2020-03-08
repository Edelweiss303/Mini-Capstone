using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0, 1)]
    public float deadZone;
    public float rotationSpeed;
    public GameObject shield;

    private ShieldController sc;
    private Animator animator;
    private Vector2 stickInput; //Holds left stick input values
    private float theta; //The amount we should turn

    public List<GameObject> pickups = new List<GameObject>();

    void Awake()
    {
        sc = shield.GetComponent<ShieldController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StickInput();

        if (sc.shieldHealth > 0.0f)
        {
            ShieldInput();
        }
    }

    private void StickInput()
    {
        stickInput.x = Input.GetAxis("LeftHorizontal");
        stickInput.y = Input.GetAxis("LeftVertical");

        animator.SetFloat("inputX", stickInput.x);
        animator.SetFloat("inputY", stickInput.y);
        animator.SetFloat("inputMag", stickInput.magnitude);

        if (stickInput.magnitude > deadZone)
        {
            theta = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90 - theta, 0), rotationSpeed * Time.deltaTime);
        }

        
    }

    private void ShieldInput()
    {
        if (Input.GetKeyDown("joystick button 7"))
        {
            shield.SetActive(true);
        }

        if (Input.GetKey("joystick button 7"))
        {
            sc.ShieldRotation();
        }

        if (Input.GetKeyUp("joystick button 7"))
        {
            shield.SetActive(false);
        }
    }
}
