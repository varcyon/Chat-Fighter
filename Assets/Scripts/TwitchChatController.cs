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
    public List<Player> chatPlayers = new List<Player>();
    List<UserInfo> userInfo = new List<UserInfo>();
    public List<Player> dataBasePlayers = new List<Player>();
    public List<Player> newPlayers = new List<Player>();
    public Dictionary<string, Player> playerDBDictionary ;
    bool StreamerExist;
    public string channelData;
    public string channelID;
    string playersFromDB;
    public Api api;
    public int coinsPerUpdate = 5;
    public bool QDB;
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
        client.OnUserJoined += OnUserJoined;
        client.OnUserLeft += OnUserLeft;
        client.OnChatCommandReceived += OnchatCommandReceived;
        StartStuff();

    }
    void StartStuff()
    {
        StartCoroutine(GetChatters());
        StartCoroutine(PlayerUpdatePerMinute());
        InvokeRepeating("QBDtoTrue", 0f, 300f);
    }
    private void OnchatCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        switch (e.Command.CommandText)
        {
            case "dict":
            break;
            case "hi":
                client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
                break;
            case "Join":
                arenaSetup.playersJoining.Add(e.Command.ChatMessage.Username);
                client.SendMessage(e.Command.ChatMessage.Channel, $"Adding, {e.Command.ChatMessage.DisplayName} to the game!");
                break;
                case "Drop":
                arenaSetup.playersJoining.Remove(e.Command.ChatMessage.Username);
                client.SendMessage(e.Command.ChatMessage.Channel, $"Removing, {e.Command.ChatMessage.DisplayName} from the game!");
                break;
            default:
                client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
                Debug.Log(e.Command.CommandIdentifier);
                Debug.Log(e.Command.CommandText);
                Debug.Log(e.Command.ChatMessage.Message);
                Debug.Log(e.Command.ChatMessage.Username);
                Debug.Log(e.Command.ArgumentsAsList.Count);
                break;
        }
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
        //remove player from chatPlayer.
        Debug.Log("User: " + e.Username + " has left");
    }
    private void OnUserJoined(object sender, OnUserJoinedArgs e)
    {
        // StartCoroutine(UserJoined(e.Username));
        Debug.Log("User: " + e.Username + " has Joined");
    }
    public void SendTwitchMessage(string message, OnMessageReceivedArgs e)
    {
        client.SendMessage(client.JoinedChannels[0], message);
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
    IEnumerator PlayerUpdatePerMinute()
    {
        yield return new WaitForSeconds(45f);
        while (true)
        {
            Debug.Log("StartingCoinsUpdate");
            foreach (var player in chatPlayers)
            {
                player.Coin += coinsPerUpdate;
            }
            Debug.Log("updated chatPlayers");
            Debug.Log("unioning chatPlayers to databasePlayers");
            dataBasePlayers.Union(chatPlayers).ToList();
            yield return new WaitForSeconds(60f);
        }
    }
    void QBDtoTrue()
    {
        Debug.Log("setting QDB to true through invoke");
        QDB = true;
    }
    private void GetChatterListCallback(List<ChatterFormatted> listOfChatters)
    {
        foreach (var chatterObject in listOfChatters)
        {
            chatUsers.Add(new User() { UserName = chatterObject.Username });
        }
    }
    public IEnumerator GetChatters()
    {
        yield return new WaitForSeconds(15f);
        while (true)
        {
            chatUsers = new List<User>();
            newUsers = new List<User>();
            userInfo = new List<UserInfo>();
            newPlayers = new List<Player>();
            chatPlayers = new List<Player>();
            api.Invoke(api.Undocumented.GetChattersAsync(client.JoinedChannels[0].Channel), GetChatterListCallback);
            StartCoroutine(CompareNewToCurrentUsers());
            yield return new WaitForSeconds(60f);
        }
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
                Debug.Log("FAILED!");
                Debug.Log(String.Format("  Error: {0}", callTask.Exception));
                return;
            }
            var result = (IDictionary)callTask.Result.Data;
            playersFromDB = JsonConvert.SerializeObject(result["playersFromDB"]);
            dataBasePlayers = JsonConvert.DeserializeObject<List<Player>>(playersFromDB);
            playerDBDictionary = dataBasePlayers.ToDictionary(p => p.UserName, p => p);
            playerDBDictionary["varcy0n"].Coin +=20;
            Debug.Log(JsonConvert.SerializeObject(playerDBDictionary));
        });
        yield return new WaitUntil(() => task.IsCompleted);
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
    IEnumerator CompareNewToCurrentUsers()
    {
        Debug.Log("compare");
        yield return StartCoroutine(DoesChannelExist());
        if (StreamerExist == true)
        {
            if (QDB)
            {
                yield return StartCoroutine(QueryChannelsDBPlayers());
                QDB = false;
                Debug.Log("set QBD to false");
            }

            // newPlayers = JsonConvert.DeserializeObject<List<Player>>(playersFromDB);
            chatPlayers = dataBasePlayers.Where(x => chatUsers.Any(y => y.UserName == x.UserName)).ToList();
            newUsers = chatUsers.Where(x => !dataBasePlayers.Any(y => y.UserName == x.UserName)).ToList();

            List<User> dummy = new List<User>(newUsers);
            newUsers = new List<User>();
            // newPlayers = new List<Player>();
            foreach (var user in dummy)
            {
                GetUserData(user.UserName);
            }
            yield return StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));
            yield return StartCoroutine(WriteNewPlayersToDB(JsonConvert.SerializeObject(newPlayers)));
            yield return StartCoroutine(UpdatePlayersToDB(JsonConvert.SerializeObject(chatPlayers)));

        }
        else if (StreamerExist == false)
        {
            foreach (var user in chatUsers)
            {
                GetUserData(user.UserName);
            }
            yield return StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(newUsers)));
            yield return StartCoroutine(WriteNewPlayersToDB(JsonConvert.SerializeObject(newPlayers)));
            chatPlayers = new List<Player>(newPlayers);
        }
        Debug.Log("DONE!");
    }
    IEnumerator UpdatePlayersToDB(string json)
    {
        var func = functions.GetHttpsCallable("UpdatePlayers");
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
    string GetChannelJson()
    {
        string data;
        string channelDataRequest = string.Format("https://api.twitch.tv/helix/streams?user_login={0}", channelName);
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

            chatPlayers.Add(new Player()
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