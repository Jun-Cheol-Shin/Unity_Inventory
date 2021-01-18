using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Valve.Newtonsoft.Json;

// json 파싱, 아이템 생성을 맡는 매니저
public class GameManager : MonoBehaviour
{
    public enum CODE
    {
        HEAD = 1,
        HAND = 2,
        ARMOR = 3,
        SHOES = 4,
        TWO_HAND = 5
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    List<Item> itemoffsetList;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        BoxComponent = BoxInventory.GetComponent<InventoryManager>();
        SaveInvenstr = "";
        SaveEquipstr = "";


        itemoffsetList = new List<Item>();
        itemoffsetList = LoadJsonFile<List<Item>>(Application.dataPath + "/SaveData", "Offset");

        for (int i = 0; i < itemoffsetList.Count; ++i)
        {
            itemoffsetList[i].Print();
            GameObject obj = CreateItemObject(itemoffsetList[i]);
            BoxComponent.AddItem(obj);
        }
    }

    // 해당 경로에 해당이름 Json파일을 불러옵니다.
    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        string jsonData = File.ReadAllText(string.Format("{0}/{1}.json", loadPath, fileName));
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    public GameObject PlayerInventory;
    public GameObject PlayerEquipment;
    public GameObject BoxInventory;

    [HideInInspector]
    public InventoryManager BoxComponent;

    GameObject CreateItemObject(Item item)
    {
        GameObject itemObj = Instantiate(Resources.Load("Prefabs/" + item.ItemName) as GameObject);
        if(itemObj == null)
        {
            Debug.LogFormat("{0} 생성 실패!", item.ItemName);
            return null;
        }
        itemObj.layer = LayerMask.NameToLayer("Item");
        Item itemComponent = itemObj.AddComponent<Item>();
        itemComponent.itemCode = item.itemCode;
        itemComponent.ItemName = item.ItemName;
        itemComponent.InvenRot = item.InvenRot;
        itemComponent.ItemScale = item.ItemScale;
        itemComponent.isInventory = item.isInventory;
        itemComponent.isEquip = item.isEquip;
        itemObj.transform.localScale = itemComponent.ItemScale;
        itemObj.name = itemComponent.name;

        itemObj.transform.position = new Vector3(0, 3, 0);

        return itemObj;
    }



    [HideInInspector]
    string SaveInvenstr;
    string SaveEquipstr;
    public void SaveData(InventoryManager inventory, EquipmentManager equip)
    {
        SaveInvenstr = "";
        SaveEquipstr = "";

        GameObject[,] Saveinven = inventory.GetInventory();

        for(int i=0; i<inventory.row; ++i)
        {
            for(int j=0; j<inventory.column; ++j)
            {
                if(Saveinven[i,j] != null)
                {
                    Item item = Saveinven[i, j].GetComponent<Item>();
                    SaveInvenstr += string.Format("{0},{1},{2}|", item.ItemName, i, j);
                }

                else
                {
                    SaveInvenstr += string.Format("null,{0},{1}|", i, j);
                }
            }
        }

        SaveInvenstr = SaveInvenstr.Substring(0, SaveInvenstr.Length - 1);

        Dictionary<string, GameObject> SaveEquip = equip.GetEquip();

        foreach(var tem in SaveEquip)
        {
            if(tem.Value != null)
            {
                Item item = tem.Value.GetComponent<Item>();
                SaveEquipstr += string.Format("{0},{1}|", item.ItemName, tem.Key);
            }

            else
            {
                SaveEquipstr += string.Format("null,{0}|", tem.Key);
            }
        }

        SaveEquipstr = SaveEquipstr.Substring(0, SaveEquipstr.Length - 1);

        Debug.LogFormat("확인값 : {0}", SaveInvenstr);
        Debug.LogFormat("확인값 : {0}", SaveEquipstr);
    }

    public void ParsingData(InventoryManager inventory, EquipmentManager equip)
    {
        if (SaveInvenstr == "" || SaveEquipstr == "")
        {
            return;
        }

        inventory.ClearInventory();
        equip.ClearEquipment();


        // 인벤토리 로드...
        string[] valarr = SaveInvenstr.Split('|');


        foreach (var item in valarr)
        {
            // parsingArr[0] = name [1] = y [2] = x
            string[] parsingArr = item.Split(',');
            if (parsingArr[0] != "null")
            {
                GameObject gameObject = null;
                for (int i = 0; i < itemoffsetList.Count; ++i)
                {
                    if (parsingArr[0] == itemoffsetList[i].ItemName)
                    {
                        gameObject = CreateItemObject(itemoffsetList[i]);
                    }
                }
                if (gameObject != null)
                {
                    inventory.SettingInventoryItemSize(gameObject, int.Parse(parsingArr[1]), int.Parse(parsingArr[2]));
                }
            }
        }

        // 장비창 로드...

        string[] valarr2 = SaveEquipstr.Split('|');
        foreach (var item in valarr2)
        {
            string[] parsingArr = item.Split(',');
            if (parsingArr[0] != "null")
            {
                GameObject gameObject = null;
                for (int i = 0; i < itemoffsetList.Count; ++i)
                {
                    if (parsingArr[0] == itemoffsetList[i].ItemName)
                    {
                        gameObject = CreateItemObject(itemoffsetList[i]);
                    }
                }
                GameObject slot = equip.GetEquipSlot(parsingArr[1]);

                if(gameObject && slot)
                {
                    equip.EquipItem(gameObject, slot);
                }
            }
        }
    }
}
