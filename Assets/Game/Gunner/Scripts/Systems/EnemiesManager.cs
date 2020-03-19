using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : Singleton<EnemiesManager>
{
    private List<EnemyBase> aimableTargets = new List<EnemyBase>();
    private List<GameObject> allEnemies = new List<GameObject>();
    private Dictionary<int, Dictionary<EnemyBase, bool>> protectionPriorityMap = new Dictionary<int, Dictionary<EnemyBase, bool>>();

    public void Update()
    {
        SpawnUpdate();
    }

    public void SpawnUpdate()
    {
        for (int i = allEnemies.Count - 1; i >= 0; i--)
        {
            if (!allEnemies[i])
            {
                allEnemies.RemoveAt(i);
            }
        }

        for (int i = aimableTargets.Count - 1; i >= 0; i--)
        {
            if (!aimableTargets[i])
            {
                aimableTargets.RemoveAt(i);
            }
        }
    }

    public void addEnemy(GameObject newEnemyObject)
    {
        newEnemyObject.transform.parent = transform;
        allEnemies.Add(newEnemyObject);

        List<EnemyBase> newEnemies = GetComponentsInChildren<EnemyBase>().ToList();
        aimableTargets.AddRange(newEnemies.Where(e => e.AutoAimable));

        foreach(EnemyBase newEnemy in newEnemies)
        {
            if(newEnemy.ProtectionPriority > 0)
            {
                if (!protectionPriorityMap.ContainsKey(newEnemy.ProtectionPriority))
                {
                    protectionPriorityMap.Add(newEnemy.ProtectionPriority, new Dictionary<EnemyBase, bool>());
                }
                if (!protectionPriorityMap[newEnemy.ProtectionPriority].ContainsKey(newEnemy))
                {
                    protectionPriorityMap[newEnemy.ProtectionPriority].Add(newEnemy, false);
                }
                
            }
        }
    }

    public void addToAimables(EnemyBase eBehaviour)
    {
        if (!aimableTargets.Contains(eBehaviour))
        {
            aimableTargets.Add(eBehaviour);
        }
    }

    public void removeAllEnemies()
    {
        foreach(GameObject enemy in allEnemies)
        {
            GameObject.Destroy(enemy);
        }
        allEnemies = new List<GameObject>();
        aimableTargets = new List<EnemyBase>();
    }

    public EnemyBase GetClosestEnemy(Ray inRay, float maxRange, out Vector3 directionToMoveIn)
    {
        EnemyBase closestEnemy = null;
        directionToMoveIn = Vector3.zero;

        float closestDistance = maxRange;
        float currentDistance = -1.0f;
        float dotproduct = 0;

        for(int i = aimableTargets.Count -1; i >= 0; i--)
        {
            if(aimableTargets[i] == null)
            {
                aimableTargets.RemoveAt(i);
                continue;
            }
            else
            {
                dotproduct = Vector3.Dot(inRay.direction, (aimableTargets[i].transform.position - inRay.origin));
                currentDistance = (inRay.origin + inRay.direction * dotproduct - aimableTargets[i].transform.position).magnitude;

                if (!closestEnemy || closestDistance > currentDistance)
                {
                    if (currentDistance < maxRange && aimableTargets[i].IsAlive())
                    {
                        closestEnemy = aimableTargets[i];
                        closestDistance = currentDistance;
                    }
                }
            }
        }

        if (closestEnemy)
        {
            Vector3 test = (inRay.origin + inRay.direction * dotproduct);
            if (test.x > closestEnemy.transform.position.x)
            {
                directionToMoveIn.x--;
            }
            else
            {
                directionToMoveIn.x++;
            }

            if (test.y > closestEnemy.transform.position.y)
            {
                directionToMoveIn.y--;
            }
            else
            {
                directionToMoveIn.y++;
            }
            return closestEnemy;
        }
        
        
        return null;
    }

    public EnemyBase GetProtectionTarget(Vector3 position)
    {
        float closestDistance = 1000.0f, currentDistance = 0.0f;
        EnemyBase protectionTarget = null;
        int highestPriority = 0;

        foreach(KeyValuePair<int, Dictionary<EnemyBase, bool>> priorityLevel in protectionPriorityMap)
        {
            if(priorityLevel.Key > highestPriority)
            {
                foreach (KeyValuePair<EnemyBase, bool> enemy in priorityLevel.Value)
                {
                    if (!enemy.Value)
                    {
                        currentDistance = (position - enemy.Key.transform.position).magnitude;

                        if (currentDistance < closestDistance)
                        {
                            protectionTarget = enemy.Key;
                            highestPriority = priorityLevel.Key;
                            closestDistance = currentDistance;
                        }
                    }
                }
            }
        }

        if (protectionTarget)
        {
            protectionPriorityMap[highestPriority][protectionTarget] = true;
        }

        return protectionTarget;
    }
}
