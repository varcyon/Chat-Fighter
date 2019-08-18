using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    readonly float timer = 3;
    NavMeshAgent NavMeshAgent;
    bool inDoThings;
    public GameObject[] players = new GameObject[5];
    public GameObject Target;
    public bool chase;
    public bool flee;
    public Image statusImage;
    public Text statusText;
    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent = this.GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        if (!inDoThings)
            StartCoroutine(DoThings());
    }

   
    IEnumerator DoThings()
    {
        inDoThings = true;
        yield return new WaitForSeconds(timer);
        GetNewPath();
        inDoThings = false;
    }

    void GetNewPath()
    {
        NavMeshAgent.SetDestination(GetNewRandomTarget());
    }

    private Vector3 GetNewRandomTarget()
    {
        statusImage.enabled = true;
        statusText.enabled = true;
        Vector3 pos = new Vector3(0,0,0);
        int mode = Random.Range(0, 50);
        if(mode%2 == 0) {
            chase = true;
            flee = false;

           
            int rp = Random.Range(0,4);
            Target = players[rp];
            if (players[rp] != null )
            {
                float x = players[rp].transform.position.x;
                float z = players[rp].transform.position.z;
                pos = new Vector3(x, 0, z);
                statusText.text = "Chasing " + Target.name;
                return pos;
            } 
        }
        if (mode%2 == 1)
        {
            chase = false;
            flee = true;
           
            float x = Random.Range(-35, 35);
            float z = Random.Range(-35, 35);
            pos = new Vector3(x, 0, z);
            statusText.text = "Fleeing..!";
            return pos;
        }
        else return pos;
    }

}