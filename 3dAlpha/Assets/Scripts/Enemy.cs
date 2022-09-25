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
    public AnimationClip reload;
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

    [Header("Ragdoll")]
    [SerializeField] Rigidbody[] ragRigid;
    [SerializeField] Collider[] ragCol;

    [Header("Gun Enemy")]
    [SerializeField] Transform firePos;
    [SerializeField] GameObject bullet;
    [SerializeField] int leftAmmo;
    [SerializeField] int fullAmmo;
    [SerializeField] float damage;
    float reloadAnimTime;

    private void Awake()
    {
        enemyCount = 0;
    }

    private void Start()
    {
        enemyCount += 1;
        if(targetPlayer == null)
        {
            targetPlayer = FindObjectOfType<PlayerShooter>().gameObject.transform;
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
        reloadAnimTime = reload.length;

        //ragdoll
        ragRigid = transform.GetComponentsInChildren<Rigidbody>();
        ragCol = transform.GetComponentsInChildren<Collider>();
        RagdollOff();
    }

    void RagdollOff()
    {
        animator.enabled = true;
        nav.enabled = true;
        foreach(Rigidbody rb in ragRigid)
        {
            if (rb.gameObject.name == gameObject.name) continue;//본인 말고 자식만 활성화
            rb.isKinematic = true;
        }
        foreach(Collider col in ragCol)
        {
            if (col.gameObject.name == gameObject.name) continue;
            col.enabled = false;
        }
    }

    void RagdolOn()
    {
        animator.enabled = false;
        nav.enabled = false;
        foreach (Rigidbody rb in ragRigid)
        {
            if (rb.gameObject.name == gameObject.name) continue;
            rb.isKinematic = false;
        }
        foreach (Collider col in ragCol)
        {
            if (col.gameObject.name == gameObject.name) continue;
            col.enabled = true;
        }
    }

    void WhenDeath()
    {
        onDeath += RagdolOn;
        onDeath += () => isTargetingImageObj.SetActive(false);//타켓될 때 뜨는 이미지 없애기
        onDeath += () => GetComponent<BoxCollider>().enabled = false;//죽은뒤 총알 통과를 위해
        onDeath += () => enemyCount -= 1;//스테이지 적 수 카운팅
        //onDeath += () => Destroy(gameObject, 2f);//적 삭제
        
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
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(triggerName);
    }

    private void Update()
    {
        Debug.Log(enemyCount);
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
        
        if(kind == Kind.kinfe)
        {
            KnifeEnemyAttack();
        }
        else if(kind == Kind.gun)
        {
            GunEnemyAttack();
        }

        animator.SetBool("Walking", isRun);
    }

    void KnifeEnemyAttack()
    {
        if (targetPlayer != null && nav != null)
        {
            nav.SetDestination(targetPlayer.position);
            if (Vector3.Distance(targetPlayer.position, transform.position) < 1.5f)
            {
                animator.speed = 0.7f;
                isRun = false;
                currentTime += Time.deltaTime;
                if (currentTime > attackAnimTime + 1f && !dead)
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
    }

    void GunEnemyAttack()
    {
        if (targetPlayer != null && nav != null)
        {
            nav.SetDestination(targetPlayer.position);
            if (Vector3.Distance(targetPlayer.position, transform.position) < nav.stoppingDistance)
            {
                transform.LookAt(targetPlayer.position);

                animator.speed = 1f;
                isRun = false;
                currentTime += Time.deltaTime;
                if (currentTime > attackAnimTime + 1f && !dead)
                {
                    attackAnimTime = attack.length;
                    currentTime = 0;
                    animator.SetTrigger("Attack");
                    Shot();
                }
            }
            else
            {
                animator.speed = 1.5f;
                isRun = true;
            }
        }
    }

    void Shot()
    {
        leftAmmo--;
        if(leftAmmo < 0)
        {
            Debug.Log("?");
            attackAnimTime = reload.length;
            currentTime = 0;
            leftAmmo = fullAmmo;
            animator.SetTrigger("Reload");
            return;
        }
        GameObject bulletInst = Instantiate(bullet, firePos.position, Quaternion.identity);
        Vector3 targetPos = targetPlayer.position;
        targetPos.y = firePos.position.y;
        bulletInst.AddComponent<Rigidbody>();
        bulletInst.GetComponent<Rigidbody>().useGravity = false;
        bulletInst.GetComponent<Bullet>().damage = damage;
        bulletInst.GetComponent<Rigidbody>().velocity = (targetPos - firePos.position).normalized * 10;

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
