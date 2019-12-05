using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using OatsUtil;

public class GameManager : MonoBehaviour
{
    [Header("Speech")]
    [SerializeField] private AudioClip LoadingSound;
    [SerializeField] private AudioClip IntroSound;
    [SerializeField] private AudioClip AboutSound;

    [Header("Text content")]
    [SerializeField] private TMPro.TMP_Text TextElement;
    [TextArea] [SerializeField] private string LoadingText;
    [TextArea] [SerializeField] private string IntroText;
    [TextArea] [SerializeField] private string AboutText;

    [SerializeField] private GameObject FootstepPlayerPrefab;

    public enum GameStates
    {
        LOADING, INTRO, GAMEPLAY
    }
    public GameStates GameState { get; private set; }

    private AudioSource audioSource;

    public float GameElapsedTime { get; private set; } = 0f;

    private LinkedList<Footstep> Footsteps;

    private int fetchedTime = 0;
    private int fetchRange = 8;

    void Start()
    {
        audioSource = this.RequireComponent<AudioSource>();
        Footsteps = new LinkedList<Footstep>();

        GameState = GameStates.LOADING;
        if (TextElement == null)
        {
            throw new MissingReferenceException("Text element required");
        }
        StartCoroutine(OnStartSequence());
    }

    private IEnumerator OnStartSequence()
    {
        ShowLoading();
        yield return new WaitForSeconds(0.2f);
        yield return FetchAtInterval();
    }

    private IEnumerator FetchAtInterval()
    {
        yield return FetchNextFootsteps();
        yield return new WaitForSeconds(fetchRange / 2);
        while (true)
        {
            yield return FetchNextFootsteps();
            yield return new WaitForSeconds(fetchRange);
        }
    }

    private IEnumerator FetchNextFootsteps()
    {
        string query = string.Format("orderBy=\"time\"&startAt={0}&endAt={1}", fetchedTime, fetchedTime + fetchRange);
        yield return NetworkHandler.GetRequest(Secrets.DB_URL + "/footsteps.json?" + query, OnDataFetched);
        fetchedTime += fetchRange;
    }

    private void OnDataFetched(string footstepJSON)
    {
        if (GameState == GameStates.LOADING)
        {
            ShowIntro();
        }

        // parse the data
        List<Footstep> fetchedSteps = ParseFootstepData(footstepJSON);
        Debug.Log("Fetched " + fetchedSteps.Count + " steps");

        fetchedSteps = fetchedSteps.OrderBy(footstep => footstep.time).ToList();

        for (int i = 0; i < fetchedSteps.Count; i++)
        {
            Footsteps.AddLast(fetchedSteps[i]);
        }
    }

    private List<Footstep> ParseFootstepData(string footstepJSON)
    {
        // todo: data validation and error handling
        // todo: run in background? this could be long
        List<Footstep> FetchedFootsteps = new List<Footstep>();
        JSONObject resultObj = JSONNode.Parse(footstepJSON).AsObject;
        foreach (string key in resultObj.Keys)
        {
            var keyObj = resultObj[key];

            Footstep newFootStep = new Footstep(
                keyObj["posX"],
                keyObj["posY"],
                keyObj["time"],
                keyObj["velo"]
            );

            FetchedFootsteps.Add(newFootStep);
        }
        return FetchedFootsteps;
    }

    private void ShowLoading()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = LoadingSound;
        audioSource.Play();
        TextElement.text = LoadingText;
    }

    private void ShowIntro()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.PlayOneShot(IntroSound);
        TextElement.text = IntroText;
        GameState = GameStates.INTRO;
    }

    private void ShowAbout()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.PlayOneShot(AboutSound);
        TextElement.text = AboutText;
    }

    private void Update()
    {
        if (GameState == GameStates.INTRO)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // start game
                // stop intro sounds
                audioSource.Stop();
                TextElement.text = "";
                GameState = GameStates.GAMEPLAY;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ShowIntro();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                ShowAbout();
            }
        }

        if (GameState == GameStates.GAMEPLAY)
        {
            GameElapsedTime += Time.deltaTime;
            while (Footsteps.Count > 0 && GameElapsedTime > float.Parse(Footsteps.First.Value.time))
            {
                Footstep fsToPlay = Footsteps.First.Value;
                var footstepSoundPlayer = ObjectTub.ObjectPool.TakeObjectFromTub(FootstepPlayerPrefab).transform.RequireComponent<FootstepSoundPlayer>();
                footstepSoundPlayer.PlayAtPosition(float.Parse(fsToPlay.posX), float.Parse(fsToPlay.posY), float.Parse(fsToPlay.velo));
                Footsteps.RemoveFirst();
            }
        }
    }
}
