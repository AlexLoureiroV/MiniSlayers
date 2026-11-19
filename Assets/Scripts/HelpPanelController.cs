using UnityEngine;

namespace MiniSlayers
{
    /// <summary>
    /// Muestra u oculta la guia de controles en pantalla al pulsar la tecla H.
    /// Extra de MiniSlayers: no interfiere con los 4 eventos del enunciado,
    /// gestiona directamente su propio panel de la UI.
    /// </summary>
    public class HelpPanelController : MonoBehaviour
    {
        #region Properties
        /// <summary>True si la guia esta visible ahora mismo.</summary>
        public bool IsVisible
        {
            get { return _helpPanel != null && _helpPanel.activeSelf; }
        }
        #endregion

        #region Fields
        [Header("Guia de controles")]
        [SerializeField] private GameObject _helpPanel;

        [Header("Ajustes")]
        [SerializeField] private KeyCode _toggleKey = KeyCode.H;
        [SerializeField] private bool _visibleAtStart = false;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            if (_helpPanel != null)
            {
                _helpPanel.SetActive(_visibleAtStart);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                if (_helpPanel == null)
                {
                    return;
                }

                bool nuevoEstado = !_helpPanel.activeSelf;
                _helpPanel.SetActive(nuevoEstado);
                Debug.Log("[HelpPanel] Guia de controles: " + (nuevoEstado ? "VISIBLE" : "OCULTA"));
            }
        }
        #endregion

        #region Public Methods
        /// <summary>Por si se quiere forzar el estado desde otro modulo.</summary>
        public void SetVisible(bool visible)
        {
            if (_helpPanel != null)
            {
                _helpPanel.SetActive(visible);
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
