using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using System.Text;

namespace Game{
    public class PlayerMouvement : NetworkBehaviour
    {
        
        [SyncVar]
        public float speed;
        public Vector2 forward;

        public GameObject inv;
        public GameObject hud;
        public HUD HUDscript;
        public Inventory inventory;
        public GameObject cam;
        public GameObject BoutonFinJeu;
        public Button BoutonFin;
        public GameObject player;
        public int team = 0;
        public int HP;
        public bool mort = false;
        public BasicNetManager networkManager;

        private Rigidbody2D myRigidbody;
        private Vector3 change;
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody2D>();
            inv = Instantiate(inv);
            inventory = inv.GetComponent<Inventory>();
            hud = Instantiate(hud);
            HUDscript = hud.GetComponent<HUD>();
            HUDscript.inventory = inventory;
            cam=Instantiate(cam);
            cam.GetComponent<CameraMovement>().target=gameObject.GetComponent<Transform>();
            if(!this.isLocalPlayer){
                hud.SetActive(false);
            }
            BoutonFinJeu = Instantiate(BoutonFinJeu);
            BoutonFin = BoutonFinJeu.GetComponent<Button>();
            BoutonFin.gameObject.SetActive(false);
            networkManager = GameObject.FindGameObjectsWithTag("NM")[0].GetComponent(typeof(BasicNetManager)) as BasicNetManager;
        }

        void Update()
        {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            if (change.x == 0 && change.y == 1)
            {
                forward = Vector2.up;
            }
            else if (change.x == 0 && change.y == -1)
            {
                forward = Vector2.down;
            }
            else if (change.x == 1 && change.y == 0)
            {
                forward = Vector2.right;
            }
            else if (change.x == -1 && change.y == 0)
            {
                forward = Vector2.left;
            }
            UpdateAnimationAndMove();
            UpdateAnimationAttack();
            

            this.team = (GetComponent(typeof(Player)) as Player).team;
            if(this.isLocalPlayer){
                if(inventory.mItems.Contains("_BlueFlag(Clone)")){
                    speedFlag();
                }else if(inventory.mItems.Contains("_RedFlag(Clone)")){
                    speedFlag();
                }
                this.mort = (GetComponent(typeof(Player)) as Player).Dead;
                if(mort == true){
                    speedWithoutFlag();
	                if(inventory.mItems.Contains("_BlueFlag(Clone)")){
	                	//Debug.Log("Contains");
	               		inventory.mItems.Remove("_BlueFlag(Clone)");
	            		
	            		//Debug.Log("Remove");
	             		HUDscript.DelItem("_BlueFlag(Clone)");
	            		
	               		RespawnBlueFlagCom(gameObject.transform.position.x, gameObject.transform.position.y);
	             		//Debug.Log("Respawn en "+gameObject.transform.position.x+","+gameObject.transform.position.y);
                		
	                	(GetComponent(typeof(Player)) as Player).Dead = false;
	                }

	               	if(inventory.mItems.Contains("_RedFlag(Clone)")){
	                	//Debug.Log("Contains");
	               		inventory.mItems.Remove("_RedFlag(Clone)");
	                		
	               		//Debug.Log("Remove");
	                	HUDscript.DelItem("_RedFlag(Clone)");
	                		
	                	RespawnRedFlagCom(gameObject.transform.position.x, gameObject.transform.position.y);
	                	//Debug.Log("Respawn en "+gameObject.transform.position.x+","+gameObject.transform.position.y);
	                	
	                	(GetComponent(typeof(Player)) as Player).Dead = false;
                	}
	            }
            }
        }
        
        [Command]
        void speedFlag(){
            speed = 6;
        }

        [Command]
        void speedWithoutFlag(){
            speed = 8;
        }

        [Command]
        void RespawnBlueFlagCom(float x, float y){

            GameObject.Find("_BlueFlag(Clone)").transform.SetPositionAndRotation(new Vector3(x, y),new Quaternion(0,0,0,0));
            RpcRespawnBlueFlagCom(x,y);
        }

        [ClientRpc]
        void RpcRespawnBlueFlagCom(float x, float y){

            GameObject.Find("_BlueFlag(Clone)").transform.SetPositionAndRotation(new Vector3(x, y),new Quaternion(0,0,0,0));
        }

        [Command]
        void RespawnRedFlagCom(float x, float y){

            GameObject.Find("_RedFlag(Clone)").transform.SetPositionAndRotation(new Vector3(x, y),new Quaternion(0,0,0,0));
            RpcRespawnRedFlagCom(x,y);
        }

        [ClientRpc]
        void RpcRespawnRedFlagCom(float x, float y){

            GameObject.Find("_RedFlag(Clone)").transform.SetPositionAndRotation(new Vector3(x, y),new Quaternion(0,0,0,0));
        }

        void UpdateAnimationAndMove(){

            if(this.isLocalPlayer){
                if (change != Vector3.zero){
                    animator.SetFloat("moveX", change.x);
                    animator.SetFloat("moveY", change.y);
                    animator.SetBool("moving", true);
                }else{
                    animator.SetBool("moving", false);
                }
            }
        }

        void UpdateAnimationAttack(){

            if(this.isLocalPlayer){
                if (Input.GetKey("space")){
                    animator.SetBool("attack", true);
                }else{
                    animator.SetBool("attack", false);
                }
            }
        }
 
        void FixedUpdate(){
            if (isLocalPlayer){
                myRigidbody.MovePosition(
                transform.position + change.normalized * speed * Time.deltaTime
            );
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("collisionnnnn");
            string item = collision.gameObject.name;

            if (item[0] == '_' ){
                if (collision.collider.enabled){
                    if (item != null){
                        if(this.isLocalPlayer){
                            //Debug.Log("item ajoutééés");
                            if(team == 3){
                                if(item == "_RedFlag(Clone)"){
                                    addItemComClient(collision.gameObject, item);
                                }
                                if(item == "_BlueQG(Clone)"){
                                    if(inventory.mItems.Contains("_RedFlag(Clone)") == true){
                                        EndGameCom();
                                    }   
                                }
                            }else if(team == 1){
                                if(item == "_BlueFlag(Clone)"){
                                    addItemComClient(collision.gameObject, item);
                                }
                                if(item == "_RedQG(Clone)"){
                                    if(inventory.mItems.Contains("_BlueFlag(Clone)") == true){
                                        EndGameCom();
                                    }
                                
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command]
        void addItemComClient(GameObject collision, string item){
        	addItemCom(collision, item);
            collision.transform.SetPositionAndRotation(new Vector3(-500, -500),new Quaternion(0,0,0,0));
        }

        [ClientRpc]
        void addItemCom(GameObject collision, string item){
        	inventory.AddItem(item);
            collision.transform.SetPositionAndRotation(new Vector3(-500, -500),new Quaternion(0,0,0,0));
        }

        [ClientRpc]
        void addItemClientCom(GameObject collision, string item){
        	addItemClient(collision, item);
            collision.transform.SetPositionAndRotation(new Vector3(-500, -500),new Quaternion(0,0,0,0));
        }

        [Command]
        void addItemClient(GameObject collision, string item){
        	inventory.AddItem(item);
            collision.transform.SetPositionAndRotation(new Vector3(-500, -500),new Quaternion(0,0,0,0));
        }

        void CoroutineBoutonFin(){
            Application.Quit();
        }

        [Command]
        private void EndGameCom(){
            EndGameClient();
            //Debug.Log("Fin");
        }

        [ClientRpc]
        private void EndGameClient(){
            //Debug.Log("Fin Client");

            BoutonFin.gameObject.SetActive(true);
            BoutonFin.onClick.AddListener(CoroutineBoutonFin);
            if(team == 3){
                BoutonFin.GetComponentInChildren<Text>().text = "L'équipe bleue a gagné ! \n Bravo !!! Cliquez pour terminer la partie";
            }else if(team == 1){
                BoutonFin.GetComponentInChildren<Text>().text = "L'équipe rouge a gagné ! \n Bravo !!! Cliquez pour terminer la partie";
            }else{
                BoutonFin.GetComponentInChildren<Text>().text = "Bravo vous avez cassé le jeu !";
            }     
        }
    }
}
