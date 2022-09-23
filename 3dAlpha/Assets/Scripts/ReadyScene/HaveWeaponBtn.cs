using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HaveWeaponBtn : MonoBehaviour, IPointerDownHandler
{
    public int id;
    public bool isEquip = false;
    GameObject clickPanel;
    GameObject equipBtn;
    public Gun gun;
    public bool isOpen = false;


    private void Start()
    {
        clickPanel = transform.GetChild(0).gameObject;
        equipBtn = clickPanel.transform.GetChild(0).gameObject;
        equipBtn.GetComponent<Button>().onClick.AddListener( () =>
             transform.parent.gameObject.GetComponent<HaveWeaponContent>().EquipWeapon(id)
        );
        equipBtn.GetComponent<Button>().onClick.AddListener(() =>
            isEquip = true
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*isClick = !isClick;
        clickPanel.SetActive(isClick);*/
        SoundManager.instance.OpenUISound();
        if(transform.parent.gameObject.GetComponent<HaveWeaponContent>() != null)
        {
            transform.parent.gameObject.GetComponent<HaveWeaponContent>().EquipBtnActive(id);
        }
        isOpen = !isOpen;
        clickPanel.SetActive(isOpen);
    }
    public void ClickPanel(bool condition)
    {
        clickPanel.SetActive(condition);
        //isOpen = condition;
    }
}