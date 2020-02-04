using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Range(0,1)]
    public float interpolant;

    private GameObject player;
    private Vector3 targetPosition;
    private float cameraHeight;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        targetPosition = player.transform.position;
        targetPosition.y = cameraHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolant);
    }
}
