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
                rigid.angularVelocity = Vector3.zero;//잔여 움직임 멈추기
                if (player_fov.hasTarget)
                {
                    Vector3 dir = (player_fov.visibleTargets[player_fov.nearestDistIndex].position - transform.position).normalized;//가장 가까운 적의 방향
                    dir.y = 0;//적 크기가 클시 dir이 기울어짐을 방지
                    dir = Quaternion.AngleAxis(65, Vector3.up) * dir; //총 쏘는 애니메이션이 90도 가까이 돌아가 있어서 맞추어주는 과정
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
