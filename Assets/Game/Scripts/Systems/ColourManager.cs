using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyBase;

public class ColourManager : Singleton<ColourManager>
{
    public Material EnemyTypeAMaterial, EnemyTypeBMaterial, EnemyTypeCMaterial;
    public Material ShieldTypeAMaterial, ShieldTypeBMaterial, ShieldTypeCMaterial;

    public Color AmmoColorA, AmmoColorB, AmmoColorC;
    public Dictionary<EnemyColour, Material> EnemyMaterialMap = new Dictionary<EnemyColour, Material>();
    public Dictionary<EnemyColour, Material> ShieldMaterialMap = new Dictionary<EnemyColour, Material>();
    public Dictionary<EnemyColour, Color> AmmoColorMap = new Dictionary<EnemyColour, Color>();
    public List<EnemyColour> AllEnemyTypes = new List<EnemyColour>() { EnemyColour.A, EnemyColour.B, EnemyColour.C };

    public void Awake()
    {
        EnemyMaterialMap.Add(EnemyColour.A, EnemyTypeAMaterial);
        EnemyMaterialMap.Add(EnemyColour.B, EnemyTypeBMaterial);
        EnemyMaterialMap.Add(EnemyColour.C, EnemyTypeCMaterial);

        ShieldMaterialMap.Add(EnemyColour.A, ShieldTypeAMaterial);
        ShieldMaterialMap.Add(EnemyColour.B, ShieldTypeBMaterial);
        ShieldMaterialMap.Add(EnemyColour.C, ShieldTypeCMaterial);

        AmmoColorMap.Add(EnemyColour.A, AmmoColorA);
        AmmoColorMap.Add(EnemyColour.B, AmmoColorB);
        AmmoColorMap.Add(EnemyColour.C, AmmoColorC);
    }
}
