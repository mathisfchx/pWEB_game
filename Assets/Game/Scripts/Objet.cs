﻿using UnityEngine;
using Mirror;

class Objet : NetworkBehaviour {
    [SyncVar]
    public int Color;

    public override void OnStartClient() {
    }
}