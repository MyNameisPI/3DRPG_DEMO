using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI;
    SlotHolder currentHolder;
    SlotHolder targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;


        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //跟随鼠标
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //放下物品
        //是否指向UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position)
                || InventoryManager.Instance.CheckInActionUI(eventData.position)
                || InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {//拖拽到空格子时 下面的item时关闭的 所以射线检测到的是SlotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {//拖拽到空格子时 下面的item时开启的 所以射线检测到的是Item里的Image
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }
                if (targetHolder != currentHolder)
                {
                    switch (targetHolder.slotType)
                    {
                        case SlotType.BAG:
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                        default:
                            break;
                    }
                }
                
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        // 修改父级
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    public void SwapItem()
    {
        var targetItme = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];

        bool isSameItem = tempItem.itemData == targetItme.itemData;
        if (isSameItem&&targetItme.itemData.stackable)
        {
            targetItme.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItme;
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
