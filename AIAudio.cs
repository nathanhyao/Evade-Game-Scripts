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

    private float volume;
    private float pitch;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        volume = source.volume;
        pitch = source.pitch;
    }

    public void playWalkSound()
    {
        source.volume = Random.Range(volume - volumeChangeMultiplier, volume);
        source.pitch = Random.Range(pitch - pitchChangeMultiplier, pitch + pitchChangeMultiplier);
        source.PlayOneShot(walkSound);
    }
}
