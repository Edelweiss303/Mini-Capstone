using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOL_Init : MonoBehaviour
{
    public GameObject DDOLPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (!DDOL.Instance)
        {
            Instantiate(DDOLPrefab);
        }
    }
}
