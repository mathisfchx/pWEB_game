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
        Player player_hit;
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
                    //print(Player_mv.forward);
                    //print(this.gameObject);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Player_mv.forward),3f, player_mask) ;
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Player_mv.forward) * 3f, Color.red);
                    //print(hit.collider.gameObject);
                    if (hit.collider != null)
                    {
                            if (hit.collider.gameObject != this.gameObject)
                            {
                                player_hit = hit.transform.gameObject.GetComponent(typeof(Player)) as Player;
                                print("Touché");
                                Attack(player_hit);
                            }
                    }
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
            print(player);
        }

        [Command]
        void CmdAttack(Player player)
        {

            player.HealthPoint -= 1;
        }
    }
}
