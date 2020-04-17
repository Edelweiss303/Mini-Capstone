using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : Singleton<AddressablesManager>
{
    public enum Addressable_Tag
    {
        chaser, bigchaser, sentry, skulker, interceptor, collector,
        chaser_shell, bigchaser_shell, sentry_shell, skulker_shell, interceptor_shell, collector_shell,
        enemy_projectile, enemy_projectile_shell, explosion, default_hit_effect, beam_impact_effect,
        ammo_pickup, ammo_pickup_shell, main_spawner_shell
    }
    public List<Addressable_Tag> EnemyAddressables;
    public List<Addressable_Tag> EnemyShellAddressables;
    public List<Addressable_Tag> PickupsAddressables;
    public List<Addressable_Tag> ImpactEffectAddressables;

    [SerializeField]
    private Dictionary<Addressable_Tag, AssetReference> _addressablesCollection = new Dictionary<Addressable_Tag, AssetReference>();

    private readonly Dictionary<AssetReference, List<GameObject>> _spawnedAssets =
        new Dictionary<AssetReference, List<GameObject>>();

    private readonly Dictionary<AssetReference, Queue<SpawnedAsset>> _queuedSpawnRequests =
        new Dictionary<AssetReference, Queue<SpawnedAsset>>();

    private readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncOperationHandles =
        new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    private bool isInitialized = false;

    public AssetReference ChaserReference, BigChaserReference, SentryReference, SkulkerReference, InterceptorReference, CollectorReference,
                        ChaserShellReference, BigChaserShellReference, SentryShellReference, SkulkerShellReference, InterceptorShellReference, CollectorShellReference,
                        EnemyProjectileReference, EnemyProjectileShellReference, ExplosionReference, DefaultHitEffectReference, BeamImpactEffectReference,
                        AmmoPickupReference, AmmoPickupShellReference, MainSpawnerShellReference;

    private void initialize()
    {
        //Addressables.DownloadDependenciesAsync()
        _addressablesCollection.Add(Addressable_Tag.chaser, ChaserReference);
        _addressablesCollection.Add(Addressable_Tag.bigchaser, BigChaserReference);
        _addressablesCollection.Add(Addressable_Tag.sentry, SentryReference);
        _addressablesCollection.Add(Addressable_Tag.skulker, SkulkerReference);
        _addressablesCollection.Add(Addressable_Tag.interceptor, InterceptorReference);
        _addressablesCollection.Add(Addressable_Tag.collector, CollectorReference);
        _addressablesCollection.Add(Addressable_Tag.chaser_shell, ChaserShellReference);
        _addressablesCollection.Add(Addressable_Tag.bigchaser_shell, BigChaserShellReference);
        _addressablesCollection.Add(Addressable_Tag.sentry_shell, SentryShellReference);
        _addressablesCollection.Add(Addressable_Tag.skulker_shell, SkulkerShellReference);
        _addressablesCollection.Add(Addressable_Tag.interceptor_shell, InterceptorShellReference);
        _addressablesCollection.Add(Addressable_Tag.collector_shell, CollectorShellReference);
        _addressablesCollection.Add(Addressable_Tag.enemy_projectile, EnemyProjectileReference);
        _addressablesCollection.Add(Addressable_Tag.enemy_projectile_shell, EnemyProjectileShellReference);
        _addressablesCollection.Add(Addressable_Tag.explosion, ExplosionReference);
        _addressablesCollection.Add(Addressable_Tag.default_hit_effect, DefaultHitEffectReference);
        _addressablesCollection.Add(Addressable_Tag.beam_impact_effect, BeamImpactEffectReference);
        _addressablesCollection.Add(Addressable_Tag.ammo_pickup, AmmoPickupReference);
        _addressablesCollection.Add(Addressable_Tag.ammo_pickup_shell, AmmoPickupShellReference);
        _addressablesCollection.Add(Addressable_Tag.main_spawner_shell, MainSpawnerShellReference);
        
        isInitialized = true;
    }

    public void Spawn(SpawnedAsset spawnDetails)
    {
        if (!isInitialized)
        {
            initialize();
        }

        if (!_addressablesCollection.ContainsKey(spawnDetails.Tag))
        {
            return;
        }

        AssetReference assetReference = _addressablesCollection[spawnDetails.Tag];
        
        if (assetReference.RuntimeKeyIsValid() == false)
        {
            Debug.Log("Invalid Key " + assetReference.RuntimeKey.ToString());
            return;
        }

        if (_asyncOperationHandles.ContainsKey(assetReference))
        {
            if (_asyncOperationHandles[assetReference].IsDone)
            {
                SpawnAssetFromReference(assetReference, spawnDetails);
            }
            else
            {
                EnqueueSpawnForAfterInitialization(assetReference, spawnDetails);
            }
            return;
        }

        LoadAndSpawn(assetReference, spawnDetails);
    }

   

    public void LoadAndSpawn(AssetReference assetReference, SpawnedAsset inSpawnDetails)
    {
        var op = Addressables.LoadAssetAsync<GameObject>(assetReference);
        _asyncOperationHandles[assetReference] = op;
        op.Completed += (operation) =>
        {
            SpawnAssetFromReference(assetReference, inSpawnDetails);
            if (_queuedSpawnRequests.ContainsKey(assetReference))
            {
                while (_queuedSpawnRequests[assetReference]?.Any() == true)
                {
                    SpawnedAsset spawnDetails = _queuedSpawnRequests[assetReference].Dequeue();
                    SpawnAssetFromReference(assetReference, spawnDetails);
                }
            }
        };
    }


    private void EnqueueSpawnForAfterInitialization(AssetReference assetReference, SpawnedAsset spawnedAsset)
    {
        if (_queuedSpawnRequests.ContainsKey(assetReference) == false)
        {
            _queuedSpawnRequests[assetReference] = new Queue<SpawnedAsset>();

        }
        _queuedSpawnRequests[assetReference].Enqueue(spawnedAsset);
    }

    private void SpawnAssetFromReference(AssetReference assetReference, SpawnedAsset spawnDetails)
    {
        assetReference.InstantiateAsync(spawnDetails.Position, spawnDetails.Rotation, spawnDetails.Parent).Completed += (asyncOperationHandle) =>
        {
            if (_spawnedAssets.ContainsKey(assetReference) == false)
            {
                _spawnedAssets[assetReference] = new List<GameObject>();
            }
            GameObject madeObject = asyncOperationHandle.Result;
            if (!madeObject)
            {
                return;
            }
            _spawnedAssets[assetReference].Add(madeObject);
            var notify = asyncOperationHandle.Result.AddComponent<OnDestroyNotify>();
            notify.Destroyed += Remove;
            notify.AssetReference = assetReference;

            if (EnemyAddressables.Contains(spawnDetails.Tag))
            {
                asyncOperationHandle.Result.GetComponent<EnemyBase>().SetEnemyColour(spawnDetails.Colour);
                spawnDetails.Spawner.GetComponent<MainSpawner>().SpawnedEnemies.Add(asyncOperationHandle.Result);
                WavesManager.Instance.WaveSpawn.Add(asyncOperationHandle.Result);
                WavesManager.Instance.CurrentAmountSpawnedInWave++;
            }
            else if (EnemyShellAddressables.Contains(spawnDetails.Tag))
            {
                EnemiesManager.Instance.addEnemy(spawnDetails.GameID, asyncOperationHandle.Result, EnemyBase.EnemyType.interceptor, spawnDetails.Colour);
                Shell sBehaviour = asyncOperationHandle.Result.GetComponent<Shell>();
                sBehaviour.ChangeColour(spawnDetails.Colour);
            }
            else if (PickupsAddressables.Contains(spawnDetails.Tag))
            {
                asyncOperationHandle.Result.GetComponent<Shell>().ChangeColour(spawnDetails.Colour);
                GunnerController.Instance.PickupsMap.Add(spawnDetails.GameID, asyncOperationHandle.Result);
            }
            else if (ImpactEffectAddressables.Contains(spawnDetails.Tag))
            {
                asyncOperationHandle.Result.GetComponent<Renderer>().material = ColourManager.Instance.EnemyMaterialMap[spawnDetails.Colour];
            }

        };


    }

    private void Remove(AssetReference assetReference, OnDestroyNotify obj)
    {
        Addressables.ReleaseInstance(obj.gameObject);

        _spawnedAssets[assetReference].Remove(obj.gameObject);
        if (_spawnedAssets[assetReference].Count == 0)
        {
            Debug.Log("Removed all {assetReference.RuntimeKey.ToString()}");

            if (_asyncOperationHandles[assetReference].IsValid())
            {
                Addressables.Release(_asyncOperationHandles[assetReference]);
            }

            _asyncOperationHandles.Remove(assetReference);
        }
    }
}
