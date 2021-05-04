using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    
    public float radius = 1f; // Distance à laquelle doit être le joueur de l'objet pour intéragir avec

    void OnDrawGizmosSelected (){

    	Gizmos.color = Color.yellow;
    	Gizmos.DrawWireSphere(transform.position, radius);

    }

}
