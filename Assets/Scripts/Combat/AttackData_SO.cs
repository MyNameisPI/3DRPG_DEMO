using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;

    //暴击计算
    public float criticalMultipler;
    public float criticalChance;

    //应用攻击数据
    public void ApplyWeaponData(AttackData_SO weapon)
    {
        attackRange += weapon.attackRange;
        skillRange += weapon.skillRange;
        coolDown += weapon.coolDown;
        minDamage += weapon.minDamage;
        maxDamage += weapon.maxDamage;
        criticalMultipler += weapon.criticalMultipler;
        criticalChance += weapon.criticalChance;
    }

    public void ApplyBaseData(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;
        criticalMultipler = weapon.criticalMultipler;
        criticalChance = weapon.criticalChance;
    }

}
