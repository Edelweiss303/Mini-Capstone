using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Vector3 DirectionalVelocity;
    public float MoveSpeed = 3.0f;
    public LayerMask PlayerLayerMask, DestroyLayerMask;
    public float MaxLifeSpan = 10.0f;
    public float Damage = 1.0f;
    
    private EnemyBase.EnemyColour Colour;
    private int gameID;
    private float timeAlive = 0.0f;
    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();

        timeAlive += Time.deltaTime;
        if(timeAlive > MaxLifeSpan)
        {
            Destroy(gameObject);
        }
    }

    void MoveUpdate()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    public void SetColour(Material inMaterial, EnemyBase.EnemyColour inColour)
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        meshRenderer.material = inMaterial;
        Colour = inColour;
        gameID = gameObject.GetInstanceID();
        ProjectilesManager.Instance.addProjectile(gameID, gameObject, Colour);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DestroyLayerMask == (DestroyLayerMask | (1 << other.gameObject.layer)))
        {
            Destroy(gameObject);
        }
        else if(PlayerLayerMask == (PlayerLayerMask | (1 << other.gameObject.layer)))
        {
            PlayerBehaviour pBehaviour = other.GetComponent<PlayerBehaviour>();
            if (pBehaviour)
            {
                pBehaviour.TakeDamage(Damage);
                Destroy(gameObject);
            }
        }
    }
}
