using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class Elevator : MonoBehaviour
{
    enum Kind { start, end};
    [SerializeField] Kind kindElvator;
    GameObject doorCollider;
    [SerializeField] CinemachineVirtualCamera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        MoveStartElevator();
        SettingStartEndDoor();
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
        }
    }

    void CloseStartElevatorDoor()
    {
        //���� ���������Ϳ��� ���� ���� �� �������� collider Ȱ��ȭ
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
            other.GetComponent<PlayerMovement>().enabled = false;//�÷��̾� ������ ��Ȱ��ȭ
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
