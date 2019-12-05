using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using OatsUtil;

public class CharController : MonoBehaviour
{
    [Header("Footstep movement")]
    [SerializeField] private float StepDistance = 1f;
    [SerializeField] private float StepTime = 0.1f;

    [Header("Footstep sound")]
    [SerializeField] private AudioClip StartStepSound = default;
    [SerializeField] private AudioClip FootStepSound = default;
    [SerializeField] private float PanSpread = 1f;

    AudioSource audioSource;

    [SerializeField] private bool networkWriteEnabled = true;

    private class KeyBinding
    {
        public KeyCode Key;
        public Vector3 Direction;
    }
    private List<KeyBinding> KeyBindings;

    private Collider2D[] colliders = new Collider2D[1];

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = SceneUtils.FindComponentInScene<GameManager>();

        audioSource = GetComponent<AudioSource>();

        KeyBindings = new List<KeyBinding>();
        KeyBindings.Add(new KeyBinding { Key = KeyCode.UpArrow, Direction = Vector3.up });
        KeyBindings.Add(new KeyBinding { Key = KeyCode.DownArrow, Direction = Vector3.down });
        KeyBindings.Add(new KeyBinding { Key = KeyCode.LeftArrow, Direction = Vector3.left });
        KeyBindings.Add(new KeyBinding { Key = KeyCode.RightArrow, Direction = Vector3.right });
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GameState == GameManager.GameStates.GAMEPLAY)
        {
            foreach (var keyBinding in KeyBindings)
            {
                if (Input.GetKeyDown(keyBinding.Key))
                {
                    StartCoroutine(Step(keyBinding.Key, keyBinding.Direction));
                }
            }
        }
    }

    private IEnumerator Step(KeyCode keyCode, Vector3 direction)
    {
        // wait for key to be released
        //audioSource.PlayOneShot(StartStepSound);
        float heldTime = 0f;
        while (Input.GetKey(keyCode))
        {
            heldTime += Time.deltaTime;
            yield return null;
        }

        // calculate velocity of step
        float stepVelocity = FootstepCalculations.KeystrokeTimeToVelocity(heldTime);

        // play step sound and add it to db
        FootstepCalculations.ConfigureAudioSourceForVelocity(audioSource, stepVelocity);
        var footstepClip = FootstepCalculations.FootstepSoundAtPosition(transform.position, colliders);

        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
        }

        audioSource.Play();
        if (networkWriteEnabled)
        {
            var newFS = new Footstep(
                transform.position.x.ToString("0.0"),
                transform.position.y.ToString("0.0"),
                gameManager.GameElapsedTime.ToString("0.000"),
                stepVelocity.ToString("0.00")
            );
            StartCoroutine(NetworkHandler.PostNewFootstep(newFS));
        }

        // move player object
        Vector3 oldPosition = transform.position;
        Vector3 newPosition = oldPosition + (direction * StepDistance);

        float startingPan = 0f;
        float endingPan = 0f;
        if (keyCode == KeyCode.LeftArrow)
        {
            startingPan = -PanSpread;
            endingPan = PanSpread;
        }
        if (keyCode == KeyCode.RightArrow)
        {
            startingPan = PanSpread;
            endingPan = -PanSpread;
        }

        float elapsedTime = 0f;
        while (elapsedTime < StepTime)
        {
            audioSource.panStereo = Mathf.Lerp(startingPan, endingPan, elapsedTime / StepTime);
            transform.position = Vector3.Lerp(oldPosition, newPosition, elapsedTime / StepTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;
    }
}
