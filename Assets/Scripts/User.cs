using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class User
{
    public string Id;
    public string DisplayName;
    public string UserName;
    public string ProfileUrl;
    public List<string> Fighters = new List<string>();


    public User(string userName)
    {
        this.UserName = userName;
    }

    public User(string id, string displayName, string login, string profileUrl, string channelName)
    {
        this.Id = id;
        this.DisplayName = displayName;
        this.UserName = login;
        this.ProfileUrl = profileUrl;
        this.Fighters.Add(channelName);
    }
}



