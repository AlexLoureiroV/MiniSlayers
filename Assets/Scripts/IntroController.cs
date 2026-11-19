using System.Collections;
using TMPro;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra de MiniSlayers: tarjeta de titulo de intro.
    /// Muestra el panel de intro, espera un minimo de segundos y, al pulsar
    /// cualquier tecla, hace un fade suave hacia el juego.
    /// El juego queda en pausa (Time.timeScale = 0) hasta que termina el fade.
    /// IMPORTANTE: el script va en un objeto siempre activo (el Canvas);
    /// el panel de intro se desactiva al final, como HelpPanelController.
    /// Mover este archivo a F:\MUSK\MiniSlayers\Assets\Scripts\IntroController.cs
    /// </summary>
    public class IntroController : MonoBehaviour
    {
        #region Properties
        /// <summary>True mientras la intro sigue en pantalla.</summary>
        public bool IsIntroActive => _introGroup != null && _introGroup.gameObject.activeSelf;

        /// <summary>Bloqueo total del juego: durante la intro O la cuenta atras.</summary>
        public bool IsGameplayBlocked
        {
            get { return IsIntroActive || _countdownRunning; }
        }
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private CanvasGroup _introGroup;

        [Header("Ajustes de la intro")]
        [SerializeField] private float _minSeconds = 2f;
        [SerializeField] private float _fadeSeconds = 0.8f;
        [SerializeField] private KeyCode _skipKey = KeyCode.None;

        [Header("Press any button")]
        [SerializeField] private TMP_Text _pressAnyText;
        [SerializeField] private float _blinkSeconds = 0.5f;

        [Header("Cuenta atras")]
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private float _countdownStepSeconds = 0.8f;

        private bool _canSkip;
        private bool _countdownRunning;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_introGroup == null)
            {
                return;
            }

            _introGroup.alpha = 1f;
            _introGroup.interactable = false;
            _introGroup.blocksRaycasts = false;
            _introGroup.gameObject.SetActive(true);

            // Pausa el juego mientras dura la intro.
            Time.timeScale = 0f;
            _canSkip = false;

            if (_pressAnyText != null)
            {
                _pressAnyText.gameObject.SetActive(false);
            }

            if (_countdownText != null)
            {
                _countdownText.gameObject.SetActive(false);
            }

            StartCoroutine(MinSecondsRoutine());
        }

        // Update is called once per frame
        void Update()
        {
            if (_introGroup == null || !_introGroup.gameObject.activeSelf)
            {
                return;
            }

            bool pulsado = _skipKey != KeyCode.None
                ? Input.GetKeyDown(_skipKey)
                : Input.anyKeyDown;

            if (_canSkip && pulsado)
            {
                StartCoroutine(FadeOutRoutine());
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private IEnumerator MinSecondsRoutine()
        {
            yield return new WaitForSecondsRealtime(_minSeconds);
            _canSkip = true;

            if (_pressAnyText != null)
            {
                _pressAnyText.gameObject.SetActive(true);
                StartCoroutine(BlinkRoutine());
            }
        }

        private IEnumerator BlinkRoutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(_blinkSeconds);
                _pressAnyText.enabled = !_pressAnyText.enabled;
            }
        }

        private IEnumerator FadeOutRoutine()
        {
            // La cuenta atras arranca YA: se ve sobre los personajes
            // mientras el fade va revelando la escena.
            if (_countdownText != null)
            {
                _countdownRunning = true;
                _countdownText.gameObject.SetActive(true);
                StartCoroutine(CountdownRoutine());
            }

            float t = 0f;
            while (t < _fadeSeconds)
            {
                t += Time.unscaledDeltaTime;
                _introGroup.alpha = 1f - Mathf.Clamp01(t / _fadeSeconds);
                yield return null;
            }

            _introGroup.alpha = 0f;
            _introGroup.gameObject.SetActive(false);

            if (_countdownText == null)
            {
                // Sin cuenta atras configurada: reanuda el juego al terminar el fade.
                Time.timeScale = 1f;
            }
        }

        private IEnumerator CountdownRoutine()
        {
            int segundos = 3;
            while (segundos > 0)
            {
                _countdownText.text = segundos.ToString();
                segundos--;
                yield return new WaitForSecondsRealtime(_countdownStepSeconds);
            }

            _countdownText.text = "¡YA!";
            yield return new WaitForSecondsRealtime(_countdownStepSeconds * 0.6f);

            _countdownText.gameObject.SetActive(false);
            _countdownRunning = false;

            // Reanuda el juego.
            Time.timeScale = 1f;
        }
        #endregion
    }
}
