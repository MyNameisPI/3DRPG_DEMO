using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{

    public EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    protected CharacterStats enemyStats;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;
    public float speed;

    protected GameObject attackTarget;
    public float lookTime;
    private float remainlookTime;
    private float lastAttackTime;
    private Quaternion guardRotation;

    //bool≈‰∫œ∂Øª≠
    bool isWalk;
    bool isChase;
    bool isFollow;
    public bool isGuard;
    protected bool isDeath;
    bool isPlayerDead;


    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    private Vector3 guardPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainlookTime = lookTime;
        enemyStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
    }
    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
    }

    private void OnEnable()
    {
        //GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>()&&isDeath)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }

    }
    virtual protected void Update()
    {
        isDeath = enemyStats.CurrentHealth == 0;
        if (!isPlayerDead)
        {
            SwitchStates();
            SwitchAnimation();
        }
        
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", enemyStats.isCritical);
        anim.SetBool("Death", isDeath);
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchStates()
    {
        if (isDeath) enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
        { 
            enemyStates = EnemyStates.CHASE;
            Debug.Log("’“µΩPlayer");
        
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.Distance(guardPos,transform.position)<=agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                if (Vector3.Distance(wayPoint, transform.position) <= 1f)
                {
                    isWalk = false;
                    if (remainlookTime>0)
                    {
                        remainlookTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                   
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //TODO£∫◊∑Player

                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainlookTime>0)
                    {
                        agent.destination = transform.position;
                        remainlookTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }

                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = enemyStats.attackData.coolDown;
                        //±©ª˜º∆À„
                        enemyStats.isCritical = Random.value < enemyStats.attackData.criticalChance;
                        Attack();
                    }

                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;

                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
            default:
                break;
        }
    }
    
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }


    bool FoundPlayer()
    {
        var collliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in collliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.attackRange;
        }
        else return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.skillRange;
        }
        else return false;
    }

    void GetNewWayPoint()
    {
        remainlookTime = lookTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : randomPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //AnimationEvent
    void Hit()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(enemyStats, targetStats);
        }
    }

    public void EndNotify()
    {
        Debug.Log("ENemy win");
        anim.SetBool("Win", true);
        isPlayerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }

    public void PlayerWin()
    {
        
    }
}
