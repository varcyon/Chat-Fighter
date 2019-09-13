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
using TwitchLib.Api.Models.Undocumented.Chatters;

// getting and using the data from
// the database
//  FirestoreDoc doc = JsonConvert.DeserializeObject<FirestoreDoc>(www.downloadHandler.text);
//  Debug.Log(doc._fieldsProto.id.stringValue);

public class TwitchChatController : MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    FirebaseFunctions functions;
    string platform = "Twitch";
    public string channelName;

    public static TwitchChatController instance;
    //Variables for Twitch IRC

    public Client client;


    //Variables for Twitch chat UI and Players
    public List<string> playersJoined = new List<string>();
    public GameObject arenaSetupUI;
    ArenaSetup arenaSetup;
    public Text chatBox;

    //lists for getting twitch chat users
    public List<User> chatUsers = new List<User>();

    public List<User> newUsers = new List<User>();
    public List<UserInfo> userInfo = new List<UserInfo>();
    public List<Player> dataBasePlayers = new List<Player>();
    public List<Player> newPlayers = new List<Player>();
    bool StreamerExist;
    public string channelData;
    public string channelID;
    string playersFromDB;
    public Api api;
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
        api = new Api();
        api.Settings.AccessToken = Secrets.Access_token;
        api.Settings.ClientId = Secrets.Client_ID;
        client.OnMessageReceived += OnMessageReceived;
        client.OnUserJoined += OnUserJoined;
        client.OnUserLeft += OnUserLeft;
    }
    void Connect()
    {
        Application.runInBackground = true;
        ConnectionCredentials credentials = new ConnectionCredentials(Secrets.botName, Secrets.Access_token);
        client = new Client();
        client.Initialize(credentials, channelName);
        client.Connect();
    }
    private void OnUserLeft(object sender, OnUserLeftArgs e)
    {
    }

    private void OnUserJoined(object sender, OnUserJoinedArgs e)
    {
        StartCoroutine(UserJoined(e.Username));
        Debug.Log("User: " + e.Username + " has Joined");
        //querry streamer player collection
        //if exists , add player to databasePlayers
        //else
        //add player to the streamer/players collection
        //then querry user collection
        //if exist add channel to fighters array
        //else
        //get user info and add them to user collection
    }

    IEnumerator UserJoined(string userName)
    {
        newUsers = new List<User>();
        newPlayers = new List<Player>();
        var func = functions.GetHttpsCallable("UserJoinedQuery");
        var data = new Dictionary<string, object>();
        data["channel"] = channelName;
        data["platform"] = platform;
        data["user"] = userName;
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

                   Debug.Log("Player doc, "+userName+" is empty ,result: " + result["results"]);
                   if (result["results"].ToString() == "data")
                   {
                       Debug.Log("adding user joined , " + userName + " ,to database players");
                       string json = JsonConvert.SerializeObject(result["playerData"]);
                       Debug.Log(json);
                      dataBasePlayers.Add(JsonConvert.DeserializeObject<Player>(json));
                    //    dataBasePlayers.Add(new Player(){
                    //        player.
                    //    });
                   }
                   if (result["results"].ToString() == "1" || result["results"].ToString() == "2")
                   {
                       GetUserJoinedData(userName);
                       newPlayers.Add(new Player()
                       {
                           Id = newUsers[0].Id,
                           DisplayName = newUsers[0].DisplayName,
                           UserName = newUsers[0].UserName,
                           Platform = platform,
                           Channel = channelName
                       });
                       StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));
                       StartCoroutine(WriteNewPlayersToDB(JsonConvert.SerializeObject(newPlayers)));
                       dataBasePlayers.Add(new Player()
                       {
                           Id = newUsers[0].Id,
                           DisplayName = newUsers[0].DisplayName,
                           UserName = newUsers[0].UserName,
                           Platform = platform,
                           Channel = channelName
                       });
                   }

               });
        yield return new WaitUntil(() => task.IsCompleted);


    }
    public void GetUserJoinedData(string userName)
    {
        string userDataRequest = string.Format("https://api.twitch.tv/helix/users?login={0}", userName);
        WebRequest requestObject = WebRequest.Create(userDataRequest);
        requestObject.PreAuthenticate = true;
        requestObject.Headers.Add("Client-ID", Secrets.Client_ID);
        requestObject.Headers.Add("Authorization: Bearer " + Secrets.Access_token);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            UserInfo data = JsonConvert.DeserializeObject<UserInfo>(responseJSON);
            User newUser = new User()
            {
                Id = data.Data[0].Id,
                DisplayName = data.Data[0].Display_name,
                UserName = data.Data[0].Login,
                ProfileUrl = data.Data[0].Profile_image_url
            };
        }
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

    void Update()
    {
    }

    private void GetChatterListCallback(List<ChatterFormatted> listOfChatters)
    {
        foreach (var chatterObject in listOfChatters)
        {
            chatUsers.Add(new User() { UserName = chatterObject.Username });
        }
    }

    public void TryGettingChatters()
    {
        chatUsers = new List<User>();
        newUsers = new List<User>();
        userInfo = new List<UserInfo>();
        api.Invoke(api.Undocumented.GetChattersAsync(client.JoinedChannels[0].Channel), GetChatterListCallback);
        StartCoroutine(CompareNewToCurrentUsers());
    }

    IEnumerator DoesChannelExist()
    {
        yield return GetChannelData();
        var func = functions.GetHttpsCallable("doesChannelExist");
        var data = new Dictionary<string, object>();
        data["platform"] = platform;
        data["channel"] = channelName;
        data["channelData"] = channelData;
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
                   if (result["streamerExists"].ToString() == "streamer")
                   {
                       StreamerExist = true;
                   }
                   else if (result["streamerExists"].ToString() == "noStreamer")
                   {
                       StreamerExist = false;
                   }
               });
        yield return new WaitUntil(() => task.IsCompleted);
    }
    public IEnumerator QueryChannelsDBPlayers()
    {

        var func = functions.GetHttpsCallable("QueryChannelsDBPlayers");
        var data = new Dictionary<string, object>();
        data["platform"] = platform;
        data["channel"] = channelName;
        var task = func.CallAsync(data).ContinueWithOnMainThread((callTask) =>
        {
            if (callTask.IsFaulted)
            {
                // The function unexpectedly failed.
                Debug.Log("FAILED!");
                Debug.Log(String.Format("  Error: {0}", callTask.Exception));
                return;
            }
            var result = (IDictionary)callTask.Result.Data;
            playersFromDB = JsonConvert.SerializeObject(result["playersFromDB"]);
            dataBasePlayers = JsonConvert.DeserializeObject<List<Player>>(playersFromDB);
        });
        yield return new WaitUntil(() => task.IsCompleted);
    }
    IEnumerator CompareNewToCurrentUsers()
    {
        yield return StartCoroutine(DoesChannelExist());
        if (StreamerExist == true)
        {
            yield return StartCoroutine(QueryChannelsDBPlayers());
            newPlayers = JsonConvert.DeserializeObject<List<Player>>(playersFromDB);
            yield return newUsers = chatUsers.Where(x => !dataBasePlayers.Any(y => y.UserName == x.UserName)).ToList();
            List<User> dummy = new List<User>(newUsers);
            newUsers = new List<User>();
            newPlayers = new List<Player>();
            foreach (var user in dummy)
            {
                GetUserData(user.UserName);
            }
            yield return StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));
            yield return StartCoroutine(WriteNewPlayersToDB(JsonConvert.SerializeObject(newPlayers)));
        }
        else if (StreamerExist == false)
        {
            foreach (var user in chatUsers)
            {
                GetUserData(user.UserName);
            }
            yield return StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));
            yield return StartCoroutine(WriteNewPlayersToDB(JsonConvert.SerializeObject(newPlayers)));
            dataBasePlayers = new List<Player>(newPlayers);
        }
        Debug.Log("DONE!");
    }
    IEnumerator WriteNewUsersToDB(string json)
    {
        var func = functions.GetHttpsCallable("AddNewUsers");
        var data = new Dictionary<string, object>();
        data["dataFromUnity"] = json;
        data["channel"] = channelName;
        var task = func.CallAsync(data).ContinueWithOnMainThread((callTask) =>
        {
            if (callTask.IsFaulted)
            {
                // The function unexpectedly failed.
                Debug.Log("FAILED!");
                Debug.Log(String.Format("  Error: {0}", callTask.Exception));
                return;
            }
            var result = (IDictionary)callTask.Result.Data;
        });
        yield return new WaitUntil(() => task.IsCompleted);
    }
    IEnumerator WriteNewPlayersToDB(string json)
    {
        var func = functions.GetHttpsCallable("AddNewPlayers");
        var data = new Dictionary<string, object>();
        data["dataFromUnity"] = json;
        data["platform"] = platform;
        data["channel"] = channelName;
        var task = func.CallAsync(data).ContinueWithOnMainThread((callTask) =>
        {
            if (callTask.IsFaulted)
            {
                Debug.Log("FAILED!");
                Debug.Log(String.Format("  Error: {0}", callTask.Exception));
                return;
            }
            var result = (IDictionary)callTask.Result.Data;
        });
        yield return new WaitUntil(() => task.IsCompleted);
    }
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

    string GetChannelID()
    {
        string userDataRequest = string.Format(string.Format("https://api.twitch.tv/helix/users?login={0}", channelName));
        WebRequest requestObject = WebRequest.Create(userDataRequest);
        requestObject.Headers.Add("Client-ID", Secrets.Client_ID);
        requestObject.Headers.Add("Authorization: Bearer " + Secrets.Access_token);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string CNdata = sr.ReadToEnd();
            UserInfo id = JsonConvert.DeserializeObject<UserInfo>(CNdata);
            channelID = id.Data[0].Id;
            sr.Close();
        }
        return channelID;
    }

    string GetChannelJson()
    {
        string data;
        string channelDataRequest = string.Format("https://api.twitch.tv/kraken/channels/{0}", channelID);
        WebRequest requestObject2 = WebRequest.Create(channelDataRequest);
        requestObject2.PreAuthenticate = true;
        requestObject2.Headers.Add("Client-ID", Secrets.Client_ID);
        requestObject2.Headers.Add("Authorizatio: OAuth " + Secrets.OAuth);
        HttpWebResponse responseObject2 = (HttpWebResponse)requestObject2.GetResponse();
        using (Stream stream = responseObject2.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            data = sr.ReadToEnd();
            sr.Close();
        }
        return data;
    }

    IEnumerator GetChannelData()
    {
        yield return channelID = GetChannelID();
        yield return channelData = GetChannelJson();
    }


    public void GetUserData(string userName)
    {
        string userDataRequest = string.Format("https://api.twitch.tv/helix/users?login={0}", userName);
        WebRequest requestObject = WebRequest.Create(userDataRequest);
        requestObject.PreAuthenticate = true;
        requestObject.Headers.Add("Client-ID", Secrets.Client_ID);
        requestObject.Headers.Add("Authorization: Bearer " + Secrets.Access_token);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            UserInfo data = JsonConvert.DeserializeObject<UserInfo>(responseJSON);
            User newUser = new User()
            {
                Id = data.Data[0].Id,
                DisplayName = data.Data[0].Display_name,
                UserName = data.Data[0].Login,
                ProfileUrl = data.Data[0].Profile_image_url
            };
            newUser.Fighters.Add(channelName);
            newUsers.Add(newUser);

            newPlayers.Add(new Player()
            {
                Id = data.Data[0].Id,
                DisplayName = data.Data[0].Display_name,
                UserName = data.Data[0].Login,
                Platform = platform,
                Channel = channelName
            });

            dataBasePlayers.Add(new Player()
            {
                Id = data.Data[0].Id,
                DisplayName = data.Data[0].Display_name,
                UserName = data.Data[0].Login,
                Platform = platform,
                Channel = channelName
            });
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
    }
}