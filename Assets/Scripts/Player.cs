using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public string Id;
    public string DisplayName;
    public string UserName;
    public string Platform;
    public string Channel;
    public int Exp;
    public int Coin;
    public List<int> Items = new List<int>();
    public int Level;
    public int Hp;
    public int Power;
    public int Dodge;
    public int Armor;
    public int Weight;
    public int Speed;

    public Player(string id, string displayName, string login, string platform, string channelName)
    {
        this.Id = id;
        this.DisplayName = displayName;
        this.UserName = login;
        this.Platform  = platform;
        this.Channel = channelName;

    }

}