using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public string name;
    public List<Player> players = new List<Player>();
    public static List<Team> teams = new List<Team>();

    public static void AddPlayer(Player player)
    {
        Team containsTeam = null;
        for(int i = 0; i < teams.Count; i++)
        {
            if(teams[i].name == player.teamName)
            {
                containsTeam = teams[i];
                player.team = teams[i];
                break;
            }
        }

        if (containsTeam != null)
        {
            Team team = player.team;
            if(!team.players.Contains(player))
            {
                team.players.Add(player);
            }
        } else
        {
            Team team = new Team();
            player.team = team;
            team.name = player.teamName;
            team.players.Add(player);
            teams.Add(team);
        }
    }

    public static void RemovePlayer(Player player)
    {
        if (player.team == null) return;
        Team team = player.team;
        team.players.Remove(player);
    }
}
