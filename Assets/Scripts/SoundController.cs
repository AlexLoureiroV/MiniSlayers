using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Controla el audio de MiniSlayers (SoundController del PDF 5.3.5).
    /// Propiedades damageSound y dieSound, metodos playDamage y playDie
    /// (requisitos literales del enunciado) y los extras coherentes:
    /// playHeal, playPoint y playLevelUp.
    /// </summary>
    public class SoundController : MonoBehaviour
    {
        #region Properties
        /// <summary>Sonido al recibir dano (propiedad del PDF).</summary>
        public AudioClip damageSound => _damageSound;

        /// <summary>Sonido al morir (propiedad del PDF).</summary>
        public AudioClip dieSound => _dieSound;
        #endregion

        #region Fields
        [Header("Sonidos de combate (AudioClips)")]
        [SerializeField] private AudioClip _damageSound;
        [SerializeField] private AudioClip _dieSound;
        [SerializeField] private AudioClip _healSound;
        [SerializeField] private AudioClip _pointSound;
        [SerializeField] private AudioClip _levelUpSound;

        [Header("Sonidos de combate real (v2)")]
        [SerializeField] private AudioClip _laserSound;
        [SerializeField] private AudioClip _chargeSound;
        [SerializeField] private AudioClip _blockSound;

        [Header("Escuchadores de eventos")]
        [SerializeField] private Health _health;
        [SerializeField] private Points _points;

        private AudioSource _source;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            // Nos aseguramos de tener un AudioSource propio en este objeto.
            _source = GetComponent<AudioSource>();

            if (_source == null)
            {
                _source = gameObject.AddComponent<AudioSource>();
            }

            _source.playOnAwake = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Un mismo evento desencadena varias acciones: UI (UIController)
            // y sonido (este script) escuchan a la vez a Health y Points.
            if (_health != null)
            {
                _health.onDamaged += playDamage;
                _health.onHealed += playHeal;
            }

            if (_points != null)
            {
                _points.onGetPoints += playPoint;
                _points.onLevelUp += playLevelUp;
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.onDamaged -= playDamage;
                _health.onHealed -= playHeal;
            }

            if (_points != null)
            {
                _points.onGetPoints -= playPoint;
                _points.onLevelUp -= playLevelUp;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>Sonido de dano (metodo del PDF).</summary>
        public void playDamage()
        {
            PlayOneShotSafe(_damageSound, "playDamage");
        }

        /// <summary>Sonido de muerte (metodo del PDF). Lo llama el EventSystem con onDeath.</summary>
        public void playDie()
        {
            PlayOneShotSafe(_dieSound, "playDie");
        }

        /// <summary>Sonido de curacion (extra).</summary>
        public void playHeal()
        {
            PlayOneShotSafe(_healSound, "playHeal");
        }

        /// <summary>Sonido de puntos (extra). Firma con int para casar con Action<int> de onGetPoints.</summary>
        public void playPoint(int amount)
        {
            PlayOneShotSafe(_pointSound, "playPoint (" + amount + " pts)");
        }

        /// <summary>Sonido de subida de nivel (extra). Firma con int para casar con Action<int> de onLevelUp.</summary>
        public void playLevelUp(int newLevel)
        {
            PlayOneShotSafe(_levelUpSound, "playLevelUp (nivel " + newLevel + ")");
        }

        /// <summary>Sonido del laser del heroe (v2).</summary>
        public void playLaser()
        {
            PlayOneShotSafe(_laserSound, "playLaser");
        }

        /// <summary>Sonido de carga del Angel (v2).</summary>
        public void playCharge()
        {
            PlayOneShotSafe(_chargeSound, "playCharge");
        }

        /// <summary>Sonido del campo AT bloqueando (v2).</summary>
        public void playBlock()
        {
            PlayOneShotSafe(_blockSound, "playBlock");
        }
        #endregion

        #region Private Methods
        private void PlayOneShotSafe(AudioClip clip, string origin)
        {
            if (clip == null || _source == null)
            {
                Debug.LogWarning($"[SoundController] {origin}: no hay AudioClip asignado");
                return;
            }

            _source.PlayOneShot(clip);
        }
        #endregion
    }
}
