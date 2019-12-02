using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FootstepSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAtPosition(float x, float y, float velocity)
    {
        transform.position = new Vector3(x, y, 0f);
        FootstepCalculations.ConfigureAudioSourceForVelocity(audioSource, velocity);
        audioSource.Play();
    }
}
