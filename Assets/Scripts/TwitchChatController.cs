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



public class TwitchChatController : MonoBehaviour
{
    const String platform = "Twitch";
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
    public  void Start()
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
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
            data = JsonConvert.DeserializeObject(responseJSON);
            Debug.Log(data);
        }
        //AddChatUsersToList(data);

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
            WriteNewUsersToDB(JsonConvert.SerializeObject(chatUsers));
       // }
    }

    void WriteNewUsersToDB(dynamic data)
    {
        string chattersRequest = string.Format("https://us-central1-tough-ivy-251300.cloudfunctions.net/AddNewPlayersToStreamer/?data=", data);
        WebRequest requestObject = WebRequest.Create(chattersRequest);
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            string responseJSON = sr.ReadToEnd();
            sr.Close();
        }

        // //Writes users to Json file
        // StringBuilder sb = new StringBuilder();
        // JsonWriter writer = new JsonWriter(sb);
        // writer.WriteObjectStart();
        // writer.WritePropertyName("users");
        // writer.WriteArrayStart();
        // foreach (var user in chatUsers)
        // {
        //     writer.WriteObjectStart();
        //     writer.WritePropertyName("userName");
        //     writer.Write(user.UserName);
        //     writer.WritePropertyName("Exp");
        //     writer.Write(0);
        //     writer.WriteObjectEnd();
        // }
        // writer.WriteArrayEnd();
        // writer.WriteObjectEnd();
        // //Writing file
        // string path = Application.dataPath + "/UserData.json";
        // File.WriteAllText(path, sb.ToString());
        // Debug.Log("file written");
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

    public void  GetUserData(){

       TwitchAPI api = new TwitchAPI();
        api.Settings.ClientId = Secrets.Client_ID;
        api.Settings.AccessToken = Secrets.Access_token;
        dynamic results =  api.Users.v5.GetUserAsync("varcy0n");
        
    }

}
