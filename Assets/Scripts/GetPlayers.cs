using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class GetPlayers : MonoBehaviour
{

    public List<GameObject> PlayerUITExt = new List<GameObject>();
    public List<string> playersJoining;

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < playersJoining.Count)
            {
                PlayerUITExt[i].GetComponent<Text>().text = playersJoining[i];
            }
            else
            {
               PlayerUITExt[i].GetComponent<Text>().text = String.Format("PLAYER {0}", i + 1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
