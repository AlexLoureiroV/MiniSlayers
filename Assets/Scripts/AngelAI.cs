using System;
using System.Collections;
using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Extra v2 de MiniSlayers: IA del Angel (EnemyEventSystem en miniatura).
    /// Ciclo: reposo -> CARGA (se ilumina y es VULNERABLE) -> dispara rayos
    /// desde los vertices del octaedro. Solo lanza eventos; el dano lo decide
    /// el EventSystem. Se adjunta al objeto Enemy.
    /// </summary>
    public class AngelAI : MonoBehaviour
    {
        #region Properties
        /// <summary>True mientras carga: unica ventana en la que recibe dano.</summary>
        public bool IsCharging { get; private set; }
        #endregion

        #region Fields
        [Header("Referencias")]
        [SerializeField] private Transform _target;

        [Header("Reglas de combate")]
        [SerializeField] private float _idleSeconds = 3f;
        [SerializeField] private float _chargeSeconds = 1.5f;
        [SerializeField] private float _beamSeconds = 0.18f;

        [Header("Visual")]
        [SerializeField] private Color _chargeColor = new Color(0.85f, 0.95f, 1f, 1f);
        [SerializeField] private Color _beamColor = new Color(0.4f, 0.75f, 1f, 1f);
        [SerializeField] private float _beamWidth = 0.08f;

        private SpriteRenderer _sprite;
        private Color _baseColor;
        private Vector3 _baseScale;
        private Vector3 _basePosition;
        private LineRenderer[] _vertexBeams;
        #endregion

        #region Events
        /// <summary>El Angel empieza a cargar (momento vulnerable).</summary>
        public event Action onChargeStart;

        /// <summary>El Angel dispara desde sus vertices.</summary>
        public event Action onFire;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _baseColor = _sprite != null ? _sprite.color : Color.white;
            _baseScale = transform.localScale;
            _basePosition = transform.position;
        }

        void Start()
        {
            CreateVertexBeams();
            StartCoroutine(BattleRoutine());
        }
        #endregion

        #region Public Methods
        /// <summary>Destello al recibir un golpe cuando NO es vulnerable.</summary>
        public void BlinkInvulnerable()
        {
            StartCoroutine(InvulnerableBlinkRoutine());
        }

        /// <summary>Detiene el ciclo de combate (fin de partida).</summary>
        public void StopCombat()
        {
            StopAllCoroutines();
            IsCharging = false;

            if (_sprite != null)
            {
                _sprite.color = _baseColor;
            }

            transform.localScale = _baseScale;
            transform.position = _basePosition;
        }
        #endregion

        #region Private Methods
        private IEnumerator BattleRoutine()
        {
            yield return new WaitForSeconds(_idleSeconds);

            while (true)
            {
                // FASE DE CARGA: se ilumina, es vulnerable
                IsCharging = true;
                onChargeStart?.Invoke();
                Debug.Log("[AngelAI] Cargando ataque... (vulnerable!)");

                float t = 0f;
                while (t < _chargeSeconds)
                {
                    t += Time.deltaTime;

                    if (_sprite != null)
                    {
                        _sprite.color = Color.Lerp(_baseColor, _chargeColor, t / _chargeSeconds);
                    }

                    // Carga inconfundible: pulso de escala + temblor de posicion.
                    float k = t / _chargeSeconds;
                    transform.localScale = _baseScale * Mathf.Lerp(1f, 1.22f, k);
                    transform.position = _basePosition +
                        new Vector3(UnityEngine.Random.Range(-0.04f, 0.04f), UnityEngine.Random.Range(-0.04f, 0.04f), 0f);

                    yield return null;
                }

                // DISPARO
                IsCharging = false;

                if (_sprite != null)
                {
                    _sprite.color = _baseColor;
                }

                transform.localScale = _baseScale;
                transform.position = _basePosition;
                FireVertexBeams();
                onFire?.Invoke();
                Debug.Log("[AngelAI] Disparo desde los vertices!");

                yield return new WaitForSeconds(_idleSeconds);
            }
        }

        private void CreateVertexBeams()
        {
            _vertexBeams = new LineRenderer[4];

            for (int i = 0; i < _vertexBeams.Length; i++)
            {
                GameObject go = new GameObject("VertexBeam" + i);
                go.transform.SetParent(transform, false);
                LineRenderer lr = go.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.startWidth = _beamWidth;
                lr.endWidth = _beamWidth * 0.5f;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = _beamColor;
                lr.endColor = new Color(_beamColor.r, _beamColor.g, _beamColor.b, 0f);
                lr.sortingOrder = 25;
                lr.enabled = false;
                _vertexBeams[i] = lr;
            }
        }

        private void FireVertexBeams()
        {
            if (_sprite == null || _target == null)
            {
                return;
            }

            Bounds b = _sprite.bounds;
            Vector3 c = transform.position;
            Vector3[] vertices = new Vector3[]
            {
                c + new Vector3(0f, b.extents.y, 0f),
                c + new Vector3(0f, -b.extents.y, 0f),
                c + new Vector3(-b.extents.x, 0f, 0f),
                c + new Vector3(b.extents.x, 0f, 0f)
            };

            for (int i = 0; i < vertices.Length; i++)
            {
                _vertexBeams[i].SetPosition(0, vertices[i]);
                _vertexBeams[i].SetPosition(1, _target.position);
                _vertexBeams[i].enabled = true;
            }

            StartCoroutine(BeamHideRoutine());
        }

        private IEnumerator BeamHideRoutine()
        {
            yield return new WaitForSeconds(_beamSeconds);

            foreach (LineRenderer lr in _vertexBeams)
            {
                lr.enabled = false;
            }
        }

        private IEnumerator InvulnerableBlinkRoutine()
        {
            if (_sprite == null)
            {
                yield break;
            }

            _sprite.color = new Color(1f, 1f, 1f, 0.35f);
            yield return new WaitForSeconds(0.12f);
            _sprite.color = _baseColor;
        }
        #endregion
    }
}
