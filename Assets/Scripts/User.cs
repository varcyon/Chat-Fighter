using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : List<User>
{
    public string userName { set; get; }
    public  int exp  { set; get; }

public User(string name, int xp)
    {
        this.userName = name;
        this.exp = xp;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
