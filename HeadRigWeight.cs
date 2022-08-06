using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class HeadRigWeight : MonoBehaviour
{
    private bool isLooking;

    private float currentVelocity;
    private float weightTarget;
    private float timer = 0.0f;
    private float maxTime = 1.0f;

    [SerializeField] private float transitionTime;
    [SerializeField, Range(0.0f, 180.0f)] public float degStopLook;
    [SerializeField, Range(0.0f, 180.0f)] public float degStartLook;
    [SerializeField] private float distStopLook;
    [SerializeField] public float distStartLook;

    [SerializeField] private GameObject gts;
    [SerializeField] private GameObject player;

    private NavMeshAgent gtsAgent;
    private Rig rig;

    // Start is called before the first frame update
    void Start()
    {
        gtsAgent = gts.GetComponent<NavMeshAgent>();
        rig = GetComponent<Rig>();
        currentVelocity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            LookControl();

            timer = maxTime;
        }

        rig.weight = Mathf.SmoothDamp(rig.weight, weightTarget, ref currentVelocity, transitionTime);
    }

    private void LookControl()
    {
        Vector3 gtsToPlayer = player.transform.position - gts.transform.position;
        float deg = Vector3.Angle(gts.transform.forward, gtsToPlayer);

        if (isLooking && (gtsToPlayer.sqrMagnitude < Mathf.Pow(distStopLook, 2) || deg > degStopLook))
        {
            isLooking = false;
            weightTarget = 0.0f;
        }
        else if (!isLooking && (gtsToPlayer.sqrMagnitude >= Mathf.Pow(distStartLook, 2) && deg <= degStartLook))
        {
            isLooking = true;
            weightTarget = 1.0f;
        }
    }
}
