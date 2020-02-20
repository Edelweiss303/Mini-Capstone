using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBarBehaviour : MonoBehaviour
{
    public List<AmmoRowBehaviour> Rows;

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
        int index = 0;
        if(ammoAmount != lastAmmoAmount)
        {
            lastAmmoAmount = ammoAmount;

            foreach(AmmoRowBehaviour row in Rows)
            {
                foreach(AmmoSlotBehaviour slot in row.AmmoSlots)
                {
                    if (index >= ammoAmount)
                    {
                        slot.FillingObject.SetActive(false);
                    }
                    else
                    {
                        slot.FillingObject.SetActive(true);
                    }
                    index++;
                }
            }
        }
    }

    public void SetAmmo(float inAmmoAmount)
    {
        ammoAmount = inAmmoAmount;
    }
}
