using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.IO;

public class TwitchChatController : MonoBehaviour
{
    TcpClient tcpClient;
    StreamReader reader;
    StreamWriter writer;
    public GameObject arenaSetupUI;
    public ArenaSetup arenaSetup;
    public Text chatBox;
    private readonly string userName = "chatfighter";
    private readonly string password = "oauth:50s1tpqugy3wowbrejq2l2ab8hafb0";
    public string channelName;
    string prefixForSendingChatMessages;
    DateTime lastMessageSendTime;

    Queue<string> sendMessageQueue;
    private void Awake()
    {
        arenaSetup = arenaSetupUI.GetComponent<ArenaSetup>();
    }
    public void Start()
    {
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
        writer = new StreamWriter(tcpClient.GetStream());
        writer.AutoFlush = true;

        writer.WriteLine(String.Format("PASS {0}\r\nNick {1}\r\nUser {1} 8 * :{1}", password, userName));
        //writer.WriteLine("CAP REQ :twitch.tv/membership");
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
        
    }

    void TryReceivingMessages()
    {
        if(tcpClient.Available > 0)
        {
            var message = reader.ReadLine();
            print(String.Format("\r\nNew Message: {0}", message));

            var iCollon = message.IndexOf(":", 1);
            if(iCollon > 0)
            {
                var command = message.Substring(1, iCollon);
                if (command.Contains("PRIVMSG #"))
                {
                    var iBang = command.IndexOf("!");
                    if(iBang > 0)
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
            print(String.Format("\r\n{0}: {1}", speaker, message));
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
        if(DateTime.Now - lastMessageSendTime > TimeSpan.FromSeconds(2))
        {
            if(sendMessageQueue.Count > 0)
            {
                var message = sendMessageQueue.Dequeue();
                writer.WriteLine(String.Format("{0}{1}", prefixForSendingChatMessages, message));
                lastMessageSendTime = DateTime.Now;
            }
        }
    }
    
   
   
}
