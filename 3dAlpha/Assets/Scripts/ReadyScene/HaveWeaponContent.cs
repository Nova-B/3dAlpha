using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class HaveWeaponContent : MonoBehaviour
{
    [SerializeField] GameObject[] guns;
    [SerializeField] GameObject weaponBox;
    [SerializeField] UpperPanel upperPanel;
    bool isClick;
    bool isSelect;
    public Action<int> EquipBtnActive;
    public Action<int> EquipWeapon;
    int curId = 0;
    int prevId = 0;

    private void Start()
    {
        SettingHaveWeaponBox();
        EquipBtnActive += ClickWeaponBox;//각각의 weapon 버튼에서 고유의 id값을 받아 실행될 것
        EquipWeapon += ClickEquipBtn;
    }

    void SettingHaveWeaponBox()
    {
        for(int i = 0; i < guns.Length; i++)
        {
            GameObject box = Instantiate(weaponBox, transform);
            box.AddComponent<HaveWeaponBtn>();
            box.GetComponent<HaveWeaponBtn>().id = i;
            box.GetComponent<HaveWeaponBtn>().gun = guns[i].GetComponent<Gun>();
            box.transform.GetChild(2).GetComponent<Image>().sprite = guns[i].GetComponent<Gun>().gunImg;
            box.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    void ClickWeaponBox(int id)
    {
        //장비 버튼 활성화
        //이전에 누른 장비 버튼은 비활성화 되도록 함
        prevId = curId;
        curId = id;
        if (prevId == curId && prevId != 0)
        {
            prevId--;
        }
        else if(prevId == curId && prevId == 0)
        {
            prevId++;
        }
        transform.GetChild(prevId).GetComponent<HaveWeaponBtn>().ClickPanel(false);
        transform.GetChild(prevId).GetComponent<HaveWeaponBtn>().isOpen = false;
    }

    void ClickEquipBtn(int id)
    {
        SoundManager.instance.WeaponBtnClickSound();
        if (transform.GetChild(id).GetComponent<HaveWeaponBtn>().isEquip) return;
        upperPanel.EquipGun(guns[id], id);
    }

    public void DeEquipWeapon(int id)
    {
        SoundManager.instance.DeEquipWeaponSound();
        transform.GetChild(id).GetComponent<HaveWeaponBtn>().isEquip = false;
    }
}
