using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject[] Weapons;
    public int curWeaponId = -1;
    float weaponTouchTimer = 0f;
    float weaponMinTouchTimeforReload = 1.5f;

    public Action<int> itemSet;
    public Func<int, Gun> getGunInfo; 

    public static UIManager Instance
    {
        get
        {
           if(instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    private static UIManager instance;

    bool[] weaponBtnisUp;

    private void OnEnable()
    {
        ClickWeaponSetUp();
    }

    private void Start()
    {
        startAmmoSetUp();
    }

    void ClickWeaponSetUp()
    {
        for (int i = 0; i < Weapons.Length; i++) //��ư �ִϸ��̼� �Ҵ�
        {
            Weapons[i].GetComponent<WeaponBtnTouch>().id = i;
        }

        weaponBtnisUp = new bool[Weapons.Length];//��ư�� ���� ������ ���� ���·� �ʱ�ȭ

        for (int i = 0; i < Weapons.Length; i++)
        {
            weaponBtnisUp[i] = false;
        }
    }

    void startAmmoSetUp()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (getGunInfo(i) != null)
            {
                Weapons[i].transform.GetChild(1).GetComponent<Image>().sprite = getGunInfo(i).gunImg;
                Weapons[i].transform.GetChild(2).GetComponent<Slider>().maxValue = getGunInfo(i).ammoCapacity;
                Weapons[i].transform.GetChild(2).GetComponent<Slider>().value = getGunInfo(i).ammoCapacity;
                Weapons[i].transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + getGunInfo(i).ammoCapacity;
            }
        }
    }

    public void ShowCurAmmo(int id)//playershooter���� ����. shot�� �ؼ� gun �Լ��κ��� ammo�� -�� ���� ���� �Ѿ� UI�� ǥ���ϱ� ���� �Լ�
    {
        Weapons[id].transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + getGunInfo(id).curAmmo;
        Weapons[id].transform.GetChild(2).GetComponent<Slider>().value = getGunInfo(id).curAmmo;
    }

    public void WeaponBtnUpandDownAnim(int id)
    {
        if(!weaponBtnisUp[id])//�� ��ư�� ������ ���� ���¿��� ��ư�� ������ ���� ��¦ �ö󰡴� �ִϸ��̼� �߰�
        {
            Weapons[id].GetComponent<RectTransform>().DOMoveY(87.5f, 1).SetEase(Ease.InOutElastic);
            weaponBtnisUp[id] = true;
            curWeaponId = id;
        }
        for (int i = 0; i < Weapons.Length; i++) //������ ���� ��ư�� ������ ���������� ��.
        {
            if (!weaponBtnisUp[id] || i == id) continue;
            Weapons[i].GetComponent<RectTransform>().DOMoveY(37.5f, 1).SetEase(Ease.InOutElastic);
            weaponBtnisUp[i] = false;
        }
    }
}
