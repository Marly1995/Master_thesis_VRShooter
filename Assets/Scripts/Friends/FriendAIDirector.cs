using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Command
{
    Follow,
    Stay,
    Rush,
    CeaseFire,
    ResumeFire,
    Sneak,
    Snipe,
    Revive
}

public class FriendAIDirector : MonoBehaviour
{
    public FriendStateManager stateManager;
    public FriendlyBehaviour[] team;

    int playerTeleportIndex;

    public Command command;
    
    bool commandGiven = true;

    public float sightRadius = 30f;
    public float snipingSightRadius = 60f;

    public AudioSource audioSource;

    #region voice clips
    public AudioClip blastem;
    public AudioClip ceasefire;
    public AudioClip follow;
    public AudioClip keephidden;
    public AudioClip onme;
    public AudioClip pushforward;
    public AudioClip recoverteammate;
    public AudioClip revive;
    public AudioClip rush;
    public AudioClip sneakforward;
    public AudioClip startshooting;
    public AudioClip staybackandsnipe;
    public AudioClip stopfiring;
    public AudioClip takethemoutfromafar;
    #endregion

    public bool tutorial;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!tutorial)
        {
            if (commandGiven)
            {
                switch (command)
                {
                    case Command.Follow:
                        Follow();
                        break;
                    case Command.Rush:
                        Rush();
                        break;
                    case Command.CeaseFire:
                        CeaseFire();
                        break;
                    case Command.ResumeFire:
                        ResumeFire();
                        break;
                    case Command.Sneak:
                        Sneak();
                        break;
                    case Command.Snipe:
                        Snipe();
                        break;
                    case Command.Revive:
                        Revive();
                        break;
                }
                commandGiven = false;
            }
        }
    }

    public void RecieveCommand(string str)
    {
        switch (str)
        {
            case "follow":
                command = Command.Follow;
                break;
            case "rush":
                command = Command.Rush;
                break;
            case "startFiring":
                command = Command.ResumeFire;
                break;
            case "ceaseFire":
                command = Command.CeaseFire;
                break;
            case "sneak":
                command = Command.Sneak;
                break;
            case "danger":
                command = Command.Snipe;
                break;
            case "revive":
                command = Command.Revive;
                break;
        }
        commandGiven = true;
    }

    public void Follow()
    {
        if (WorldState.TeleportIndex != playerTeleportIndex)
        {
            playerTeleportIndex = WorldState.TeleportIndex;
            stateManager.UpdateCoverPoints(playerTeleportIndex);
        }
        foreach (FriendlyBehaviour instance in team)
        {
            instance.RemoveCurrentCover();
            instance.State = FriendState.TakingCover;
        }
        audioSource.clip = follow;
        audioSource.Play();
    }

    public void Rush()
    {
        if (WorldState.TeleportIndex != playerTeleportIndex)
        {
            playerTeleportIndex = WorldState.TeleportIndex;
        }
        stateManager.UpdateCoverPoints(playerTeleportIndex - 1);
        foreach (FriendlyBehaviour instance in team)
        {
            instance.RemoveCurrentCover();
            instance.State = FriendState.TakingCover;
        }
        audioSource.clip = rush;
        audioSource.Play();
    }

    public void CeaseFire()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = true;
            instance.State = FriendState.Idle;
        }
        audioSource.clip = ceasefire;
        audioSource.Play();
    }

    public void ResumeFire()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = false;
            instance.State = FriendState.Shooting;
        }
        audioSource.clip = blastem;
        audioSource.Play();
    }

    public void Sneak()
    {
        if (WorldState.TeleportIndex != playerTeleportIndex)
        {
            playerTeleportIndex = WorldState.TeleportIndex;
        }
        stateManager.UpdateCoverPoints(playerTeleportIndex - 1);
        foreach (FriendlyBehaviour instance in team)
        {
            instance.RemoveCurrentCover();
            instance.CeaseFire = true;
            instance.State = FriendState.Sneak;
        }
        audioSource.clip = sneakforward;
        audioSource.Play();
    }

    public void Snipe()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = false;
            instance.State = FriendState.Shooting;
        }
        stateManager.sightRadius = snipingSightRadius;
        StartCoroutine(SnipeDuration());
        audioSource.clip = staybackandsnipe;
        audioSource.Play();
    }

    IEnumerator SnipeDuration()
    {
        yield return new WaitForSeconds(10f);
        stateManager.sightRadius = sightRadius;
    }

    public void Revive()
    {
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].State == FriendState.Downed)
            {
                for (int j = 0; j < team.Length; j++)
                {
                    if (j != i)
                    {
                        team[j].Revive(team[i]);
                    }
                }
            }
        }
        audioSource.clip = recoverteammate;
        audioSource.Play();
    }
}