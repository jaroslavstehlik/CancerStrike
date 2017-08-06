using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamCounter : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform container;
    Text[] teamLabels;

    private void Start()
    {
        teamLabels = new Text[Team.teams.Count];
        for (int i = 0; i < Team.teams.Count; i++)
        {
            GameObject go = Instantiate<GameObject>(prefab);
            go.transform.SetParent(container);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            teamLabels[i] = go.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    for(int i = 0; i < Team.teams.Count; i++)
        {
            Team team = Team.teams[i];
            PlayerStats stats = new PlayerStats();
            for (int j = 0; j < team.players.Count; j++)
            {
                stats.shotsFired += team.players[j].stats.shotsFired;
                stats.deaths += team.players[j].stats.deaths;
                stats.kills += team.players[j].stats.kills;
                stats.hits += team.players[j].stats.hits;
            }

            teamLabels[i].text = team.name + ", Kills: "+stats.kills+", Deaths: "+stats.deaths;
        }
	}
}
