using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckForVictor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] allPlayers = new GameObject[6];
    public string Winner;
    public int deathCount = 0;
    public Text winText;
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (deathCount == 5)
        {
            
            foreach (GameObject p in allPlayers)
            {
                if (p != null)
                {
                    Winner = p.name;
                    winText.text = "Winner is " + Winner + "!";
                    winText.enabled = true;
                }
            }
        } else if(deathCount == 6)
        {
            winText.text = "DRAW!!";
            winText.enabled = true;
        }

        
    }

    
}
