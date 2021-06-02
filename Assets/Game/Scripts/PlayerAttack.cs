using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Game
{
    public class PlayerAttack : NetworkBehaviour
    {
        // Start is called before the first frame update
        // Update is called once per frame
        public PlayerMouvement Player_mv;
        public Player player_hit;
        int player_mask;
        void Awake()
        {
            player_mask = LayerMask.GetMask("Characte Collision Blocker");
            Player_mv = GetComponent(typeof(PlayerMouvement)) as PlayerMouvement; 
            //Player_mv = this.GetComponent<PlayerMouvement>(); 
        }
        void Update()
        {
            if (isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    this.GetComponent<BoxCollider2D>().enabled = false;
                    this.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false; 
                    //print(Player_mv.forward);
                    //print(this.gameObject);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Player_mv.forward),3f, player_mask) ;
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Player_mv.forward) * 3f, Color.red);
                    //print(hit.collider.gameObject);
                    if (hit && hit.collider != null)
                    {
                            if (hit.transform.parent.gameObject != this.gameObject)
                            {
                                player_hit = hit.transform.parent.gameObject.GetComponent(typeof(Player)) as Player;
                                print(player_hit);
                                //print(hit.collider);
                                print("Touché");
                                if(player_hit.team != (this.GetComponent(typeof(Player)) as Player).team )
                                Attack(player_hit);
                            }
                    }
                    this.GetComponent<BoxCollider2D>().enabled = true;
                    this.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }


        void Attack(Player player)
        {
            if (isLocalPlayer)
            {
                if (isClient)
                {
                    CmdAttack(player);
                }
            }
            //print(player);
        }

        [Command]
        void CmdAttack(Player player)
        {

            player.HealthPoint -= 1;
        }
    }
}
