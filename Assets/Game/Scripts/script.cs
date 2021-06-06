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
        public GameObject start_text;

        // Start is called before the first frame update
        void Start()
        {
            start = 0;
            start_text.SetActive(false);
        }

        private IEnumerator waiter(){
            start_text.SetActive(true);
            start_text.GetComponent<TextMeshProUGUI>().text = "CAPTURE THE FLAG !";
            yield return new WaitForSeconds(3);
            start_text.SetActive(false);
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
                networkManager = GameObject.FindWithTag("NM").GetComponent(typeof(BasicNetManager)) as BasicNetManager;
                Debug.Log("Commencer la partie");
                foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
                    Debug.Log("on rentre dans le foreach de lateupdate");
                    if (player.GetComponent<PlayerMouvement>().isLocalPlayer){
                        Debug.Log("is localplayer");
                        if (player.GetComponent<Player>().team == 3) {
                            player.transform.SetPositionAndRotation(new Vector3(-65, (float) -47.5),new Quaternion(0,0,0,0));
                        } else if (player.GetComponent<Player>().team == 1){
                            player.transform.SetPositionAndRotation(new Vector3(60, 24),new Quaternion(0,0,0,0));
                        } else {
                            player.transform.SetPositionAndRotation(new Vector3(5, -5), new Quaternion(0, 0, 0, 0));
                        }
                        Debug.Log("les spawn ont étés executés");
                        player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().enabled = true ;
                        Debug.Log("cam movement enabled");
                        if (player.GetComponent<Player>().team == 3){
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)-38, (float)-22.3) ;
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-57.3, (float)-42.5) ;
                        } else if (player.GetComponent<Player>().team == 1){
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)50.8, (float)46.3) ;
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)31.5, (float)26.1) ;
                        } else {
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)6.5, (float)12) ;
                            player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-12.8,(float)-8.2);
                        }
                        Debug.Log("cam positions fixed");
                        player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().target = player.transform;
                        Debug.Log("cam target = player");
                    }
                }
                CmdStartGame();
                Debug.Log("cmdstartgame est partie");
                this.enabled = false;
                Debug.Log("le script est down");
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdStartGame()
        {
            if (GameObject.Find("_BlueFlag(Clone)") == null){
                /*
                var blueFlag = Instantiate(networkManager.spawnPrefabs[1], new Vector2(-43,-46), Quaternion.identity);
                NetworkServer.Spawn(blueFlag);
                var redFlag = Instantiate(networkManager.spawnPrefabs[2], new Vector2(54,47), Quaternion.identity);
                NetworkServer.Spawn(redFlag);
                */
            }
            GameObject.Find("_BlueFlag(Clone)").transform.SetPositionAndRotation(new Vector3(-43, -46),new Quaternion(0,0,0,0));
            GameObject.Find("_RedFlag(Clone)").transform.SetPositionAndRotation(new Vector3(54, 47),new Quaternion(0,0,0,0));
            StartCoroutine(waiter());
            RpcStartGame();
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            Debug.Log("on est dans le rpc");

            GameObject.Find("_BlueFlag(Clone)").transform.SetPositionAndRotation(new Vector3(-43, -46),new Quaternion(0,0,0,0));
            GameObject.Find("_RedFlag(Clone)").transform.SetPositionAndRotation(new Vector3(54, 47),new Quaternion(0,0,0,0));
            
            foreach(GameObject start in GameObject.FindGameObjectsWithTag("start")){
                start.SetActive(false);
            }
            foreach(GameObject counter in GameObject.FindGameObjectsWithTag("counter")){
                counter.GetComponent<TextMeshProUGUI>().enabled = false;
            }
            foreach(GameObject start in GameObject.FindGameObjectsWithTag("start")){
                start.SetActive(false);
            }
            /*
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
                if (player.GetComponent<Player>().team == 3) {
                    player.transform.SetPositionAndRotation(new Vector3(-65, (float) -47.5),new Quaternion(0,0,0,0));
                } else if (player.GetComponent<Player>().team == 1){
                    player.transform.SetPositionAndRotation(new Vector3(60, 24),new Quaternion(0,0,0,0));
                } else {
                    player.transform.SetPositionAndRotation(new Vector3(5, -5), new Quaternion(0, 0, 0, 0));
                }
                player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().enabled = true ;
                if (player.GetComponent<Player>().team == 3){
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)-38, (float)-22.3) ;
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-57.3, (float)-42.5) ;
                } else if (player.GetComponent<Player>().team == 1){
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)50.8, (float)46.3) ;
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)31.5, (float)26.1) ;
                } else {
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)6.5, (float)12) ;
                    player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-12.8,(float)-8.2);
                }
                player.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().target = player.transform;
            }
            */
            StartCoroutine(waiter());
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
