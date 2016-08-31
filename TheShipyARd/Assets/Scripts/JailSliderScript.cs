using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class JailSliderScript : MonoBehaviour
{

    public Slider jailSlider;

    public void updateValue(float value)
    {
        jailSlider.value = value;
    }
}
