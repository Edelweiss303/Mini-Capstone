using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    abstract public void TakeDamage(float damage);
    abstract public bool IsAlive();
    public abstract void Die();
    public bool AutoAimable = true;
    public bool DieOnCollision = true;
    public float Damage;
    public int ProtectionPriority;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.PlayerProjectileMask == (GameManager.Instance.PlayerProjectileMask | (1 << other.gameObject.layer)))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
            return;
        }

        PlayerBehaviour pBehaviour = other.gameObject.GetComponent<PlayerBehaviour>();
        if (pBehaviour)
        {
            pBehaviour.TakeDamage(Damage);
            Die();
        }
    }


}

