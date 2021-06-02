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
        public int team;
    


        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public void Awake()
        {
            conn_tab = GameObject.FindGameObjectsWithTag("Conn_tag")[0].GetComponent(typeof(Connection_tab)) as Connection_tab;
            userselect= GameObject.FindGameObjectsWithTag("ServerScript")[0].GetComponent(typeof(UserSelect)) as UserSelect;
        }

        public void Update()
        {
            if (isLocalPlayer){
                Username = userselect.UsernameString;
                CmdChangeName(Username);
                CmdSetTeam(userselect.team);
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
        public void CmdChangeName(string newName)
        {
            pseudo = GetComponentInChildren<TextMeshPro>();
            pseudo.text = newName;
            if (team == 0){
                pseudo.color = Color.blue;
            } else if (team == 1) {
                pseudo.color = Color.red;
            } else {
                pseudo.color = Color.white;
            }
            
            RpcChangeName(newName);
        }

        [ClientRpc]
        public void RpcChangeName(string newName)
        {
            pseudo = GetComponentInChildren<TextMeshPro>();
            pseudo.text = newName;
            if (team == 0){
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

            Destroy(gameObject.GetComponent<PlayerMouvement>().cam); // les erreurs dans le serv viennent sans doute d'ici

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
    }
}