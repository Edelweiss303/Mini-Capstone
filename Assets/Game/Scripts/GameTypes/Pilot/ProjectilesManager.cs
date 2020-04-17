using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesManager : Singleton<ProjectilesManager>
{
    public float NetworkingUpdateThreshold = 0.1f;
    private Dictionary<int, GameObject> allProjectileObjects = new Dictionary<int, GameObject>();
    public GameObject ProjectileShellPrefab;
    private string projectilesSpawnMsg = "ProjectilesSpawn", projectilesMoveMsg = "ProjectilesMove", projectilesDestroyMsg = "ProjectilesDestroy";
    private float timeSinceLastNetworkUpdate = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        SpawnUpdate();
        timeSinceLastNetworkUpdate += Time.deltaTime;
        if (timeSinceLastNetworkUpdate > NetworkingUpdateThreshold)
        {
            NetworkingUpdate();
            timeSinceLastNetworkUpdate = 0.0f;
        }
    }

    public void SpawnUpdate()
    {
        List<int> idsToRemove = new List<int>();
        foreach (KeyValuePair<int, GameObject> projectile in allProjectileObjects)
        {
            if (projectile.Value == null)
            {
                idsToRemove.Add(projectile.Key);
            }
        }

        foreach (int idToRemove in idsToRemove)
        {
            allProjectileObjects.Remove(idToRemove);
            projectilesDestroyMsg += ":" + idToRemove;
        }
    }

    private void NetworkingUpdate()
    {

        if (GameNetwork.Instance.Type == GameNetwork.PlayerType.Gunner)
        {
            Vector3 currentPosition;

            string projectilesUpdateMsg = "ProjectilesUpdate:";

            foreach (KeyValuePair<int, GameObject> projectile in allProjectileObjects)
            {
                if (projectile.Value != null)
                {
                    currentPosition = projectile.Value.transform.position;
                    projectilesMoveMsg += ":" + projectile.Key + ":" + currentPosition.x + ":" + currentPosition.y + ":" + currentPosition.z;
                }
            }

            projectilesUpdateMsg += projectilesSpawnMsg + "_" + projectilesDestroyMsg + "_" + projectilesMoveMsg;

            GameNetwork.Instance.UpdateProjectiles(projectilesUpdateMsg);
            projectilesSpawnMsg = "ProjectilesSpawn";
            projectilesMoveMsg = "ProjectilesMove";
            projectilesDestroyMsg = "ProjectilesDestroy";

        }
    }

    public void addProjectile(int projectileID, GameObject newProjectileObject, EnemyBase.EnemyColour enemyColour)
    {
        projectilesSpawnMsg += ":" + projectileID + ":" + enemyColour.ToString();

        newProjectileObject.transform.parent = transform;
        allProjectileObjects.Add(projectileID, newProjectileObject);
    }

    public void removeProjectile(int projectileID)
    {
        if (allProjectileObjects.ContainsKey(projectileID))
        {
            allProjectileObjects.Remove(projectileID);
            projectilesDestroyMsg += ":" + projectileID;
        }
    }


    public void UpdateProjectiles(string inMessage)
    {

        string[] messageSections = inMessage.Split('_');
        string[] messageSegments = messageSections[0].Split(':');

        int objectID;
        GameObject currentObject;
        EnemyBase.EnemyColour colour;
        for (int i = 2; i < messageSegments.Length; i += 2)
        {
            objectID = int.Parse(messageSegments[i]);
            switch (messageSegments[i + 1])
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
            Shell sBehaviour = null;

            currentObject = Instantiate(ProjectileShellPrefab);
            addProjectile(objectID, currentObject, colour);
            sBehaviour = currentObject.GetComponent<Shell>();
            sBehaviour.ChangeColour(colour);
        }

        messageSegments = messageSections[1].Split(':');

        for (int i = 1; i < messageSegments.Length; i++)
        {
            objectID = int.Parse(messageSegments[i]);
            if (allProjectileObjects.ContainsKey(objectID))
            {
                Destroy(allProjectileObjects[objectID]);
                allProjectileObjects.Remove(objectID);
            }
        }

        Vector3 temp;
        messageSegments = messageSections[2].Split(':');
        for (int i = 1; i < messageSegments.Length; i += 4)
        {
            objectID = int.Parse(messageSegments[i]);
            temp = new Vector3(float.Parse(messageSegments[i + 1]), float.Parse(messageSegments[i + 2]), float.Parse(messageSegments[i + 3]));
            if (allProjectileObjects.ContainsKey(objectID))
            {
                allProjectileObjects[objectID].transform.position = temp;
            }
        }
    }
}
