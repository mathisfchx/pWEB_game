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
            //S'active quand le joueur meurt

            if (newHealthPoint <= 0)
            {
                if (hasAuthority)
                {
                    //Déplacement du joueur et mise à jour de ses attributs

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

                    //Déplacement de la caméra 

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
            ((BasicNetManager)NetworkManager.singleton).playersList.Add(this);
        }

        public override void OnStopServer()
        {
            CancelInvoke();
            ((BasicNetManager)NetworkManager.singleton).playersList.Remove(this);
        }

        public override void OnStartClient()
        {

            //Debug.Log("OnStartClient");
            ((BasicNetManager)NetworkManager.singleton).mainPanel.gameObject.SetActive(true);
            if (hasAuthority)
            {
                if (isServer)
                {   
                    conn_tab.Username = userselect.UsernameString;
                    conn_tab.conn_tab[conn_tab.Username] = conn_tab.conn_id;    
                }
                else
                {
                    if (isLocalPlayer)
                    {
                        //print("player");
                        CmdSetUsername(userselect.UsernameString);
                        CmdSetDico();
                    }
                }
            }
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

        public override void OnStopClient()
        {
            Destroy(GetComponent<PlayerMouvement>().cam); 

            if (isLocalPlayer)
                ((BasicNetManager)NetworkManager.singleton).mainPanel.gameObject.SetActive(false);
        }

        [Command]
        public void CmdSetDico()
        {
            //print(hasAuthority);
            conn_tab.conn_tab[conn_tab.Username] = conn_tab.conn_id;
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

        //Fonctions de mise à jour des HP

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