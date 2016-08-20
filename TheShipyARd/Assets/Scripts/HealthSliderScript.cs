using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class HealthSliderScript : MonoBehaviour {

    public Slider healthSlider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void updateValue(float value)
    {
        healthSlider.value = value;
    }
}
