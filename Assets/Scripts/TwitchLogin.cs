using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TwitchLogin : MonoBehaviour
{
    // Start is called before the first frame update
    public void TwitchLoginFunc()
    {

        Application.OpenURL("https://twitchapps.com/tmi/");
    }

 
}
