using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : LivingEntity
{
    Slider hpSlider;
    TextMeshProUGUI hpText;
    Animator animator;

    public static float curHealth = 100f;
    protected override void OnEnable()
    {
        base.OnEnable();
        hpSlider = GetComponentInChildren<Slider>();
        hpText = GetComponentInChildren<TextMeshProUGUI>();
        health = curHealth;
        hpSlider.value = health;
        hpText.text = "" + health;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        hpSlider.value = health;
        hpText.text = "" + health;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        onDeath += () => WhenDead();
    }

    private void Update()
    {
        Debug.Log(health);
    }

    void WhenDead()
    {
        animator.SetTrigger("Die");
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<PlayerHealth>().enabled = false;
        gameObject.GetComponent<PlayerShooter>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            float damage = 0;
            if (other.gameObject.GetComponent<Bullet>() != null)
            {
                damage = other.gameObject.GetComponent<Bullet>().damage;
            }
            //Debug.Log("Damage" + playerShooter.ThrowGunInfo(playerShooter.curWeaponId).damage);
            OnDamage(damage, transform.position, transform.forward);
            Destroy(other.gameObject);
        }
    }
}
