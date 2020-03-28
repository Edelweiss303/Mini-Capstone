using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDestroyBehaviour : MonoBehaviour
{
    public LayerMask InvalidLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if(InvalidLayerMask != (InvalidLayerMask | (1 << other.gameObject.layer)))
        {
            Destroy(this.gameObject);
        }
        
    }
}
