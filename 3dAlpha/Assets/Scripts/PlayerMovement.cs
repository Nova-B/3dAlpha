using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] FieldOfView player_fov;
    Rigidbody rigid;
    Animator animator;

    float moveSpeed = 5;
    float rotSpeed = 5000;

    void SetUp()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        SetUp();
        player_fov = GetComponent<FieldOfView>();
    }

    public void Move(Vector3 movement)
    {
        if(playerInput != null && rigid != null)
        {
            if(playerInput.walking)
            {
                rigid.MovePosition(rigid.position + movement * moveSpeed * Time.deltaTime);
                if(movement!= Vector3.zero)
                {
                    animator.SetBool("Walk", true);
                    transform.rotation = Quaternion.Slerp(rigid.rotation, Quaternion.LookRotation(movement), rotSpeed * Time.deltaTime);
                }
            }
            else
            {
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;//�ܿ� ������ ���߱�
                if (player_fov.hasTarget)
                {
                    Vector3 dir = (player_fov.visibleTargets[player_fov.nearestDistIndex].position - transform.position).normalized;//���� ����� ���� ����
                    dir.y = 0;//�� ũ�Ⱑ Ŭ�� dir�� �������� ����
                    dir = Quaternion.AngleAxis(65, Vector3.up) * dir; //�� ��� �ִϸ��̼��� 90�� ������ ���ư� �־ ���߾��ִ� ����
                    transform.rotation = Quaternion.Slerp(rigid.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
                }
            }
        }
        MoveAnim();
    }

    void MoveAnim()
    {
        if (playerInput != null)
        {
            animator.SetBool("Walk", playerInput.walking);
        }
    }
    private void FixedUpdate()
    {
        Move(playerInput._MoveVec);
    }
}
