using System;
using OatsUtil;
using UnityEngine;

public static class FootstepCalculations
{
    public static float KeystrokeTimeToVelocity(float keystrokeTime)
        => Mathf.Clamp01(NumberUtils.MapRange(0f, 0.5f, 1f, 0f, keystrokeTime));

    public static float VelocityToPitch(float velocity)
        => Mathf.Lerp(0.4f, 1.2f, velocity);

    public static float VelocityToVolume(float velocity)
        => Mathf.Lerp(0.5f, 1.0f, velocity);

    public static void ConfigureAudioSourceForVelocity(AudioSource audioSource, float velocity)
    {
        audioSource.pitch = VelocityToPitch(velocity);
        audioSource.volume = VelocityToVolume(velocity);
    }

    public static AudioClip FootstepSoundAtPosition(Vector3 position, Collider2D[] colliders)
    {
        if (Physics2D.OverlapCircleNonAlloc(position, 0.1f, colliders) > 0)
        {
            var region = colliders[0].GetComponent<SurfaceRegion>();
            if (region != null)
            {
                return region.Sound;
            }
        }
        return null;
    }
}
