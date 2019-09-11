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
}

