using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Game
{
    public class ProjectileMovement : NetworkBehaviour
    {
        public float speed;
        public Vector3 mouse_position;
        public CircleCollider2D thiscollider;
        public float TimeBtwCollision; 
        [SyncVar]
        public int team;
        [SyncVar]
        public Player caster;

        void Awake()
        {
            thiscollider = gameObject.GetComponent<CircleCollider2D>();
            TimeBtwCollision = 0; 
        }
        
        void Start()
        {

        }


        void Update()
        {
            caster.GetComponent<PlayerAttack>().MoveProjectile(this.transform.gameObject);
            TimeBtwCollision -= Time.deltaTime;
            //print(TimeBtwCollision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (TimeBtwCollision <= 0)
            {
                if (collision.gameObject != caster.transform.gameObject && collision.gameObject != caster.transform.GetChild(1).gameObject && collision.gameObject != null)
                {
                    if (collision.gameObject.transform.parent.GetComponent<Player>() != null)
                    {
                        Player target = collision.gameObject.transform.parent.GetComponent<Player>();
                        if (team != target.team)
                        {
                            caster.GetComponent<PlayerAttack>().RangedAttack(target); 
                        }
                    }
                    caster.GetComponent<PlayerAttack>().DestroyProjectile(this.transform.gameObject);
                    TimeBtwCollision = (float)0.5;
                }
            }

        }

        public void SetTeam(int newTeam)
        {
            //print("je suis dans setteam");

            //print("Je suis Autorité");
            if (isServer)
            {
                team = newTeam;
            }
            else
            {
                CmdSetTeam(newTeam);
            }
        }


        [Command]
        public void CmdSetTeam(int newTeam)
        {
            team = newTeam;
            RpcSetTeam(newTeam);
        }
        [ClientRpc]
        public void RpcSetTeam(int newTeam)
        {
            team = newTeam;
        }

        [Command]
        public void CmdRangedDamage(Game.Player player)
        {
            player.HealthPoint -= 1;
            RpcSetDamage(player);
        }
        [ClientRpc]
        public void RpcSetDamage(Game.Player player)
        {
            player.HealthPoint -= 1;
        }
    }
}
