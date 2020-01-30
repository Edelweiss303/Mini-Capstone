using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBarBehaviour : MonoBehaviour
{
    public List<GameObject> Slots;

    private float ammoAmount;
    private float lastAmmoAmount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AmmoBarUpdate();
    }

    private void AmmoBarUpdate()
    {
        if(ammoAmount != lastAmmoAmount)
        {
            lastAmmoAmount = ammoAmount;
            AmmoSlotBehaviour temp;
            //Do the update
            for (int i = 0; i < Slots.Count; i++)
            {
                temp = Slots[i].GetComponent<AmmoSlotBehaviour>();

                if (i >= ammoAmount)
                {
                    temp.FillingObject.SetActive(false);
                }
                else
                {
                    temp.FillingObject.SetActive(true);
                }
            }
        }
    }

    public void SetAmmo(float inAmmoAmount)
    {
        ammoAmount = inAmmoAmount;
    }
}
