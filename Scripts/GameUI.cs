using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUI : MonoBehaviour
{


    protected GameObject thisCanvas;
    protected GameObject thisPanel;
    protected Vector3 slotItemScale;

    public abstract bool CheckSlotParent(GameObject slot);
    protected abstract void InitUIWindow();

    public abstract void PullItem(GameObject obj);
}
