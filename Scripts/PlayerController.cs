using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera MainCamera;
    public float RayDistance = 3f;

    public LayerMask layerMask;

    public Canvas Inventory;
    public Canvas Equipment;
    public KeyCode InventoryOpenKey;
    public KeyCode EquipmentOpenKey;

    RectTransform InventoryTransform;
    RectTransform EquipmentTransform;

    InventoryManager PlayerInventory;
    EquipmentManager PlayerEquipment;

    GameObject slot = null;

    bool moveInventoryItem = false;
    GameObject moveInventoryItemObj;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        InventoryTransform = Inventory.GetComponent<RectTransform>();
        EquipmentTransform = Equipment.GetComponent<RectTransform>();
        StartCoroutine(RayCheckObject());

        PlayerInventory = Inventory.GetComponent<InventoryManager>();
        PlayerEquipment = Equipment.GetComponent<EquipmentManager>();
    }


    IEnumerator RayCheckObject()
    {
        // 마우스 왼쪽 버튼 클릭 시
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, RayDistance, layerMask))
                {
                    Debug.Log(hit.collider.name);
                    switch (hit.collider.gameObject.layer)
                    {
                        // item
                        case 8:
                            if (PlayerInventory != null)
                            {
                                Item clickItem = hit.collider.GetComponent<Item>();
                                if (!clickItem.isInventory)
                                {
                                    PlayerInventory.AddItem(hit.collider.gameObject);
                                }
                                else
                                {
                                    moveInventoryItemObj = hit.collider.gameObject;
                                    moveInventoryItemObj.transform.SetParent(null);
                                    //moveInventoryItemObj.transform.SetParent(GameManager.Instance.PlayerInventory.transform);
                                    moveInventoryItem = true;
                                    //PlayerInventory.InventoyItemMove(hit.collider.gameObject);
                                }
                            }
                            break;
                        // function
                        case 9:
                            switch(hit.collider.name)
                            {
                                case "Load":
                                    GameManager.Instance.ParsingData(PlayerInventory, PlayerEquipment);
                                    break;
                                case "Save":
                                    GameManager.Instance.SaveData(PlayerInventory, PlayerEquipment);
                                    break;
                            }
                            break;
                        // Box
                        case 10:
                            if (GameManager.Instance.BoxInventory != null)
                            {
                                if (GameManager.Instance.BoxInventory.activeSelf)
                                {
                                    GameManager.Instance.BoxInventory.SetActive(false);
                                }
                                else
                                {
                                    GameManager.Instance.BoxInventory.SetActive(true);
                                }
                            }
                            break;
                    }
                }
            }
            yield return null;
        }
    }

    void AutoDistanceOffUI(Canvas UI)
    {
        if (UI.gameObject.activeSelf)
        {
            if (Vector3.Distance(MainCamera.transform.position, UI.transform.position) > 2.5f)
            {
                UI.gameObject.SetActive(false);
            }
        }
    }

    void KeyDownMethod()
    {
        if(Input.GetKeyDown(InventoryOpenKey))
        {
            OpenUI(Inventory, InventoryTransform, 
                MainCamera.transform.localPosition +
                MainCamera.transform.forward * 1.5f -
                MainCamera.transform.right * 0.5f);
        }

        if (Input.GetKeyDown(EquipmentOpenKey))
        {
            OpenUI(Equipment, EquipmentTransform,
              MainCamera.transform.localPosition +
              MainCamera.transform.forward * 1.5f +
              MainCamera.transform.right * 0.8f);
        }

        AutoDistanceOffUI(Inventory);
        AutoDistanceOffUI(Equipment);
    }

    void OpenUI(Canvas UI, RectTransform UIRect, Vector3 Openpos)
    {
        if (UI.gameObject.activeSelf)
        {
            UI.gameObject.SetActive(false);
        }

        // 위치를 설정하면서 키기
        else
        {
            UIRect.anchoredPosition3D = Openpos;
            UIRect.localRotation = MainCamera.transform.localRotation;
            UI.gameObject.SetActive(true);
        }
    }

    bool CheckMousePos()
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f, LayerMask.GetMask("UI")))
        {
            slot = hit.collider.gameObject;
            Debug.Log(hit.collider.gameObject.name);
            return true;
        }
        return false;
    }

    void MovingInventoryItem()
    {
        if (moveInventoryItem)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 screenSpace = MainCamera.WorldToScreenPoint(moveInventoryItemObj.transform.position);
                Vector3 mousePosition = MainCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
                moveInventoryItemObj.transform.position = mousePosition;

                if(!CheckMousePos())
                {
                    slot = null;
                }

            }

            else if (Input.GetMouseButtonUp(0))
            {
                if (slot != null)
                {
                    // 아이템이 원래 인벤토리에 있었다면...
                    if(moveInventoryItemObj.GetComponent<Item>().onInventory == PlayerInventory.transform)
                    {
                        // 슬롯이 플레이어 인벤토리의 슬롯이면..
                        if(PlayerInventory.CheckSlotParent(slot))
                        {
                            Debug.Log("인벤 -> 인벤");
                            PlayerInventory.InventoyItemMove(moveInventoryItemObj, slot);
                        }

                        else if(PlayerEquipment.CheckSlotParent(slot))
                        {
                            Debug.Log("인벤 -> 장비");
                            // 실패 시
                            if(!PlayerEquipment.EquipItem(moveInventoryItemObj, slot))
                            {
                                // 다시 인벤토리로 돌아가기
                                PlayerInventory.ReturnInvenItem(moveInventoryItemObj);
                            }
                            // 성공 시 인벤토리 슬롯 비우기
                            else
                            {
                                PlayerInventory.SetInventoryEmpty(moveInventoryItemObj);
                            }
                        }

                        // 박스의 슬롯이면..
                        else if(GameManager.Instance.BoxComponent.CheckSlotParent(slot))
                        {
                            Debug.Log("인벤 => 박스");
                            if (GameManager.Instance.BoxComponent.CheckEmptySlot(slot))
                            {
                                PlayerInventory.InvenToInven(GameManager.Instance.BoxComponent, moveInventoryItemObj, slot);
                            }
                            else
                            {
                                PlayerInventory.ReturnInvenItem(moveInventoryItemObj);
                            }
                        }
                    }

                    // 아이템이 원래 박스에 있었다면..
                    else if(moveInventoryItemObj.GetComponent<Item>().onInventory == GameManager.Instance.BoxInventory.transform)
                    {
                        // 슬롯이 플레이어 인벤토리의 슬롯이면..
                        if (PlayerInventory.CheckSlotParent(slot))
                        {
                            Debug.Log("박스 => 인벤");
                            if(PlayerInventory.CheckEmptySlot(slot))
                            {
                                GameManager.Instance.BoxComponent.InvenToInven(PlayerInventory, moveInventoryItemObj, slot);
                            }
                            else
                            {
                                GameManager.Instance.BoxComponent.ReturnInvenItem(moveInventoryItemObj);
                            }
                        }

                        else if(PlayerEquipment.CheckSlotParent(slot))
                        {
                            Debug.Log("박스 => 장비창");
                            // ...
                            if (!PlayerEquipment.EquipItem(moveInventoryItemObj, slot))
                            {
                                // 다시 인벤토리로 돌아가기
                                GameManager.Instance.BoxComponent.ReturnInvenItem(moveInventoryItemObj);
                            }
                            // 성공 시 인벤토리 슬롯 비우기
                            else
                            {
                                GameManager.Instance.BoxComponent.SetInventoryEmpty(moveInventoryItemObj);
                            }
                        }

                        // 박스의 슬롯이면..
                        else if (GameManager.Instance.BoxComponent.CheckSlotParent(slot))
                        {
                            Debug.Log("박스 -> 박스");
                            GameManager.Instance.BoxComponent.InventoyItemMove(moveInventoryItemObj, slot);
                        }
                    }

                    else if(moveInventoryItemObj.GetComponent<Item>().onInventory == PlayerEquipment.transform)
                    {
                        // 슬롯이 플레이어 인벤토리의 슬롯이면..
                        if (PlayerInventory.CheckSlotParent(slot))
                        {
                            Debug.Log("장비 => 인벤");
                            PlayerEquipment.TakeOffEquip(PlayerInventory, moveInventoryItemObj, slot);
                        }

                        else if (PlayerEquipment.CheckSlotParent(slot))
                        {
                            Debug.Log("장비 => 장비창");
                            // ...
                            PlayerEquipment.EquipToEquip(moveInventoryItemObj, slot);

                        }
                        // 박스의 슬롯이면..
                        else if (GameManager.Instance.BoxComponent.CheckSlotParent(slot))
                        {
                            Debug.Log("장비 -> 박스");
                            PlayerEquipment.TakeOffEquip(GameManager.Instance.BoxComponent, moveInventoryItemObj, slot);
                        }
                    }

                }

                else
                {
                    // 인벤에서 버릴때
                    if (moveInventoryItemObj.GetComponent<Item>().onInventory == PlayerInventory.transform)
                    {
                        PlayerInventory.PullItem(moveInventoryItemObj);
                    }

                    // 박스에서 버릴때
                    else if (moveInventoryItemObj.GetComponent<Item>().onInventory == GameManager.Instance.BoxInventory.transform)
                    {
                        GameManager.Instance.BoxComponent.PullItem(moveInventoryItemObj);
                    }

                    else if(moveInventoryItemObj.GetComponent<Item>().onInventory == PlayerEquipment.transform)
                    {
                        PlayerEquipment.PullItem(moveInventoryItemObj);
                    }
                }

                moveInventoryItem = false;
                moveInventoryItemObj = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyDownMethod();
        MovingInventoryItem();
    }
}
