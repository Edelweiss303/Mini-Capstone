using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [Range(0, 1)]
    public float deadZone;
    public float shieldSpeed;
    public float shieldHealth;
    public float shieldRadius = 10.0f; 

    public Vector2 input; //Holds right stick input values
    private Transform player;

    [SerializeField] private float radius; //How far away the shield is from the player
    [SerializeField] private float theta; //The amount we should turn

    [SerializeField] private Vector3 playerPosition; // Point where the shield is right now (the x and z of the shield position)
    [SerializeField] private Vector3 shieldDestination; //where the shield is going (right stick x and y)

    private void Awake()
    {
        player = transform.parent.gameObject.GetComponent<Transform>();
    }

    private void Update()
    {
        if(shieldHealth <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
    public void ShieldRotation()
    {
        input.x = Input.GetAxis("RightHorizontal");
        input.y = Input.GetAxis("RightVertical");
        
        playerPosition.x = player.position.x;
        playerPosition.y = player.position.y + transform.position.y;
        playerPosition.z = player.position.z;

        
        if (input.magnitude > deadZone)
        {
            input.Normalize();

            theta = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            shieldDestination = new Vector3(
                player.transform.position.x + input.x * shieldRadius,
                player.transform.position.y + transform.position.y,
                player.transform.position.z + input.y * shieldRadius
                ); 

            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                Quaternion.Euler(0, 90 - theta, 0), 
                shieldSpeed * Time.deltaTime
                );

            transform.position = Vector3.Slerp(
                transform.position - playerPosition, 
                shieldDestination - playerPosition, 
                shieldSpeed * Time.deltaTime
                ) 
                + playerPosition;
        }

    }
}