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



public class TwitchChatController : MonoBehaviour
{
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
    public List<User> chatUsers = new List<User>();
    public List<User> currentUsers = new List<User>();
    public List<User> newUsers = new List<User>();

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
            //Debug.Log(data);
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
        CompareNewToCurrentUsers();
    }

    void CompareNewToCurrentUsers()
    {
        //if (File.Exists(Application.dataPath + "/UserData.json"))
        // {
        //    //compare current and new list
        //   String jsonFile;

        //    jsonFile = File.ReadAllText(Application.dataPath + "/UserData.json");
        //   dynamic jsonData = JsonConvert.SerializeObject(jsonFile);

        //  for(int i = 0; i < jsonData["users"].Count; i++)
        //  {
        //     currentUsers.Add(new User(jsonData["users"][i]["userName"].ToString()));
        // }
        // newUsers = chatUsers.Where(x => !currentUsers.Any(y => y.UserName == x.UserName)).ToList();
        //  } else
        //{// if file doesn't exsist go ahead and write current users
        StartCoroutine(WriteNewUsersToDB(JsonConvert.SerializeObject(chatUsers).ToString()));
        // }
    }

    IEnumerator WriteNewUsersToDB(string data)
    {

        string url = "https://us-central1-tough-ivy-251300.cloudfunctions.net/AddNewPlayersToStreamer/?data=" + data;
        Debug.Log("start writing......");
        // List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        // wwwForm.Add(new MultipartFormDataSection("data",data));
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }


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

    public void GetUserData(string userName)
    {
        //UserInfo data;
        List<UserInfo> userInfo = new List<UserInfo>();
        string userDataRequest = string.Format("https://api.twitch.tv/helix/users?login={0}", userNameTest);
        WebRequest requestObject = WebRequest.Create(userDataRequest);
        requestObject.Headers.Add("Client-ID", Secrets.Client_ID);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            UserInfo data = JsonConvert.DeserializeObject<UserInfo>(responseJSON);
            Debug.Log(data.Data[0].Id);

        }
        // foreach (var item in data["data"])
        // {
        //     userInfo.Add(new UserInfo(item.id, item.login, item.DisplayName, item.imageUrl));
        // }
        // Debug.Log(userInfo);

        // foreach (var item in data["data"])
        // {
        //     userInfo.Add(new UserInfo());
        //     userInfo.r
        // }
        //GetTexture(responseJSON);
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



}
