using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCore : EnemyBase
{
    public float Health = 15.0f;
    public FactoryMachine mainBehaviour;

    public override bool IsAlive()
    {
        return Health > 0;
    }

    public override void TakeDamage(float damage)
    {
        Health -= damage;
        if (!IsAlive())
        {
            Destroy(mainBehaviour.gameObject);
        }
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Die()
    {
        
    }
}
