using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private MeshRenderer shieldRenderer;
    private float initialShieldAlpha;
    private int shieldAlphaFluctuationDirection = 1;

    public float ShieldAlphaFluctuationLimit = 0.2f;
    public float ShieldAlphaIncrement = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        shieldRenderer = GetComponent<MeshRenderer>();
        initialShieldAlpha = shieldRenderer.material.color.a;
    }

    public void SetShieldType(GameManager.EnemyType inEnemyType)
    {
        shieldRenderer.material = GameManager.Instance.ShieldMaterialMap[inEnemyType];
        initialShieldAlpha = shieldRenderer.material.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        ShieldAlphaUpdate();
    }

    private void ShieldAlphaUpdate()
    {
        Color currentColor = shieldRenderer.material.color;
        shieldRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a + ShieldAlphaIncrement * shieldAlphaFluctuationDirection);

        if(shieldAlphaFluctuationDirection == 1)
        {
            if (shieldRenderer.material.color.a > initialShieldAlpha + ShieldAlphaFluctuationLimit)
            {
                shieldAlphaFluctuationDirection = -1;
            }
        }
        else
        {
            if (shieldRenderer.material.color.a < initialShieldAlpha - ShieldAlphaFluctuationLimit)
            {
                shieldAlphaFluctuationDirection = 1;
            }
        }
        
    }
}
