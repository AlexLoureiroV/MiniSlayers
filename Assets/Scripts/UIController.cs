using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniSlayers
{
    /// <summary>
    /// Maneja la interfaz de usuario de MiniSlayers (UIController del enunciado 5.3.5).
    /// Incluye el slider de vida (updateLive), el texto de puntos (updatePoints,
    /// con ToString y alineado a la derecha) y el nuevo _levelText con su
    /// metodo publico UpdateLevel (requisito nuevo del enunciado).
    /// Usa TextMeshPro (TmPro) tal y como recomienda el PDF.
    /// Extra: texto de sabor aleatorio en el fin de partida (_flavorText).
    /// </summary>
    public class UIController : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        [Header("Vida")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Image _damageFlash;

        [Header("Textos TMP")]
        [SerializeField] private TMP_Text _pointText;
        [SerializeField] private TMP_Text _levelText;

        [Header("Referencias de eventos")]
        [SerializeField] private Health _health;
        [SerializeField] private Points _points;

        [Header("Fin de partida")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TMP_Text _gameOverText;
        [SerializeField] private TMP_Text _flavorText;

        [Header("Textos de sabor (aleatorios al terminar)")]
        [SerializeField] private string[] _victoryFlavorTexts;
        [SerializeField] private string[] _defeatFlavorTexts;

        private const float FLASH_ALPHA = 0.35f;
        private const float FLASH_SECONDS = 0.1f;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // Escuchadores: un evento puede desencadenar varias acciones
            // (la UI escucha por su cuenta, sin depender del EventSystem).
            if (_health != null)
            {
                _health.onHealthChanged += UpdateLive;
                _health.onDamaged += HandleDamageFeedback;
            }

            if (_points != null)
            {
                _points.onPointsChanged += UpdatePoints;
                _points.onLevelUp += UpdateLevel;
            }

            // Pintado inicial (Health y Points ya se han inicializado en Awake).
            UpdateLive(_health != null ? _health.CurrentHealth : 0,
                       _health != null ? _health.MaxHealth : 100);
            UpdatePoints(_points != null ? _points.CurrentPoints : 0);
            UpdateLevel(_points != null ? _points.CurrentLevel : 1);

            if (_damageFlash != null)
            {
                _damageFlash.color = new Color(1f, 0f, 0f, 0f);
            }

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.onHealthChanged -= UpdateLive;
                _health.onDamaged -= HandleDamageFeedback;
            }

            if (_points != null)
            {
                _points.onPointsChanged -= UpdatePoints;
                _points.onLevelUp -= UpdateLevel;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>Actualiza el slider de vida: (vidaActual, vidaMaxima).</summary>
        public void UpdateLive(int current, int max)
        {
            if (_healthSlider == null)
            {
                return;
            }

            _healthSlider.maxValue = max;
            _healthSlider.value = current;
        }

        /// <summary>Actualiza el texto de puntos (updatePoints del PDF).
        /// Conversion de int a string con ToString y alineado a la derecha.</summary>
        public void UpdatePoints(int points)
        {
            if (_pointText == null)
            {
                return;
            }

            _pointText.text = points.ToString();
        }

        /// <summary>Metodo publico nuevo del enunciado: actualiza _levelText.</summary>
        public void UpdateLevel(int level)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.text = "Nivel " + level;
        }

        /// <summary>Muestra el panel de fin de partida (derrota o victoria)
        /// con la indicacion de reinicio con la tecla R y un texto de sabor
        /// aleatorio (victoria o derrota) en _flavorText.</summary>
        public void ShowGameOver(string message, bool victory)
        {
            if (_gameOverText != null)
            {
                _gameOverText.text = message + "\n\nPulsa R para reiniciar";
            }

            if (_flavorText != null)
            {
                string[] banco = victory ? _victoryFlavorTexts : _defeatFlavorTexts;
                if (banco != null && banco.Length > 0)
                {
                    _flavorText.text = banco[Random.Range(0, banco.Length)];
                }
                else
                {
                    _flavorText.text = "";
                }
            }

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>Flash rojo en la UI al recibir dano (extra visual).</summary>
        private void HandleDamageFeedback()
        {
            StartCoroutine(DamageFlashRoutine());
        }

        private IEnumerator DamageFlashRoutine()
        {
            if (_damageFlash == null)
            {
                yield break;
            }

            _damageFlash.color = new Color(1f, 0f, 0f, FLASH_ALPHA);
            yield return new WaitForSeconds(FLASH_SECONDS);
            _damageFlash.color = new Color(1f, 0f, 0f, 0f);
        }
        #endregion
    }
}
