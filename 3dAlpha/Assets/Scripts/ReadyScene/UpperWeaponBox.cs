using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpperWeaponBox : MonoBehaviour, IPointerDownHandler
{
    public int id;
    public UpperPanel upperPanel;
    public void OnPointerDown(PointerEventData eventData)
    {
        upperPanel.deEquip(id);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
