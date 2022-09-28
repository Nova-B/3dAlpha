using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

public class Elevator : MonoBehaviour
{
    enum Kind { start, end};
    [SerializeField] Kind kindElvator;
    GameObject doorCollider;
    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] GameObject fade;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        MoveStartElevator();
        SettingStartEndDoor();
        if (fade != null)
        { 
            fade.SetActive(false);
        }
    }

    void SettingStartEndDoor()
    {
        doorCollider = transform.GetChild(1).gameObject;
        if (kindElvator == Kind.start)
        {
            doorCollider.SetActive(false);
        }
        else if (kindElvator == Kind.end)
        {
            doorCollider.SetActive(true);
        }
    }

    void MoveStartElevator()
    {
        if (kindElvator == Kind.start)
        {
            transform.DOMoveY(0, 3f).SetEase(Ease.InOutSine);
            SoundManager.instance.StartEleLiftUpSound();
        }
    }

    /*Enumerator MoveEndElevator()
    {
        yield return new WaitForSeconds(0.5f);
        if (kindElvator == Kind.end)
        {
            transform.DOMoveY(12, 2f).SetEase(Ease.Linear);
        }
    }*/

    void MoveEndElevator()
    {
        if (kindElvator == Kind.end)
        {
            transform.DOMoveY(12, 4f).SetEase(Ease.InOutSine);
            SoundManager.instance.EndEleLiftUpSound();
            StartCoroutine(FadeOut(50f));
        }
    }

    void CloseStartElevatorDoor()
    {
        //시작 엘리베이터에서 나간 이후 못 들어오도록 collider 활성화
        if(doorCollider != null)
        {
            doorCollider.SetActive(true);
        }
    }

    void OpenEndElevatorDoor()
    {
        if(kindElvator == Kind.end)
        {
            if (doorCollider != null)
            {
                doorCollider.SetActive(false);
            }
        }
    }

    IEnumerator FadeOut(float animTime)
    {
        /*float time = 0;
        fade.SetActive(true);
        Color alpha = fade.GetComponent<Image>().color;
        while (time >= 1)
        {
            time += Time.deltaTime / animTime;

            alpha.a = Mathf.Lerp(0, 1, time);
            fade.GetComponent<Image>().color = alpha;
        }*/
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if(Enemy.EnemyCount == 0)
        {
            OpenEndElevatorDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (kindElvator == Kind.end && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerMovement>().enabled = false;//플레이어 움직임 비활성화
            other.transform.position = transform.position - Vector3.right * (1f);
            cam.LookAt = null;
            cam.Follow = null;
            MoveEndElevator();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (kindElvator == Kind.start && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CloseStartElevatorDoor();
        }
    }
}
