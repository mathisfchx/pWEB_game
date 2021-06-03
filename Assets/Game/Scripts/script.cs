using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Game
{
    
    public class script : NetworkBehaviour
    {
        [SyncVar]
        public int start;
        [SerializeField] public Connection_tab conn_tab;

        // Start is called before the first frame update
        void Start()
        {
            start = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (hasAuthority){
                CmdSetStart();
                if (start == 1){
                    Debug.Log("Commencer la partie");
                    CmdStartGame();
                }
            }
        }

        [Command]
        public void CmdStartGame()
        {
            Debug.Log("cmd start game");
            //gérer position des perso pour les faire spawn chez eux (délire de positions en x,y)
            //mais spawner les objets (voir du coté de network manager peut etre je suis aps sur)
            //lancer les règles ()
        }

        [Command]
        public void CmdSetStart()
        {
            start = conn_tab.start;
            RpcSetStart();
        }

        [ClientRpc]
        public void RpcSetStart()
        {
            start = conn_tab.start;
        }
    }
}
