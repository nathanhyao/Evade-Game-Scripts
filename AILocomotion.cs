using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    public MovementState state;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Player Reference")]
    [SerializeField] private GameObject player = default;
    [SerializeField] private Rigidbody playerRb = default;

    internal Vector3 playerToAgent;
    internal Vector3 agentToPlayer;
    private float deg;

    [Header("Navigation")]
    [SerializeField, Range(0.0f, 300.0f)] private float maxDistance = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float maxNavigationTime = 0.5f;
    private float navigationTimer = 0.0f;

    [Header("Attacking")]
    [SerializeField] private float heavyStompRange = 55.0f;
    [SerializeField] private float lightStompRange = 25.5f;
    [SerializeField] private float jumpRange = 20.0f;

    [Header("Attacking Tweaks")]
    [SerializeField, Range(0.0f, 10.0f)] private float rangeFix = 6.5f;
    [SerializeField, Range(2.5f, 7.5f)] private float rangeError = 3.0f;
    [SerializeField, Range(0.0f, 45.0f)] private float attackFOV = 20.0f;

    private float rangeFixMultiplier;

    [SerializeField] private float jumpSpeed = 15.0f;
    [SerializeField] private float animationTurnSpeed = 40.0f;
    [SerializeField] private float heavyStompTurnSpeed = 10.0f;
    [SerializeField] private float lightStompTurnSpeed = 50.0f;

    private bool isStomping;
    private bool isJumping;
    private bool isAerial;
    private bool isTurning;

    public enum MovementState
    {
        walking,
        stomping,
        jumping,
        turning
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FindRelativePosition();

        if (isStomping || isTurning)
        {
            RotateTowards();
            return;
        }
        else if (isAerial)
        {
            JumpTowards();
            return;
        }
        else if (isJumping)
        {
            return;
        }

        RangeCorrection();
        MobilityControl();
        AttackControl();
        TurnControl();

        // timer avoids recalculating destination every frame
        navigationTimer -= Time.deltaTime;
        if (navigationTimer < 0.0f)
        {
            FindDestination();
            navigationTimer = maxNavigationTime;
        }
    }

    private void RangeCorrection()
    {
        // If player is moving away then agent will attack more forward (path prediction)
        if (playerRb.velocity == Vector3.zero)
        {
            rangeFixMultiplier = 0.0f;
            return;
        }
        rangeFixMultiplier = (180.0f - Vector3.Angle(playerRb.velocity, transform.forward)) / 180.0f;

        if (playerRb.velocity.sqrMagnitude > Mathf.Pow(6.0f, 2.0f) && rangeFixMultiplier > 0.975f)
            rangeFixMultiplier = 2.0f;
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
            state = MovementState.walking;
        }
    }

    private void MobilityControl()
    {
        // Core animation parameter - Idle / Walking
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (InHeavyStompRange() || InLightStompRange() || InJumpRange() || InTurnRange())
        {
            agent.ResetPath(); // prevent stopping distance overshoot
            agent.isStopped = true;
        }
        else
            agent.isStopped = false;
    }

    private void AttackControl()
    {
        // Attack - Heavy Stomp (Long Range)
        if (InHeavyStompRange())
        {
            isStomping = true;
            state = MovementState.stomping;
            animator.SetBool("canHeavyStomp", true);
            return;
        }

        // Attack - Light Stomp (Short Range)
        else if (InLightStompRange())
        {
            isStomping = true;
            state = MovementState.stomping;
            animator.SetBool("canLightStomp", true);
            return;
        }

        // Attack - Jump at Player
        else if (InJumpRange())
        {
            isJumping = true;
            state = MovementState.jumping;
            animator.SetBool("canJump", true);
        }
    }

    private void JumpTowards()
    {
        // Agent moves towards player in airtime
        var step = jumpSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
    }

    private void TurnControl()
    {
        // Play TURN animation if player > 90 degrees from agent line of sight
        if (Vector3.Angle(agentToPlayer, (transform.right + transform.forward * -1)) < 45.0f)
        {
            isTurning = true;
            state = MovementState.turning;
            animator.SetBool("canRightTurn", true);
        }
        else if (Vector3.Angle(agentToPlayer, (transform.right * -1 + transform.forward * -1)) < 45.0f)
        {
            isTurning = true;
            state = MovementState.turning;
            animator.SetBool("canLeftTurn", true);
        }
    }

    private void RotateTowards()
    {
        // Rotate enemy to face player during turn animation or stomp attack
        float step = 0.0f;

        if (animator.GetBool("canHeavyStomp"))
            step = heavyStompTurnSpeed * Time.deltaTime;
        else if (animator.GetBool("canLightStomp"))
            step = lightStompTurnSpeed * Time.deltaTime;
        else if (isTurning)
            step = animationTurnSpeed * Time.deltaTime;

        Quaternion toRotation = Quaternion.LookRotation(agentToPlayer, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, step);
    }

    private bool InHeavyStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(heavyStompRange + rangeError - rangeFix * rangeFixMultiplier, 2.0f)
            && playerToAgent.sqrMagnitude > Mathf.Pow(heavyStompRange - rangeError - rangeFix * rangeFixMultiplier, 2.0f);
    }

    private bool InLightStompRange()
    {
        return deg < attackFOV
            && playerToAgent.sqrMagnitude < Mathf.Pow(lightStompRange + rangeError - rangeFix * rangeFixMultiplier, 2.0f)
            && playerToAgent.sqrMagnitude > Mathf.Pow(lightStompRange - rangeError - rangeFix * rangeFixMultiplier, 2.0f);
    }

    private bool InJumpRange()
    {
        return deg < attackFOV && playerToAgent.sqrMagnitude < Mathf.Pow(jumpRange, 2.0f);
    }

    private bool InTurnRange()
    {
        return Vector3.Angle(agentToPlayer, transform.forward * -1) < 90.0f;
    }

    // This function is called when the behaviour becomes disabled
    private void OnDisable()
    {
        Time.timeScale = 0.7f;
        state = MovementState.walking;
        StartCoroutine(ResetAnimation());
    }

    private IEnumerator ResetAnimation()
    {
        float timeElapsed = 0.0f;
        float lerpDuration = 1.5f;
        float startValue = animator.GetFloat("Speed");

        while (timeElapsed < lerpDuration)
        {
            animator.SetFloat("Speed", Mathf.Lerp(startValue, 0.0f, timeElapsed / lerpDuration));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("Speed", 0.0f);
    }

    // Animation Events

    public void IsAerialToggle()
    {
        isAerial = !isAerial;
    }

    public void ResetStomp()
    {
        isStomping = false;
        animator.SetBool("canHeavyStomp", false);
        animator.SetBool("canLightStomp", false);
    }

    public void ResetJump()
    {
        isJumping = false;
        animator.SetBool("canJump", false);
    }

    public void ResetTurn()
    {
        isTurning = false;
        animator.SetBool("canLeftTurn", false);
        animator.SetBool("canRightTurn", false);
    }
}