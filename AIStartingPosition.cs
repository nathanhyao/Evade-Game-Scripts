using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStartingPosition : MonoBehaviour
{
    [SerializeField] private float distance = 340.0f;

    [SerializeField] private Transform playerTransform = default;
    [SerializeField] private Cloth capeCloth = default;

    // Start is called before the first frame update
    void Start()
    {
        float xRandom = Random.Range(-1.0f, 1.0f);
        float zRandom = Random.Range(-1.0f, 1.0f);

        transform.position = new Vector3(xRandom, 0.0f, zRandom).normalized * distance;

        Vector3 agentToPlayer = playerTransform.position - transform.position;
        transform.forward = agentToPlayer;

        if (capeCloth && capeCloth.enabled)
        {
            // Prevent cloth glitching from sudden position change
            capeCloth.enabled = false;
            capeCloth.enabled = true;
        }
    }
}
