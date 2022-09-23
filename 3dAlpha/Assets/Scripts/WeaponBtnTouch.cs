using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponBtnTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int id = -1;
    float minTouchTimeforReloading = 0.5f;
    ReloadPanel reloadPanel;

    private void Start()
    {
        reloadPanel = GetComponentInChildren<ReloadPanel>();
        reloadPanel.gameObject.SetActive(false);
    }
    public bool IsTouch { get; private set; }
    public float TouchTime { 
        get {
            return touchTime;
        }
        private set { }
    }
    float touchTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine("CalculateTouchTime");
        IsTouch = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("CalculateTouchTime");
        if(touchTime < minTouchTimeforReloading)
        {
            UIManager.Instance.WeaponBtnUpandDownAnim(id);
            UIManager.Instance.itemSet(id);
            SoundManager.instance.WeaponBtnClickSound();
        }
        else
        {
            if (UIManager.Instance.getGunInfo(id).curAmmo < UIManager.Instance.getGunInfo(id).ammoCapacity)
            {
                reloadPanel.gameObject.SetActive(true);
                UIManager.Instance.getGunInfo(id).Reload(id);
                reloadPanel.reloadTime = UIManager.Instance.getGunInfo(id).reloadTime;
            }
        }

        UIManager.Instance.ShowCurAmmo(id);
        touchTime = 0;
        IsTouch = false;
    }

    IEnumerator CalculateTouchTime()
    {
        while(true)
        {
            touchTime += Time.deltaTime;
            yield return null;
        }
    }
}
