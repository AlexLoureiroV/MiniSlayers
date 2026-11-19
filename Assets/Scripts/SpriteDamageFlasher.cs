using System.Collections;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra de MiniSlayers: tiñe el SpriteRenderer de este objeto de rojo
    /// un instante cuando su Health recibe dano (onDamaged).
    /// Pensado para el Angel; reutilizable en cualquier sprite con Health.
    /// </summary>
    public class SpriteDamageFlasher : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Ajustes del flash")]
        [SerializeField] private Color _flashColor = new Color(1f, 0.35f, 0.35f, 1f);
        [SerializeField] private float _seconds = 0.1f;

        private Color _originalColor;
        private Coroutine _routine;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_health == null)
            {
                _health = GetComponent<Health>();
            }

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }

            if (_health != null)
            {
                _health.onDamaged += FlashRed;
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.onDamaged -= FlashRed;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>Lanza el flash rojo (tambien invocable desde otros modulos).</summary>
        public void FlashRed()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(FlashRoutine());
        }
        #endregion

        #region Private Methods
        private IEnumerator FlashRoutine()
        {
            _spriteRenderer.color = _flashColor;
            yield return new WaitForSeconds(_seconds);
            _spriteRenderer.color = _originalColor;
        }
        #endregion
    }
}
