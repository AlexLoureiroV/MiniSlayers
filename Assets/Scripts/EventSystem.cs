using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniSlayers
{
    /// <summary>
    /// NUCLEO que conecta todo en MiniSlayers (EventSystem del enunciado 5.3.5).
    /// En su Start anade los listeners de los 4 eventos del InputSystem y
    /// ejecuta los metodos de Dano, Curacion, Puntos y Nivel de Health y Points.
    /// Tambien escucha onDeath (derrota) y onLevelUp (victoria al nivel 5).
    /// </summary>
    public class EventSystem : MonoBehaviour
    {
        #region Properties
        /// <summary>Bloquea la entrada del juego mientras dura la intro,
        /// para que la tecla que salta la intro no dispare acciones de juego.</summary>
        private bool InputBlocked
        {
            get { return _intro != null && _intro.IsGameplayBlocked; }
        }
        #endregion

        #region Fields
        [Header("Referencia al lanzador de eventos")]
        [SerializeField] private InputSystem _inputs;

        [Header("Logica de juego")]
        [SerializeField] private Health _health;
        [SerializeField] private Points _points;
        [SerializeField] private Health _enemyHealth;
        [SerializeField] private PotionInventory _potions;

        [Header("Modulos de presentacion")]
        [SerializeField] private UIController _ui;
        [SerializeField] private SoundController _sound;

        [Header("Intro")]
        [SerializeField] private IntroController _intro;

        [Header("Actores de la arena")]
        [SerializeField] private GameObject _hero;
        [SerializeField] private GameObject _enemy;

        [Header("Reglas de MiniSlayers")]
        [SerializeField] private int _damageAmount = 10;
        [SerializeField] private int _healAmount = 15;
        [SerializeField] private int _pointsPerKill = 25;
        [SerializeField] private int _damagePerKill = 25;
        [SerializeField] private int _victoryLevel = 5;

        [Header("Mensajes de fin de partida")]
        [SerializeField] private string _victoryMessage = "¡VICTORIA!";
        [SerializeField] private string _defeatMessage = "DERROTA";

        [Header("Reinicio")]
        [SerializeField] private KeyCode _restartKey = KeyCode.R;

        [Header("Combate real (v2)")]
        [SerializeField] private EvaLaser _laser;
        [SerializeField] private ATField _atField;
        [SerializeField] private AngelAI _angelAI;
        [SerializeField] private int _laserDamage = 20;
        [SerializeField] private int _angelFireDamage = 20;

        private bool _gameOver;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // Listeners de los 4 eventos del InputSystem (requisito del enunciado).
            if (_inputs != null)
            {
                _inputs.OnKeyDamage += HandleDamage;
                _inputs.OnKeyHeal += HandleHeal;
                _inputs.OnKeyPoints += HandlePoints;
                _inputs.OnKeyAddLevel += HandleAddLevel;
                _inputs.OnKeyLaser += HandleLaser;
                _inputs.OnKeyAT += HandleAT;
            }

            if (_angelAI != null)
            {
                _angelAI.onFire += HandleAngelFire;
                _angelAI.onChargeStart += HandleChargeStart;
            }

            // Listeners de los eventos internos de la logica.
            if (_health != null)
            {
                _health.onDeath += HandleDeath;
            }

            if (_points != null)
            {
                _points.onLevelUp += HandleLevelUp;
            }

            if (_enemyHealth != null)
            {
                _enemyHealth.onDeath += HandleEnemyDeath;
            }

            Debug.Log("[EventSystem] Listeners registrados. Pulsa A / D / K / L para jugar.");
        }

        void Update()
        {
            // Reinicio: con la partida terminada, R recarga la escena.
            if (_gameOver && !InputBlocked && Input.GetKeyDown(_restartKey))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void OnDestroy()
        {
            // Buena practica: desuscribirse para evitar referencias colgantes.
            if (_inputs != null)
            {
                _inputs.OnKeyDamage -= HandleDamage;
                _inputs.OnKeyHeal -= HandleHeal;
                _inputs.OnKeyPoints -= HandlePoints;
                _inputs.OnKeyAddLevel -= HandleAddLevel;
                _inputs.OnKeyLaser -= HandleLaser;
                _inputs.OnKeyAT -= HandleAT;
            }

            if (_health != null)
            {
                _health.onDeath -= HandleDeath;
            }

            if (_points != null)
            {
                _points.onLevelUp -= HandleLevelUp;
            }

            if (_enemyHealth != null)
            {
                _enemyHealth.onDeath -= HandleEnemyDeath;
            }

            if (_angelAI != null)
            {
                _angelAI.onFire -= HandleAngelFire;
                _angelAI.onChargeStart -= HandleChargeStart;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>OnKeyDamage -> Daño (tecla A).</summary>
        private void HandleDamage()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _health?.Damage(_damageAmount);
        }

        /// <summary>OnKeyHeal -> Curación (tecla D).</summary>
        private void HandleHeal()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            if (_health != null && _health.IsDead)
            {
                return;
            }

            if (_potions != null && !_potions.TryUse())
            {
                Debug.Log("[EventSystem] Sin pociones: la curacion esta agotada");
                return;
            }

            _health?.Heal(_healAmount);
        }

        /// <summary>OnKeyPoints -> Puntos (tecla K).</summary>
        private void HandlePoints()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _points?.AddPoints(_pointsPerKill);

            if (_angelAI != null && !_angelAI.IsCharging)
            {
                Debug.Log("[EventSystem] El Angel es invulnerable mientras no carga");
                _angelAI.BlinkInvulnerable();
                return;
            }

            _enemyHealth?.Damage(_damagePerKill);
        }

        /// <summary>OnKeyAddLevel -> Nivel (tecla L).</summary>
        private void HandleAddLevel()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _points?.AddLevel();
        }

        /// <summary>OnKeyLaser -> el heroe dispara (v2). Solo dana mientras el Angel carga.</summary>
        private void HandleLaser()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            if (_laser == null || !_laser.TryFire())
            {
                return;
            }

            if (_sound != null)
            {
                _sound.playLaser();
            }

            if (_angelAI != null && _angelAI.IsCharging)
            {
                Debug.Log("[EventSystem] Impacto directo en el Angel mientras carga");
                _enemyHealth?.Damage(_laserDamage);
            }
            else if (_angelAI != null)
            {
                Debug.Log("[EventSystem] El Angel es invulnerable fuera de la carga");
                _angelAI.BlinkInvulnerable();
            }
            else
            {
                _enemyHealth?.Damage(_laserDamage);
            }
        }

        /// <summary>OnKeyAT -> activa el campo AT (v2).</summary>
        private void HandleAT()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _atField?.TryActivate();
        }

        /// <summary>El Angel dispara (v2): el campo AT puede bloquear el impacto.</summary>
        private void HandleAngelFire()
        {
            if (_gameOver)
            {
                return;
            }

            if (_atField != null && _atField.TryBlock())
            {
                if (_sound != null)
                {
                    _sound.playBlock();
                }

                return;
            }

            Debug.Log("[EventSystem] Impacto del Angel sobre el heroe");
            _health?.Damage(_angelFireDamage);
        }

        /// <summary>El Angel empieza a cargar (v2): momento vulnerable + sonido.</summary>
        private void HandleChargeStart()
        {
            if (_sound != null)
            {
                _sound.playCharge();
            }
        }

        /// <summary>La vida llega a 0: sonido de muerte, pantalla de derrota y Destroy del heroe.</summary>
        private void HandleDeath()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _gameOver = true;

            if (_angelAI != null)
            {
                _angelAI.StopCombat();
            }

            Debug.Log("[EventSystem] Derrota: el heroe ha caido");

            if (_sound != null)
            {
                _sound.playDie();
            }

            if (_ui != null)
            {
                _ui.ShowGameOver(_defeatMessage, false);
            }

            if (_hero != null)
            {
                Destroy(_hero);
            }
        }

        /// <summary>El enemigo acumula 100 de dano: Destroy + victoria.</summary>
        private void HandleEnemyDeath()
        {
            if (_gameOver || InputBlocked)
            {
                return;
            }

            _gameOver = true;
            Debug.Log("[EventSystem] ¡Victoria! El enemigo ha sido derrotado");

            if (_ui != null)
            {
                _ui.ShowGameOver(_victoryMessage, true);
            }

            if (_enemy != null)
            {
                Destroy(_enemy);
            }
        }

        /// <summary>onLevelUp: al alcanzar el nivel de victoria, el jefe cae (Destroy).</summary>
        private void HandleLevelUp(int newLevel)
        {
            if (_gameOver || newLevel < _victoryLevel)
            {
                return;
            }

            _gameOver = true;
            Debug.Log($"[EventSystem] ¡Victoria! Nivel {newLevel}: el jefe de la arena cae");

            if (_enemy != null)
            {
                Destroy(_enemy);
            }

            if (_ui != null)
            {
                _ui.ShowGameOver("¡VICTORIA!", true);
            }
        }
        #endregion
    }
}
