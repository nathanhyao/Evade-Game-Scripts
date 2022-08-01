using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform playerTransform;

    internal Vector3 playerToAgent;
    internal Vector3 agentToPlayer;

    private float deg; // player degrees (angle) offset from agent FOV centerline

    [Header("Navigation")]
    public float maxDistance = 1.0f;

    [Range(0.0f, 1.0f)] public float maxTime = 0.5f;
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
        isTurning = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("isStomping " + isStomping);
        Debug.Log("isJumping " + isJumping);
        Debug.Log("isAerial " + isAerial);
        Debug.Log("isTurning " + isTurning);

        if (isStomping || isTurning)
        {
            RotationControl();
            return;
        }
        else if (isAerial)
        {
            JumpTowards();
            return;
        }
        // isAerial -> isJumping (conditional)
        else if (isJumping)
        {
            return;
        }

        FindRelativePosition();

        // timer avoids recalculating destination every frame
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            FindDestination();

            timer = maxTime;
        }

        MovementControl();
        AttackControl();
        TurnLeftOrRight();
        RotationControl();
    }

    private void FindRelativePosition()
    {
        // Useful information to find relative positions of agent and player
        playerToAgent = transform.position - playerTransform.position;
        agentToPlayer = playerToAgent * -1;
        deg = Vector3.Angle(transform.forward, agentToPlayer);
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
        // Core animation parameter - Idle / Walking
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (inHeavyStompRange() || inLightStompRange() || inJumpRange() || inTurnRange())
        {
            agent.ResetPath(); // prevent stopping distance overshoot while agent.autoBreaking true
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
            isStomping = true;
            animator.SetBool("canHeavyStomp", true);
            return;
        }

        // Attack - Light Stomp (Short Range)
        else if (inLightStompRange())
        {
            isStomping = true;
            animator.SetBool("canLightStomp", true);
            return;
        }

        // Attack - Jump at Player
        else if (inJumpRange())
        {
            isJumping = true;
            animator.SetBool("canJump", true);
        }
    }

    private void JumpTowards()
    {
        // Agent towards player in airtime
        var step = jumpingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);
    }

    private void TurnLeftOrRight()
    {
        // Play TURN animation if player > 90 degrees offset from line of agent line of sight
        if (Vector3.Angle(agentToPlayer, (transform.right + transform.forward * -1)) < 45.0f)
        {
            isTurning = true;
            animator.SetBool("canRightTurn", true);
        }
        else if (Vector3.Angle(agentToPlayer, (transform.right * -1 + transform.forward * -1)) < 45.0f)
        {
            isTurning = true;
            animator.SetBool("canLeftTurn", true);
        }
    }

    private void RotationControl()
    {
        // Turn agent to face player after a stomp attack or agent is playing turning animation
        if (inHeavyStompRange() || inLightStompRange() || isTurning)
        {
            var step = isTurning ? turningSpeed * Time.deltaTime * 2.0f : turningSpeed * Time.deltaTime;

            Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, step);
        }
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

    private bool inTurnRange()
    {
        return Vector3.Angle(agentToPlayer, transform.forward * -1) < 90.0f;
    }

    // Animation Events

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
        isJumping = false;
        animator.SetBool("canJump", false);
    }

    public void resetTurn()
    {
        isTurning = false;
        animator.SetBool("canLeftTurn", false);
        animator.SetBool("canRightTurn", false);
    }
}
