using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReloadPanel : MonoBehaviour
{
    public bool reload = false;
    public float reloadTime;
    TextMeshProUGUI percentText;
    Slider percentSlider;
    float current = 0;
    float percent = 0;
    


    private void Start()
    {
        percentSlider = GetComponentInChildren<Slider>();
        percentSlider.maxValue = 1;
        percentText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

    }

    public void SetReloadtime(float time)
    {
        reloadTime = time;
    }

    private void Update()
    {
        current += Time.deltaTime;
        if(percent < 1)
        { 
            percent = current / reloadTime;

            percentSlider.value = percent;
            percentText.text = "" + (int)(percent * 100);
        }
        else
        {
            current = 0;
            percent = 0;
            gameObject.SetActive(false);
        }
    }

}
