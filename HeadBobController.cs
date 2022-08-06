using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField] private float toggleSpeed = 3.0f;

    [SerializeField, Range(0, 0.1f)] public float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] public float frequency = 10.0f;

    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform camHolder = null;

    [SerializeField] private PlayerMovement playerMovementScript = null;

    private Vector3 startPos;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = cam.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) return;

        CheckMotion();
        ResetPosition();
        cam.LookAt(FocusTarget());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void PlayMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (speed < toggleSpeed || !playerMovementScript.grounded) return;

        PlayMotion(FootStepMotion());
    }

    private void ResetPosition()
    {
        if (cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 1 * Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + camHolder.localPosition.y, transform.position.z);
        pos += camHolder.forward * 15.0f;
        return pos;
    }
}
