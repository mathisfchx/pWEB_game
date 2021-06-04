using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Game
{
    public class PlayerAttack : NetworkBehaviour
    {
        public float TimeBtwCacAttack;
        public float TimeBtwDistantAttack; 

        public GameObject projectile;
        public GameObject current_projectile;
        public PlayerMouvement Player_mv;
        public Player player_hit;
        int player_mask;
        void Awake()
        {
            player_mask = LayerMask.GetMask("Characte Collision Blocker");
            Player_mv = GetComponent(typeof(PlayerMouvement)) as PlayerMouvement;
            //Player_mv = this.GetComponent<PlayerMouvement>(); 
            TimeBtwDistantAttack = 0 ;
            TimeBtwCacAttack = 0 ;
        }
        void Update()
        {
            if (isLocalPlayer)
            {
                if (TimeBtwCacAttack <= 0)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {

                        this.GetComponent<BoxCollider2D>().enabled = false;
                        this.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
                        //print(Player_mv.forward);
                        //print(this.gameObject);
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Player_mv.forward), 1f, player_mask);
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
                                if (player_hit.team != (this.GetComponent(typeof(Player)) as Player).team)
                                    AttackCac(player_hit);
                            }
                        }
                        this.GetComponent<BoxCollider2D>().enabled = true;
                        this.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = true;
                        TimeBtwCacAttack = 1; 
                    }
                }
                else
                {
                    TimeBtwCacAttack -= Time.deltaTime;
                }
                if(TimeBtwDistantAttack <= 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        print(Player_mv.cam.GetComponent<Camera>().transform.position);
                        if(Player_mv.forward == Vector2.up)
                        {
                            current_projectile = Instantiate(projectile, transform.position + new Vector3(0, (float)0.5, 0), transform.rotation);
                            current_projectile.GetComponent<ProjectileMovement>().team = transform.parent.gameObject.GetComponent<Player>().team; 
                            TimeBtwDistantAttack = 1;
                        }
                        else if (Player_mv.forward == Vector2.down)
                        {
                            current_projectile = Instantiate(projectile, transform.position + new Vector3(0,(float)-0.5,0), transform.rotation);
                            current_projectile.GetComponent<ProjectileMovement>().team = transform.parent.gameObject.GetComponent<Player>().team;
                            TimeBtwDistantAttack = 1;
                        }
                        else if(Player_mv.forward == Vector2.left)
                        {
                            current_projectile = Instantiate(projectile, transform.position + new Vector3((float)-0.5, 0, 0), transform.rotation);
                            current_projectile.GetComponent<ProjectileMovement>().team = transform.parent.gameObject.GetComponent<Player>().team;
                            TimeBtwDistantAttack = 1;
                        }
                        else
                        {
                            current_projectile = Instantiate(projectile, transform.position + new Vector3((float)0.5, 0, 0), transform.rotation);
                            current_projectile.GetComponent<ProjectileMovement>().team = transform.parent.gameObject.GetComponent<Player>().team;
                            TimeBtwDistantAttack = 1;
                        }

                    }
                }
                else
                {
                    TimeBtwDistantAttack -= Time.deltaTime; 
                }
            }
        }


        void AttackCac(Player player)
        {
            if (isLocalPlayer)
            {
                if (isClient)
                {
                    CmdAttackCac(player);
                }
            }
            //print(player);
        }

        [Command]
        void CmdAttackCac(Player player)
        {

            player.HealthPoint -= 2;
        }
    }
}
