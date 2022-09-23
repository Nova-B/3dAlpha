using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] Gun gun;
    [SerializeField] FieldOfView fov;
    [SerializeField] PlayerInput playerInput;

    Animator animator;

    [SerializeField] GameObject weaponHand;
    [SerializeField] List<GameObject> weapons = new List<GameObject>();

    public LayerMask targetLayer;
    public int curWeaponId { get; private set; }

    float timer;

    private void Awake()
    {
        WeaponSetup();
        UIManager.Instance.itemSet += ChangeWeapon;
        UIManager.Instance.getGunInfo += ThrowGunInfo;//플레이어가 현재 들고 있는 Gun script를 던져줌
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = 0;
    }

    void WeaponSetup() //들고있는 무기 배열에 저장. 전부 비활성화
    {
        for(int i = 0; i < EquipWeaponData.instance.equipGun.Count; i++)
        {
            weapons.Add(Instantiate(EquipWeaponData.instance.equipGun[i], weaponHand.transform));
        }

        /*if (weaponHand.transform.childCount < 4)
        {
            weapons = new GameObject[weaponHand.transform.childCount];
        }
        else if (weaponHand.transform.childCount >= 4)
        {
            weapons = new GameObject[4];
        }*/
        for (int i = 0; i < weapons.Count; i++)
        {
            //weapons[i] = weaponHand.transform.GetChild(i).gameObject;
            //weapons[i].SetActive(false);
            if (weapons[i].GetComponent<MeshRenderer>() != null)
            {
                Debug.Log("Yes");
                weapons[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public Gun ThrowGunInfo(int id)
    {
        return weapons[id].GetComponent<Gun>();
    }

    void ChangeWeapon(int id) //UI매니저의 액션에 넣을 것. 누른 버튼에 따라 무기활성화, 비활성화 + PlayerShooter의 gun 변수 재할당
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            if(id == i)
            {
                weapons[i].GetComponent<MeshRenderer>().enabled = true;
                //weapons[i].SetActive(true);
                fov.viewRadius = weapons[i].GetComponent<Gun>().fireDistance; //field of view 볼 수 있는 범위 변경
                gun = weapons[i].GetComponent<Gun>();
                curWeaponId = id;
            }
            else
            {
                //weapons[i].SetActive(false);
                weapons[i].GetComponent<MeshRenderer>().enabled = false;

            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Shot();
    }

    void Shot()
    {
        if(!fov.hasTarget)
        {
            Enemy.isTargeted = false;
        }
        else
        {
            Enemy.isTargeted = true;
        }
        if (gun != null && (gun.state == Gun.State.Reloading || gun.state == Gun.State.Empty))
        {
            timer = 0;
            return;
        }
        if (gun != null && fov.hasTarget)
        {

            for (int i = 0; i < fov.visibleTargets.Count; i++)
            {
                if (i == fov.nearestDistIndex)
                {
                    if(!fov.visibleTargets[i].GetComponent<Enemy>().dead)
                    {
                        fov.visibleTargets[i].GetComponent<Enemy>().isTargetingImageObj.SetActive(true);
                        continue;
                    }
                }
                fov.visibleTargets[i].GetComponent<Enemy>().isTargetingImageObj.SetActive(false);
            }

            if(playerInput._MoveVec == Vector3.zero)
            {
                RaycastHit hit;
                Vector3 hitPosition = Vector3.zero;
                Vector3 dir = fov.visibleTargets[fov.nearestDistIndex].position - transform.position;
                dir.y = 1.5f;
                Debug.DrawLine(transform.position, transform.position + dir * 50);

                if (Physics.Raycast(transform.position, dir, out hit, gun.fireDistance, targetLayer))
                {
                    Debug.Log(hit.collider.gameObject.name);

                    IDamageble target = hit.collider.gameObject.GetComponent<IDamageble>();
                    if (target != null)
                    {
                        Debug.Log("fire");
                        timer += Time.deltaTime;
                        if (timer > gun.fireInterval && gun.curAmmo > 0)
                        {
                            timer = 0;
                            StartCoroutine(ShotAnim(gun.fireInterval, fov.visibleTargets[fov.nearestDistIndex].position));
                        }
                        hitPosition = hit.point;
                    }
                    else
                    {
                        timer = 0;
                        hitPosition = transform.position + transform.forward * gun.fireDistance;
                    }
                }
            }
        }
    }

    IEnumerator ShotAnim(float delay, Vector3 enemyVec)
    {
        Debug.Log("shot");
        animator.SetTrigger("Shot");
        //yield return new WaitForSeconds(0.3f);
        gun.Shot(enemyVec);
        UIManager.Instance.ShowCurAmmo(curWeaponId);
        yield return new WaitForSeconds(delay);
            
    }
}
