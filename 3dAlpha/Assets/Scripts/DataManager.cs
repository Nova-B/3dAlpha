using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    EquipData data = new EquipData();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public void Save(GameObject[] obj)
    {
        for(int i = 0; i < obj.Length; i++)
        {
            data.equipData.Add(obj[i]);
        }

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameObject));
        FileStream stream = new FileStream(Application.dataPath + "/StreamFiles/Game_data.xml", FileMode.Create);
        xmlSerializer.Serialize(stream, data);
        stream.Close();
    }
}

[System.Serializable]
public class EquipData
{
    public List<GameObject> equipData = new List<GameObject>();
}
