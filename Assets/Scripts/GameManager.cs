using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using OatsUtil;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioClip LoadingSound;
    [SerializeField] private AudioClip IntroSound;

    [SerializeField] private GameObject FootstepPlayerPrefab;

    private AudioSource audioSource;

    private bool GameHasStarted = false;
    private float GameElapsedTime = 0f;

    //private FootstepSoundPlayer footstepSoundPlayer;

    private LinkedList<Footstep> Footsteps;

    public bool skipNetworkSteps = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.RequireComponent<AudioSource>();

        //footstepSoundPlayer = SceneUtils.FindComponentInScene<FootstepSoundPlayer>();
        if (!skipNetworkSteps)
        {
            StartCoroutine(OnStartSequence());
        }
    }

    private IEnumerator OnStartSequence()
    {
        audioSource.loop = true;
        audioSource.clip = LoadingSound;
        audioSource.Play();
        yield return null;
        yield return NetworkHandler.GetRequest(Secrets.DB_URL + "/footsteps.json", OnDataFetched);
    }

    private void OnDataFetched(string footstepJSON)
    {
        audioSource.loop = false;
        audioSource.Stop();
        List<Footstep> fetchedSteps = ParseFootstepData(footstepJSON);
        Debug.Log("Fetched " + fetchedSteps.Count + " steps");


        fetchedSteps = fetchedSteps.OrderBy(footstep => footstep.time).ToList();

        Footsteps = new LinkedList<Footstep>();
        for (int i = 0; i < fetchedSteps.Count; i++)
        {
            Footsteps.AddLast(fetchedSteps[i]);
        }

        GameHasStarted = true;
    }

    private List<Footstep> ParseFootstepData(string footstepJSON)
    {
        // todo: data validation and error handling
        List<Footstep> FetchedFootsteps = new List<Footstep>();
        JSONObject resultObj = JSONNode.Parse(footstepJSON).AsObject;
        foreach (string key in resultObj.Keys)
        {
            var keyObj = resultObj[key];

            Footstep newFootStep = new Footstep(
                keyObj["posX"].AsFloat,
                keyObj["posY"].AsFloat,
                keyObj["time"].AsFloat,
                keyObj["velo"].AsFloat
            );

            FetchedFootsteps.Add(newFootStep);
        }
        return FetchedFootsteps;
    }

    private void Update()
    {
        if (GameHasStarted)
        {
            GameElapsedTime += Time.deltaTime;
            while (Footsteps.Count > 0 && GameElapsedTime > Footsteps.First.Value.time)
            {
                Footstep fsToPlay = Footsteps.First.Value;
                var footstepSoundPlayer = ObjectTub.ObjectPool.TakeObjectFromTub(FootstepPlayerPrefab).transform.RequireComponent<FootstepSoundPlayer>();
                footstepSoundPlayer.PlayAtPosition(fsToPlay.posX, fsToPlay.posY, fsToPlay.velo);
                Footsteps.RemoveFirst();
            }
        }
    }
}
