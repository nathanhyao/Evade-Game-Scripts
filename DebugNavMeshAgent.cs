using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class DebugNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent agent = default;

    [SerializeField] private Transform playerTransform = default;

    // keep unchecked in Unity Inspector when game isn't running
    [SerializeField] private bool velocity = false;
    [SerializeField] private bool desiredVelocity = false;
    [SerializeField] private bool path = false;
    [SerializeField] private bool pathDistance = false;
    [SerializeField] private bool agentToPlayer = false;
    [SerializeField] private bool agentForward = false;

    private Vector3 textOffset;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        velocity = desiredVelocity = path = pathDistance = agentToPlayer = agentForward = true;

        Handles.color = Color.black;
        textOffset = new Vector3(0.0f, 2.5f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        if (velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }

        if (desiredVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }

        if (path)
        {
            Gizmos.color = Color.black;
            var agentPath = agent.path;
            Vector3 prevCorner = transform.position;
            foreach (var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);

                prevCorner = corner;
            }

            // line from agent destination to player
            if (pathDistance)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(prevCorner, playerTransform.position);

                Handles.Label(playerTransform.position + textOffset,
                (playerTransform.position - agent.destination).magnitude.ToString("#.00"));
            }
        }

        if (agentToPlayer)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }

        if (agentForward)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 50.0f);
        }
    }
}
