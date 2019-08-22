using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class User : List<User>
{
    public string UserName;
    public int Exp = 0;
    public User(string name, int xp)
    {
        this.UserName = name;
        this.Exp = xp;
    }
    public User(string name)
    {
        this.UserName = name;
    }
}

