using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class DBController : MonoBehaviour
{
    public static DBController instance;
    void Awake()
    {
        MakeSingleton();
    }
    void Start()
    {
        //SceneManager.LoadScene("ArenaSetup");
    }






    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}