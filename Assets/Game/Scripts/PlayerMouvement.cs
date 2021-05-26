using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static CameraMovement;
using System;
using System.Text;


public class PlayerMouvement : NetworkBehaviour
{
	
	[SyncVar]
    public float speed;

    public GameObject inv;
    public GameObject hud;
    public Inventory inventory;
    public GameObject cam;
    public GameObject BoutonFinJeu;
    public Button BoutonFin;

    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    //private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        inv = Instantiate(inv);
        inventory = inv.GetComponent<Inventory>();
        hud = Instantiate(hud);
        HUD HUDscript = hud.GetComponent<HUD>();
        HUDscript.inventory = inventory;
        cam=Instantiate(cam);
        //cam=cam.GetComponent<CameraMovement>();
        cam.GetComponent<CameraMovement>().target=gameObject.GetComponent<Transform>();
        if(!this.isLocalPlayer){
            hud.SetActive(false);
        }
        BoutonFinJeu = Instantiate(BoutonFinJeu);
        BoutonFin = BoutonFinJeu.GetComponent<Button>();
        BoutonFin.gameObject.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();

        //gameObject.transform.rotation = new Quaternion(0,0,0,0);
        
        /*
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
        */
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collisionnnnn");
        string item = collision.gameObject.name;
            if (item[0] == '_' ){
                if (collision.collider.enabled){
                    //collision.collider.enabled = false;
                    if (item != null){
                        if(this.isLocalPlayer){
                            Debug.Log("item ajoutééés");
                            inventory.AddItem(item);
                            if(item == "_Cle(Clone)"){
                                EndGameCom();
                            }
                        }
                    }
                    collision.gameObject.SetActive(false);
                }
            }
    }

    void CoroutineBoutonFin(){
        Application.Quit();
    }

    [Command]
    private void EndGameCom(){
        EndGameClient();
        Debug.Log("Fin");
                            
        //Application.Quit();
    }

    [ClientRpc]
    private void EndGameClient(){
        Debug.Log("Fin Client");
        BoutonFin.gameObject.SetActive(true);
        BoutonFin.onClick.AddListener(CoroutineBoutonFin);
    }
/*
    private void OnControllerColliderHit(ControllerColliderHit hit){
        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
        if (item != null){
            inventory.AddItem(item);
        }
    }
*/
}
