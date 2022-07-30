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

    [Header("Attack")]
    public float heavyStompRange;
    public float lightStompRange;
    public float jumpRange;
    public float rotationSpeed;
    public float jumpingSpeed;

    [Range(0.0f, 180.0f)] public float attackFOV;
    [Range(2.5f, 5.0f)] public float size;

    private bool isStomping;
    private bool isJumping;
    private bool isAerial;

    private string jumpParameter;
    private bool isChosen;

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
            // agent.ResetPath(); // helps prevent offset overshoot with agent.autoBreaking true
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
            return;
        }
        else
        {
            animator.SetBool("canHeavyStomp", false);
        }

        // Attack - Light Stomp (Short Range)
        if (inLightStompRange())
        {
            animator.SetBool("canLightStomp", true);
            return;
        }
        else
        {
            animator.SetBool("canLightStomp", false);
        }

        // Attack - Jump at Player
        if (!isChosen && inJumpRange())
        {
            jumpParameter = Random.value > 0.5f ? "doSquatJump" : "doSkipJump";
            animator.SetBool(jumpParameter, true);
            isChosen = true;
        }
    }

    private void RotationControl()
    {
        // Rotate agent to face player after agent finishes a stomp attack
        if (agent.isStopped && deg < attackFOV && (animator.GetBool("canHeavyStomp") || animator.GetBool("canLightStomp")))
        {
            Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private bool inHeavyStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(heavyStompRange + size, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(heavyStompRange - size, 2);
    }

    private bool inLightStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(lightStompRange + size, 2)
            && playerToAgent.sqrMagnitude > Mathf.Pow(lightStompRange - size, 2);
    }

    private bool inJumpRange()
    {
        return playerToAgent.sqrMagnitude < Mathf.Pow(jumpRange + size, 2);
    }

    public void isStompingToggle()
    {
        isStomping = !isStomping;
    }

    public void isJumpingToggle()
    {
        isJumping = !isJumping;
    }

    public void isAerialToggle()
    {
        isAerial = !isAerial;
    }

    public void resetJump()
    {
        animator.SetBool(jumpParameter, false);
        isChosen = false;
    }
}
