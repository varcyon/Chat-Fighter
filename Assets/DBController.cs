using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class DBController : MonoBehaviour
{
    public static DBController instance;

    protected private string DATABASE_URL = "chatfighter-3a784";
    protected private DatabaseReference DBRef;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATABASE_URL);
        DBRef = FirebaseDatabase.DefaultInstance.RootReference;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
