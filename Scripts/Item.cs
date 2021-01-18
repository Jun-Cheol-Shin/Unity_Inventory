using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    public GameManager.CODE itemCode;

    public Vector3 InvenRot;
    public Vector3 ItemScale;
    public string ItemName = "";

    public Transform onInventory = null;

    // 인벤토리 안에 있으냐 없느냐...
    public bool isInventory = false;
    public bool isEquip = false;

    public void Print()
    {
        Debug.LogFormat(" Rot : {0} Scale : {1} Name : {2} Code : {3}", InvenRot, ItemScale, ItemName, itemCode);
    }
}
