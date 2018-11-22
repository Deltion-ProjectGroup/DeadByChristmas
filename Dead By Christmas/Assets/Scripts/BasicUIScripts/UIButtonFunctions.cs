using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour {

    [Header("FromToButtonFunctions")]
    public GameObject from;
    public GameObject to;
    [Header("SliderValueChange")]
    public Slider slider;
    public Text valueInput;

    //Makes a certain UI element switch
    public void FromToButton()
    {
        from.SetActive(false);
        to.SetActive(true);
    }

    //changes the valueInput to the slider value
    public void OnSliderChange()
    {
        valueInput.text = slider.value.ToString();
    }

    //Application.Quit()
    public void ExitButton()
    {
        Application.Quit();
    }
}
