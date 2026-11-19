using System;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Registro de puntos y NIVEL del jugador de MiniSlayers (parte nueva del enunciado 5.3.5).
    /// Se adjunta al objeto "Hero". Contiene la propiedad CurrentLevel,
    /// el metodo publico AddLevel y el evento correspondiente onLevelUp.
    /// </summary>
    public class Points : MonoBehaviour
    {
        #region Properties
        /// <summary>Puntos actuales del jugador.</summary>
        public int CurrentPoints { get; private set; }

        /// <summary>Nivel actual del jugador (control de nivel del enunciado).</summary>
        public int CurrentLevel { get; private set; }
        #endregion

        #region Fields
        [Header("Puntos")]
        [SerializeField, Min(0)] private int _startPoints = 0;

        [Header("Nivel")]
        [SerializeField, Min(1)] private int _startLevel = 1;

        [Header("Seguridad de desbordamiento")]
        [SerializeField] private int _maxSafePoints = 32000000;
        #endregion

        #region Events
        /// <summary>Se lanza al ganar puntos: (puntos ganados). Nombre del PDF: onGetPoints.</summary>
        public event Action<int> onGetPoints;

        /// <summary>Se lanza al cambiar el total de puntos: (total actual).</summary>
        public event Action<int> onPointsChanged;

        /// <summary>Se lanza al subir de nivel: (nivel nuevo). Evento correspondiente al metodo AddLevel.</summary>
        public event Action<int> onLevelUp;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            // Awake se ejecuta antes que todos los Start: asi la UI puede
            // leer los puntos y el nivel inicial sin problemas de orden.
            CurrentPoints = _startPoints;
            CurrentLevel = _startLevel;
        }
        #endregion

        #region Public Methods
        /// <summary>Anade puntos (metodo addPoints del PDF). Con amount = 25 en la practica.</summary>
        public void AddPoints(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            // Control de desbordamiento: un int solo soporta hasta 2.147.483.647.
            // El PDF avisa de tener cuidado con valores que superen ~32 millones.
            if (CurrentPoints + amount > _maxSafePoints || CurrentPoints + amount < 0)
            {
                Debug.LogWarning("[Points] Los puntos superarian el limite seguro, no se suman.");
                return;
            }

            CurrentPoints += amount;
            Debug.Log($"[Points] +{amount} puntos. Total: {CurrentPoints}");

            onGetPoints?.Invoke(amount);
            onPointsChanged?.Invoke(CurrentPoints);
        }

        /// <summary>Metodo publico de subida de nivel (parte nueva del enunciado).</summary>
        public void AddLevel()
        {
            CurrentLevel++;
            Debug.Log($"[Points] Nivel nuevo: {CurrentLevel}");

            onLevelUp?.Invoke(CurrentLevel);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
