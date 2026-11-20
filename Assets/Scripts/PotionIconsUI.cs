using UnityEngine;
using UnityEngine.UI;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: pinta los iconos de pocion como emblemas
    /// EVA-01 (verde = disponible, rojo = gastada). Cambia el SPRITE de los
    /// iconos segun las pociones restantes.
    /// Se adjunta al Canvas: enlaza el PotionInventory del Hero y los Images.
    /// </summary>
    public class PotionIconsUI : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private PotionInventory _potions;

        [Header("Iconos (en orden de gasto)")]
        [SerializeField] private Image[] _icons;

        [Header("Sprites")]
        [SerializeField] private Sprite _fullSprite;
        [SerializeField] private Sprite _usedSprite;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_potions != null)
            {
                _potions.onPotionsChanged += HandlePotionsChanged;
            }

            Paint(_potions != null ? _potions.Remaining : 0);
        }

        void OnDestroy()
        {
            if (_potions != null)
            {
                _potions.onPotionsChanged -= HandlePotionsChanged;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void HandlePotionsChanged(int remaining)
        {
            Paint(remaining);
        }

        private void Paint(int remaining)
        {
            if (_icons == null)
            {
                return;
            }

            for (int i = 0; i < _icons.Length; i++)
            {
                if (_icons[i] == null)
                {
                    continue;
                }

                _icons[i].sprite = i < remaining ? _fullSprite : _usedSprite;
            }
        }
        #endregion
    }
}
