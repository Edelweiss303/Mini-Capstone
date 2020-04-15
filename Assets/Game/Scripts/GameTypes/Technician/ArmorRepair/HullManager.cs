using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullManager : Singleton<HullManager>
{

    public float health = 100.0f; //Replace all instances of this float and it *should* work okay
    public float holeOffsetLimit = 0.3f;
    public int shapeTwoRotationRange = 6;

    public float[] damageThresholds = { 92.0f, 84.0f, 76.0f, 68.0f, 60.0f, 52.0f, 44.0f, 36.0f, 28.0f, 20.0f, 12.0f, 4.0f };
    public int damageIndex = 0;

    public Renderer[] armourRenderers;
    public List<int> armourIndexes;

    void Awake()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        armourRenderers = GetComponentsInChildren<Renderer>();

        for(int i = 0; i < armourRenderers.Length; i++)
        {
            armourIndexes.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= damageThresholds[damageIndex] )
        {
            GenerateHole();

            if(damageIndex < damageThresholds.Length - 2)
            {
                damageIndex++;
            }
        }
    }

    private void CheckForDamage()
    {
    }

    public void GenerateHole()
    {
        if (armourIndexes.Count > 0)
        {

            int index = UnityEngine.Random.Range(0, armourIndexes.Count - 1);
            Material sharedMaterial = armourRenderers[armourIndexes[index]].sharedMaterial;

            int shapeTwoRotation = UnityEngine.Random.Range(0, 6);
            float dec = UnityEngine.Random.Range(0.0f, 0.99f);

            Vector2 offset = new Vector2(
                Mathf.Round(UnityEngine.Random.Range(-holeOffsetLimit, holeOffsetLimit) * 10) / 10,
                Mathf.Round(UnityEngine.Random.Range(-holeOffsetLimit, holeOffsetLimit) * 10) / 10
                );

            sharedMaterial.SetVector("Polygon_Tiling", Vector2.one);
            sharedMaterial.SetVector("Polygon_Offset", offset);

            sharedMaterial.SetFloat("Shape_2_Rotation", shapeTwoRotation + dec);

            armourIndexes.RemoveAt(index);
        }
    }
}
