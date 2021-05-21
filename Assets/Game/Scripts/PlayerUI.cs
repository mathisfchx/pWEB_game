using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Game
{
    public class PlayerUI : NetworkBehaviour
    {
        [Header("Player Components")]
        public Image image;

        [Header("Child Text Objects")]
        public Text playerNameText;

        Player player;

        /// <summary>
        /// Caches the controlling Player object, subscribes to its events
        /// </summary>
        /// <param name="player">Player object that controls this UI</param>
        /// <param name="isLocalPlayer">true if the Player object is the Local Player</param>
        public void SetPlayer(Player player, bool isLocalPlayer)
        {
            // cache reference to the player that controls this UI object
            this.player = player;

            // subscribe to the events raised by SyncVar Hooks on the Player object
            player.OnPlayerNumberChanged += OnPlayerNumberChanged;
            player.OnPlayerColorChanged += OnPlayerColorChanged;

            // add a visual background for the local player in the UI
            if (isLocalPlayer)
                image.color = new Color(1f, 1f, 1f, 0.1f);
        }

        void OnDisable()
        {
            player.OnPlayerNumberChanged -= OnPlayerNumberChanged;
            player.OnPlayerColorChanged -= OnPlayerColorChanged;
        }

        // This value can change as clients leave and join
        void OnPlayerNumberChanged(int newPlayerNumber)
        {
            playerNameText.text = player.Username;
        }

        // Random color set by Player::OnStartServer
        void OnPlayerColorChanged(Color32 newPlayerColor)
        {
            playerNameText.color = newPlayerColor;
        }

    }
}
