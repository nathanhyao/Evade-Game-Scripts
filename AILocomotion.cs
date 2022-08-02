using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    [Header("Player Ref")]
    public GameObject player;
    PlayerMovement playerMovement;

    internal Vector3 playerToAgent;
    internal Vector3 agentToPlayer;

    // player degrees (angle) offset from agent FOV centerline
    private float deg;

    [Header("Navigation")]
    public float maxDistance = 1.0f;

    [Range(0.0f, 1.0f)] public float maxTime = 0.5f;
    private float timer = 0.0f;

    [Header("Attacking")]
    public float heavyStompRange;
    public float lightStompRange;
    public float jumpRange;

    [Header("Attacking Tweaks")]
    [Range(0.0f, 10.0f)] public float rangeFix;
    [Range(2.5f, 7.5f)] public float rangeError;
    [Range(0.0f, 45.0f)] public float attackFOV;

    private float rangeFixMultiplier;

    public float jumpSpeed;
    public float turnSpeed;

    private bool isStomping;
    private bool isJumping;
    private bool isAerial;
    private bool isTurning;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement>();

        isStomping = false;
        isJumping = false;
        isAerial = false;
        isTurning = false;
    }

    // Update is called once per frame
    void Update()
    {
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

        rangeCorrection();
        FindRelativePosition();

        // timer avoids recalculating destination every frame
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            FindDestination();

            timer = maxTime;
        }

        MobilityControl();
        AttackControl();
        TurnLeftOrRight();
        RotationControl();
    }

    private void rangeCorrection()
    {
        // If player is moving away, agent will attack more forward (path prediction)
        if (playerMovement.moveDirection == Vector3.zero)
        {
            rangeFixMultiplier = 0.0f;
            return;
        }
        rangeFixMultiplier = (180.0f - Vector3.Angle(playerMovement.moveDirection, transform.forward)) / 180.0f;
    }

    private void FindRelativePosition()
    {
        // Useful information for other methods
        playerToAgent = transform.position - player.transform.position;
        agentToPlayer = playerToAgent * -1;
        deg = Vector3.Angle(transform.forward, agentToPlayer);
    }

    private void FindDestination()
    {
        float sqDistance = (transform.position - player.transform.position).sqrMagnitude;

        if (!agent.isStopped && sqDistance > maxDistance * maxDistance)
        {
            // Vector3 destination = playerTransform.position + playerToAgent.normalized * heavyStompRange;
            agent.SetDestination(player.transform.position);
        }
    }

    private void MobilityControl()
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
        // Agent moves towards player in airtime
        var step = jumpSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
    }

    private void TurnLeftOrRight()
    {
        // Play TURN animation if player > 90 degrees from agent line of sight
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
        // Rotate agent to face player after stomp attacks or during turn animations
        if (inHeavyStompRange() || inLightStompRange() || isTurning)
        {
            var step = isTurning ? turnSpeed * Time.deltaTime * 2.0f : turnSpeed * Time.deltaTime;

            Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, step);
        }
    }

    private bool inHeavyStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(heavyStompRange + rangeError - rangeFix * rangeFixMultiplier, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(heavyStompRange - rangeError - rangeFix * rangeFixMultiplier, 2);
    }

    private bool inLightStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(lightStompRange + rangeError - rangeFix * rangeFixMultiplier, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(lightStompRange - rangeError - rangeFix * rangeFixMultiplier, 2);
    }

    private bool inJumpRange()
    {
        return playerToAgent.sqrMagnitude < Mathf.Pow(jumpRange, 2);
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
