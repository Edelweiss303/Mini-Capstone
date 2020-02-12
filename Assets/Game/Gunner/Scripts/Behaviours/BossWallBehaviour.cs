using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWallBehaviour : MonoBehaviour
{
    public List<ComponentBehaviour> Components;

    public void initialize()
    {
        Components.AddRange(GetComponentsInChildren<ComponentBehaviour>());
    }
}
