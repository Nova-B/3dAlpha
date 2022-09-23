using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class Enemy : LivingEntity, IDamageble
{
    public static bool isTargeted;
    public static int EnemyCount
    {
        get
        {
            return enemyCount;
        }

        private set { }
    }
    private static int enemyCount;
    public enum Kind { kinfe, gun };
    [SerializeField] Kind kind;

    Slider hpSlider;
    TextMeshProUGUI hpText;
    PlayerShooter playerShooter;

    Rigidbody rigid;
    Animator animator;
    FieldOfView enemy_fov;
    NavMeshAgent nav;

    public AnimationClip attack;
    float attackAnimTime;
    public GameObject isTargetingImageObj;
    public static Transform targetPlayer;

    [Header("action")]
    [SerializeField] float waitTime;
    [SerializeField] float walkTime;
    [SerializeField] float walkSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] bool isNormalAciton;
    float speed;
    int moveId = 1;
    Vector3 direction;
    bool isWalking;
    bool isChase;
    bool isRun;
    float currentTime;

    private void Start()
    {
        enemyCount += 1;
        if(targetPlayer == null)
        {
            targetPlayer = FindObjectOfType<PlayerShooter>().gameObject.transform;
            Debug.Log("Ȯ��");
        }
        isTargetingImageObj.SetActive(false);
        isNormalAciton = true;
        currentTime = waitTime;
        WhenDeath();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemy_fov = GetComponent<FieldOfView>();
        attackAnimTime = attack.length;
    }

    void WhenDeath()
    {
        onDeath += () => isTargetingImageObj.SetActive(false);//Ÿ�ϵ� �� �ߴ� �̹��� ���ֱ�
        onDeath += () => GetComponent<BoxCollider>().enabled = false;//������ �Ѿ� ����� ����
        onDeath += () => enemyCount -= 1;//�������� �� �� ī����
        onDeath += () => Destroy(gameObject, 2f);//�� ����
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hpSlider = GetComponentInChildren<Slider>();
        hpText = GetComponentInChildren<TextMeshProUGUI>();
        playerShooter = FindObjectOfType<PlayerShooter>();
    }
    
     void DieAnim()
    {
        animator.SetTrigger("Dead");
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dead) return;
        base.OnDamage(damage, hitPoint, hitNormal);
        if(health <= 0)
        {
            animator.speed = 0.7f;
            Die();
            nav.enabled = false;
            //Invoke("DieAnim", 0.1f);
            StartCoroutine(DelayAnimTrigger(0.1f, "Dead"));
        }
        hpSlider.value = health;
        hpText.text = "" + health;
    }

    IEnumerator DelayAnimTrigger(float delay, string triggerName)
    {
        Debug.Log("Die");
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(triggerName);
    }

    private void Update()
    {
        if(!playerShooter.gameObject.GetComponent<PlayerHealth>().dead)
        {
            ElapseTime();
            Move();
            Rotation();
            if(!isTargeted)
            {
                isTargetingImageObj.SetActive(false);
            }
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }
    #region About Action
    void ElapseTime()
    {
        if (dead) return;
        if(isNormalAciton)
        {
            currentTime -= Time.deltaTime;
            if(currentTime<=0)
            {
                NormalEnemyAction();
            }
        }
    }

    void Move()
    {
        if (dead) return;
        Patrol();
        if (isWalking && !dead)
        {
            rigid.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
        if(isChase && !dead)
        {
            Chase();
        }
    }

    void NormalEnemyAction()
    {
        animator.speed = 0.8f;
        moveId *= -1;
        if(moveId == -1)
        {
            Wait();
        }
        else if(moveId == 1)
        {
            direction.Set(0, Random.Range(0, 360), 0);
            Walk();
        }
    }

    void Rotation()
    {
        if (dead) return;
        if (isWalking )
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0, direction.y, 0), 0.01f);
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    void Wait()
    {
        waitTime = Random.Range(1.5f, 2.5f);
        currentTime = waitTime;
        isWalking = false;
        animator.SetBool("Walking", isWalking);
    }

    void Walk()
    {
        walkTime = Random.Range(1.5f, 4.5f);
        currentTime = walkTime;
        speed = walkSpeed;
        isWalking = true;
        animator.SetBool("Walking", isWalking);
    }

    void Patrol()
    {
        /*if(enemy_fov.hasTarget && !isChase)
        {
            animator.SetTrigger("Find");
        }*/
        if(enemy_fov.hasTarget || health <startHealth)
        {
            //targetPlayer = enemy_fov.visibleTargets[0];
            isWalking = false;
            isChase = true;
        }
    }

    void Chase()
    {
        if (dead) return;
        isNormalAciton = false;
        speed = chaseSpeed;
        if (targetPlayer != null && nav != null)
        {
            nav.SetDestination(targetPlayer.position);
            if(Vector3.Distance(targetPlayer.position, transform.position) < 1.5f)
            {
                animator.speed = 0.7f;
                isRun = false;
                currentTime += Time.deltaTime;
                if(currentTime > attackAnimTime + 1f && !dead)
                {
                    currentTime = 0;
                    animator.SetTrigger("Attack");
                    if (Vector3.Distance(targetPlayer.position, transform.position) < 1.5f)
                    {
                        targetPlayer.GetComponent<LivingEntity>().OnDamage(10, transform.position, transform.position);
                    }
                }
            }
            else
            {
                animator.speed = 1.5f;
                isRun = true;
            }
        }
        animator.SetBool("Walking", isRun);
    }


    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            float damage = 0;
            if(other.gameObject.GetComponent<Bullet>() != null)
            {
                damage = other.gameObject.GetComponent<Bullet>().damage;
            }
            //Debug.Log("Damage" + playerShooter.ThrowGunInfo(playerShooter.curWeaponId).damage);
            OnDamage(damage, transform.position, transform.forward);
            Destroy(other.gameObject);
        }
    }
}