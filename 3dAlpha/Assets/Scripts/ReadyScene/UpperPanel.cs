using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class UpperPanel : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject player_rHand;
    [SerializeField] HaveWeaponContent haveWeaponContent;
    [SerializeField] GameObject[] weaponsBox;
    [SerializeField] GameObject[] guns;
    [SerializeField] bool[] hasWeapon;

    Dictionary<int, int> weaponLinkId = new Dictionary<int, int>();
    public Action<int> deEquip;
    
    Vector2 prevTouchPos;
    int weaponBoxId = 0;

    GameObject equipWeapon;

    private void Start()
    {
        deEquip += DeEquipGun;
    }

    private void Update()
    {
        PlayerRotation();
    }

    #region Player
    void PlayerRotation()
    {
        if (Input.touchCount == 1)//Rotation
        {
            Touch touchZero = Input.GetTouch(0);
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled)
            {
                return;
            }
            if (touchZero.phase == TouchPhase.Began)
            {
                prevTouchPos = Input.GetTouch(0).position;
            }
            else
            {
                var deltaY = -(Input.mousePosition.x - prevTouchPos.x) * 0.6f;

                player.transform.Rotate(0, deltaY, 0, Space.Self);
                prevTouchPos = Input.GetTouch(0).position;
            }

        }
    }
    #endregion

    #region Weapon
    void FindEmptyBox()
    {
        for(int i = 0; i < hasWeapon.Length; i++)
        {
            if(!hasWeapon[i])//장착되지 않은 가장 빠른 슬롯 찾기
            {
                hasWeapon[i] = true;
                weaponBoxId = i;
                return;
            }
        }
        weaponBoxId = -1;
    }
    void ShowGun()
    {
        for (int i = 0; i < hasWeapon.Length; i++)
        {
            if (hasWeapon[i]) //장착한 무기중 가장 첫 번째 무기 보여주기
            {
                if(equipWeapon != null)
                {
                    Destroy(equipWeapon);
                }
                equipWeapon = Instantiate(guns[i], player_rHand.transform);
                return;
            }
        }
        Destroy(equipWeapon); //무기가 하나도 장착되있지 않은 경우 파괴
    }
    public void EquipGun(GameObject gun, int id) //action에 추가될 함수. 하단 패널 content에서 장비할 gun 정보와 그 아이템의 index 정보를 받는다. 해체할 때를 대비해 상단 itembox ID와 하단 itembox ID는 딕셔너리에 저장된다
    {
        FindEmptyBox();
        guns[weaponBoxId] = gun;
        weaponsBox[weaponBoxId].transform.GetChild(1).GetComponent<Image>().sprite = gun.GetComponent<Gun>().gunImg;
        weaponsBox[weaponBoxId].transform.GetChild(1).gameObject.SetActive(true);
        weaponLinkId.Add(weaponBoxId, id);
        ShowGun();
    }

    public void DeEquipGun(int keyId) //action에 추가될 함수. 상단 패널의 ItemBox에서 각각의 터치 값을 읽을 때 사용될 것임. keyId와 하단 패널의 아이템 id는 dictionary구조로 연결되어 있다. keyId에 해당하는 하단 패널 아이템을 장착 가능하다는 bool값 변경을 위해 사용한다
    {
        weaponsBox[keyId].transform.GetChild(1).GetComponent<Image>().sprite = null;
        weaponsBox[keyId].transform.GetChild(1).gameObject.SetActive(false);
        hasWeapon[keyId] = false;
        haveWeaponContent.DeEquipWeapon(weaponLinkId[keyId]);//장착 가능 bool 값으로 변경
        weaponLinkId.Remove(keyId);//dictionary 값 삭제
        ShowGun();
        
    }
    #endregion

    public void ClickPlayBtn()
    {
        EquipWeaponData.instance.Save(guns);
        //DataManager.instance.Save(guns);
        SceneManager.LoadScene(2);
    }
}
