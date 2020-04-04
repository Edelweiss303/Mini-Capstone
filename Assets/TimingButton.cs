using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingButton : MonoBehaviour
{
    public Image BGImage;
    public Image PatternImage;

    public void TimingButton_OnClick()
    {
        TimingButtonController.Instance.TryClick(this);
    }
}
