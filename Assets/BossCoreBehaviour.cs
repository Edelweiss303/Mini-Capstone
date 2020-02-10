using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoreBehaviour : EnemyBehaviour
{
    public float Health = 15.0f;
    public BossBehaviour bBehaviour;

    public override bool IsAlive()
    {
        return Health > 0;
    }

    public override void TakeDamage(float damage)
    {
        Health -= damage;
        if (!IsAlive())
        {
            Destroy(bBehaviour.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
