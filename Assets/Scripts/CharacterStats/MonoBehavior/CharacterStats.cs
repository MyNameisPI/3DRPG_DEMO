using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterDate_SO templateData;
    public CharacterDate_SO characterData;

    public AttackData_SO attackData;
    public AttackData_SO baseAttactData;
    public RuntimeAnimatorController baseAnimatorController;

    [Header("Weapon")]
    public Transform weaponSlot;
    public Transform Shieldslot;
    public Transform ShieldNoWeaponslot;


    
    public bool isCritical;

    private void Awake()
    {
        if (templateData!=null)
        {
            characterData = Instantiate(templateData);
        }
        attackData = Instantiate(baseAttactData);
    }

    #region Read form Data_SO
    public int MaxHealth
    {
        get{if (characterData != null)return characterData.maxHealth;else return 0;}
        set{characterData.maxHealth = value;}
    }

    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealt; else return 0; }
        set { characterData.currentHealt = value; }
    }

    public int BaseDeftence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        Debug.Log("Attack");
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - damage, 0);
        if (attacker.isCritical) defener.GetComponent<Animator>().SetTrigger("Hit");

        //TODO:UpdateUI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //TODO:¾­Ñéupdate;
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);
    }

    public void TakeDamage(int damage,CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if(CurrentHealth<=0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultipler;
            Debug.Log("±©»÷");
        }
        return (int)coreDamage;
    }
    #endregion

    #region Equip Weapon

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }
    public void EquipWeapon(ItemData_SO weapon)
    {
        if (weapon.weaponPrefab != null)
        {
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }
        //TODO£ºÌæ»»¹¥»÷ÊôÐÔ
        attackData.ApplyWeaponData(weapon.weaponAttackDate);
        //Ìæ»»¶¯»­
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;

        if(Shieldslot.childCount == 0&& ShieldNoWeaponslot.childCount!=0&&weapon.itemName == "Sword")
        {
            Transform child = ShieldNoWeaponslot.GetChild(0);
            child.SetParent(Shieldslot,false);
        }
    }

    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount!=0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyBaseData(baseAttactData);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimatorController;
        if (Shieldslot.childCount != 0 && ShieldNoWeaponslot.childCount == 0)
        {
            Transform child = Shieldslot.GetChild(0);
            child.SetParent(ShieldNoWeaponslot,false);
        }
    }
    #endregion

    #region Armor
    public void ChangeArmor(ItemData_SO Armor)
    {
        UnEquipAromr();
        EquipArmor(Armor);
    }
    public void EquipArmor(ItemData_SO Armor)
    {
        if (Armor.armorPrefab != null)
        {
            if (weaponSlot.childCount != 0)
            {
                if (weaponSlot.GetChild(0).name == "GreatSword_eq(Clone)")
                {
                    Instantiate(Armor.armorPrefab, ShieldNoWeaponslot);
                    return;
                }
                Instantiate(Armor.armorPrefab, Shieldslot);

            }
            else
            {
                Instantiate(Armor.armorPrefab, ShieldNoWeaponslot);
            }

        }
        //TODO£ºÌæ»»¹¥»÷ÊôÐÔ
        characterData.ApplyArmor(Armor);
    }

    public void UnEquipAromr()
    {
        if (Shieldslot.transform.childCount != 0)
        {
            for (int i = 0; i < Shieldslot.transform.childCount; i++)
            {
                Destroy(Shieldslot.transform.GetChild(i).gameObject);
            }
        }
        if (ShieldNoWeaponslot.transform.childCount != 0)
        {
            for (int i = 0; i < ShieldNoWeaponslot.transform.childCount; i++)
            {
                Destroy(ShieldNoWeaponslot.transform.GetChild(i).gameObject);
            }
        }

        characterData.currentDefence = characterData.baseDefence;
    }
    #endregion

    #region ApplyDateChange
    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth )
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion
}
