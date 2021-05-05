using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    
    public float radius = 1f; // Distance à laquelle doit être le joueur de l'objet pour intéragir avec

    void OnDrawGizmosSelected (){

    	Gizmos.color = Color.yellow;
    	Gizmos.DrawWireSphere(transform.position, radius);

    }

}
