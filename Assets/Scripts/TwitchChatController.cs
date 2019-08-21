using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using LitJson;

public class TwitchChatController : MonoBehaviour
{
    public static TwitchChatController instance;
    //Variables for Twitch IRC

    TcpClient tcpClient;
    StreamReader reader;
    StreamWriter writer;
    private readonly string userName = "chatfighter";
    private readonly string password = "oauth:50s1tpqugy3wowbrejq2l2ab8hafb0";
    public string channelName;
    string prefixForSendingChatMessages;
    DateTime lastMessageSendTime;

    //Variables for Twitch chat UI and Players
    public List<string> playersJoined = new List<string>();
    public GameObject arenaSetupUI;
    ArenaSetup arenaSetup;
    public Text chatBox;
    public List<User> chatters = new List<User>();

    public string listOfChatters = "Tom, George, Monkey, Ginger, Alice,";



    Queue<string> sendMessageQueue;
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

        chatters.Add(new User("Tom", 500));
        Debug.Log("user Tom add");
        Debug.Log(chatters[0].userName + "," + chatters[0].exp);

        sendMessageQueue = new Queue<string>();
        prefixForSendingChatMessages = String.Format(":{0}{0}@{0}.tmi.twitch.tv PRIVMSG #{1} :", userName, channelName);
        Connect();
    }

    public void SendTwitchMessage(string message)
    {
        sendMessageQueue.Enqueue(message);
    }


    void Connect()
    {
        tcpClient = new TcpClient("irc.twitch.tv", 6667);
        reader = new StreamReader(tcpClient.GetStream());
        writer = new StreamWriter(tcpClient.GetStream())
        {
            AutoFlush = true
        };

        writer.WriteLine(String.Format("PASS {0}\r\nNick {1}\r\nUser {1} 8 * :{1}", password, userName));
        writer.WriteLine("JOIN #" + channelName);
        lastMessageSendTime = DateTime.Now;
    }

    void Update()
    {
        if (!tcpClient.Connected)
        {
            Connect();

        }
        TryReceivingMessages();
        TrySendingMessages();
        TryGettingChatters();

    }

    public void TryGettingChatters()
    {
        string jsonString;
        JsonData jsonData;
        string chattersRequest = string.Format("http://tmi.twitch.tv/group/user/{0}/chatters", channelName);
        WebRequest requestObject = WebRequest.Create(chattersRequest);

        // requestObject.Credentials = new NetworkCredential("username", "password"); /// sending user and password if needed

        requestObject.Method = "GET";
        HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
        string responseJSON;
        using (Stream stream = responseObject.GetResponseStream())
        {
            StreamReader sr = new StreamReader(stream);
            responseJSON = sr.ReadToEnd();
            sr.Close();

            //string path = Application.dataPath + "/chatters.json";
            // File.WriteAllText(path, responseJSON);

            jsonString = File.ReadAllText(Application.dataPath + "/chatters.json");
            jsonData = JsonMapper.ToObject(jsonString);

            Debug.Log(jsonData["chatters"]["viewers"][0]);

            /* 
             foreach(var item in jsonData["chatters"]["viewers"]){
             
            }
             */
            
        }

    }

    void TryReceivingMessages()
    {
        if (tcpClient.Available > 0)
        {
            //Reads Twitch message
            var message = reader.ReadLine();
            // print(String.Format("\r\nNew Message: {0}", message));

            //Get speaker and message
            var iCollon = message.IndexOf(":", 1);
            if (iCollon > 0)
            {
                var command = message.Substring(1, iCollon);

                if (command.Contains("PRIVMSG #"))
                {
                    var iBang = command.IndexOf("!");
                    if (iBang > 0)
                    {
                        var speaker = command.Substring(0, iBang);
                        var chatMessage = message.Substring(iCollon + 1);

                        ReceiveMessage(speaker, chatMessage);
                    }
                }


            }

        }

    }
    void ReceiveMessage(String speaker, string message)
    {
        //print(String.Format("\r\n{0}: {1}", speaker, message));
        chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", speaker, message);

        //Twitch Command
        if (message.StartsWith("!hi"))
        {
            SendTwitchMessage(String.Format("Hello, {0}", speaker));
        }

        if (message.StartsWith("!join"))
        {
            arenaSetup.playersJoining.Add(speaker);
            SendTwitchMessage(String.Format("Adding, {0} to the game!", speaker));
        }
        if (message.StartsWith("!drop"))
        {
            arenaSetup.playersJoining.Remove(speaker);
            SendTwitchMessage(String.Format("Removing, {0} from the game!", speaker));
        }


    }
    void TrySendingMessages()
    {
        if (DateTime.Now - lastMessageSendTime > TimeSpan.FromSeconds(2))
        {
            if (sendMessageQueue.Count > 0)
            {
                var message = sendMessageQueue.Dequeue();
                writer.WriteLine(String.Format("{0}{1}", prefixForSendingChatMessages, message));
                lastMessageSendTime = DateTime.Now;
            }
        }
    }

    [System.Serializable]
    public class ChatRoom
    {
        public int chatter_count;
        public Chatters chatters;
        public class Chatters
        {
            public string[] broadcaster;
            public object[] vips;
            public string[] moderators;
            public object[] staff;
            public object[] admins;
            public object[] global_mods;
            public string[] viewers;
        }
    }

    

    

}
