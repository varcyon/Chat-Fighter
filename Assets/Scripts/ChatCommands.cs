using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCommands
{

    public static ChatCommands instance;

    public int points(string userName)
    {
        int coin = 0;
        foreach (var player in TwitchChatController.instance.dataBasePlayers)
        {
            if (player.UserName == userName)
            {
                coin = player.Coin;
            }
        }
        return coin;
    }

}
