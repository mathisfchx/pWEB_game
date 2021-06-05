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
        [SyncVar]
        public int team;
        [SyncVar]
        public Player caster;

        void Awake()
        {
            thiscollider = gameObject.GetComponent<CircleCollider2D>();

            //cam = new Camera();
            //new WaitForSeconds((float)0.2);
            //print(mouse_position);
            //print(cam.transform.position);
        }
        void Start()
        {
            //Physics2D.IgnoreCollision(caster.gameObject.transform.GetChild(1).GetComponent<BoxCollider2D>(), thiscollider ,true);
            //Invoke("DestroyProjectile", TimeToLive);
            //print(Camera.main.transform.position);
            //mouse_position = Input.mousePosition;
            //mouse_position.z = Camera.main.nearClipPlane;
            //mouse_position = Camera.main.ScreenToWorldPoint(mouse_position) - transform.position;
            //print(mouse_position);
            //print(mouse_position.normalized);

        }


        void Update()
        {

            //transform.Translate(new Vector2(mouse_position.x, mouse_position.y).normalized * speed * Time.deltaTime);
        }

        void DestroyProjectile()
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject != caster.gameObject && collision.gameObject != caster.transform.GetChild(1).gameObject && collision.gameObject != null )
            {
                if (collision.gameObject.transform.parent.GetComponent<Player>() != null)
                {
                    Player target = collision.gameObject.transform.parent.GetComponent<Player>();
                    if (team != target.team)
                    {
                        //caster.SendMessage("RangedAttack", target);                   //Méthode 1 : crash au niveau des clients non host lors de l'émission du message. Pas fonctionnel non plus pour l'host.
                        //CmdRangedDamage(target);                                      //Méthode 2 : absence d'autorité pour activer la commande.
                        caster.GetComponent<PlayerAttack>().RangedAttack(target);  //Méthode 3 : absence d'autorité malgré le passage par le joueur ?
                    }
                }
                DestroyProjectile();
            }
        }

        public void SetTeam(int newTeam)
        {
            print("je suis dans setteam");

            print("Je suis Autorité");
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

        //Tentative d'interaction avec le serv, non concluante.
        [Command]
        public void CmdRangedDamage(Game.Player player)
        {
            player.HealthPoint -= 1;
            RpcSetDamage(player);
        }
        [ClientRpc]
        public void RpcSetDamage(Game.Player player)
        {
            player.HealthPoint -= 30;
        }
    }
}
