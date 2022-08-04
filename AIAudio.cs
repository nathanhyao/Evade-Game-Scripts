using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudio : MonoBehaviour
{
    private AudioSource source;

    [Header("Movement Audio")]
    public AudioClip walkSound;
    public AudioClip attackSound;
    public AudioClip stompWhooshSound;
    public AudioClip jumpWhooshSound;

    [Header("Randomizer")]
    [Range(0.0f, 0.5f)] public float volumeChangeMultiplier = 0.2f;
    [Range(0.0f, 0.5f)] public float pitchChangeMultiplier = 0.2f;

    [Header("Speech Audio")]
    public AudioClip[] encounterSpeech;
    public AudioClip[] killSpeech;

    private float volume;
    private float pitch;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        volume = source.volume;
        pitch = source.pitch;

        // play agent speech for first encounter
        if (encounterSpeech.Length == 0) return;
        source.PlayOneShot(encounterSpeech[Random.Range(0, encounterSpeech.Length)]);
    }

    // Update is called once per frame
    void Update()
    {
        // play agent speech after player death
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (killSpeech.Length == 0) return;
            source.PlayOneShot(killSpeech[Random.Range(0, killSpeech.Length)]);
        }
    }

    private void RandomizeSound()
    {
        source.volume = Random.Range(volume - volumeChangeMultiplier, volume);
        source.pitch = Random.Range(pitch - pitchChangeMultiplier, pitch + pitchChangeMultiplier);
    }

    // Animation Events

    public void PlayWalkSound()
    {
        RandomizeSound();
        source.PlayOneShot(walkSound);
    }

    public void PlayAttackSound()
    {
        RandomizeSound();
        source.PlayOneShot(attackSound);
    }

    public void PlayStompWhooshSound()
    {
        RandomizeSound();
        source.PlayOneShot(stompWhooshSound);
    }

    public void PlayJumpWhooshSound()
    {
        RandomizeSound();
        source.PlayOneShot(jumpWhooshSound);
    }
}
