using UnityEngine;
using System.Collections;

public class SurfaceRegion : MonoBehaviour
{
    [SerializeField] private AudioClip FootstepSound = default;

    public AudioClip Sound => FootstepSound;
}
