using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class HealthSliderScript : MonoBehaviour {

    public Slider healthSlider;

    public void updateValue(float value)
    {
        healthSlider.value = value;
    }
}
