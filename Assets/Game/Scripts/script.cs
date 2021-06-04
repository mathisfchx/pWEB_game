using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

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
            //CmdSetStart();
            start = conn_tab.start;
        }

        void LateUpdate()
        {
            if (start == 1){
                Debug.Log("Commencer la partie");
                CmdStartGame();
                this.enabled = false;
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdStartGame()
        {
            RpcStartGame();
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            foreach(GameObject counter in GameObject.FindGameObjectsWithTag("counter")){
                counter.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }

        [Command(requiresAuthority = false)]
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
