using Bagheads.UnityConsole;
using Bagheads.UnityConsole.Data;

using UnityEngine;

namespace Bagheads.Bagheads.UnityConsole.Components
{
    public class KonsoleBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Font m_font;

        [SerializeField]
        private int m_fontSize;

        protected void Awake()
        {
            Konsole.IntegrateInExistingCanvas(new IntegrationOptions
            {
                FontSize = m_fontSize,
                DefaultTextFont = m_font
            });
        }
    }
}