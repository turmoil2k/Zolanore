using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderNumScript : MonoBehaviour
{
    Slider sliderSetting;
    TextMeshProUGUI sliderText;
    // Start is called before the first frame update

    private void Awake()
    {
        sliderSetting = GetComponentInChildren<Slider>();
        sliderText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTextValue()
    {
        sliderText.text = sliderSetting.value.ToString();
    }

    private void OnEnable()
    {
        sliderText.text = sliderSetting.value.ToString();
    }
}