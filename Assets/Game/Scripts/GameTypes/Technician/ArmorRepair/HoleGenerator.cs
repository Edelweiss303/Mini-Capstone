using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleGenerator : MonoBehaviour
{
    public static float health = 100.0f;
    public float[] damageThresholds = { 92.0f, 84.0f, 76.0f, 68.0f, 60.0f, 52.0f, 44.0f, 36.0f, 28.0f, 20.0f, 12.0f, 4.0f };
    private Renderer[] armorRenderers;
    private static int index = 0;
    private float time = 0.0f;
    void Start()
    {
        armorRenderers = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= 1.5f)
        {

        }
    }
}
