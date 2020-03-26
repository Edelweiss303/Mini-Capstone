using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        KeySquare currentKey = collision.GetComponent<KeySquare>();
        if (currentKey && currentKey.GetComponentInParent<CircleLock>().Active)
        {
            LockRotationGameController.Instance.CurrentCheckingKey = currentKey;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        KeySquare currentKey = collision.GetComponent<KeySquare>();
        if (currentKey && currentKey.GetComponentInParent<CircleLock>().Active)
        {
            if(LockRotationGameController.Instance.CurrentCheckingKey == currentKey)
            {
                LockRotationGameController.Instance.CurrentCheckingKey = null;
            }
        }
    }
}
