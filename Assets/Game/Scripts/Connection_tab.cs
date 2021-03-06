﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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
            public int maxTeam;

        void Start()
        {
            start = 5;
            maxTeam = 4;
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
                start = 1;
            }
            
        }

        void LateUpdate()
        {
            try {
                if (nB == maxTeam){
                    if (GameObject.Find("TeamB").active){
                        GameObject.Find("TeamB").SetActive(false);
                    }
                }

                if (nA == maxTeam){
                    if (GameObject.Find("TeamA").active){
                        GameObject.Find("TeamA").SetActive(false);
                    }
                }
            } catch (Exception e) {} 
        }

        [Command]
        public void CmdSetDico(string username, int conn_id)
        {
            //print(hasAuthority);
            conn_tab[Username] = conn_id;

        }

    }
}
