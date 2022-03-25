using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType { BAG, WEAPON, ARMOR, ACTION }
public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;
    bool isPointerEnter;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount%2 == 0)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (itemUI.GetItem() != null)
        {
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().usableItemData.healthPoint);
                itemUI.Bag.items[itemUI.Index].amount -= 1;
            }
            UpdateItem();
        }
        
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.InventotyData_SO;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData!=null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeArmor(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipAromr();
                }
                    break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
            default:
                break;
        }
        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
            //StartCoroutine(OpenTooltip());
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
        //StopCoroutine(OpenTooltip());
        
    }

    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
        //StopCoroutine(OpenTooltip());
    }

    private IEnumerator OpenTooltip(float staytime=1f)
    {
        yield return null;
        InventoryManager.Instance.tooltip.gameObject.SetActive(true);
    }
}
