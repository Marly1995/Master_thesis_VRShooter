﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Tutorial : MonoBehaviour
{
    bool start = false;

    List<int> gestures;

    public VideoClip[] clips;

    public VideoPlayer player;

    public string current;

    private void Start()
    {
        gestures = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            gestures.Add(i);
            gestures.Add(i);
            gestures.Add(i);
        }
    }

    public void ShowNext()
    {
        int index = ChooseNext();
        int next = gestures[index];
        gestures.RemoveAt(index);
        player.clip = clips[next];
        StartCoroutine(DelayedPlay(1.0f));
        current = clips[next].name;
    }

    int ChooseNext()
    {
        return Random.Range(0, gestures.Count);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!start)
        {
            ShowNext();
            start = true;
        }
    }

    IEnumerator DelayedPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.Play();
    }
}
