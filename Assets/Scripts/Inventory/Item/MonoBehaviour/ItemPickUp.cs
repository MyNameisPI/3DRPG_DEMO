using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO:
            InventoryManager.Instance.InventotyData_SO.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();


            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            Destroy(gameObject);
        }
    }
}
