using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]


public class UserInfo
{
    public Datum[] Data { get; set; }
}

public class Datum
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string Display_name { get; set; }
    public string Type { get; set; }
    public string Broadcaster_type { get; set; }
    public string Description { get; set; }
    public string Profile_image_url { get; set; }
    public string Offline_image_url { get; set; }
    public int View_count { get; set; }
}



