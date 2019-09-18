using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ArenaSetup : MonoBehaviour
{
    public List<string> playersJoining = new List<string>();
    [HideInInspector]
    public List<GameObject> joinedPlayer = new List<GameObject>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //int playerIndex = 0;
        if(playersJoining.Count == 7)
        {
            playersJoining.RemoveRange(1,1);
            
            TwitchChatController.instance.SendTwitchMessage(String.Format("All positions filled!"),null);
        }
        for (int i = 0; i < 6; i++)
        {
            if (i < playersJoining.Count)
            {
                joinedPlayer[i].GetComponent<Text>().text = playersJoining[i];
            }
            else
            {
                joinedPlayer[i].GetComponent<Text>().text = String.Format("Player {0}",i+1);
            }
        }
        TwitchChatController.instance.playersJoined = playersJoining;
    }
}

