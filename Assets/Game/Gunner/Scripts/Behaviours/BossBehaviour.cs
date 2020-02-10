using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public List<BossWallBehaviour> wallBehaviours;
    public bool IsAlive = true;
    private BossWallBehaviour selectedWall;
    private ComponentBehaviour selectedComponent;
    private bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            wallBehaviours.AddRange(GetComponentsInChildren<BossWallBehaviour>());
            foreach(BossWallBehaviour wBehaviour in wallBehaviours)
            {
                wBehaviour.initialize();
            }
            selectComponent();
            initialized = true;
        }

        MoveUpdate();
    }

    void MoveUpdate()
    {
        transform.Rotate(0, 0.5f, 0);
    }

    void selectComponent()
    {
        selectedWall = wallBehaviours[Random.Range(0, wallBehaviours.Count)];
        selectedComponent = selectedWall.Components[Random.Range(0, selectedWall.Components.Count)];
        selectedComponent.Select();
    }

    public void killSelectedComponent()
    {
        selectedWall.Components.Remove(selectedComponent);
        Destroy(selectedComponent.gameObject);
        
        selectComponent();
    }
}
