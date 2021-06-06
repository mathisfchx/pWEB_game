using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Game
{
    [AddComponentMenu("")]
    public class BasicNetManager : NetworkManager
    {
        internal readonly List<Player> playersList = new List<Player>();
        [SerializeField] Connection_tab conn_tab ;
        [SerializeField] UserSelect userselect ; 

        [Header("Canvas UI")]

        [Tooltip("Assign Main Panel so it can be turned on from Player:OnStartClient")]
        public RectTransform mainPanel;

        public override void OnStartServer()
        {
            SpawnFlagsAndQG();
        }


        void SpawnFlagsAndQG()
        {
            GameObject blueFlag = (GameObject)Instantiate(spawnPrefabs[1], new Vector2(-43,-46), Quaternion.identity);
            NetworkServer.Spawn(blueFlag);
            GameObject redFlag = (GameObject)Instantiate(spawnPrefabs[2], new Vector2(54,47), Quaternion.identity);
            NetworkServer.Spawn(redFlag);

            blueFlag.transform.SetPositionAndRotation(new Vector3(-300, -300),new Quaternion(0,0,0,0));
            redFlag.transform.SetPositionAndRotation(new Vector3(300, 300),new Quaternion(0,0,0,0));

            //GameObject cleGo = Instantiate(spawnPrefabs[0], new Vector2(-11,-13), Quaternion.identity);
            //NetworkServer.Spawn(cleGo);
            GameObject blueQG = Instantiate(spawnPrefabs[3], new Vector2(-38,-47), Quaternion.identity);
            NetworkServer.Spawn(blueQG);
            GameObject redQG = Instantiate(spawnPrefabs[4], new Vector2(57,47), Quaternion.identity);
            NetworkServer.Spawn(redQG);
        }

        public void RespawnBlueFlag(float x, float y){
            var blueFlag = Instantiate(spawnPrefabs[1], new Vector2(x,y), Quaternion.identity);
            NetworkServer.Spawn(blueFlag);
        }

        public void RespawnRedFlag(float x, float y){
            var redFlag = Instantiate(spawnPrefabs[2], new Vector2(x,y), Quaternion.identity);
            NetworkServer.Spawn(redFlag);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            conn_tab.conn_id= conn.connectionId;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            userselect.CoroutineDisconnect(FindUsername(conn.connectionId));
        }

        string FindUsername(int id){
            foreach(string key in conn_tab.conn_tab.Keys){
                if(conn_tab.conn_tab[key]==id){
                    return key;                
                }
            }

            return " ";
        }
        public override void OnStopServer()
        {
            //print("onStopServer");
            base.OnStopServer();
            userselect.CoroutineDisconnectAll();
        }
    }
}
