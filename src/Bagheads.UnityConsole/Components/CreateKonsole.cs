using Bagheads.UnityConsole.Data;

using UnityEngine;

namespace Bagheads.UnityConsole.Components
{
    public class CreateKonsole : MonoBehaviour
    {
        [SerializeField]
        private int m_fontSize = 14;

#if KONSOLE_TEXT_MESH_PRO
        private const bool _useTextMeshPro = true;

        [SerializeField]
        private TMPro.TMP_FontAsset m_textMeshFont;
#else
        private const bool _useTextMeshPro = false;

        [SerializeField]
        private Font m_font;
#endif

        protected void Awake()
        {
            Konsole.IntegrateInExistingCanvas(new IntegrationOptions
            {
                FontSize = m_fontSize,
#if KONSOLE_TEXT_MESH_PRO
                UseTextMeshPro = _useTextMeshPro,
                TMpFontAsset = m_textMeshFont,
#else
                DefaultTextFont = m_font,
#endif
            });
        }
    }
}