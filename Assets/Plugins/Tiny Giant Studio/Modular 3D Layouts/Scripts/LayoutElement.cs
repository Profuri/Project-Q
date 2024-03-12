using UnityEngine;

namespace TinyGiantStudio.Layout
{
    /// <summary>
    /// Instead of directly assigning positions/rotations, using Layout Elements open up the possibility to use tweening library of your choice or your own methods to animate movements.
    /// </summary>
    [DisallowMultipleComponent]
    [HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-element")]
    public class LayoutElement : MonoBehaviour
    {
        [Tooltip("Ignores this element in layout group")]
        public bool ignoreElement;

        [Tooltip("This is overwritten on texts")]
        public bool autoCalculateSize = false;
        //TODO: Custom editor to make these read only when autocalculate size is open
        public float width = 1;
        public float height = 1;
        [HideInInspector]
        public float depth = 1; //TODO


        public float xOffset = 0;
        public float yOffset = 0;
        public float zOffset = 0;

        [Tooltip("Used in Grid layout.\nEnds current line and moves everything after it to next one.")]
        public bool lineBreak = false;
        public bool space = false;
    }
}