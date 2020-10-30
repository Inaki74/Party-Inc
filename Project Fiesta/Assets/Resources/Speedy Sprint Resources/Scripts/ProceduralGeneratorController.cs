using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace SS
    {
        /// <summary>
        /// Responsible for Generating the games tiles.
        /// Obstacles will be generated for each lane available along with its tiles (this simulates infinity).
        /// First we place the tiles -> Then we place obstacles if there is one, this obstacle is placed at a certain percentage of the tile (30%-70% of the planes length) and is managed by each tile.
        /// We see roughly 13 tiles at one time. With movement, that can amount to 15 tiles. So there should be 20 tiles already loaded. Plus three tiles that will be unloaded once they
        /// reach the end of the line (to save memory, TilePool? ObstaclePool?).
        /// </summary>
        public class ProceduralGeneratorController : MonoBehaviour
        {
            private Tile[,] _tiles; // 20 4

            // Start is called before the first frame update
            void Start()
            {
                _tiles = new Tile[20,GameManager.Current.playerCount];

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}


