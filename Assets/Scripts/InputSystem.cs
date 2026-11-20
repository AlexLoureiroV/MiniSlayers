using System;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// LANZADOR de eventos de MiniSlayers.
    /// Detecta las 4 teclas de combate y lanza un evento por cada una.
    /// No conoce a ningun otro modulo: solo emite (desacoplamiento, Modulo 5 - 5.3.5).
    ///     A -> OnKeyDamage | D -> OnKeyHeal | K -> OnKeyPoints | L -> OnKeyAddLevel
    /// </summary>
    public class InputSystem : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Fields
        #endregion

        #region Events
        /// <summary>Tecla A: el enemigo golpea al heroe.</summary>
        public event Action OnKeyDamage;

        /// <summary>Tecla D: el heroe bebe una pocion de curacion.</summary>
        public event Action OnKeyHeal;

        /// <summary>Tecla K: el heroe elimina a un enemigo (puntos).</summary>
        public event Action OnKeyPoints;

        /// <summary>Tecla L: el heroe sube de nivel.</summary>
        public event Action OnKeyAddLevel;

        /// <summary>Tecla J: el heroe dispara su laser (v2, cooldown en EvaLaser).</summary>
        public event Action OnKeyLaser;

        /// <summary>Barra espaciadora: activa el campo AT (v2).</summary>
        public event Action OnKeyAT;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("[InputSystem] Tecla A pulsada -> OnKeyDamage");
                OnKeyDamage?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("[InputSystem] Tecla D pulsada -> OnKeyHeal");
                OnKeyHeal?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("[InputSystem] Tecla K pulsada -> OnKeyPoints");
                OnKeyPoints?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("[InputSystem] Tecla L pulsada -> OnKeyAddLevel");
                OnKeyAddLevel?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("[InputSystem] Tecla J pulsada -> OnKeyLaser");
                OnKeyLaser?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("[InputSystem] Espaciadora pulsada -> OnKeyAT");
                OnKeyAT?.Invoke();
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
