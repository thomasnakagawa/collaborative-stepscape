﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool GameHasStarted = false;
    private float GameElapsedTime = 0f;

    private FootstepSoundPlayer footstepSoundPlayer;

    private LinkedList<Footstep> Footsteps;

    // Start is called before the first frame update
    void Start()
    {
        footstepSoundPlayer = FindObjectOfType<FootstepSoundPlayer>();
        StartCoroutine(NetworkHandler.GetRequest(Secrets.DB_URL + "/footsteps.json", OnDataFetched));
    }

    private void Update()
    {
        if (GameHasStarted)
        {
            GameElapsedTime += Time.deltaTime;
            while (Footsteps.Count > 0 && GameElapsedTime > Footsteps.First.Value.time)
            {
                Footstep fsToPlay = Footsteps.First.Value;
                footstepSoundPlayer.PlayAtPosition(fsToPlay.posX, fsToPlay.posY, fsToPlay.velo);
                Footsteps.RemoveFirst();
            }
        }
    }

    private void OnDataFetched(string footstepJSON)
    {
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
}
