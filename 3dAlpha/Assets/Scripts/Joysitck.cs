using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joysitck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform pad;

    public Transform player;
    Vector3 moveForward;
    Vector3 moveRotate;
    public float moveSpeed;
    public float rotateSpeed;

    bool walking;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.5f);

        moveForward = new Vector3(0, 0, transform.localPosition.y).normalized;
        moveRotate = new Vector3(0, transform.localPosition.x, 0).normalized;

        if(!walking)
        {
            walking = true;
            player.GetComponent<Animator>().SetBool("Walk", walking);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine("PlayerMove");
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = Vector2.zero;
        moveForward = Vector3.zero;
        moveRotate = Vector3.zero;
        StopCoroutine("PlayerMove");

        walking = false;
        player.GetComponent<Animator>().SetBool("Walk", walking);
    }

    IEnumerator PlayerMove()
    {
        while(true)
        {
            player.Translate(moveForward * moveSpeed * Time.deltaTime);
            if(Mathf.Abs(transform.localPosition.x) > pad.rect.width*0.1f)
            {
                Debug.Log(1);
                player.Rotate(moveRotate * rotateSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
