using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ObjectTub;
using OatsUtil;

public class FootstepSoundPlayer : PoolableObject
{
    [SerializeField] private AudioClip DefaultFootstepSound = default;
    private AudioSource audioSource;
    private Collider2D[] colliders = new Collider2D[1];

    void Awake()
    {
        audioSource = this.RequireComponent<AudioSource>();

        if (DefaultFootstepSound != null)
        {
            audioSource.clip = DefaultFootstepSound;
        }
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

        float soundTime = audioSource.clip.length * audioSource.pitch;
        StartCoroutine(ReleaseAfterTime(soundTime));
    }

    private IEnumerator ReleaseAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.PutObjectBackInTub(gameObject);
    }

    public override void InitializeForUse()
    {
        // nothing needed here
    }

    public override void PutAway()
    {
        // nothing needed here
        //ObjectPool.put
    }
}
