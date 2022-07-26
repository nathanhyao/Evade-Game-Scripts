using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform playerTransform;
    private bool isClose; // player within maxRadius

    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    private float timer = 0.0f;

    [Header("Attack")]
    public float maxRadius;
    public float minRadius;

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

            if (playerToAgent.sqrMagnitude < maxRadius * maxRadius)
                isClose = true;
            else
                isClose = false;

            Vector3 destination = playerTransform.position + playerToAgent.normalized * maxRadius;
            float sqDistance = (transform.position - destination).sqrMagnitude;
            if (!isClose && sqDistance > maxDistance * maxDistance)
            {
                agent.SetDestination(destination);
            }

            timer = maxTime;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
