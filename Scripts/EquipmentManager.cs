using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : GameUI
{
    // 리소스
    public Sprite slot;
    public GameObject[] slotList;
    Dictionary<string, GameObject> PlayerEquip;


    public void ClearEquipment()
    {
        string[] temp = new string[5];
        int count = 0;

        foreach(var tem in PlayerEquip)
        {
            if (tem.Value != null)
            {
                temp[count++] = tem.Key;
            }
        }

        for(int i=0; i < count; ++i)
        {
            GameObject obj = PlayerEquip[temp[i]];
            PlayerEquip[temp[i]] = null;
            Destroy(obj);
        }
    }

    public GameObject GetEquipSlot(string name)
    {
        switch(name)
        {
            case "Head":
                return slotList[0];
            case "Armor":
                return slotList[1];
            case "Left":
                return slotList[2];
            case "Right":
                return slotList[3];
            case "Shoes":
                return slotList[4];
        }

        return null;
    }

    public Dictionary<string, GameObject> GetEquip()
    {
        return PlayerEquip;
    }

    private void Start()
    {
        PlayerEquip = new Dictionary<string, GameObject>();

        slot = Resources.Load("Sprites/slot", typeof(Sprite)) as Sprite;

        if (slot != null)
        {
            Debug.LogFormat("슬롯 스프라이트 로드 성공! " + slot.name);
        }

        InitUIWindow();
    }

    public override bool CheckSlotParent(GameObject slot)
    {
        for (int i = 0; i < 5; ++i)
        {
            if (slot == slotList[i])
            {
                return true;
            }
        }

        return false;
    }

    protected override void InitUIWindow()
    {
        if (this.gameObject.GetComponent<Canvas>() == null)
        {
            Debug.Log("장비창이 아닌 곳에 코드가 삽입됨!");
            return;
        }

        if (this.transform.childCount == 0)
        {
            Debug.Log("캔버스에 패널이 없습니다!");
            return;
        }

        slotList = new GameObject[5];

        thisCanvas = this.gameObject;
        thisPanel = this.transform.GetChild(0).gameObject;

        for (int i=0; i<5; ++i)
        {
            GameObject slotObj = new GameObject();
            RectTransform rowRect = slotObj.AddComponent<RectTransform>();
            slotObj.transform.SetParent(thisPanel.transform);
            slotObj.layer = LayerMask.NameToLayer("UI");

            rowRect.sizeDelta = new Vector2(250, 250);
            rowRect.localScale = Vector3.one;
            rowRect.localRotation = Quaternion.Euler(Vector3.zero);

            Image image = slotObj.AddComponent<Image>();
            image.sprite = slot;

            BoxCollider box = slotObj.AddComponent<BoxCollider>();
            box.size = new Vector3(rowRect.sizeDelta.x, rowRect.sizeDelta.y, 1);
            box.isTrigger = true;

            if (i == 4)
            {
                float scale = box.size.x / 3;
                slotItemScale = new Vector3(scale, scale, scale);
            }

            GameObject slotIcon = new GameObject();
            RectTransform Rect = slotIcon.AddComponent<RectTransform>();
            slotIcon.transform.SetParent(slotObj.transform);

            Rect.sizeDelta = new Vector3(250, 250);
            Rect.localScale = Vector3.one;
            Rect.localRotation = Quaternion.Euler(Vector3.zero);

            Image imageIcon = slotIcon.AddComponent<Image>();

            Color iconColor = imageIcon.color;

            iconColor.a = 0.2f;

            switch (i)
            {
                case 0:
                    imageIcon.sprite = Resources.Load("Sprites/Head", typeof(Sprite)) as Sprite;
                    rowRect.anchoredPosition3D = new Vector3(0, 350, 0);
                    PlayerEquip.Add("Head", null);
                    slotIcon.name = "Head";
                    break;
                case 1:
                    imageIcon.sprite = Resources.Load("Sprites/Armor", typeof(Sprite)) as Sprite;
                    rowRect.anchoredPosition3D = new Vector3(0, 0, 0);
                    PlayerEquip.Add("Armor", null);
                    slotIcon.name = "Armor";
                    break;
                case 2:
                    imageIcon.sprite = Resources.Load("Sprites/Left", typeof(Sprite)) as Sprite;
                    rowRect.anchoredPosition3D = new Vector3(-350, 0, 0);
                    PlayerEquip.Add("Left", null);
                    slotIcon.name = "Left";
                    break;
                case 3:
                    imageIcon.sprite = Resources.Load("Sprites/Right", typeof(Sprite)) as Sprite;
                    rowRect.anchoredPosition3D = new Vector3(350, 0, 0);
                    PlayerEquip.Add("Right", null);
                    slotIcon.name = "Right";
                    break;
                case 4:
                    imageIcon.sprite = Resources.Load("Sprites/Shoes", typeof(Sprite)) as Sprite;
                    rowRect.anchoredPosition3D = new Vector3(0, -350, 0);
                    PlayerEquip.Add("Shoes", null);
                    slotIcon.name = "Shoes";
                    break;
            }

            slotList[i] = slotObj;
            imageIcon.color = iconColor;
        }
        this.gameObject.SetActive(false);
    }

    //  해당 장비창이 비어있는지 확인
    public bool CheckEquipEmpty(string name)
    {
        return PlayerEquip[name] == null ? true : false;
    }

    public bool CheckItemCode(Item objItem, GameObject slot)
    {
        int equipnum = -1;

        for(int i=0; i < PlayerEquip.Count; ++i)
        {
            if(slot == slotList[i])
            {
                equipnum = i;
                break;
            }
        }

        if (equipnum == -1) return false;

        switch(objItem.itemCode)
        {
            case GameManager.CODE.HEAD:
                if(equipnum == 0)
                {
                    //PlayerEquip["Head"] = objItem.gameObject;
                    return true;
                }
                break;
            case GameManager.CODE.ARMOR:
                if(equipnum == 1)
                {
                    //PlayerEquip["Armor"] = objItem.gameObject;
                    return true;
                }
                break;
            case GameManager.CODE.HAND:
                if(equipnum == 2)
                {
                    //PlayerEquip["Left"] = objItem.gameObject;
                    return true;
                }
                else if(equipnum == 3)
                {
                    //PlayerEquip["Right"] = objItem.gameObject;
                    return true;
                }
                break;
            case GameManager.CODE.SHOES:
                if(equipnum == 4)
                {
                    //PlayerEquip["Shoes"] = objItem.gameObject;
                    return true;
                }
                break;

            case GameManager.CODE.TWO_HAND:
                if (equipnum == 2)
                {
                    //PlayerEquip["Left"] = objItem.gameObject;
                    return true;
                }
                else if (equipnum == 3)
                {
                    //PlayerEquip["Right"] = objItem.gameObject;
                    return true;
                }
                break;
        }

        return false;
    }

    public void SetItemEquipArray(Item objItem, GameObject slot)
    {
        int equipnum = -1;

        for (int i = 0; i < PlayerEquip.Count; ++i)
        {
            if (slot == slotList[i])
            {
                equipnum = i;
                break;
            }
        }

        if (equipnum == -1) return;

        switch (objItem.itemCode)
        {
            case GameManager.CODE.HEAD:
                if (equipnum == 0)
                {
                    PlayerEquip["Head"] = objItem.gameObject;
                }
                break;
            case GameManager.CODE.ARMOR:
                if (equipnum == 1)
                {
                    PlayerEquip["Armor"] = objItem.gameObject;
                }
                break;
            case GameManager.CODE.HAND:
                if (equipnum == 2)
                {
                    PlayerEquip["Left"] = objItem.gameObject;
                }
                else if (equipnum == 3)
                {
                    PlayerEquip["Right"] = objItem.gameObject;
                }
                break;
            case GameManager.CODE.SHOES:
                if (equipnum == 4)
                {
                    PlayerEquip["Shoes"] = objItem.gameObject;
                }
                break;
            case GameManager.CODE.TWO_HAND:
                if (equipnum == 2)
                {
                    PlayerEquip["Left"] = objItem.gameObject;
                }
                else if (equipnum == 3)
                {
                    PlayerEquip["Right"] = objItem.gameObject;
                }
                break;
        }
    }

    string GetSlotItemCode(GameObject slot)
    {
        int SlotNum = -1;
        for(int i=0; i<5; i++)
        {
            if (slot == slotList[i])
            {
                SlotNum = i;
                break;
            }
        }

        if (SlotNum == -1) return "Null";

        switch(SlotNum)
        {
            case 0:
                return "Head";
            case 1:
                return "Armor";
            case 2:
                return "Left";
            case 3:
                return "Right";
            case 4:
                return "Shoes";
        }

        return "Null";
    }

    int SetEquipNull(Item itemComponent)
    {
        int oldSlotNum = -1;

        switch (itemComponent.itemCode)
        {
            case GameManager.CODE.HEAD:
                PlayerEquip["Head"] = null;
                oldSlotNum = 0;
                break;

            case GameManager.CODE.ARMOR:
                PlayerEquip["Armor"] = null;
                oldSlotNum = 1;
                break;

            case GameManager.CODE.HAND:
                if (PlayerEquip["Left"] == itemComponent.gameObject)
                {
                    PlayerEquip["Left"] = null;
                    oldSlotNum = 2;
                }
                else if (PlayerEquip["Right"] == itemComponent.gameObject)
                {
                    PlayerEquip["Right"] = null;
                    oldSlotNum = 3;
                }
                break;

            case GameManager.CODE.SHOES:
                PlayerEquip["Shoes"] = null;
                oldSlotNum = 4;
                break;

            case GameManager.CODE.TWO_HAND:
                if (PlayerEquip["Left"] == itemComponent.gameObject)
                {
                    PlayerEquip["Left"] = null;
                    oldSlotNum = 2;
                }
                else if (PlayerEquip["Right"] == itemComponent.gameObject)
                {
                    PlayerEquip["Right"] = null;
                    oldSlotNum = 3;
                }
                break;
        }

        return oldSlotNum;
    }

    void SetItemEquip(Item itemComponent, GameObject obj, GameObject slot)
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.SetParent(slot.transform);
        obj.transform.localScale = slotItemScale;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(itemComponent.InvenRot);
        itemComponent.isEquip = true;
        itemComponent.onInventory = this.transform;
    }

    // 장비를 장착하는 함수
    public bool EquipItem(GameObject obj, GameObject slot)
    {
        Item itemComponent = obj.GetComponent<Item>();
        if (itemComponent == null)
            return false;

        // 이동할 슬롯이 비어있는지...
        if (CheckEquipEmpty(GetSlotItemCode(slot)))
        {
            // 맞는 슬롯인지 체크
            if(CheckItemCode(itemComponent, slot))
            {
                if (itemComponent.itemCode == GameManager.CODE.TWO_HAND)
                {
                    if(CheckEquipEmpty("Left") && CheckEquipEmpty("Right"))
                    {
                        SetItemEquip(itemComponent, obj, slot);
                        SetItemEquipArray(itemComponent, slot);
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (PlayerEquip["Left"] != null && PlayerEquip["Left"].GetComponent<Item>().itemCode == GameManager.CODE.TWO_HAND)
                    {
                        return false;
                    }

                    if(PlayerEquip["Right"] != null && PlayerEquip["Right"].GetComponent<Item>().itemCode == GameManager.CODE.TWO_HAND)
                    {
                        return false;
                    }

                    else
                    {
                        SetItemEquip(itemComponent, obj, slot);
                        SetItemEquipArray(itemComponent, slot);
                        return true;
                    }
                }
            }
            // 아닌 경우
            else
            {
                return false;
            }
        }

        // 비어있지 않으면...
        else
        {
            // 이동할 슬롯에 맞는 장비인지 확인
            if (CheckItemCode(itemComponent, slot))
            {
                InventoryManager preInven = itemComponent.onInventory.GetComponent<InventoryManager>();
                if (!preInven) return false;

                // 이동할 오브젝트 인벤토리에 제거
                preInven.SetInventoryEmpty(obj);


                if (itemComponent.itemCode == GameManager.CODE.TWO_HAND)
                {
                    GameObject tempObj;
                    tempObj = PlayerEquip["Left"];
                    if (tempObj != null)
                    {
                        Item itemComponent2 = PlayerEquip["Left"].GetComponent<Item>();
                        SetEquipNull(itemComponent2);

                        preInven.AddItem(tempObj);
                    }
                    
                    GameObject tempObj2;
                    tempObj2 = PlayerEquip["Right"];

                    if(tempObj2 != null)
                    {
                        Item itemComponent2 = PlayerEquip["Right"].GetComponent<Item>();
                        SetEquipNull(itemComponent2);
                        preInven.AddItem(tempObj2);
                    }

                    EquipItem(obj, slot);
                    return true;
                }

                else
                {
                    GameObject tempObj;
                    // 장비된 아이템 장비창에 제거
                    tempObj = PlayerEquip[GetSlotItemCode(slot)];
                    Item itemComponent2 = PlayerEquip[GetSlotItemCode(slot)].GetComponent<Item>();
                    SetEquipNull(itemComponent2);

                    preInven.AddItem(tempObj);
                    EquipItem(obj, slot);

                    return true;
                }
            }

            else
            {
                // ReturnInvenItem 함수를 실행시키도록 한다.
                return false;
            }
        }
    }

    public void TakeOffEquip(InventoryManager nextInven, GameObject obj, GameObject slot)
    {
        int oldSlotNum = -1;
        Item itemComponent = obj.GetComponent<Item>();
        if (itemComponent == null)
            return;

        oldSlotNum = SetEquipNull(itemComponent);

        itemComponent.isEquip = false;
        itemComponent.onInventory = nextInven.transform;

        if (!nextInven.EquipToInven(obj, slot))
        {
            // 다시 원상태로...
            EquipItem(obj, slotList[oldSlotNum]);
        }
    }
    public void EquipToEquip(GameObject obj, GameObject slot)
    {
        Item itemComponent = obj.GetComponent<Item>();
        if (itemComponent == null)
            return;

        int oldNum = -1;
        oldNum = SetEquipNull(itemComponent);

        // 현재 슬롯이 비어있는가...
        if (CheckEquipEmpty(GetSlotItemCode(slot)))
        {
            if (!EquipItem(obj,slot))
            {
                EquipItem(obj, slotList[oldNum]);
            }
        }

        // 아닌 경우는 스왑을 해야함
        else
        {
            if (CheckItemCode(itemComponent, slot))
            {
                Item itemComponent2 = PlayerEquip[GetSlotItemCode(slot)].GetComponent<Item>();
                SetItemEquip(itemComponent2, PlayerEquip[GetSlotItemCode(slot)], slotList[oldNum]);
                SetItemEquipArray(itemComponent2, slotList[oldNum]);
                SetItemEquip(itemComponent, obj, slot);
                SetItemEquipArray(itemComponent, slot);
            }
        }

    }

    public override void PullItem(GameObject obj)
    {
        Item itemComponent = obj.GetComponent<Item>();
        if (itemComponent == null)
            return;
        SetEquipNull(itemComponent);
        obj.transform.localScale = itemComponent.ItemScale;
        itemComponent.isInventory = false;
        itemComponent.isEquip = false;
        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 5f);
    }


}
