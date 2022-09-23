using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform pad;
    [SerializeField] RectTransform stick;

    [HideInInspector]
    Vector3 moveVec;
    [HideInInspector]
    public bool walking;

    Image panelImage;//�г��� �ι� ��ġ�� ���̽�ƽ�� �����̴� ���� �����ϱ� ����

    public Vector3 _MoveVec
    {
        get
        {
            return moveVec;
        }
        private set { }
    }

    private void Start()
    {
        panelImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pad.position = eventData.position;
        pad.gameObject.SetActive(true);
        panelImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;
        stick.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.5f);

        moveVec = new Vector3(stick.localPosition.x, 0, stick.localPosition.y).normalized;
        if(!walking)
        {
            walking = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector2.zero;
        pad.gameObject.SetActive(false);

        moveVec = Vector3.zero;
        walking = false;
        panelImage.raycastTarget = true;
    }
}
