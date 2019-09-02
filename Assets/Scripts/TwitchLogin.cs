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
public class TwitchLogin : MonoBehaviour
{

    string UserName;
    string OAuth;
    public Text statusText;
    public InputField user;
    public InputField pass;
    TcpClient tcpClient;
    StreamReader reader;
    StreamWriter writer;
    public void TwitchOAuthCode()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }

    
    void Login()
    {
        UserName = user.text;
        OAuth = pass.text;
        tcpClient = new TcpClient("irc.twitch.tv", 6667);
        reader = new StreamReader(tcpClient.GetStream());
        writer = new StreamWriter(tcpClient.GetStream())
        {
            AutoFlush = true
        };
       writer.WriteLine(String.Format("PASS {0}\r\nNick {1}\r\nUser {1} 8 * :{1}", OAuth, UserName));
    }

    public void CheckUser()
    {
        statusText.text = "";
        Login();
        new WaitForSeconds(2.0f);
        if (!tcpClient.Client.Connected)
        {
            statusText.text = "Wrong User or OAuth";
        } else if(tcpClient.Connected)
        {
            statusText.text = "logged in!";
        }
    }
}
