using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip DefaultFootstepSound = default;
    private AudioSource audioSource;
    private Collider2D[] colliders = new Collider2D[1];
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (DefaultFootstepSound != null)
        {
            audioSource.clip = DefaultFootstepSound;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAtPosition(float x, float y, float velocity)
    {
        transform.position = new Vector3(x, y, 0f);
        FootstepCalculations.ConfigureAudioSourceForVelocity(audioSource, velocity);

        AudioClip footstepClip = FootstepCalculations.FootstepSoundAtPosition(transform.position, colliders);
        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
        }

        audioSource.Play();
    }
}
