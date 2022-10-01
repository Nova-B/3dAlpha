using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject[] Weapons;
    public int curWeaponId = -1;
    //float weaponTouchTimer = 0f;
    //float weaponMinTouchTimeforReload = 1.5f;

    public Action<int> itemSet;
    public Func<int, Gun> getGunInfo;

    [SerializeField] GameObject canvas;

    //pause
    [SerializeField] GameObject pauseBtn;
    [SerializeField] GameObject pausePanel;

    //Dead
    [SerializeField] GameObject deadPanel;
    PlayerHealth playerHealth;

    //victory
    [SerializeField] GameObject victoryPanel;

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
        SettingUIVariable();
        startAmmoSetUp();
        playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        playerHealth.onDeath += () => deadPanel.SetActive(true);
        playerHealth.onDeath += () => SoundManager.instance.MissionFailSound();
        pausePanel.SetActive(false);
    }

    #region Setting

    void SettingUIVariable()
    {
        canvas = GameObject.Find("Canvas");

        pauseBtn = canvas.transform.GetChild(3).gameObject;
        pausePanel = canvas.transform.GetChild(5).gameObject;
        deadPanel = canvas.transform.GetChild(6).gameObject;
        victoryPanel = canvas.transform.GetChild(7).gameObject;

        pauseBtn.GetComponent<Button>().onClick.AddListener(() =>
            Pause()
        );

        pausePanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            Continue()
        );
        pausePanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            Restart()
        );
    }

    void ClickWeaponSetUp()
    {
        for (int i = 0; i < Weapons.Length; i++) //버튼 애니메이션 할당
        {
            Weapons[i].GetComponent<WeaponBtnTouch>().id = i;
        }

        weaponBtnisUp = new bool[Weapons.Length];//버튼을 전부 누르지 않은 상태로 초기화

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
            else
            {
                Weapons[i].transform.GetChild(1).GetComponent<Image>().enabled = false;
            }
        }
    }
    #endregion

    #region WeaponUI
    public void ShowCurAmmo(int id)//playershooter에서 사용됨. shot을 해서 gun 함수로부터 ammo가 -된 이후 남은 총알 UI를 표시하기 위한 함수
    {
        Weapons[id].transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + getGunInfo(id).curAmmo;
        Weapons[id].transform.GetChild(2).GetComponent<Slider>().value = getGunInfo(id).curAmmo;
    }

    public void WeaponBtnUpandDownAnim(int id)
    {
        if(!weaponBtnisUp[id])//총 버튼을 누르지 않은 상태에서 버튼을 누르면 위로 살짝 올라가는 애니메이션 추가
        {
            Weapons[id].GetComponent<RectTransform>().DOMoveY(87.5f, 1).SetEase(Ease.InOutElastic);
            weaponBtnisUp[id] = true;
            curWeaponId = id;
        }
        for (int i = 0; i < Weapons.Length; i++) //이전에 누른 버튼이 있으면 내려가도록 함.
        {
            if (!weaponBtnisUp[id] || i == id) continue;
            Weapons[i].GetComponent<RectTransform>().DOMoveY(37.5f, 1).SetEase(Ease.InOutElastic);
            weaponBtnisUp[i] = false;
        }
    }
    #endregion

    #region PauseUI
    public void Pause()
    {
        Time.timeScale = 0;
        SoundManager.instance.BtnClickSound();
        pausePanel.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        SoundManager.instance.BtnClickSound();
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        EquipWeaponData.instance.equipGun.Clear();
        SoundManager.instance.BtnClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        PlayerHealth.curHealth = playerHealth.startHealth;
    }
    #endregion

    #region Victory
    public void Victory()
    {
        SoundManager.instance.MissionClearSound();
        victoryPanel.SetActive(true);
    }
    #endregion
}
