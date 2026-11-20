using System;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: inventario de pociones limitadas.
    /// Se adjunta al objeto Hero. La D consume una pocion; cuando se acaban,
    /// no hay curacion. Lanza onPotionsChanged para que la UI se pinte.
    /// </summary>
    public class PotionInventory : MonoBehaviour
    {
        #region Properties
        /// <summary>Pociones que quedan.</summary>
        public int Remaining
        {
            get { return _remaining; }
        }
        #endregion

        #region Fields
        [Header("Inventario")]
        [SerializeField, Min(0)] private int _potions = 2;

        private int _remaining;
        #endregion

        #region Events
        /// <summary>Se lanza al gastar una pocion: (pociones restantes).</summary>
        public event Action<int> onPotionsChanged;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            _remaining = _potions;
        }
        #endregion

        #region Public Methods
        /// <summary>Intenta gastar una pocion. False si no quedan.</summary>
        public bool TryUse()
        {
            if (_remaining <= 0)
            {
                return false;
            }

            _remaining--;
            onPotionsChanged?.Invoke(_remaining);
            Debug.Log("[PotionInventory] Pocion usada. Quedan " + _remaining);
            return true;
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
