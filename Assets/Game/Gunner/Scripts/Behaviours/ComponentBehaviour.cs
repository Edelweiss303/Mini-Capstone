using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentBehaviour : EnemyBehaviour
{
    private SentryDroneBehaviour sdBehaviour;
    private BossBehaviour bBehaviour;
    private MeshRenderer sdcRenderer;
    public bool Selected;
    public Material SelectionMaterial;
    public float Health = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        sdBehaviour = transform.parent.GetComponent<SentryDroneBehaviour>();
        bBehaviour = transform.GetComponentInParent<BossBehaviour>();
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

    public override bool IsAlive()
    {
        if (sdBehaviour)
        {
            return sdBehaviour.Alive;
        }
        else if (bBehaviour)
        {
            return bBehaviour.IsAlive;
        }

        return false;
        
        
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
