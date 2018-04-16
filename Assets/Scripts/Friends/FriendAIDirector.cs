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

    public float sightRadius;
    public float snipingSightRadius;

    private void Update()
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
            case "startFire":
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
    }

    public void CeaseFire()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = true;
        }
    }

    public void ResumeFire()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = false;
            instance.State = FriendState.Shooting;
        }
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
    }
}