using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class HaveWeaponContent : MonoBehaviour
{
    [SerializeField] GameObject[] guns;
    [SerializeField] GameObject weaponBox;
    [SerializeField] UpperPanel upperPanel;
    [SerializeField] GameObject gunInfoPanel;
    public Action<int> EquipBtnActive;
    public Action<int> EquipWeapon;
    public Action<int> InfoWeapon;
    int curId = 0;
    int prevId = 0;

    bool isInfoOpen = false;   

    private void Start()
    {
        gunInfoPanel.SetActive(false);
        gunInfoPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            CloseInfoPanel()
        );//총 정보 패널 닫기

        SettingHaveWeaponBox();
        EquipBtnActive += ClickWeaponBox;//각각의 weapon 버튼에서 고유의 id값을 받아 실행될 것
        EquipWeapon += ClickEquipBtn;
        InfoWeapon += ClickInfoBtn;
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

    void InfoPanelAnim()
    {
        Sequence infoPanelSeq = DOTween.Sequence();
        StartCoroutine(InfoPanelAnimSound());
        infoPanelSeq.Append(gunInfoPanel.transform.DOScale(Vector3.one * 1.5f, 1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(2).transform.DOScale(Vector3.one * 1f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(3).transform.DOScale(Vector3.one * 0.7f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(4).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(5).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(6).transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(7).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(8).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(9).transform.DOScale(Vector3.one * 0.7f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(10).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(11).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(12).transform.DOScale(Vector3.one * 0.7f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(13).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(14).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(15).transform.DOScale(Vector3.one * 0.7f, 0.2f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(16).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack))
            .Append(gunInfoPanel.transform.GetChild(17).transform.DOScale(Vector3.one * 1f, 0.1f).SetEase(Ease.OutBack));
    }

    IEnumerator InfoPanelAnimSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.instance.WhickSound();
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.WhickSound();
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.WhickSound();
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.WhickSound();
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.WhickSound();
    }

    void ClickInfoBtn(int id)
    {
        SoundManager.instance.WeaponBtnClickSound();

        gunInfoPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = guns[id].GetComponent<Gun>().kind.ToString();
        gunInfoPanel.transform.GetChild(2).GetComponent<Image>().sprite = guns[id].GetComponent<Gun>().gunImg;

        TextMeshProUGUI[] text = new TextMeshProUGUI[5];
        int index = 0;
        for(int i = 5; i <= 17; i+=3)
        {
            text[index++] = gunInfoPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        text[0].text = guns[id].GetComponent<Gun>().ammoCapacity.ToString();
        text[1].text = guns[id].GetComponent<Gun>().reloadTime.ToString() + " s";
        text[2].text = guns[id].GetComponent<Gun>().fireInterval.ToString() + " s";
        text[3].text = guns[id].GetComponent<Gun>().damage.ToString();
        text[4].text = string.Format("{0:F1}", guns[id].GetComponent<Gun>().fireDistance.ToString()) + " m";
        gunInfoPanel.SetActive(true);

        if(!isInfoOpen)
        {
            InfoPanelAnim();
        }

        isInfoOpen = true;
    }

    void CloseInfoPanelAnim()
    {
        gunInfoPanel.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
    }

    void CloseInfoPanel()
    {
        gunInfoPanel.SetActive(false);
        SoundManager.instance.WeaponBtnClickSound();

        for(int i = 2; i < gunInfoPanel.transform.childCount; i++)
        {
            gunInfoPanel.transform.GetChild(i).GetComponent<RectTransform>().localScale = Vector3.zero;//다음 애니메이션을 위해 scale zero
        }

        gunInfoPanel.transform.localScale = Vector3.zero;
        isInfoOpen = false;
    }
}
