using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanupBehaviour : MonoBehaviour
{
    public float Lifespan = 3.0f;
    private float timeAlive = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if(timeAlive > Lifespan)
        {
            Destroy(this.gameObject);
        }
    }
}
