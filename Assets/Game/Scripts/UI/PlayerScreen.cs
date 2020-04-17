using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScreen : MonoBehaviour
{
    public GameObject DamageVisionPanel;
    public float DamageVisionEffectTime;
    public string TakeDamageSoundEffectName;

    [SerializeField]
    private float damageVisionEffectTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DamageVisionUpdate();
    }

    private void DamageVisionUpdate()
    {
        if (damageVisionEffectTimer != 0)
        {
            if (damageVisionEffectTimer > DamageVisionEffectTime)
            {
                damageVisionEffectTimer = 0.0f;
                DamageVisionPanel.SetActive(false);
            }
            else
            {
                damageVisionEffectTimer += Time.deltaTime;
            }
        }
    }

    public void SetDamageScreen()
    {
        AudioManager.Instance.PlaySound(TakeDamageSoundEffectName);
        if (DamageVisionPanel)
        {
            damageVisionEffectTimer = Time.deltaTime;
            DamageVisionPanel.SetActive(true);
        }
    }

}
