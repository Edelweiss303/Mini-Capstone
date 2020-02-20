using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : Singleton<EnemiesManager>
{
    private List<EnemyBehaviour> aimableTargets = new List<EnemyBehaviour>();
    private List<GameObject> allEnemies = new List<GameObject>();

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

    public void addEnemy(GameObject newEnemy)
    {
        newEnemy.transform.parent = transform;
        allEnemies.Add(newEnemy);

        List<EnemyBehaviour> newEnemies = GetComponentsInChildren<EnemyBehaviour>().ToList();
        aimableTargets.AddRange(newEnemies.Where(e => e.AutoAimable));
    }

    public void addToAimables(EnemyBehaviour eBehaviour)
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
        aimableTargets = new List<EnemyBehaviour>();
    }

    public EnemyBehaviour GetClosestEnemy(Ray inRay, float maxRange, out Vector3 directionToMoveIn)
    {
        EnemyBehaviour closestEnemy = null;
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
            Debug.Log(closestEnemy.transform.position);
            Debug.DrawLine(inRay.origin, closestEnemy.transform.position, Color.red);
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
}
