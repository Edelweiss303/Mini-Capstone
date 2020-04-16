using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullManager : Singleton<HullManager>
{
    public float holeOffsetLimit = 0.3f;
    public int shapeTwoRotationRange = 6;

    public float[] damageThresholds = { 0.92f, 0.84f, 0.76f, 0.68f, 0.60f, 0.52f, 0.44f, 0.36f, 0.28f, 0.20f, 0.12f, 0.4f };
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

    void Update()
    {
        if(TechnicianController.Instance.HealthBar.Health <= damageThresholds[damageIndex] )
        {
            GenerateHole();

            if(damageIndex < damageThresholds.Length - 2)
            {
                damageIndex++;
            }
        }
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
