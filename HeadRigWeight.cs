using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadRigWeight : MonoBehaviour
{
    private Rig rig = default;
    private bool isLooking = false;

    private float currentVelocity = 0.0f;
    private float weightTarget = 0.0f;

    private float timer = 0.0f;
    private float maxTime = 1.0f;

    [Header("Head Rig Weight")]
    [SerializeField] private float transitionTime;
    [SerializeField, Range(0.0f, 180.0f)] public float degStopLook;
    [SerializeField, Range(0.0f, 180.0f)] public float degStartLook;
    [SerializeField] private float distStopLook;
    [SerializeField] public float distStartLook;

    [Header("AI & Player Reference")]
    [SerializeField] private GameObject ai;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rig>();
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

        // Smooth transition AI from not-looking to looking, or vice versa
        rig.weight = Mathf.SmoothDamp(rig.weight, weightTarget, ref currentVelocity, transitionTime);
    }

    private void LookControl()
    {
        Vector3 gtsToPlayer = player.transform.position - ai.transform.position;
        float deg = Vector3.Angle(ai.transform.forward, gtsToPlayer);

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
