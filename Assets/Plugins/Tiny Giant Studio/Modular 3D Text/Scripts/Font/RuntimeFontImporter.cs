//using System.IO;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// This handles importing font files during runtime.
    /// Call CreateFontFromTTFFile(byte[] rfontBytes), pass the TTF file as a byte array and it will return a new Font.
    /// </summary>
    public class RuntimeFontImporter : MonoBehaviour
    {
        //[SerializeField] string path;

        //[ContextMenu("Test")]
        //public void Test()
        //{
        //    byte[] fontdata = File.ReadAllBytes(path);
        //    CreateFontFromTTFFile(fontdata);
        //}

        /// <summary>
        /// Pass the TTF file as a byte array and it will return a new Font.
        /// </summary>
        /// <param name="fontBytes">Content of the TTF file converted into a byte array.</param>
        /// <returns></returns>
        public Font CreateFontFromTTFFile(byte[] fontBytes)
        {
            Font font = ScriptableObject.CreateInstance<Font>();

            font.SetFontBytes(fontBytes);
            font.GetTypeFaceFromBytes();

            font.unitPerEM = font.TypeFace.unitsPerEm;
            font.lineHeight = font.TypeFace.lineHeight;

            float emptySpaceSpacing = 200;

            font.TypeFace.SetGlyphData(' ');
            if (font.TypeFace.glyphs.ContainsKey(' '))
            {
                int index = font.TypeFace.glyphs[' '].glyphIndex;

                if (index < font.TypeFace.advanceArray.Length && index >= 0)
                {
                    emptySpaceSpacing = font.TypeFace.advanceArray[index];
                }
            }

            font.emptySpaceSpacing = emptySpaceSpacing;

            return font;
        }

    }
}