using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform playerTransform;

    Vector3 playerToAgent;
    Vector3 agentToPlayer;
    private float deg; // player degrees (angle) offset from agent FOV centerline

    public float maxTime = 0.5f;
    [Tooltip("Will not recalculate destination until farther than maxDistance.")]
    public float maxDistance = 1.0f;
    private float timer = 0.0f;

    [Header("Attack")]
    [Tooltip("Align stoppingDistance with stomp attack range.")]
    public float stoppingDistance;
    [Tooltip("Agent rotates to face player after agent finishes attacking.")]
    public float rotationSpeed;
    [Tooltip("Player must be within attackFOV degrees of agent line of sight for attack to trigger.")]
    public float attackFOV;
    [Range(2.5f, 5)]
    public float size;

    private bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking) return;

        playerToAgent = transform.position - playerTransform.position;
        agentToPlayer = playerToAgent * -1;
        deg = Vector3.Angle(transform.forward, agentToPlayer);

        MovementControl();

        AttackControl();

        RotationControl();

        // timer to avoid recalculating destination every frame
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            FindDestination();

            timer = maxTime;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void FindDestination()
    {
        float sqDistance = (transform.position - playerTransform.position).sqrMagnitude;

        if (!agent.isStopped && sqDistance > maxDistance * maxDistance)
        {
            Vector3 destination = playerTransform.position + playerToAgent.normalized * stoppingDistance;
            agent.SetDestination(destination);
        }
    }

    private void MovementControl()
    {
        if (playerToAgent.sqrMagnitude < Mathf.Pow(stoppingDistance + size, 2))
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void AttackControl()
    {
        if (playerToAgent.sqrMagnitude < Mathf.Pow(stoppingDistance + size, 2) &&
        playerToAgent.sqrMagnitude > Mathf.Pow(stoppingDistance - size, 2) && deg < attackFOV)
        {
            animator.SetBool("canStompAttack", true);
        }
        else
        {
            animator.SetBool("canStompAttack", false);
        }
    }

    private void RotationControl()
    {
        // Rotate agent to face player after agent finishes attacking
        if (agent.isStopped && deg < attackFOV && animator.GetBool("canStompAttack"))
        {
            Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void isAttackingToggle()
    {
        isAttacking = !isAttacking;
    }
}
