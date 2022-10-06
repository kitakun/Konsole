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

#if KONSOLE_TEXT_MESH_PRO
        [SerializeField]
        private bool m_useTextMeshPro = true;

        [SerializeField]
        private TMPro.TMP_FontAsset m_textMeshFont;
#endif

        protected void Awake()
        {
            Konsole.IntegrateInExistingCanvas(new IntegrationOptions
            {
                FontSize = m_fontSize,
                DefaultTextFont = m_font,
#if KONSOLE_TEXT_MESH_PRO
                UseTextMeshPro = m_useTextMeshPro,
                TMpFontAsset = m_textMeshFont,
#endif
            });
        }
    }
}