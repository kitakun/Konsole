using Bagheads.UnityConsole.Data;

using UnityEngine;

namespace Bagheads.UnityConsole.Components
{
    public class CreateKonsole : MonoBehaviour
    {
        [SerializeField]
        private Font m_font;

        [SerializeField]
        private int m_fontSize = 14;

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