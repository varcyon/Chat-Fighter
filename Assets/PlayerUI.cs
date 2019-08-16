using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image ouch;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ouchPos = Camera.main.WorldToScreenPoint(this.transform.position);
        ouch.transform.position = ouchPos;
    }
}
