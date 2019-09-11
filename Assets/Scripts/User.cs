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

}



