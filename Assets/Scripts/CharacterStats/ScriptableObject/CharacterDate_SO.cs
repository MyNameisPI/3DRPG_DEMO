using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Date",menuName ="Character Stats/Data")]
public class CharacterDate_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;

    public int currentHealt;

    public int baseDefence;
    public int currentDefence;

    public int killPoint;

    [Header("Level")]
    public int currentLevel;

    public int maxLevel;

    public int baseExp;

    public int currentExp;


    public float levelBuff;
    public float levelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp) LeveUp();
    }

    private void LeveUp()
    {
        //ÌáÉý
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * levelMultiplier);
        maxHealth = (int)(maxHealth * levelMultiplier);
        baseDefence = (int)(baseDefence * levelMultiplier);
        currentDefence = baseDefence;
        Debug.Log("LEVEL UP!" + currentLevel + "Max Health" + maxHealth);
    }

    public void ApplyArmor(ItemData_SO armordata)
    {
        currentDefence = armordata.armorDate.currentDefence;
    }
}
