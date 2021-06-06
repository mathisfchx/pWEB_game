using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace Game
{
    public class Player : NetworkBehaviour
    {

        public Connection_tab conn_tab; 
        public TextMeshPro pseudo;
        public UserSelect userselect;
        [SyncVar]
        public string Username;
        [SyncVar]
        public int team = 2;
        [SyncVar(hook = nameof(OnCheckIfAlive))]
        public int HealthPoint;
        public bool Dead = false;
        

        void OnCheckIfAlive(int oldHealthPoint , int newHealthPoint)
        {
            if (newHealthPoint <= 0)
            {
                if (hasAuthority)
                {
                    this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().enabled = false; 
                    if (isServer)
                    {
                        this.Dead = true;
                        this.HealthPoint = 4;
                        if (this.team == 3) {
                            this.transform.SetPositionAndRotation(new Vector3(-38, -44),new Quaternion(0,0,0,0));
                        } else if (this.team == 1){
                            this.transform.SetPositionAndRotation(new Vector3(55, 46),new Quaternion(0,0,0,0));
                        } else {
                            this.transform.SetPositionAndRotation(new Vector3(5, -5), new Quaternion(0, 0, 0, 0));
                        }
                    }
                    if (isClient)
                    {
                        this.Dead = true;
                        CmdDead(this);
                    }
                    this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().enabled = true ;
                    if (team == 3){
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)-38, (float)-22.3) ;
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-57.3, (float)-42.5) ;
                    } else if (team == 1){
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)50.8, (float)46.3) ;
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)31.5, (float)26.1) ;
                    } else {
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition = new Vector2((float)6.5, (float)12) ;
                        this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition = new Vector2((float)-12.8,(float)-8.2);
                    }
                    this.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().target = this.transform;
                }

            }
        }

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public void Awake()
        {
            conn_tab = GameObject.FindGameObjectsWithTag("Conn_tag")[0].GetComponent(typeof(Connection_tab)) as Connection_tab;
            userselect= GameObject.FindGameObjectsWithTag("ServerScript")[0].GetComponent(typeof(UserSelect)) as UserSelect;
            HealthPoint = 4; 
            if (hasAuthority){
                if (isClient){
                    CmdSetHp(this);
                }
            }
        }

        public void Update()
        {
            if (conn_tab.start == 1 && team == 2){
                    this.gameObject.SetActive(false);
                }
            if (isLocalPlayer){
                Username = userselect.UsernameString;
                CmdChangeName(Username, this);
                CmdSetTeam(userselect.team);

                CmdTeam();
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            // Add this to the static Players List
            ((BasicNetManager)NetworkManager.singleton).playersList.Add(this);
        }

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer()
        {
            CancelInvoke();
            ((BasicNetManager)NetworkManager.singleton).playersList.Remove(this);
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient()
        {

            Debug.Log("OnStartClient");
            // Activate the main panel
            ((BasicNetManager)NetworkManager.singleton).mainPanel.gameObject.SetActive(true);
            if (hasAuthority)
            {
                if (isServer)
                {   
                    conn_tab.Username = userselect.UsernameString;
                    conn_tab.conn_tab[conn_tab.Username] = conn_tab.conn_id;
                    //CmdSetDico(conn_tab.Username, this.netIdentity.connectionToClient.connectionId);
                    
                }
                else
                {
                    if (isLocalPlayer)
                    {
                        print("player");
                        CmdSetUsername(userselect.UsernameString);
                        CmdSetDico();
                    }
                }
            }
            
            /* else {
            pseudo = GetComponentInChildren<TextMeshPro>();
            pseudo.text = Username;
            }
            */
        }

        [Command(requiresAuthority = false)]
        public void CmdChangeName(string newName, Player player)
        {
            pseudo = GetComponentInChildren<TextMeshPro>();
            pseudo.text = newName+"\nPv : "+player.HealthPoint+"/4";
            if (team == 3){
                pseudo.color = Color.blue;
            } else if (team == 1) {
                pseudo.color = Color.red;
            } else {
                pseudo.color = Color.white;
            }
            
            RpcChangeName(newName, player);
        }

        [ClientRpc]
        public void RpcChangeName(string newName, Player player)
        {
            pseudo = GetComponentInChildren<TextMeshPro>();
            pseudo.text = newName+"\nPv : "+player.HealthPoint+"/4";
            if (team == 3){
                pseudo.color = Color.blue;
            } else if (team == 1) {
                pseudo.color = Color.red;
            }

        }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient()
        {
            Destroy(GetComponent<PlayerMouvement>().cam); // les erreurs dans le serv viennent sans doute d'ici

            // Disable the main panel for local player
            if (isLocalPlayer)
                ((BasicNetManager)NetworkManager.singleton).mainPanel.gameObject.SetActive(false);
        }

        [Command]
        public void CmdSetDico()
        {
            print(hasAuthority);
            //if(hasAuthority)
            //{
                conn_tab.conn_tab[conn_tab.Username] = conn_tab.conn_id;
                //RpcModifDico(username, conn_id);
            //}

        }
        [Command]
        public void CmdSetUsername(string username)
        {
            conn_tab.Username = username;
        }

        [Command]
        public void CmdSetTeam(int T)
        {
            team = T;
        }

        [Command]
        public void CmdDead(Player player)
        {
            player.HealthPoint = 4;
            if (this.team == 3) {
                this.transform.SetPositionAndRotation(new Vector3(-65, (float) -47.5),new Quaternion(0,0,0,0));
            } else if (this.team == 1){
                this.transform.SetPositionAndRotation(new Vector3(60, 24),new Quaternion(0,0,0,0));
            } else {
                this.transform.SetPositionAndRotation(new Vector3(5, -5), new Quaternion(0, 0, 0, 0));
            }
            RpcDead();
        }
        [ClientRpc]
        public void RpcDead()
        {
            this.HealthPoint = 4;
            if (this.team == 3) {
                this.transform.SetPositionAndRotation(new Vector3(-65, (float) -47.5),new Quaternion(0,0,0,0));
            } else if (this.team == 1){
                this.transform.SetPositionAndRotation(new Vector3(60, 24),new Quaternion(0,0,0,0));
            } else {
                this.transform.SetPositionAndRotation(new Vector3(5, -5), new Quaternion(0, 0, 0, 0));
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdTeam()
        {
            GameObject.Find("counterA").GetComponent<nteamA>().nA = conn_tab.nA;
            GameObject.Find("counterB").GetComponent<nteamB>().nB = conn_tab.nB;
            GameObject.Find("counterA").GetComponent<nteamA>().maxTeam = conn_tab.maxTeam;
            GameObject.Find("counterB").GetComponent<nteamB>().maxTeam = conn_tab.maxTeam;
            RpcTeam();
        }

        [ClientRpc]
        public void RpcTeam()
        {
            GameObject.Find("counterA").GetComponent<nteamA>().nA = conn_tab.nA;
            GameObject.Find("counterB").GetComponent<nteamB>().nB = conn_tab.nB;
            GameObject.Find("counterA").GetComponent<nteamA>().maxTeam = conn_tab.maxTeam;
            GameObject.Find("counterB").GetComponent<nteamB>().maxTeam = conn_tab.maxTeam;
        }

        [Command]
        public void CmdSetHp(Player player){
            player.HealthPoint = 4;
            RpcSetHp(player);
        }

        [Command]
        public void RpcSetHp(Player player){
            player.HealthPoint = 4;
        }
    }
}