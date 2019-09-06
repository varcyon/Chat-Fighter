using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using Newtonsoft.Json;
using TwitchLib.Api;
using TwitchLib.Api.Models.Helix.Users;
using TwitchLib.Api.Models.v5;
using TwitchLib.Api.Internal;
using UnityEngine.Networking;
using Firebase.Functions;
using Firebase.Extensions;
using Firebase;

// getting and using the data from
// the database
//  FirestoreDoc doc = JsonConvert.DeserializeObject<FirestoreDoc>(www.downloadHandler.text);
//  Debug.Log(doc._fieldsProto.id.stringValue);

public class TwitchChatController : MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    FirebaseFunctions functions;
    const String platform = "Twitch";
    public string channelName;
    public string userNameTest = "varcy0n";

    public static TwitchChatController instance;
    //Variables for Twitch IRC

    public Client client;


    //Variables for Twitch chat UI and Players
    public List<string> playersJoined = new List<string>();
    public GameObject arenaSetupUI;
    ArenaSetup arenaSetup;
    public Text chatBox;

    //lists for getting twitch chat users
    public List<User> currentUsers = new List<User>();
    public List<User> chatUsers = new List<User>();

    public List<User> newUsers = new List<User>();
    public List<UserInfo> userInfo = new List<UserInfo>();
    public bool StreamerExist;

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
    private void Awake()
    {
        MakeSingleton();
        arenaSetup = arenaSetupUI.GetComponent<ArenaSetup>();
    }
    public void Start()
    {
        Debug.Log("Starting....");
        checkFirebaseDepen();
        Connect();
        client.OnMessageReceived += OnMessageReceived;
        Debug.Log("streamer check complete");
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string speaker = e.ChatMessage.DisplayName;
        string message = e.ChatMessage.Message;
        chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", speaker, message);
        ReceiveMessage(speaker, e);
    }
    public void SendTwitchMessage(string message, OnMessageReceivedArgs e)
    {
        client.SendMessage(client.JoinedChannels[0], message);
    }
    void Connect()
    {
        Application.runInBackground = true;
        ConnectionCredentials credentials = new ConnectionCredentials(Secrets.botName, Secrets.Access_token);
        client = new Client();
        client.Initialize(credentials, channelName);
        client.Connect();
    }
    void Update()
    {
    }

    public void TryGettingChatters()
    {
        chatUsers = new List<User>();
        newUsers = new List<User>();
        userInfo = new List<UserInfo>();

        dynamic data;
        string chattersRequest = string.Format("http://tmi.twitch.tv/group/user/{0}/chatters", channelName);
        WebRequest requestObject = WebRequest.Create(chattersRequest);
        // requestObject.Headers.Add("Authorization", "Basic ashAHasd87asdHasdas");
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            data = JsonConvert.DeserializeObject(responseJSON);
            Debug.Log(data);
        }
        AddChatUsersToList(data);

    }
    public void AddChatUsersToList(dynamic data)
    {
        foreach (var item in data["chatters"]["broadcaster"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["moderators"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["viewers"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["staff"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["admins"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["global_mods"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        foreach (var item in data["chatters"]["vips"])
        {
            chatUsers.Add(new User(item.ToString()));
        }
        Debug.Log(JsonConvert.SerializeObject(chatUsers));
        StartCoroutine(CompareNewToCurrentUsers());
    }

    IEnumerator DoesChannelExist()
    {

        string channelExistUrl = "https://us-central1-tough-ivy-251300.cloudfunctions.net/doesStreamerExist/?platform=" + platform + "&channel=" + channelName;

        // List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        // wwwForm.Add(new MultipartFormDataSection("data",data));
        Debug.Log("start query......");
        UnityWebRequest www = UnityWebRequest.Get(channelExistUrl);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            if (www.downloadHandler.text == "1")
            {
                StreamerExist = true;
            }
            else if (www.downloadHandler.text == "0")
            {
                StreamerExist = false;
            }
        }
    }
    IEnumerator CompareNewToCurrentUsers()
    {
        //DOES STREAMER EXIST
        Debug.Log("Does streamer exist? ");
        yield return StartCoroutine(DoesChannelExist());
        if (StreamerExist == true)
        {
            //if(){
            //  for(int i = 0; i < jsonData["users"].Count; i++)
            //  {
            //     currentUsers.Add(new User(jsonData["users"][i]["userName"].ToString()));
            // }
            // newUsers = chatUsers.Where(x => !currentUsers.Any(y => y.UserName == x.UserName)).ToList();
            //} else {
        }
        else if (StreamerExist == false)
        {
            foreach (var user in chatUsers)
            {   
                int i = 0;    
                GetUserData(user.UserName,i);
                i++;
            }

            StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));


            // if file doesn't exsist go ahead and write current users
            //StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(chatUsers)));
            // }
        }

    }
    IEnumerator WriteNewUsersToDB(string json)
    {
        var func = functions.GetHttpsCallable("AddNewPlayersToStreamer");
        var data = new Dictionary<string, object>();
        data["dataFromUnity"] = json;

        var task = func.CallAsync(data).ContinueWithOnMainThread((callTask) =>
        {
            if (callTask.IsFaulted)
            {
                // The function unexpectedly failed.
                Debug.Log("FAILED!");
                Debug.Log(String.Format("  Error: {0}", callTask.Exception));
                return;
            }

            // The function succeeded.
            var result = (IDictionary)callTask.Result.Data;
            Debug.Log(String.Format("Results {0}", result["fuctionran"]));
        });
        yield return new WaitUntil(() => task.IsCompleted);
    }
    // IEnumerator WriteNewUsersToDB(String data)
    // {

    //     string url = "https://us-central1-tough-ivy-251300.cloudfunctions.net/AddNewPlayersToStreamer/?data=";
    //     Debug.Log("start writing......");
    //     // List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
    //     // wwwForm.Add(new MultipartFormDataSection("data",data));
    //     UnityWebRequest www = UnityWebRequest.Get(url);
    //     yield return www.SendWebRequest();
    //     if (www.isNetworkError || www.isHttpError)
    //     {
    //         Debug.LogError(www.error);
    //     }
    //     else
    //     {
    //         Debug.Log(www.downloadHandler.text);
    //     }
    //     Debug.Log("done writing");

    // }

    void ReceiveMessage(String speaker, OnMessageReceivedArgs e)
    {
        //Twitch Command
        if (e.ChatMessage.Message.StartsWith("!hi"))
        {
            SendTwitchMessage(String.Format("Hello, {0}", speaker), e);
        }

        if (e.ChatMessage.Message.StartsWith("!join"))
        {
            arenaSetup.playersJoining.Add(speaker);
            SendTwitchMessage(String.Format("Adding, {0} to the game!", speaker), e);
        }
        if (e.ChatMessage.Message.StartsWith("!drop"))
        {
            arenaSetup.playersJoining.Remove(speaker);
            SendTwitchMessage(String.Format("Removing, {0} from the game!", speaker), e);
        }


    }

    public void GetUserData(string userName,int i)
    {
        string userDataRequest = string.Format("https://api.twitch.tv/helix/users?login={0}", userName);
        WebRequest requestObject = WebRequest.Create(userDataRequest);
        requestObject.Headers.Add("Client-ID", Secrets.Client_ID);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            UserInfo data = JsonConvert.DeserializeObject<UserInfo>(responseJSON);
            //Debug.Log(data.Data[i].Id);
            newUsers.Add(new User(data.Data[i].Id,data.Data[i].Display_name,data.Data[i].Login,data.Data[i].Profile_image_url,channelName));
        }
    }

    IEnumerator GetTexture(string playerTexture)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(playerTexture);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }


    void checkFirebaseDepen()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    protected virtual void InitializeFirebase()
    {
        functions = FirebaseFunctions.DefaultInstance;

        // To use a local emulator, uncomment this line:
        //   functions.UseFunctionsEmulator("http://localhost:5005");
        // Or from an Android emulator:
        //   functions.UseFunctionsEmulator("http://10.0.2.2:5005");
    }


}
