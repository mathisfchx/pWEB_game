using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


public class nteamA : NetworkBehaviour
{

    public int nA;
    public int nB;
    public int maxTeam;

    void Start()
    {
        
    }

    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = ("Team A : "+nA+"/"+maxTeam);
    }
}
