using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: barra de vida generica para cualquier Health.
    /// Se adjunta al Canvas y se enlaza a un Slider + el Health que toque
    /// (en esta practica, al Health del Angel).
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private Slider _slider;
        [SerializeField] private Health _health;

        [Header("Etiqueta (opcional)")]
        [SerializeField] private TMP_Text _label;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_health != null)
            {
                _health.onHealthChanged += UpdateLive;
                UpdateLive(_health.CurrentHealth, _health.MaxHealth);
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.onHealthChanged -= UpdateLive;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>Actualiza la barra: (vidaActual, vidaMaxima).</summary>
        public void UpdateLive(int current, int max)
        {
            if (_slider == null)
            {
                return;
            }

            _slider.maxValue = max;
            _slider.value = current;

            if (_label != null)
            {
                _label.text = current + " / " + max;
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
