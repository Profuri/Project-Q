using System.Collections.Generic;
using UnityEngine;

namespace TinyGiantStudio.Text.SampleScene
{
    public class MText_SampleScene_FontTest : MonoBehaviour
    {
        [SerializeField] Modular3DText modular3DText = null;
        [SerializeField] Modular3DText fontText = null;

        [Space]

        [SerializeField] List<Font> fonts = new List<Font>();
        int selectedFont = 0;

        public void NextFont()
        {
            selectedFont++;
            if (selectedFont >= fonts.Count) selectedFont = 0;

            UpdateInfo();
        }

        public void PreviousFont()
        {
            selectedFont--;
            if (selectedFont < 0) selectedFont = fonts.Count - 1;

            UpdateInfo();
        }

        void UpdateInfo()
        {
            modular3DText.Font = fonts[selectedFont];
            fontText.Font = fonts[selectedFont];
            fontText.Text = fonts[selectedFont].name;
        }
    }
}