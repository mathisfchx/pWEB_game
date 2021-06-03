using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Game
{
    
    public class Connection_tab : NetworkBehaviour
    {
        [SyncVar]
        public string Username = "" ;
        [SyncVar]
        public int conn_id = 0; 
        public SyncDictionary<string, int> conn_tab = new SyncDictionary<string, int>();
        [SyncVar]
        public int nA;
        [SyncVar]
        public int nB;
        [SyncVar]
        public int start;
        [SyncVar]
        public int maxTeam = 1;

    void Start()
    {
        maxTeam = 1;
        CmdResetStart();
    }

    void Update()
    {
        nA=0;
        nB=0;
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ((player.GetComponent(typeof (Player)) as Player).team == 3){
                nA +=1;
            } else if ((player.GetComponent(typeof (Player)) as Player).team == 1){
                nB +=1;
            }
        }

        if (nA == maxTeam && nB == maxTeam)
        {
            if (hasAuthority){
                start = 1;
            }
            
        }
        
    }

    void LateUpdate()
    {
        if (nB == maxTeam){
            Debug.Log("nb");
            GameObject.Find("TeamB").SetActive(false);
            return;
        } else {
            //GameObject.Find("TeamB").SetActive(true);
        }

        if (nA == maxTeam){
            Debug.Log("na");
            GameObject.Find("TeamA").SetActive(false);
            return;
        } else {
            //GameObject.Find("TeamA").SetActive(true);
        }
    }

    [Command]
    public void CmdSetDico(string username, int conn_id)
    {
        print(hasAuthority);
        //if(hasAuthority)
        //{
        conn_tab[Username] = conn_id;
        //RpcModifDico(username, conn_id);
        //}

    }

    [Command]
    public void CmdResetStart()
    {
        start = 0;
    }

    }
}
