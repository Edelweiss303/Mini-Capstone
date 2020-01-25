using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCameraControl : MonoBehaviour
{
    private Gyroscope gyro;

    void Awake()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        GyroMoveCamera();
    }

    void GyroMoveCamera()
    {
        this.transform.rotation = GyroToUnity(gyro.attitude);
    }


    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
