using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats playerStats;
    bool isDead;

    private GameObject attackTarget;
    private float lastAttackTime;

    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }
    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }
    private void OnEnable()
    {
        GameManager.Instance.RigisterPlayer(playerStats);
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = playerStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position,transform.position)>playerStats.attackData.attackRange)
            
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        
        while (attackTarget!=null
            &&attackTarget.CompareTag("Enemy")
            &&attackTarget.GetComponent<CharacterStats>().characterData.currentHealt>0
            && Vector3.Distance(attackTarget.transform.position, transform.position) < playerStats.attackData.attackRange)
        {
            if (lastAttackTime < 0)
            {
                playerStats.isCritical = UnityEngine.Random.value < playerStats.attackData.criticalChance;
                anim.SetBool("Critical", playerStats.isCritical);
                anim.SetTrigger("Attack");
                lastAttackTime = playerStats.attackData.coolDown;

            }
            yield return null;
        }


        if(attackTarget.CompareTag("Attackable"))
        {
            if(lastAttackTime < 0)
            {
                anim.SetBool("Critical", playerStats.isCritical);
                anim.SetTrigger("Attack");
                lastAttackTime = playerStats.attackData.coolDown;
            }
            yield return null;
        }
    }

    //AnimationEvent
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>()&& attackTarget.GetComponent<Rock>().rockStates== Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;

                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(playerStats, targetStats);
        }
       
    }



    private void Update()
    {
        isDead = playerStats.CurrentHealth <= 0;
        Debug.Log("Player isDead =" + isDead);
        if (isDead) GameManager.Instance.NotifyObserver();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    void SwitchAnimation()
    {
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void EndNotify()
    {
        
    }

    public void PlayerWin()
    {
        
    }
}
