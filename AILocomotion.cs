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

    [Header("Navigation")]
    public float maxTime = 0.5f;
    public float maxDistance = 1.0f;
    private float timer = 0.0f;

    [Header("Attacking")]
    public float heavyStompRange;
    public float lightStompRange;
    public float jumpRange;
    public float jumpingSpeed;
    public float turningSpeed;

    [Range(0.0f, 45.0f)] public float attackFOV;
    [Range(2.5f, 5.0f)] public float rangeError;

    private bool isStomping;
    private bool isJumping;
    private bool isAerial;
    private bool isTurning;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        isStomping = false;
        isJumping = false;
        isAerial = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("isStomping" + isStomping);
        // Debug.Log("isAerial" + isAerial);
        // Debug.Log("isJumping" + isJumping);

        if (isStomping)
        {
            return;
        }
        else if (isAerial)
        {
            var step = jumpingSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);
            return;
        }
        // isAerial -> isJumping (conditional)
        else if (isJumping)
        {
            return;
        }

        playerToAgent = transform.position - playerTransform.position;
        agentToPlayer = playerToAgent * -1;
        deg = Vector3.Angle(transform.forward, agentToPlayer);

        // timer to avoid recalculating destination every frame
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            FindDestination();

            timer = maxTime;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        MovementControl();

        AttackControl();

        RotationControl();

    }

    private void FindDestination()
    {
        float sqDistance = (transform.position - playerTransform.position).sqrMagnitude;

        if (!agent.isStopped && sqDistance > maxDistance * maxDistance)
        {
            // Vector3 destination = playerTransform.position + playerToAgent.normalized * heavyStompRange;
            agent.SetDestination(playerTransform.position);
        }
    }

    private void MovementControl()
    {
        if (inHeavyStompRange() || inLightStompRange() || inJumpRange())
        {
            agent.ResetPath(); // helps prevent offset overshoot with agent.autoBreaking true
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void AttackControl()
    {
        // Attack - Heavy Stomp (Long Range)
        if (inHeavyStompRange())
        {
            animator.SetBool("canHeavyStomp", true);
            isStomping = true;
            return;
        }

        // Attack - Light Stomp (Short Range)
        if (inLightStompRange())
        {
            animator.SetBool("canLightStomp", true);
            isStomping = true;
            return;
        }

        // Attack - Jump at Player
        if (inJumpRange())
        {
            animator.SetBool("canJump", true);
            isJumping = true;
        }
    }

    private void RotationControl()
    {
        // Turn agent to face player if player still within attackFOV and stomp attack finishes
        if (agent.isStopped && deg < attackFOV && (animator.GetBool("canHeavyStomp") || animator.GetBool("canLightStomp")))
        {
            Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turningSpeed * Time.deltaTime);
        }

        // todo: Play turn animation if player >= 90 degrees offset from line of agent line of sight
    }

    private bool inHeavyStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(heavyStompRange + rangeError, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(heavyStompRange - rangeError, 2);
    }

    private bool inLightStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(lightStompRange + rangeError, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(lightStompRange - rangeError, 2);
    }

    private bool inJumpRange()
    {
        return playerToAgent.sqrMagnitude < Mathf.Pow(jumpRange + rangeError, 2);
    }

    // animation events

    public void isAerialToggle()
    {
        isAerial = !isAerial;
    }

    public void resetStomp()
    {
        isStomping = false;
        animator.SetBool("canHeavyStomp", false);
        animator.SetBool("canLightStomp", false);
    }

    public void resetJump()
    {
        animator.SetBool("canJump", false);
        isJumping = false;
    }
}
