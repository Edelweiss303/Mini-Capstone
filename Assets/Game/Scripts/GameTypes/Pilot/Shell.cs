﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public List<Renderer> materialObjects = new List<Renderer>();
    public List<Renderer> shieldMaterialObjects = new List<Renderer>();

    public Material material, shieldMaterial;


    public void ChangeColour(EnemyBase.EnemyColour inEnemyColour)
    {
        material = ColourManager.Instance.EnemyMaterialMap[inEnemyColour];
        shieldMaterial = ColourManager.Instance.ShieldMaterialMap[inEnemyColour];

        foreach(Renderer materialObject in materialObjects)
        {
            materialObject.material = material;
        }

        foreach(Renderer shieldMaterialObject in shieldMaterialObjects)
        {
            shieldMaterialObject.material = shieldMaterial;
        }
    }
}