using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;

    public GameObject rockPrefab;

    public Transform handPos;

    bool IsWin;


    protected override void Update()
    {
        base.Update();
        if (isDeath)
        {
            GameManager.Instance.WInNotify();
        }
    }

    //Animation Event
    public void Kickoff()
    {
        if (attackTarget != null&&transform.isFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform);

            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }

    public void ThrowRock()
    {
        if ((attackTarget!=null))
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
