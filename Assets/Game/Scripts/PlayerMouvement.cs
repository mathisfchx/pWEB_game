using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Interactable;

public class PlayerMouvement : NetworkBehaviour
{
	
	[SyncVar]
    public float speed;
    //[SyncVar]
    //public Interactable focus;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    //private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();

        //test pour une interaction avec la touche E
        if(Input.GetKey(KeyCode.E)){

           	//if(Physics.Raycast(change, hit, 100)){

        		Interactable interactable = GetComponent<Interactable>();

           		if (interactable != null){

                    Debug.Log("Vous avez trouvé un objet");
           			//SetFocus(interactable);

            	}
            }
        //}
    }
/*
    void SetFocus(Interactable newFocus){

    	focus = newFocus;	

    }
*/
    void UpdateAnimationAndMove(){

        if(this.isLocalPlayer){
            if (change != Vector3.zero){
                MoveCharacter();
                animator.SetFloat("moveX", change.x);
                animator.SetFloat("moveY", change.y);
                animator.SetBool("moving", true);
            }else{
                animator.SetBool("moving", false);
            }
        }
    }

    void MoveCharacter()
    {
        myRigidbody.MovePosition(
            transform.position + change.normalized * speed * Time.deltaTime
        );
    }
}
