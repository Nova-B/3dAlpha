using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPBarController : MonoBehaviour
{
    TextMeshProUGUI hp_text;
    Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;    
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);
    }
}
