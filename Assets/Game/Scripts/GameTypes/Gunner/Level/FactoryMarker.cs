using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GunnerController.Instance.FactoryMarkers.Add(transform);
    }
}
