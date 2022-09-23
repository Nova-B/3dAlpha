using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeaponData : MonoBehaviour
{
    public static EquipWeaponData instance;

    public List<GameObject> equipGun;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    public void Save(GameObject[] objs)
    {
        for(int i = 0; i < 4; i++)
        {
            if (objs[i] == null) continue;
            equipGun.Add(objs[i]);
        }
    }
}
