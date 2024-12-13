using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider experienceSlider;
    // Start is called before the first frame update
    public void SetSlider(float amount)
    {
        experienceSlider.value = amount;
    }

    public void SetSliderMax(float amount)
    {
        experienceSlider.maxValue = amount;
        SetSlider(0);
    }
}
