using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform playerTransform;
    private bool isClose; // player is within stoppingDistance

    public float maxTime = 1.0f;
    [Tooltip("Will not recalculate destination until farther than maxDistance.")]
    public float maxDistance = 1.0f;
    private float timer = 0.0f;

    [Header("Attack")]
    public float stoppingDistance;
    [Range(2.5f, 5)] public float size;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        isClose = false;
    }

    // Update is called once per frame
    void Update()
    {
        // timer to avoid recalculating destination every frame
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            Vector3 playerToAgent = transform.position - playerTransform.position;

            if (playerToAgent.sqrMagnitude < Mathf.Pow(stoppingDistance, 2))
            {
                agent.isStopped = true;
            }
            else // todo: check that stomp has finished
            {
                agent.isStopped = false;
            }

            Vector3 destination = playerTransform.position + playerToAgent.normalized * stoppingDistance;
            float sqDistance = (transform.position - playerTransform.position).sqrMagnitude;
            if (!agent.isStopped && sqDistance > maxDistance * maxDistance)
            {
                agent.SetDestination(destination);
            }

            if (playerToAgent.sqrMagnitude < Mathf.Pow(stoppingDistance + size, 2) &&
            playerToAgent.sqrMagnitude > Mathf.Pow(stoppingDistance - size, 2))
            {
                animator.SetBool("atRadius", true);
            }
            else
            {
                animator.SetBool("atRadius", false);
            }

            timer = maxTime;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
