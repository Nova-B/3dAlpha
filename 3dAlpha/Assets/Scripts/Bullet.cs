using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    private void OnEnable()
    {
        Destroy(gameObject, 5f);
    }
}
