using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : Singleton<EnemiesManager>
{
    //public GameObject ChaserShellPrefab, SentryShellPrefab, DroidShellPrefab, CollectorShellPrefab, SpawnerShellPrefab, BigChaserShellPrefab, SkulkerShellPrefab, InterceptorShellPrefab;
    public float NetworkingUpdateThreshold = 0.1f;
    private Dictionary<int,GameObject> allEnemyObjects = new Dictionary<int,GameObject>();

    private string enemiesSpawnMsg = "EnemiesSpawn", enemiesMoveMsg = "EnemiesMove", enemiesRotateMsg = "EnemiesRotate", enemiesDestroyMsg = "EnemiesDestroy";
    private float timeSinceLastNetworkUpdate = 0.0f;

    public void Update()
    {
        
        SpawnUpdate();
        timeSinceLastNetworkUpdate += Time.deltaTime;
        if(timeSinceLastNetworkUpdate > NetworkingUpdateThreshold)
        {
            NetworkingUpdate();
            timeSinceLastNetworkUpdate = 0.0f;
        }
        
    }

    public void SpawnUpdate()
    {
        List<int> enemyIDsToRemove = new List<int>();
        foreach(KeyValuePair<int,GameObject> enemy in allEnemyObjects)
        {
            if(enemy.Value == null)
            {
                enemyIDsToRemove.Add(enemy.Key);
            }
        }

        foreach(int enemyIDToRemove in enemyIDsToRemove)
        {
            allEnemyObjects.Remove(enemyIDToRemove);
            enemiesDestroyMsg += ":" + enemyIDToRemove;
        }
    }

    private void NetworkingUpdate()
    {

        if (GameNetwork.Instance.Type == GameNetwork.PlayerType.Gunner)
        {
            Vector3 currentPosition;
            Vector3 eulerAngles;

            string enemiesUpdateMessage = "EnemiesUpdate:";

            foreach (KeyValuePair<int, GameObject> enemy in allEnemyObjects)
            {
                if (enemy.Value != null)
                {
                    currentPosition = enemy.Value.transform.position;
                    eulerAngles = enemy.Value.transform.rotation.eulerAngles;
                    enemiesMoveMsg += ":" + enemy.Key + ":" + currentPosition.x + ":" + currentPosition.y + ":" + currentPosition.z;
                    enemiesRotateMsg += ":" + enemy.Key + ":" + eulerAngles.x + ":" + eulerAngles.y + ":" + eulerAngles.z;
                }
            }

            enemiesUpdateMessage += enemiesSpawnMsg + "_" + enemiesDestroyMsg + "_" + enemiesMoveMsg + "_" + enemiesRotateMsg;

            GameNetwork.Instance.UpdateEnemies(enemiesUpdateMessage);
            enemiesSpawnMsg = "EnemiesSpawn";
            enemiesMoveMsg = "EnemiesMove";
            enemiesRotateMsg = "EnemiesRotate";
            enemiesDestroyMsg = "EnemiesDestroy";

        }
    }

    public void addEnemy(int enemyID, GameObject newEnemyObject, EnemyBase.EnemyType enemyType, EnemyBase.EnemyColour enemyColour)
    {
        enemiesSpawnMsg += ":" + enemyID + ":" + enemyType.ToString() + ":" + enemyColour.ToString();
        
        newEnemyObject.transform.parent = transform;
        allEnemyObjects.Add(enemyID, newEnemyObject);
    }

    public void removeEnemy(int enemyID)
    {
        if (allEnemyObjects.ContainsKey(enemyID))
        {
            allEnemyObjects.Remove(enemyID);
            enemiesDestroyMsg += ":" + enemyID;
        }
    }
    
  
    public void UpdateEnemies(string inMessage)
    {

        string[] messageSections = inMessage.Split('_');
        string[] messageSegments = messageSections[0].Split(':');

        int objectID;
        //GameObject currentObject;
        EnemyBase.EnemyColour colour;
        for (int i = 2; i < messageSegments.Length; i += 3)
        {
            objectID = int.Parse(messageSegments[i]);
            switch(messageSegments[i + 2])
            {
                case "B":
                    colour = EnemyBase.EnemyColour.B;
                    break;
                case "C":
                    colour = EnemyBase.EnemyColour.C;
                    break;
                default:
                    colour = EnemyBase.EnemyColour.A;
                    break;
            }

            SpawnedAsset spawnAsset = new SpawnedAsset(transform.position, Quaternion.identity, null);
            spawnAsset.GameID = objectID;
            spawnAsset.Colour = colour;
            switch (messageSegments[i + 1])
            {
                case "chaser":
                    spawnAsset.Type = EnemyBase.EnemyType.chaser;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.chaser_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(ChaserShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.chaser, colour);
                    //sBehaviour = currentObject.GetComponent<Shell>();
                    //sBehaviour.ChangeColour(colour);
                    break;
                case "sentry":
                    spawnAsset.Type = EnemyBase.EnemyType.sentry;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.sentry_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(SentryShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.sentry, colour);
                    break;
                case "droid":
                    //AddressablesManager.Instance.SpawnEnemyShell(AddressablesManager.Addressable_Tag.chaser_shell, transform.position, Quaternion.identity, null, objectID, EnemyBase.EnemyType.chaser, colour);
                    //currentObject = Instantiate(DroidShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.droid, colour);
                    break;
                case "collector":
                    spawnAsset.Type = EnemyBase.EnemyType.collector;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.collector_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(CollectorShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.collector, colour);
                    break;
                case "spawn":
                    spawnAsset.Type = EnemyBase.EnemyType.spawn;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.main_spawner_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //AddressablesManager.Instance.SpawnEnemyShell(AddressablesManager.Addressable_Tag.spawn, transform.position, Quaternion.identity, null, objectID, EnemyBase.EnemyType.chaser, colour);
                    //currentObject = Instantiate(SpawnerShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.spawn, colour);
                    //sBehaviour = currentObject.GetComponent<Shell>();
                    //sBehaviour.ChangeColour(colour);
                    break;
                case "bigchaser":
                    spawnAsset.Type = EnemyBase.EnemyType.bigchaser;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.bigchaser_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(BigChaserShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.bigchaser, colour);
                    //sBehaviour = currentObject.GetComponent<Shell>();
                    //sBehaviour.ChangeColour(colour);
                    break;
                case "skulker":
                    spawnAsset.Type = EnemyBase.EnemyType.skulker;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.skulker_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(SkulkerShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.skulker, colour);
                    //sBehaviour = currentObject.GetComponent<Shell>();
                    //sBehaviour.ChangeColour(colour);
                    break;
                case "interceptor":
                    spawnAsset.Type = EnemyBase.EnemyType.interceptor;
                    spawnAsset.Tag = AddressablesManager.Addressable_Tag.interceptor_shell;
                    AddressablesManager.Instance.Spawn(spawnAsset);
                    //currentObject = Instantiate(InterceptorShellPrefab);
                    //addEnemy(objectID, currentObject, EnemyBase.EnemyType.interceptor, colour);
                    //sBehaviour = currentObject.GetComponent<Shell>();
                    //sBehaviour.ChangeColour(colour);
                    break;
                default:
                    continue;
            }
        }

        messageSegments = messageSections[1].Split(':');

        for (int i = 1; i < messageSegments.Length; i++)
        {
            objectID = int.Parse(messageSegments[i]);
            if (allEnemyObjects.ContainsKey(objectID))
            {
                Destroy(allEnemyObjects[objectID]);
                allEnemyObjects.Remove(objectID);
            }
        }

        Vector3 temp;
        messageSegments = messageSections[2].Split(':');
        for (int i = 1; i < messageSegments.Length; i += 4)
        {
            objectID = int.Parse(messageSegments[i]);
            temp = new Vector3(float.Parse(messageSegments[i + 1]), float.Parse(messageSegments[i + 2]), float.Parse(messageSegments[i + 3]));
            if (allEnemyObjects.ContainsKey(objectID))
            {
                allEnemyObjects[objectID].transform.position = temp;
            }
        }

        messageSegments = messageSections[3].Split(':');
        for (int i = 1; i < messageSegments.Length; i += 4)
        {
            objectID = int.Parse(messageSegments[i]);
            temp = new Vector3(float.Parse(messageSegments[i + 1]), float.Parse(messageSegments[i + 2]), float.Parse(messageSegments[i + 3]));
            if (allEnemyObjects.ContainsKey(objectID))
            {
                allEnemyObjects[objectID].transform.eulerAngles = temp;
            }
        }
    }
}
