using System;
using System.Collections;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: campo AT del EVA (barra espaciadora).
    /// Escudo temporal que bloquea los impactos del Angel mientras esta activo.
    /// Se adjunta al objeto Hero. Lanza onATActivated y onATBlocked.
    /// </summary>
    public class ATField : MonoBehaviour
    {
        #region Properties
        /// <summary>True mientras el escudo esta activo.</summary>
        public bool IsActive
        {
            get { return Time.time < _activeUntil; }
        }

        /// <summary>True cuando el campo puede volverse a activar.</summary>
        public bool IsReady
        {
            get { return Time.time >= _readyTime; }
        }
        #endregion

        #region Fields
        [Header("Visual")]
        [SerializeField] private Sprite _shieldSprite;
        [SerializeField] private Vector2 _offset = new Vector2(0f, 1.2f);
        [SerializeField] private float _size = 3.2f;

        [Header("Reglas")]
        [SerializeField] private float _durationSeconds = 1f;
        [SerializeField] private float _cooldownSeconds = 3f;

        private SpriteRenderer _shield;
        private float _activeUntil;
        private float _readyTime;
        private Coroutine _expireRoutine;
        #endregion

        #region Events
        /// <summary>Se lanza al activar el campo.</summary>
        public event Action onATActivated;

        /// <summary>Se lanza cuando el campo bloquea un impacto.</summary>
        public event Action onATBlocked;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            CreateShield();
        }
        #endregion

        #region Public Methods
        /// <summary>Activa el campo AT si no esta activo ni en cooldown.</summary>
        public void TryActivate()
        {
            if (IsActive || !IsReady)
            {
                Debug.Log("[ATField] Campo AT no disponible todavia");
                return;
            }

            _activeUntil = Time.time + _durationSeconds;
            _readyTime = _activeUntil + _cooldownSeconds;
            _shield.enabled = true;

            if (_expireRoutine != null)
            {
                StopCoroutine(_expireRoutine);
            }

            _expireRoutine = StartCoroutine(ExpireRoutine());
            onATActivated?.Invoke();
            Debug.Log("[ATField] Campo AT activo durante " + _durationSeconds + " s");
        }

        /// <summary>Progreso de recarga de 0 a 1 (1 = listo). Lo usan las barras de UI.</summary>
        public float GetReadyProgress()
        {
            if (IsReady)
            {
                return 1f;
            }

            float restante = _readyTime - Time.time;
            return Mathf.Clamp01(1f - restante / Mathf.Max(0.01f, _cooldownSeconds));
        }

        /// <summary>Intenta bloquear un impacto. True si el campo estaba activo.</summary>
        public bool TryBlock()
        {
            if (!IsActive)
            {
                return false;
            }

            StartCoroutine(BlockFlashRoutine());
            onATBlocked?.Invoke();
            Debug.Log("[ATField] ¡Impacto bloqueado por el campo AT!");
            return true;
        }
        #endregion

        #region Private Methods
        private void CreateShield()
        {
            GameObject go = new GameObject("ATShieldVisual");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = _offset;
            _shield = go.AddComponent<SpriteRenderer>();
            _shield.sprite = _shieldSprite;
            _shield.color = new Color(1f, 0.7f, 0.2f, 0.55f);
            _shield.sortingOrder = 15;
            _shield.enabled = false;

            if (_shieldSprite != null)
            {
                float ancho = Mathf.Max(0.01f, _shieldSprite.bounds.size.x);
                float escala = _size / ancho;
                go.transform.localScale = new Vector3(escala, escala, 1f);
            }
        }

        private IEnumerator ExpireRoutine()
        {
            yield return new WaitForSeconds(_durationSeconds);
            _shield.enabled = false;
        }

        private IEnumerator BlockFlashRoutine()
        {
            _shield.color = new Color(1f, 1f, 0.6f, 0.9f);
            yield return new WaitForSeconds(0.12f);
            _shield.color = new Color(1f, 0.7f, 0.2f, 0.55f);
        }
        #endregion
    }
}
