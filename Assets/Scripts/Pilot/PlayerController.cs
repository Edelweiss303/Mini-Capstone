using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed;

    private Animator animator;
    private Vector2 input; //Holds left stick input values
    private float theta; //The amount we should turn

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ControllerInput();
    }

    private void ControllerInput()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        animator.SetFloat("inputX", input.x);
        animator.SetFloat("inputY", input.y);
        animator.SetFloat("inputMag", input.magnitude);

        if (input.magnitude > 0.1f)
        {
            theta = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90 - theta, 0), rotationSpeed * Time.deltaTime);
        }
    }
}
