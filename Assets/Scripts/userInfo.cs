using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]


public class UserInfo
{
    public Datum[] Data;

}

public class Datum
{
    public string Id;
    public string Login;
    public string Display_name;
    public string Type;
    public string Broadcaster_type;
    public string Description;
    public string Profile_image_url;
    public string Offline_image_url;
    public int View_count;
}



