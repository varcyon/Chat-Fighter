using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArenaSetup : MonoBehaviour
{
    public TwitchChatController twitchChatController;
    public List<string> playersJoining = new List<string>();
    public List<GameObject> joinedPlayer = new List<GameObject>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int playerIndex = 0;
        if(playersJoining.Count == 7)
        {
            playersJoining.RemoveRange(1,1);
            twitchChatController.SendTwitchMessage(String.Format("All positions filled!"));
        }
        foreach(string p in playersJoining)
        {
            joinedPlayer[playerIndex].GetComponent<Text>().text = playersJoining[playerIndex];
            playerIndex++;
        }
        if (playersJoining.Count == 5)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
        }
        if (playersJoining.Count == 4)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
            joinedPlayer[4].GetComponent<Text>().text = "Player 5";
        }
        if (playersJoining.Count == 3)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
            joinedPlayer[4].GetComponent<Text>().text = "Player 5";
            joinedPlayer[3].GetComponent<Text>().text = "Player 4";
        }
        if (playersJoining.Count == 2)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
            joinedPlayer[4].GetComponent<Text>().text = "Player 5";
            joinedPlayer[3].GetComponent<Text>().text = "Player 4";
            joinedPlayer[2].GetComponent<Text>().text = "Player 3";
        }
        if (playersJoining.Count == 1)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
            joinedPlayer[4].GetComponent<Text>().text = "Player 5";
            joinedPlayer[3].GetComponent<Text>().text = "Player 4";
            joinedPlayer[2].GetComponent<Text>().text = "Player 3";
            joinedPlayer[1].GetComponent<Text>().text = "Player 2";
        }
        if (playersJoining.Count == 0)
        {
            joinedPlayer[5].GetComponent<Text>().text = "Player 6";
            joinedPlayer[4].GetComponent<Text>().text = "Player 5";
            joinedPlayer[3].GetComponent<Text>().text = "Player 4";
            joinedPlayer[2].GetComponent<Text>().text = "Player 3";
            joinedPlayer[1].GetComponent<Text>().text = "Player 2";
            joinedPlayer[0].GetComponent<Text>().text = "Player 1";
        }
    }

}
