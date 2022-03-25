using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //TODO:模板
    [Header("Template")]
    public InventoryData_SO InventotyTemplate;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO equipmentTemplate;


    [Header("Inventory Data")]
    public InventoryData_SO InventotyData_SO;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentData;


    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("DragCanvas")]
    public Canvas dragCanvas;

    public DragData currentDrag;

    [Header("InventoryGO")]
    public GameObject inventoryGO;
    public GameObject characterStatsGO;
    public Text endGameText;

    bool isOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;
    public Text defenceText;

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        if (InventotyTemplate != null) InventotyData_SO = Instantiate(InventotyTemplate);
        if (actionTemplate != null) actionData = Instantiate(actionTemplate);
        if (equipmentTemplate != null) equipmentData = Instantiate(equipmentTemplate);

    }
    

    private void Start()
    {
        LoadDate();
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }
    
    
    public void SaveDate()
    {
        SaveManager.Instance.Save(InventotyData_SO, InventotyData_SO.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadDate()
    {
        SaveManager.Instance.Load(InventotyData_SO, InventotyData_SO.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            inventoryGO.SetActive(isOpen);
            characterStatsGO.SetActive(isOpen);
        }
        UpdateStatsText(GameManager.Instance.playerStats.CurrentHealth,
            GameManager.Instance.playerStats.attackData.minDamage,
            GameManager.Instance.playerStats.attackData.maxDamage,
            GameManager.Instance.playerStats.characterData.currentDefence);
    }

    public void UpdateStatsText(int health,int min,int max,int defence)
    {
        healthText.text = health.ToString();
        attackText.text = min + "-" + max;
        defenceText.text = defence.ToString();
    }

    #region 检查拖拽物品是否在一个Slot中
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            var t = inventoryUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t,position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            var t = actionUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            var t = equipmentUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

}
