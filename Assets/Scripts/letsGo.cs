using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class letsGo : MonoBehaviour
{
    public GameObject playersList;
    public List<string> players = new List<string>();
    public string  Text = "";
    public void StartArena()
    {
        players = playersList.GetComponent<ArenaSetup>().playersJoining;
        CreatePlayerList();
        //SceneManager.LoadScene("Arena");
    }

    void CreatePlayerList()
    {
        for (int i = 0; i < 6; i++)
        {
            Text = Text  + players[i] + "\n";
        }
        string path = Application.dataPath + "/players.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, Text);
        }
    }
}

