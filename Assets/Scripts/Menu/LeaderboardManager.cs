using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    Dictionary<string, int> points = new Dictionary<string, int>();
    GameObject[] players;
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] TextMeshProUGUI[] playerPoints;
    [SerializeField] TextMeshProUGUI playerPos;
    [SerializeField] TextMeshProUGUI playerPoint;

    void Update()
    {
        GetPlayerPoint();
    }
    void GetPlayerPoint()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(players.Length);
        points.Clear();
        foreach (var player in players)
        {
            if (player.transform.position.y > -5f)
                if (!points.ContainsKey(player.name))
                    points.Add(player.name, player.GetComponent<IPlayer>().GetPoint());
        }
        var sorted = (from entry in points orderby entry.Value descending select entry).ToList();

        int playerRank = PlayerRank(sorted);

        for (int i = 0; (i < playerNames.Length); i++)
        {
            if (i < points.Count)
            {
                playerNames[i].gameObject.SetActive(true);
                playerPoints[i].gameObject.SetActive(true);
                playerNames[i].text = (i + 1) + ". " + NameNormalize(sorted[i].Key);
                playerPoints[i].text = sorted[i].Value.ToString();
                if (playerRank != i)
                {
                    playerNames[i].color = Color.white;
                }
                else
                {
                    playerNames[i].text = (i + 1) + ". " + NameNormalize(PlayerHelper.Instance.playerName);
                    playerNames[i].color = Color.yellow;
                }
            }
            else
            {
                playerNames[i].gameObject.SetActive(false);
                playerPoints[i].gameObject.SetActive(false);
            }
        }
        if (playerRank > 4)
        {
            playerPos.gameObject.SetActive(true);
            playerPos.text = (playerRank + 1) + ". " + NameNormalize(PlayerHelper.Instance.playerName);
            playerPos.color = Color.yellow;
            playerPoint.gameObject.SetActive(true);
            playerPoint.text = PlayerController.Instance.GetPoint().ToString();
            playerPoint.color = Color.yellow;
        }
        else
        {
            playerPos.gameObject.SetActive(false);
            playerPoint.gameObject.SetActive(false);
        }

    }
    string NameNormalize(string s)
    {
        if (s.Length <= 10) return s;
        string tmp = s.Substring(0, 10);
        return tmp + "...";
    }

    int PlayerRank(List<KeyValuePair<string, int>> list)
    {
        return list.FindIndex(item => item.Key == "Player");
    }
}
