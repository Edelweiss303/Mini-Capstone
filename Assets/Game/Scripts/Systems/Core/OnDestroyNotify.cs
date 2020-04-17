using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class OnDestroyNotify : MonoBehaviour
{
    public event Action<AssetReference, OnDestroyNotify> Destroyed;
    public AssetReference AssetReference { get; set; }

    public void OnDestroy()
    {
        Destroyed?.Invoke(AssetReference, this);
    }
}
