using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource source = default;
    private Rigidbody playerRb = default;
    private PlayerMovement pms = default;

    [Header("Footstep")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioClip[] stepSounds = default;

    private float footstepTimer = 0.0f;
    private float GetCurrentOffset => pms.state == PlayerMovement.MovementState.crouching ? baseStepSpeed * crouchStepMultiplier
                                    : pms.state == PlayerMovement.MovementState.sprinting ? baseStepSpeed * sprintStepMultiplier
                                    : baseStepSpeed;

    [Header("Death")]
    [SerializeField] private AudioClip[] deathSounds = default;

    [Header("Randomizer")]
    [SerializeField, Range(0.0f, 1.0f)] private float volumeChangeMultiplier = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] private float pitchChangeMultiplier = 0.5f;

    private float volume;
    private float pitch;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        playerRb = GetComponent<Rigidbody>();
        pms = GetComponent<PlayerMovement>();

        volume = source.volume;
        pitch = source.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        HandleFootsteps();
    }

    private void RandomizeSound()
    {
        source.volume = Random.Range(volume - volumeChangeMultiplier, volume + volumeChangeMultiplier);
        source.pitch = Random.Range(pitch - pitchChangeMultiplier, pitch + pitchChangeMultiplier);
    }

    private void HandleFootsteps()
    {
        if (!pms.grounded || playerRb.velocity == Vector3.zero) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            RandomizeSound();
            source.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
            footstepTimer = GetCurrentOffset;
        }
    }

    public void PlayDeathSound()
    {
        source.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
    }
}
