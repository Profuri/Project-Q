namespace TinyGiantStudio.Text
{
    [System.Serializable]
    public struct KerningPair
    {
        public char left;
        public float offset;
        public char right;

        public KerningPair(char left, char right, float offset)
        {
            this.left = left;
            this.offset = offset;
            this.right = right;
        }
    }
}
