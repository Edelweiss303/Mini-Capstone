using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletDamage = 20.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Shield")
        {
            collision.gameObject.GetComponent<ShieldController>().shieldHealth -= bulletDamage;
            Destroy(gameObject);
        }
    }
}
