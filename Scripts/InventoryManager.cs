using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : GameUI
{
    public int column;
    public int row;

    GameObject[,] Inventory;
    public GameObject[,] slotList;
    
    // 리소스
    public Sprite slot;

    public void ClearInventory()
    {
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < column; ++j)
            {
                if (Inventory[i, j] != null)
                {
                    GameObject temp = Inventory[i, j];
                    Inventory[i, j] = null;
                    Destroy(temp);
                }
            }
        }

        Inventory.Initialize();
    }

    public GameObject[,] GetInventory()
    {
        return Inventory;
    }

    public void SetInventoryEmpty(GameObject obj)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if(obj == Inventory[i,j])
                {
                    Inventory[i, j] = null;
                    return;
                }
            }
        }
    }
    
    public override bool CheckSlotParent(GameObject slot)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if(slot == slotList[i,j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 빈칸 확인
    public bool CheckEmptySlot(int row, int column)
    {
        return Inventory[row, column] == null ? true : false;
    }

    public bool CheckEmptySlot(GameObject slot)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if (slotList[i, j] == slot)
                {
                    if (Inventory[i, j] == null)
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }

    // 인벤토리로 들어갈 아이템의 사이즈 조정
    public void SettingInventoryItemSize(GameObject obj, int i, int j)
    {
        Item itemComponent = obj.GetComponent<Item>();
        if (itemComponent == null)
            return;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.SetParent(slotList[i, j].transform);
        // 스케일이 문제
        obj.transform.localScale = slotItemScale;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(itemComponent.InvenRot);
        Inventory[i, j] = obj;
        itemComponent.isInventory = true;
        itemComponent.onInventory = this.transform;
    }

    // 빈칸인 경우 옮기고 True 아니면 False
    bool SettingAddingInventoryItem(int i, int j, GameObject obj)
    {
        if (CheckEmptySlot(i, j))
        {
            SettingInventoryItemSize(obj, i, j);
            return true;
        }

        return false;
    }

    // 클릭 시 인벤토리로 아이템이 들어감.
    public void AddItem(GameObject obj)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j < column; ++j)
            {
                if (SettingAddingInventoryItem(i, j, obj))
                {
                    return;
                }
            }
        }
    }

    // 인벤토리 아이템 위치 이동
    public void InventoyItemMove(GameObject obj, GameObject slot)
    {
        int oldx = 0;
        int oldy = 0;
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if(Inventory[i,j] == obj)
                {
                    Inventory[i, j] = null;
                    oldx = j;
                    oldy = i;
                    break;
                }
            }
        }

        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if (slot == slotList[i, j])
                {
                    if (!SettingAddingInventoryItem(i, j, obj))
                    {
                        SwapMoveItem(oldx, oldy, i, j, obj);
                        //SettingInventoryItemSize(obj, oldy, oldx);
                    }
                    return;
                }
            }
        }
    }

    // 아이템 외부 방출
    public override void PullItem(GameObject obj)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if (Inventory[i, j] == obj)
                {
                    Inventory[i, j] = null;
                    Item itemComponent = obj.GetComponent<Item>();
                    if (itemComponent == null)
                        return;
                    obj.transform.localScale = itemComponent.ItemScale;
                    itemComponent.isInventory = false;
                    obj.GetComponent<Rigidbody>().isKinematic = false;
                    obj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 5f);
                }
            }
        }
    }

    // 이동 할 때 같은 아이템이 있는 경우
    public void SwapMoveItem(int oldx, int oldy, int i, int j, GameObject movingObj)
    {
        SettingInventoryItemSize(Inventory[i, j], oldy, oldx);
        SettingInventoryItemSize(movingObj, i, j);
    }

    // 다른 오브젝트의 인벤토리로 이동할 때 사용 ex) 인벤 -> 아이템 박스
    public void InvenToInven(InventoryManager nextInven, GameObject obj, GameObject slot)
    {
        int oldx = 0;
        int oldy = 0;

        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < column; ++j)
            {
                if(Inventory[i,j] == obj)
                {
                    oldy = i;
                    oldx = j;
                    Inventory[i, j] = null;
                    break;
                }
            }
        }

        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < column; ++j)
            {
                if(nextInven.slotList[i,j] == slot)
                {
                    if (!nextInven.SettingAddingInventoryItem(i, j, obj))
                    {
                        nextInven.SettingInventoryItemSize(obj, oldy, oldx);
                        return;
                    }
                }
            }
        }
    }


    public bool EquipToInven(GameObject obj, GameObject slot)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j< column; ++j)
            {
                if(slot == slotList[i,j])
                {
                    if(SettingAddingInventoryItem(i, j, obj))
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
            }
        }


        return false;
    }


    // 올바른 장착이 아닌 경우 다시 원래 자리로 돌아가기
    public void ReturnInvenItem(GameObject obj)
    {
        for(int i=0; i<row; ++i)
        {
            for(int j=0; j<column; ++j)
            {
                if(Inventory[i,j] == obj)
                {
                    SettingInventoryItemSize(obj, i, j);
                    return;
                }
            }
        }
    }


    // 패널 오브젝트 리턴
    public GameObject RetInven()
    {
        return thisPanel;
    }

    private void Awake()
    {
        slot = Resources.Load("Sprites/slot", typeof(Sprite)) as Sprite;

        if (slot != null)
        {
            Debug.LogFormat("슬롯 스프라이트 로드 성공! " + slot.name);
        }

        InitUIWindow();
    }

    // UI Canvas 만들기
    protected override void InitUIWindow()
    {
        if(this.gameObject.GetComponent<Canvas>() == null)
        {
            Debug.Log("인벤토리가 아닌 곳에 코드가 삽입됨!");
            return;
        }

        if(this.transform.childCount == 0)
        {
            Debug.Log("캔버스에 패널이 없습니다!");
            return;
        }

        Inventory = new GameObject[row, column];
        slotList = new GameObject[row, column];

        thisCanvas = this.gameObject;
        thisPanel = this.transform.GetChild(0).gameObject;

        thisPanel.AddComponent<VerticalLayoutGroup>();

        for (int i = 0; i < row; ++i)
        {
            GameObject rowObj = new GameObject();
            RectTransform rowRect = rowObj.AddComponent<RectTransform>();
            rowObj.transform.SetParent(thisPanel.transform);
            rowObj.AddComponent<HorizontalLayoutGroup>();

            rowRect.sizeDelta = new Vector2(1000, 100);
            rowRect.localRotation = Quaternion.Euler(Vector3.zero);
            rowRect.localScale = new Vector3(1, 1, 1);
            rowRect.anchoredPosition3D = new Vector3(rowRect.anchoredPosition3D.x,
                rowRect.anchoredPosition3D.y, 0f);

            rowObj.layer = LayerMask.NameToLayer("UI");
            for (int j = 0; j < column; ++j)
            {
                GameObject slotObj = new GameObject();
                slotObj.transform.SetParent(rowObj.transform);
                slotObj.layer = LayerMask.NameToLayer("UI");

                RectTransform slotRect = slotObj.AddComponent<RectTransform>();
                rowObj.transform.SetParent(thisPanel.transform);

                slotRect.localRotation = Quaternion.Euler(Vector3.zero);
                slotRect.localScale = new Vector3(1, 1, 1);

                slotRect.anchoredPosition3D = new Vector3(
                slotRect.anchoredPosition3D.x,
                slotRect.anchoredPosition3D.y,
                0f);

                Image image = slotObj.AddComponent<Image>();
                image.sprite = slot;

                RectTransform rect = this.transform.GetComponent<RectTransform>();
                BoxCollider slotCol = slotObj.AddComponent<BoxCollider>();
                slotCol.size = new Vector3(rect.sizeDelta.x / column, rect.sizeDelta.y / row, 0.1f);
                slotCol.isTrigger = true;

                if (i == row - 1 && j == column - 1)
                {
                    float scale = slotCol.size.x / 3;
                    slotItemScale = new Vector3(scale, scale, scale);
                }
                Inventory[i, j] = null;
                slotList[i, j] = slotObj;
            }
        }

        thisPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
