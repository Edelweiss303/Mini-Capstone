using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentBehaviour : EnemyBase
{
    private SentryDrone sdBehaviour;
    private FactoryMachine bBehaviour;
    private MeshRenderer sdcRenderer;
    public bool Selected;
    public Material SelectionMaterial;
    public float Health = 5.0f;


    private void Awake()
    {
        sdBehaviour = transform.parent.GetComponentInParent<SentryDrone>();
        bBehaviour = transform.GetComponentInParent<FactoryMachine>();
        sdcRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void TakeDamage(float damage)
    {
        if(Selected)
        {
            if (sdBehaviour)
            {
                sdBehaviour.TakeDamage(damage);
            }
            else if (bBehaviour)
            {
                Health -= damage;
                if(Health <= 0)
                {
                    bBehaviour.killSelectedComponent();
                }
            }
        }
    }

    public override void Die()
    {
        //throw new NotImplementedException();
    }

    public override bool IsAlive()
    {
        if (sdBehaviour)
        {
            return sdBehaviour.IsAlive();
        }
        else if (bBehaviour)
        {
            return bBehaviour.IsAlive();
        }

        return false;
        
        
    }

    public void Select(EnemyBase.EnemyColour inEnemyColour, Material inEnemyMaterial)
    {
        Colour = inEnemyColour;
        EnemyMaterial = inEnemyMaterial;

        Selected = true;

        if (sdcRenderer)
        {
            sdcRenderer.material = EnemyMaterial;
        }
    }
}
