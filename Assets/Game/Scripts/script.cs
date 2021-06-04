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
        public BasicNetManager networkManager;

        // Start is called before the first frame update
        void Start()
        {
            start = 0;
            networkManager = GameObject.FindGameObjectsWithTag("NM")[0].GetComponent(typeof(BasicNetManager)) as BasicNetManager;
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
            if (GameObject.Find("_BlueFlag(Clone)") == null){
                var blueFlag = Instantiate(networkManager.spawnPrefabs[1], new Vector2(-43,-46), Quaternion.identity);
                NetworkServer.Spawn(blueFlag);
                var redFlag = Instantiate(networkManager.spawnPrefabs[2], new Vector2(54,47), Quaternion.identity);
                NetworkServer.Spawn(redFlag);
            }
            foreach(GameObject start in GameObject.FindGameObjectsWithTag("start")){
                start.SetActive(false);
            }
            RpcStartGame();
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            foreach(GameObject counter in GameObject.FindGameObjectsWithTag("counter")){
                counter.GetComponent<TextMeshProUGUI>().enabled = false;
            }
            foreach(GameObject start in GameObject.FindGameObjectsWithTag("start")){
                start.SetActive(false);
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
