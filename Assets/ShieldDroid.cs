using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDroid : EnemyBase
{
    public float Speed = 1.0f;
    public float Health = 3.0f;
    public float DeathTime = 4.0f;
    public bool Alive = true;
    public float ProtectionTargetCheckFrequency = 5.0f;
    public float ProtectionHoverDistance = 0.5f;
    public float HoveringHeight = 3.0f;

    public GameObject ExplosionPrefab;
    public string DeathAudioClipName;

    private MeshRenderer enemyRenderer;
    private BoxCollider enemyCollider;
    private float timeSinceLastTargetCheck = 0.0f;
    private Transform protectionTarget;
    private Rigidbody rb;

    override public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            WaveManager.Instance.EnemyWasKilled();
        }
    }

    public override bool IsAlive()
    {
        return Alive;
    }

    public override void Die()
    {
        Alive = false;
        enemyCollider.enabled = false;
        if (enemyRenderer)
        {
            enemyRenderer.enabled = false;
        }

        //Create a death explosion effect and sound
        AudioManager.Instance.PlaySound(DeathAudioClipName);

        if (ExplosionPrefab)
        {
            Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyRenderer = GetComponent<MeshRenderer>();
        enemyCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        EnemiesManager.Instance.addEnemy(gameObject);
        TargetUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        TargetUpdate();
        MoveUpdate();
    }

    private void TargetUpdate()
    {
        timeSinceLastTargetCheck += Time.deltaTime;

        if(timeSinceLastTargetCheck > ProtectionTargetCheckFrequency || protectionTarget == null)
        {
            timeSinceLastTargetCheck = 0.0f;
            EnemyBase temp = EnemiesManager.Instance.GetProtectionTarget(transform.position);

            if (temp)
            {
                protectionTarget = temp.transform;
            }

        }
    }

    private void MoveUpdate()
    {
        Vector3 uniformVelocity;
        Vector3 targetPosition;
        if (protectionTarget)
        {
            targetPosition = protectionTarget.position + protectionTarget.up * HoveringHeight;
            if((targetPosition - transform.position).magnitude > ProtectionHoverDistance)
            {
                uniformVelocity = (targetPosition - transform.position).normalized;
                rb.velocity = uniformVelocity * Speed * Time.deltaTime;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
