using System;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Logica de vida del heroe de MiniSlayers.
    /// Se adjunta al objeto "Hero". Emite eventos cuando la vida cambia,
    /// se recibe dano, se cura o el heroe muere, para que UI y sonido reaccionen.
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Properties
        /// <summary>Vida actual del heroe.</summary>
        public int CurrentHealth { get; private set; }

        /// <summary>Vida maxima del heroe.</summary>
        public int MaxHealth => _maxHealth;

        /// <summary>True cuando la vida ha llegado a 0.</summary>
        public bool IsDead => CurrentHealth <= 0;
        #endregion

        #region Fields
        [Header("Vida del heroe")]
        [SerializeField, Min(1)] private int _maxHealth = 100;
        #endregion

        #region Events
        /// <summary>Se lanza cada vez que cambia la vida: (vidaActual, vidaMaxima).</summary>
        public event Action<int, int> onHealthChanged;

        /// <summary>Se lanza justo despues de recibir dano.</summary>
        public event Action onDamaged;

        /// <summary>Se lanza justo despues de curarse.</summary>
        public event Action onHealed;

        /// <summary>Se lanza una unica vez cuando la vida llega a 0.</summary>
        public event Action onDeath;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            // Awake se ejecuta antes que todos los Start: asi la UI puede
            // leer la vida inicial sin problemas de orden de ejecucion.
            CurrentHealth = _maxHealth;
        }
        #endregion

        #region Public Methods
        /// <summary>El heroe recibe dano. Con amount = 10 en la practica.</summary>
        public void Damage(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            Debug.Log($"[Health] Dano recibido: -{amount}. Vida: {CurrentHealth}/{_maxHealth}");

            onHealthChanged?.Invoke(CurrentHealth, _maxHealth);
            onDamaged?.Invoke();

            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        /// <summary>El heroe se cura. Con amount = 15 en la practica.</summary>
        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            int healed = Mathf.Min(amount, _maxHealth - CurrentHealth);
            CurrentHealth += healed;
            Debug.Log($"[Health] Curacion: +{healed}. Vida: {CurrentHealth}/{_maxHealth}");

            onHealthChanged?.Invoke(CurrentHealth, _maxHealth);
            onHealed?.Invoke();
        }
        #endregion

        #region Private Methods
        /// <summary>Vida a 0: lanza onDeath una unica vez.</summary>
        private void Die()
        {
            if (onDeath == null)
            {
                return;
            }

            Debug.Log("[Health] El heroe ha muerto");
            onDeath?.Invoke();
        }
        #endregion
    }
}
