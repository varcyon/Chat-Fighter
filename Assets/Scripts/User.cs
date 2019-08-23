using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class User 
{
    public string UserName { get; set; }
    public int Exp { get; set; }
    public int Coins { get; set; }
    public int Power { get; set; }
    public int Dodge { get; set; }
    public int HP { get; set; }

 

    public User(string name)
    {
        this.UserName = name;
    }
}

