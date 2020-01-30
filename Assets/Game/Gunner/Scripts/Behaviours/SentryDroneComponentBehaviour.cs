using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryDroneComponentBehaviour : EnemyBehaviour
{
    private SentryDroneBehaviour sdBehaviour;
    private MeshRenderer sdcRenderer;
    public bool Selected;
    public Material SelectionMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        sdBehaviour = transform.parent.GetComponent<SentryDroneBehaviour>();
        sdcRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void TakeDamage(float damage)
    {
        if(Selected && sdBehaviour)
        {
            sdBehaviour.TakeDamage(damage);
        }
    }

    public override bool IsAlive()
    {
        return sdBehaviour.Alive;
    }

    public void Select()
    {
        Selected = true;
        if (SelectionMaterial && sdcRenderer)
        {
            sdcRenderer.material = SelectionMaterial;
        }
    }
}
