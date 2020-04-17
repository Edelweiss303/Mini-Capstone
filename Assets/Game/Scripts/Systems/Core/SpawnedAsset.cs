using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SpawnedAsset
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Transform Parent;
    public int GameID;
    public EnemyBase.EnemyColour Colour;
    public EnemyBase.EnemyType Type;
    public AddressablesManager.Addressable_Tag Tag;
    public GameObject Spawner;

    public SpawnedAsset(Vector3 inPosition, Quaternion inRotation, Transform inParent)
    {
        Position = inPosition;
        Rotation = inRotation;
        Parent = inParent;
    }
}
