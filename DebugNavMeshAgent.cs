using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class DebugNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private Transform playerTransform;

    // keep unchecked in Unity Inspector when game isn't running
    [Header("NavMeshAgent")]
    [SerializeField] private bool velocity;
    [SerializeField] private bool desiredVelocity;
    [SerializeField] private bool path;

    [Header("Player")]
    [SerializeField] private bool pathDistance;
    private Vector3 textOffset;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        velocity = desiredVelocity = path = pathDistance = true;

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
    }
}
