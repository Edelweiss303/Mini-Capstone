using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GunnerController;

public class PilotController : Singleton<PilotController>
{
    public Material EnemyTypeAMaterial, EnemyTypeBMaterial, EnemyTypeCMaterial;
    public Material ShieldTypeAMaterial, ShieldTypeBMaterial, ShieldTypeCMaterial;

    public Color AmmoColorA, AmmoColorB, AmmoColorC;
    public Dictionary<EnemyType, Material> EnemyMaterialMap = new Dictionary<EnemyType, Material>();
    public Dictionary<EnemyType, Material> ShieldMaterialMap = new Dictionary<EnemyType, Material>();
    public Dictionary<EnemyType, Color> AmmoColorMap = new Dictionary<EnemyType, Color>();
    public List<EnemyType> AllEnemyTypes = new List<EnemyType>() { EnemyType.A, EnemyType.B, EnemyType.C };

    public void Awake()
    {
        EnemyMaterialMap.Add(EnemyType.A, EnemyTypeAMaterial);
        EnemyMaterialMap.Add(EnemyType.B, EnemyTypeBMaterial);
        EnemyMaterialMap.Add(EnemyType.C, EnemyTypeCMaterial);

        ShieldMaterialMap.Add(EnemyType.A, ShieldTypeAMaterial);
        ShieldMaterialMap.Add(EnemyType.B, ShieldTypeBMaterial);
        ShieldMaterialMap.Add(EnemyType.C, ShieldTypeCMaterial);

        AmmoColorMap.Add(EnemyType.A, AmmoColorA);
        AmmoColorMap.Add(EnemyType.B, AmmoColorB);
        AmmoColorMap.Add(EnemyType.C, AmmoColorC);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
