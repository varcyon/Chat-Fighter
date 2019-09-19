using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;


public class ChatCommands : MonoBehaviour
{

    public static ChatCommands command;
    void MakeSingleton()
    {
        if (command != null)
        {
            Destroy(gameObject);
        }
        else
        {
            command = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        MakeSingleton();
    }
    public int points(string userName)
    {
        int coin = 0;
        var player = TwitchChatController.instance.dataBasePlayers.FirstOrDefault(p => p.UserName == userName);
        if (player.UserName == userName)
        {
            coin = player.Coin;
            Debug.Log(coin);
        }
        return coin;
    }

    public void gamble(string userName, int gambleAmount, string channel)
    {
        double winAmount = 0;
        var player = TwitchChatController.instance.dataBasePlayers.FirstOrDefault(p => p.UserName == userName);
        var cPlayer = TwitchChatController.instance.chatPlayers.FirstOrDefault(p => p.UserName == userName);
        if (player.UserName == userName && player.Coin >= gambleAmount)
        {
            player.Coin -= gambleAmount;
            cPlayer.Coin -= gambleAmount;
            System.Random random = new System.Random();
            int num1 = random.Next(0, 100);
            int num2 = random.Next(0, 100);
            int num3 = random.Next(0, 100);
            int num4 = random.Next(0, 100);
            if (num1 >= 95 && num2 >= 95 && num3 >= 95 && num4 >= 95)
            {
                winAmount = gambleAmount * 3;
                TwitchChatController.client.SendMessage(channel, $"{userName}, you hit the jackpot winning {winAmount} coins!");

            }
            else if (num1 >= 50 && num2 >= 50 && num3 >= 50 && num4 >= 50)
            {
                winAmount = gambleAmount * 1 + gambleAmount;
                TwitchChatController.client.SendMessage(channel, $"{userName}, you won {winAmount} coins!");

            }
            else if (num1 >= 50 && num2 >= 50 && num3 >= 50)
            {
                winAmount = Math.Round(gambleAmount * .75 + gambleAmount);
                TwitchChatController.client.SendMessage(channel, $"{userName}, you won {winAmount} coins!");

            }
            else if (num1 >= 50 && num2 >= 50)
            {
                winAmount = Math.Round(gambleAmount * .5 + gambleAmount);
                TwitchChatController.client.SendMessage(channel, $"{userName}, you won {winAmount} coins!");

            }
            else if (num1 >= 50)
            {
                winAmount = Math.Round(gambleAmount * .25 + gambleAmount);
                TwitchChatController.client.SendMessage(channel, $"{userName}, you won {winAmount} coins!");
            }
            else
            {
                TwitchChatController.client.SendMessage(channel, $"Sorry {userName}, you lost!");
            }
        }
        else
        {
            TwitchChatController.client.SendMessage(channel, $"{userName} , you do not have enough coin.");
        }
    }
}
