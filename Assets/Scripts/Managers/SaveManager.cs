using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{

    string sceneName = "";

    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
        Save(InventoryManager.Instance.InventotyData_SO, InventoryManager.Instance.InventotyData_SO.name);
        Save(InventoryManager.Instance.actionData, InventoryManager.Instance.actionData.name);
        Save(InventoryManager.Instance.equipmentData, InventoryManager.Instance.equipmentData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
        Load(InventoryManager.Instance.InventotyData_SO, InventoryManager.Instance.InventotyData_SO.name);
        Load(InventoryManager.Instance.actionData, InventoryManager.Instance.actionData.name);
        Load(InventoryManager.Instance.equipmentData, InventoryManager.Instance.equipmentData.name);
    }


    public void Save(Object data,string key)
    {
        var jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void Load(Object data,string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
