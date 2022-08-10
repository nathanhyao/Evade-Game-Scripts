using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private PlayerMovement playerMovementScript = default;
    [SerializeField] private Rigidbody playerRb = default;

    [Header("Player Effects")]
    [SerializeField] private float flattenYScale = 0.1f;

    [Header("Object References")]
    [SerializeField] private GameObject enemy = default;
    [SerializeField] private GameObject camHolder = default;
    [SerializeField] private GameObject damageOverlay = default;

    private AILocomotion aiLocomotionScript = default;
    private AIAudio aiAudioScript = default;
    private PlayerCam playerCamScript = default;

    private bool isDead = false;

    void Start()
    {
        aiLocomotionScript = enemy.GetComponent<AILocomotion>();
        aiAudioScript = enemy.GetComponent<AIAudio>();
        playerCamScript = camHolder.GetComponent<PlayerCam>();
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (!isDead && otherCollider.gameObject.tag == "Enemy")
        {
            isDead = true;

            damageOverlay.SetActive(true);

            aiAudioScript.Invoke("PlayKillSpeech", 1.5f);

            playerMovementScript.enabled = false;
            aiLocomotionScript.enabled = false;

            transform.localScale = new Vector3(transform.localScale.x, flattenYScale, transform.localScale.z);
            playerRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            playerCamScript.sensX = playerCamScript.sensX / 4f;
            playerCamScript.sensY = playerCamScript.sensY / 4f;
        }
    }
}