using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public float positionChangeInterval;
    public float fireRate = 1.0f;
    public float radius = 50.0f;

    public GameObject bullet;
    public GameObject player;

    private Quaternion rotation;
    private Vector2 coordinates;
    private float elapsedTime = 0.0f;
    private float coolDown = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        coordinates.x = Random.Range(0.0f, 1.0f);
        coordinates.y = Random.Range(0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown == 0.0f)
        {
            GameObject b = Instantiate(bullet, new Vector3(0, 7.5f, -50.0f), Quaternion.identity);
            b.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 50);
        }

        coolDown += Time.deltaTime;

        if(coolDown >= fireRate)
        {
            coolDown = 0.0f;
        }
    }
}
