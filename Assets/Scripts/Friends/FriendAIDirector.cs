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
        }
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
        stateManager.UpdateCoverPoints(playerTeleportIndex + 1);
        foreach (FriendlyBehaviour instance in team)
        {
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
        }
    }

    public void Sneak()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = true;
            stateManager.UpdateCoverPoints(playerTeleportIndex + 1);
            instance.State = FriendState.Sneak;
        }
    }

    public void Snipe()
    {
        foreach (FriendlyBehaviour instance in team)
        {
            instance.CeaseFire = false;
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