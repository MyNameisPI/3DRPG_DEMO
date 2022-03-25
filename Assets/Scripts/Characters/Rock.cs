using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}

    private Rigidbody rb;

    [Header("Basic Setting")]
    public float force;
    public GameObject target;

    private Vector3 direction;

    public RockStates rockStates;
    public GameObject breakEffect;


    public int damage;
    public float DestoryTime;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1)
        {
            rockStates = RockStates.HitNothing;
            DestoryTime -= Time.deltaTime;
            if (DestoryTime<0)
            {
                Instantiate(breakEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
    void FlyToTarget()
    {
        if (target == null) target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);   
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");

                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                }
                rockStates = RockStates.HitNothing;
                break;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())
                {
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);

                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
            default:
                break;
        }

    }

}
