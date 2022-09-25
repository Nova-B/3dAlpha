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
            if(!hasWeapon[i])//�������� ���� ���� ���� ���� ã��
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
            if (hasWeapon[i]) //������ ������ ���� ù ��° ���� �����ֱ�
            {
                if(equipWeapon != null)
                {
                    Destroy(equipWeapon);
                }
                equipWeapon = Instantiate(guns[i], player_rHand.transform);
                return;
            }
        }
        Destroy(equipWeapon); //���Ⱑ �ϳ��� ���������� ���� ��� �ı�
    }
    public void EquipGun(GameObject gun, int id) //action�� �߰��� �Լ�. �ϴ� �г� content���� ����� gun ������ �� �������� index ������ �޴´�. ��ü�� ���� ����� ��� itembox ID�� �ϴ� itembox ID�� ��ųʸ��� ����ȴ�
    {
        FindEmptyBox();
        guns[weaponBoxId] = gun;
        weaponsBox[weaponBoxId].transform.GetChild(1).GetComponent<Image>().sprite = gun.GetComponent<Gun>().gunImg;
        weaponsBox[weaponBoxId].transform.GetChild(1).gameObject.SetActive(true);
        weaponLinkId.Add(weaponBoxId, id);
        ShowGun();
    }

    public void DeEquipGun(int keyId) //action�� �߰��� �Լ�. ��� �г��� ItemBox���� ������ ��ġ ���� ���� �� ���� ����. keyId�� �ϴ� �г��� ������ id�� dictionary������ ����Ǿ� �ִ�. keyId�� �ش��ϴ� �ϴ� �г� �������� ���� �����ϴٴ� bool�� ������ ���� ����Ѵ�
    {
        weaponsBox[keyId].transform.GetChild(1).GetComponent<Image>().sprite = null;
        weaponsBox[keyId].transform.GetChild(1).gameObject.SetActive(false);
        hasWeapon[keyId] = false;
        haveWeaponContent.DeEquipWeapon(weaponLinkId[keyId]);//���� ���� bool ������ ����
        weaponLinkId.Remove(keyId);//dictionary �� ����
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
