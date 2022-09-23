using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,
        Empty,
        Reloading
    }

    public enum Kind
    {
        Pistol,
        Submachine,
        Rifle
    }

    public State state { get; private set; }
    [SerializeField] Kind _kind;
    public Kind kind
    {
        get
        {
            return _kind;
        }
        private set{        }
    }
    public Transform fireTransform;

    [Header("UI�� ����")]
    public Sprite gunImg;

    [Header("���")]
    public float damage = 25;
    public float fireDistance = 50f;//�����Ÿ�

    [Header("�Ѿ�")]
    public int remainAmmo = 100;
    public int ammoCapacity = 8;
    public int curAmmo;
    public GameObject bullet;

    [Header("�ð�")]
    public float reloadTime = 1.8f;
    public float fireInterval = 0.12f;
    private float lastFireTime;

    [Header("��ƼŬ")]
    [SerializeField] GameObject muzzleObj;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        state = State.Ready;
        curAmmo = ammoCapacity;
    }


    public void Shot(Vector3 enemyPos)
    {
        StartCoroutine("ShotEffect"); //�߻�����Ʈ
        SoundManager.instance.GunShotSound(kind);
        if(bullet != null && state == State.Ready)
        {
            GameObject bulletInst = Instantiate(bullet, fireTransform.position, Quaternion.identity);
            if(bulletInst.GetComponent<Bullet>() != null)
            {
                bulletInst.GetComponent<Bullet>().damage = damage;
            }
            bulletInst.AddComponent<Rigidbody>();
            bulletInst.GetComponent<Rigidbody>().useGravity = false;
            enemyPos.y = fireTransform.position.y;
            bulletInst.GetComponent<Rigidbody>().velocity = (enemyPos - fireTransform.position).normalized * 10;
            curAmmo--;
        }
       
        /*if(curAmmo <= 0 && state == State.Ready)
        {
            curAmmo = 0;
            Reload();
            state = State.Empty;
        }*/
    }

    public void Reload(int id)
    {
        StartCoroutine(RelaodTime(id));
    }

    IEnumerator RelaodTime(int id)
    {
        state = State.Reloading;
        SoundManager.instance.GunReloadSound(kind);
        yield return new WaitForSeconds(reloadTime);
        curAmmo = ammoCapacity;
        UIManager.Instance.ShowCurAmmo(id);
        state = State.Ready;
    }

    IEnumerator ShotEffect()
    {
        yield return null;
        muzzleObj.GetComponent<ParticleSystem>().Play();
    }
}
