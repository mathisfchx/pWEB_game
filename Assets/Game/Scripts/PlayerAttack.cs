using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
namespace Game
{
    public class PlayerAttack : NetworkBehaviour
    {
        public float TimeBtwCacAttack;
        public float TimeBtwDistantAttack;

        public GameObject projectile_prefab;
        GameObject[] projectiles;
        public PlayerMouvement Player_mv;
        public Player player_hit;
        //[SyncVar]
        //public Player caster;
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
            if (this.isLocalPlayer)
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
                    if (Input.GetMouseButtonDown(1))
                    {
                        Vector3 vect = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                        float speed = (float)10;
                        if (Player_mv.forward == Vector2.up)
                        {
                            CmdInstantiateProjectile(transform.position + new Vector3(0, (float)0.5, 0),vect.x,vect.y,speed, gameObject.GetComponent<Player>());
                            TimeBtwDistantAttack = 1;
                        }
                        else if (Player_mv.forward == Vector2.down)
                        {
                            CmdInstantiateProjectile(transform.position + new Vector3(0, (float)-1, 0), vect.x, vect.y, speed, gameObject.GetComponent<Player>());
                            TimeBtwDistantAttack = 1;
                        }
                        else if(Player_mv.forward == Vector2.left)
                        {
                            CmdInstantiateProjectile(transform.position + new Vector3((float)-0.5, 0, 0), vect.x, vect.y, speed, gameObject.GetComponent<Player>());
                            TimeBtwDistantAttack = 1;
                        }
                        else
                        {
                            CmdInstantiateProjectile(transform.position + new Vector3((float)0.5, 0, 0), vect.x, vect.y, speed, gameObject.GetComponent<Player>());
                            TimeBtwDistantAttack = 1;
                        }

                    }
                }
                else
                {
                    TimeBtwDistantAttack -= Time.deltaTime;
                }
                //print("Avant ");

                
            }

        }

        public void DestroyProjectile(GameObject go){
            if (hasAuthority){
                if (isLocalPlayer){
                    if (isClient){
                        CmdDestroyProjectile(go);
                    } else if (isServer){
                        NetworkServer.Destroy(go);
                    }
                }
            }
        }

        [Command]
        public void CmdDestroyProjectile(GameObject go)
        {
            RpcDestroyProjectile(go);
            Destroy(go);
        }

        [ClientRpc]
        public void RpcDestroyProjectile(GameObject go)
        {
            Destroy(go);
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

        public void RangedAttack(Player player)
        {
            if (isServer)
            {
                player.HealthPoint -= 1;
            }
            else
            {
                print("InRangedAttack");
                if (hasAuthority)
                {

                    if (isLocalPlayer)
                    {
                        print("InIsLocalPlayer");
                        if (isClient)
                        {
                            print("RANGED ATTACK");
                            CmdRangedAttack(player);
                        }
                    }
                }
            }
        }

        [Command]
        void CmdAttackCac(Player player)
        {

            player.HealthPoint -= 2;
        }

        [Command]
        void CmdRangedAttack(Player player)
        {
            player.HealthPoint -= 1;
        }


        [Command]
        public void CmdInstantiateProjectile(Vector3 vect , float x , float y , float speed ,Player player)
        {
            GameObject current_projectile;
            current_projectile = Instantiate(projectile_prefab, vect, transform.rotation);
            current_projectile.GetComponent<ProjectileMovement>().mouse_position.x = x;
            current_projectile.GetComponent<ProjectileMovement>().mouse_position.y = y ;
            current_projectile.GetComponent<ProjectileMovement>().speed = speed ;
            current_projectile.GetComponent<ProjectileMovement>().team = player.team;
            current_projectile.GetComponent<ProjectileMovement>().caster = player;
            //current_projectile.GetComponent<ProjectileMovement>().mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //mouse_position.z = Camera.main.nearClipPlane;
            Destroy(current_projectile, 10);
            NetworkServer.Spawn(current_projectile);
        }

        /*[ClientRpc]
        public void RcpInstantiateProjectile(Player player,GameObject current_projectile)
        {
            current_projectile.GetComponent<ProjectileMovement>().caster = player;
        }
        */
        /*
        public void Move(GameObject proj)
        {
            float x = proj.GetComponent<ProjectileMovement>().mouse_position.x;
            float y = proj.GetComponent<ProjectileMovement>().mouse_position.y;
            float speed = proj.GetComponent<ProjectileMovement>().speed;
            proj.transform.Translate(new Vector2(x, y).normalized * speed * Time.deltaTime);
        }
        */
        
        public void MoveProjectile(GameObject proj){
            if (hasAuthority){
                if (isLocalPlayer){
                    if (isClient){
                        CmdMoveProjectile(proj);
                    } else if (isServer){
                        float x = proj.GetComponent<ProjectileMovement>().mouse_position.x;
                        float y = proj.GetComponent<ProjectileMovement>().mouse_position.y;
                        float speed = proj.GetComponent<ProjectileMovement>().speed;
                        proj.transform.Translate(new Vector2(x, y).normalized * speed * Time.deltaTime);
                    }
                }
            }
        }

        [Command]  
        public void CmdMoveProjectile(GameObject proj){
            try{
                float x = proj.GetComponent<ProjectileMovement>().mouse_position.x;
                float y = proj.GetComponent<ProjectileMovement>().mouse_position.y;
                float speed = proj.GetComponent<ProjectileMovement>().speed;
                proj.transform.Translate(new Vector2(x, y).normalized * speed * Time.deltaTime);
            }catch(Exception e){

            }
            
        }

    }
}
