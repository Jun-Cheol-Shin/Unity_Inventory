# 유니티 인벤토리, 장비창 구현
## ⓐ. 전체 구도
![캡처화면1](https://user-images.githubusercontent.com/58795584/101902977-e67edf00-3bf6-11eb-8f59-d77b92ae5e16.PNG)

## ⓑ. 조작법
## ! 시작하시면 ITEM BOX에 5종류의 아이템이 들어있습니다.
### 아이템 종류
*	한손 검, 한손 방패, 단검, 활, 신발

### 조작법
*	WSAD로 움직이며 Q와 E로 올라가기, 내려가기가 가능합니다.
*	사물에 가까이 가서 왼쪽 클릭 시 상호작용이 일어납니다.
*	인벤토리는 F키이며 다시 누르면 UI가 꺼집니다.
*	장비창은 G키이며 다시 누르면 UI가 꺼집니다.
### 상호작용
*	아이템 : 인벤토리로 아이템이 들어갑니다
*	아이템 박스 : 박스 인벤토리 UI가 켜집니다. 다시 누르면 꺼집니다.
*	SAVE : 현재 인벤토리, 장비창이 세이브 됩니다.
*	LOAD : 세이브된 데이터로 인벤토리와 장비창이 로드됩니다.
![캡처화면2](https://user-images.githubusercontent.com/58795584/101903286-66a54480-3bf7-11eb-9ac6-a29d27d1823b.PNG)


## ⓒ. 구현
```
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
```
* 현재 아이템 5종류는 코드를 이용하여 생성하고 있습니다!
* 1. Json을 이용하여 아이템의 정보를 받아온다.
* 2. AddItem이라는 함수를 이용하여 박스에 넣는다.
```
   for (int i = 0; i < itemoffsetList.Count; ++i)
        {
            itemoffsetList[i].Print();
            GameObject obj = CreateItemObject(itemoffsetList[i]);
            BoxComponent.AddItem(obj);
        }
```

* 드래그 시 드래그할 슬롯에 아이템이 있다면 드래그하는 아이템이 원래 자리로 돌아갑니다.
* 같은 인벤토리에서 아이템을 아이템 있는 곳에 이동(자리 바꾸기)을 구현했습니다.

<img src="https://user-images.githubusercontent.com/58795584/101904839-d0265280-3bf9-11eb-8cdb-eab35bafea6c.PNG" width="450"> | <img src="https://user-images.githubusercontent.com/58795584/101904893-e8966d00-3bf9-11eb-89aa-5260074e6c47.PNG" width="450">
:-------------------------:|:-------------------------:

* 아이템을 외부에 드래그 시 필드에 떨어지는 기능 구현  또한 아이템을 장비하는 것이 가능합니다.
* 2차원 배열을 만들어 인벤토리를 관리 (아이템이 있으면 그 배열 자리에 아이템 삽입)
* 아이템 방출은 해당 아이템을 찾아 AddForce를 이용하여 던지도록 구현

![캡처화면8](https://user-images.githubusercontent.com/58795584/101905255-7eca9300-3bfa-11eb-9b62-fb09d8e928d3.PNG)

* 양손무기인 활을 장착했을 경우 활 이외의 모든 손의 장비가 장착해제됩니다.

![캡처화면10](https://user-images.githubusercontent.com/58795584/101905381-b8030300-3bfa-11eb-877c-56128a11483b.PNG)

* 세이브 로드를 이용하여 장비를 무한히 생성하는 모습
* String을 이용하여 인벤토리의 모든 아이템이름,아이템 위치정보를 하나의 String 변수로 만들어냄.
* Split을 이용하여 아이템이름, 아이템 위치정보를 쪼개어 정보를 얻은 후 아이템을 다시 생성
* 생성 후 인벤토리에 저장

```
    // 인벤토리 로드...
        // ex Sword|0,0|Shield|3,4|.....
        
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
```

#### 자세한 코드는 Assets/Scripts 폴더의 스크립트를 확인해주세요!
