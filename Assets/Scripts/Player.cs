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
    public int Exp = 0;
    public int Coin = 0;
    public List<int> Items = new List<int>();
    public int Level = 1;
    public int Hp = 100;
    public int Power = 10;
    public int Dodge = 5;
    public int Armor = 0;
    public int Weight = 2;
    public int Speed = 3;

    public Player(string id, string displayName, string login, string platform, string channelName)
    {
        this.Id = id;
        this.DisplayName = displayName;
        this.UserName = login;
        this.Platform = platform;
        this.Channel = channelName;

    }
    public Player(string id, string displayName, string login, string platform, string channelName, int exp, int coin, int level, int hp, int power, int dodge, int armor, int weight, int speed,List<int> items)
    {
        this.Id = id;
        this.DisplayName = displayName;
        this.UserName = login;
        this.Platform = platform;
        this.Channel = channelName;
        this.Exp = exp;
        this.Coin = coin;
        this.Level = level;
        this.Hp = hp;
        this.Power = power;
        this.Dodge = dodge;
        this.Armor = armor;
        this.Weight = weight;
        this.Speed = speed;
        this.Items.AddRange(items);
    }
    public Player(int items)
    {
        this.Items.Add(items);
    }
}

