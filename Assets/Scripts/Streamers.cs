using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Streamers
{
    public Twitch[] Twitch;
    public Youtube[] Youtube;
    public Mixer[] Mixer;
}

public class Twitch
{
    public string channel;
    public string[] Users;
}

public class Youtube
{
    public string channel;
    public string[] Users;
}

public class Mixer
{
    public string channel;
    public string[] Users;
}


