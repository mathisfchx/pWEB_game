// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.

using System;
using UnityEngine;

namespace Mirror
{
    /// <summary>
    /// An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.
    /// <para>This component also shows useful internal state for the networking system in the inspector window of the editor. It allows users to view connections, networked objects, message handlers, and packet statistics. This information can be helpful when debugging networked games.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.com/docs/Articles/Components/NetworkManagerHUD.html")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;

        /// <summary>
        /// Whether to show the default control HUD at runtime.
        /// </summary>
        [Obsolete("showGUI will be removed unless someone has a valid use case. Simply use or don't use the HUD component.")]
        public bool showGUI = true;

        /// <summary>
        /// The horizontal offset in pixels to draw the HUD runtime GUI at.
        /// </summary>
        public int offsetX;

        /// <summary>
        /// The vertical offset in pixels to draw the HUD runtime GUI at.
        /// </summary>
        public int offsetY;

        public GameObject teamA;
        public GameObject teamB;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
#pragma warning disable 618
            if (!showGUI) return;
#pragma warning restore 618

            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            // client ready
            if (NetworkClient.isConnected && !ClientScene.ready)
            {
                if (GUILayout.Button("Client Ready"))
                {
                    ClientScene.Ready(NetworkClient.connection);

                    if (ClientScene.localPlayer == null)
                    {
                        ClientScene.AddPlayer(NetworkClient.connection);
                    }
                }
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Host (Server + Client)"))
                    {
                        manager.networkAddress = "localhost";
                        manager.StartHost();

                        //GameObject teamA = GameObject.Find("TeamA");
                        teamA.SetActive(true);
                        //GameObject teamB = GameObject.Find("TeamB");
                        teamB.SetActive(true);

                        //GameObject.Find("Player(Clone)").<anyComponent>().enabled = false;
                    }
                }

                //Voici la nouvelle interface pour se connecter au serveur (je n'ai pas encore réussi à modifier l'emplacement du bouton)
                if (GUILayout.Button("Jouer sur le serveur AWS"))
                {
                    manager.networkAddress = "15.188.17.42";
                    manager.StartClient();
                    //GameObject teamA = GameObject.Find("TeamA");
                    teamA.SetActive(true);
                    //GameObject teamB = GameObject.Find("TeamB");
                    teamB.SetActive(true);

                    //GameObject.Find("Player(Clone)").GetComponent<PlayerMouvement>().enabled = false;
                }

                if (GUILayout.Button("Jouer en localhost"))
                {
                    manager.networkAddress = "localhost";
                    manager.StartClient();
                    //GameObject teamA = GameObject.Find("TeamA");
                    teamA.SetActive(true);
                    //GameObject teamB = GameObject.Find("TeamB");
                    teamB.SetActive(true);

                    //GameObject.Find("Player(Clone)").GetComponent<PlayerMouvement>().enabled = false;
                }

                /*
                // Client + IP
                GUILayout.BeginHorizontal();

                GUI.BeginGroup (new Rect (300, 300, 200, 200));
                if (GUI.Button(new Rect(0,0,100,100),"Jouer en localhost"))
                {
                    manager.networkAddress = "localhost";
                    manager.StartClient();
                }
                //GUILayout.TextField("13.36.61.82"); //Test
                GUI.EndGroup();



                //GUILayout.EndHorizontal();

                /*
                // Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }*/
            }
            else
            {
                // Connecting
                GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }

        void StatusLabels()
        {
            // server / client status message
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
            }
            if (NetworkClient.isConnected)
            {
                GUILayout.Label("Client: address=" + manager.networkAddress);
            }
        }

        void StopButtons()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Host"))
                {
                    manager.StopHost();
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    manager.StopClient();
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    manager.StopServer();
                }
            }
        }
    }
}
