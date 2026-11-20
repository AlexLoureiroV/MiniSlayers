using System;
using System.Collections;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: laser del EVA-01 (tecla J).
    /// Gestion SOLO el visual y el cooldown; el dano lo decide el EventSystem.
    /// Se adjunta al objeto Hero. Lanza onLaserFired al disparar.
    /// </summary>
    public class EvaLaser : MonoBehaviour
    {
        #region Properties
        /// <summary>True cuando el laser esta listo para disparar.</summary>
        public bool IsReady
        {
            get { return Time.time >= _nextFireTime; }
        }
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private Transform _target;

        [Header("Configuracion del rayo")]
        [SerializeField] private Vector2 _muzzleOffset = new Vector2(0.6f, 1.6f);
        [SerializeField] private Color _laserColor = new Color(1f, 0.85f, 0.4f, 1f);
        [SerializeField] private float _beamWidth = 0.12f;
        [SerializeField] private float _visibleSeconds = 0.12f;

        [Header("Reglas")]
        [SerializeField] private float _cooldownSeconds = 2f;

        private LineRenderer _beam;
        private float _nextFireTime;
        #endregion

        #region Events
        /// <summary>Se lanza cada vez que el laser dispara.</summary>
        public event Action onLaserFired;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            CreateBeam();
        }
        #endregion

        #region Public Methods
        /// <summary>Progreso de recarga de 0 a 1 (1 = listo). Lo usan las barras de UI.</summary>
        public float GetReadyProgress()
        {
            if (IsReady)
            {
                return 1f;
            }

            float restante = _nextFireTime - Time.time;
            return Mathf.Clamp01(1f - restante / Mathf.Max(0.01f, _cooldownSeconds));
        }

        /// <summary>Intenta disparar. Devuelve false si el cooldown no ha acabado.</summary>
        public bool TryFire()
        {
            if (!IsReady || _target == null)
            {
                Debug.Log("[EvaLaser] Laser en cooldown");
                return false;
            }

            _nextFireTime = Time.time + _cooldownSeconds;
            _beam.SetPosition(0, transform.position + (Vector3)_muzzleOffset);
            _beam.SetPosition(1, _target.position);
            _beam.enabled = true;
            StartCoroutine(HideBeamRoutine());
            onLaserFired?.Invoke();
            Debug.Log("[EvaLaser] Laser disparado. Cooldown: " + _cooldownSeconds + " s");
            return true;
        }
        #endregion

        #region Private Methods
        private void CreateBeam()
        {
            GameObject go = new GameObject("LaserBeam");
            go.transform.SetParent(transform, false);
            _beam = go.AddComponent<LineRenderer>();
            _beam.positionCount = 2;
            _beam.startWidth = _beamWidth;
            _beam.endWidth = _beamWidth * 0.4f;
            _beam.material = new Material(Shader.Find("Sprites/Default"));
            _beam.startColor = _laserColor;
            _beam.endColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, 0f);
            _beam.sortingOrder = 20;
            _beam.enabled = false;
        }

        private IEnumerator HideBeamRoutine()
        {
            yield return new WaitForSeconds(_visibleSeconds);
            _beam.enabled = false;
        }
        #endregion
    }
}
