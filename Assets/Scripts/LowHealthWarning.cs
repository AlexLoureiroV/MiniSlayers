using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: aviso de EMERGENCY parpadeante cuando la vida
    /// del heroe baja de un umbral (30% por defecto). Se adjunta al Canvas:
    /// enlaza la Image a pantalla completa, el Health del Hero y el clip de
    /// alarma en bucle. Se oculta si se cura por encima del umbral o si el
    /// heroe muere. El AudioSource se crea en el propio Canvas porque el
    /// objeto de la Image se desactiva (un AudioSource inactivo no suena).
    /// </summary>
    public class LowHealthWarning : MonoBehaviour
    {
        #region Properties
        /// <summary>True mientras el aviso esta parpadeando y sonando.</summary>
        public bool IsWarning
        {
            get { return _blinking; }
        }
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private Image _warningImage;
        [SerializeField] private Health _health;
        [SerializeField] private AudioClip _warningClip;

        [Header("Ajustes")]
        [SerializeField, Range(0.05f, 0.9f)] private float _threshold = 0.3f;
        [SerializeField] private float _blinkSeconds = 0.4f;
        [SerializeField, Range(0f, 1f)] private float _alarmVolume = 0.6f;

        private AudioSource _alarmSource;
        private bool _blinking;
        private Coroutine _routine;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_health != null)
            {
                _health.onHealthChanged += HandleHealthChanged;
            }

            if (_warningClip != null)
            {
                _alarmSource = gameObject.AddComponent<AudioSource>();
                _alarmSource.clip = _warningClip;
                _alarmSource.loop = true;
                _alarmSource.playOnAwake = false;
                _alarmSource.volume = _alarmVolume;
            }

            Hide();
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.onHealthChanged -= HandleHealthChanged;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void HandleHealthChanged(int current, int max)
        {
            bool muerto = _health != null && _health.IsDead;
            bool critico = !muerto && max > 0 && (float)current / max <= _threshold;

            if (critico)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            if (_blinking || _warningImage == null)
            {
                return;
            }

            _blinking = true;
            _warningImage.gameObject.SetActive(true);

            if (_alarmSource != null && !_alarmSource.isPlaying)
            {
                _alarmSource.Play();
            }

            _routine = StartCoroutine(BlinkRoutine());
            Debug.Log("[LowHealthWarning] ¡Energia critica! EMERGENCY parpadeando");
        }

        private void Hide()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _blinking = false;

            if (_warningImage != null)
            {
                _warningImage.gameObject.SetActive(false);
            }

            if (_alarmSource != null && _alarmSource.isPlaying)
            {
                _alarmSource.Stop();
            }
        }

        private IEnumerator BlinkRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_blinkSeconds);
                _warningImage.enabled = !_warningImage.enabled;
            }
        }
        #endregion
    }
}
