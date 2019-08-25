using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class AuthController : MonoBehaviour
{
    public static AuthController instance;
    protected private string authUser = "chatfighter.varcyon@gmail.com";
    protected private string authUserPass = "Naeuu1nur1varDragon20SariouChatFighter2019";


    private void Awake()
    {
        MakeSingleton();
    }

    void Start()
    {
        Login();

    }
   
    void Login()
    {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(authUser, authUserPass).ContinueWith((task =>
         {
         if (task.IsCanceled)
         {
                 Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                 GetErrorMessage((AuthError)e.ErrorCode);
                 return;
             }
         if (task.IsFaulted)
         {
             Firebase.FirebaseException e = task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
             GetErrorMessage((AuthError)e.ErrorCode);
                 return;
         }
         if (task.IsCompleted)
         {
                 print("Login complete");
         }
        }));
    }

    void GetErrorMessage(AuthError errorCode)
    {
        string msg = errorCode.ToString();
        print(msg);

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
