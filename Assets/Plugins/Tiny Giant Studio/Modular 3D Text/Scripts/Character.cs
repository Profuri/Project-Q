using UnityEngine;

namespace TinyGiantStudio.Text
{
    [System.Serializable]
    public class Character
    {
        public char character;
        public GameObject prefab;
        public Mesh meshPrefab;

        public int glyphIndex;
        /// <summary>
        /// Named advance in typeface. Can't rename here because it would break a lot of existing fonts
        /// https://learn.microsoft.com/en-us/typography/opentype/spec/hmtx
        /// Name: advanceWidth
        /// </summary>
        public float spacing = 700; //to-do: shouldn't this be atleast a int? //note: this is calculated AFTER multiplied by EM in some cases. Why? This is why its a flot
        /// <summary>
        /// https://learn.microsoft.com/en-us/typography/opentype/spec/hmtx
        /// Name: lsb
        /// </summary>
        public int leftSideBearing = 0; //not implemented

        public Vector3 offset;
        public float xOffset;
        public float yOffset;
        public float zOffset;
    }
}
