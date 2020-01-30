using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarBehaviour : MonoBehaviour
{
    public float Health;
    public Transform HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarUpdate();
    }

    private void HealthBarUpdate()
    {
        if (HealthBar)
        {
            HealthBar.localScale = new Vector3(Health, 1.0f);
        }
    }
}
