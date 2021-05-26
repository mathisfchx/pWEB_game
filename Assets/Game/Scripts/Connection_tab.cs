using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Connection_tab : NetworkBehaviour
{
    [SyncVar]
    public string Username = "" ;
    [SyncVar]
    public int conn_id = 0; 
    public SyncDictionary<string, int> conn_tab = new SyncDictionary<string, int>();
    [SyncVar]
    public int nTeamA;
    [SyncVar]
    public int nTeamB;
    [SyncVar]
    public int start;

void Start()
{
    CmdResetStart();
}

void Update()
{
    if (nTeamA == 1 && nTeamB == 1)
    {
        if (hasAuthority){
            start = 1;
        }
        
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

