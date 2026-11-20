using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: barra de cooldown para el laser o el campo AT.
    /// Enlaza _laser O _atField (solo uno por barra) y pinta el progreso
    /// de recarga en una Image en modo Filled.
    /// </summary>
    public class CooldownBarUI : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        [Header("Referencias (solo una de las dos)")]
        [SerializeField] private EvaLaser _laser;
        [SerializeField] private ATField _atField;

        [Header("Visual")]
        [SerializeField] private Image _fill;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Color _readyColor = new Color(0.3f, 1f, 0.4f, 1f);
        [SerializeField] private Color _loadingColor = new Color(1f, 0.6f, 0.2f, 1f);
        #endregion

        #region Unity Callbacks
        void Update()
        {
            if (_fill == null)
            {
                return;
            }

            float progreso;
            string nombre;

            if (_laser != null)
            {
                progreso = _laser.GetReadyProgress();
                nombre = "Laser";
            }
            else if (_atField != null)
            {
                progreso = _atField.GetReadyProgress();
                nombre = "AT";
            }
            else
            {
                _fill.fillAmount = 0f;
                return;
            }

            bool listo = progreso >= 1f;
            _fill.fillAmount = Mathf.Clamp01(progreso);
            _fill.color = listo ? _readyColor : _loadingColor;

            if (_label != null)
            {
                _label.text = listo ? nombre + " LISTO" : nombre;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
