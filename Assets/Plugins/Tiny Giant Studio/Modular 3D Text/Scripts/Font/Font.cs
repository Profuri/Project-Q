using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [PreferBinarySerialization]
    [CreateAssetMenu(fileName = "New 3D Font", menuName = "Tiny Giant Studio/Modular 3D Text/Font/New Font")]
    public class Font : ScriptableObject
    {
        #region Variable declaration

        public List<Character> characters = new List<Character>();

        #region Spacing Settings

        [Tooltip("Use UpperCase If LowerCase Is Missing")]
        public bool useUpperCaseLettersIfLowerCaseIsMissing = true;

        [Tooltip("Mono space means all characters are spaced equally.\nIf turned on, individual spacing value from list below is ignored. The information is not removed to avoid accidentally turning it on ruin the font. \nCharacter spacing is used for everything")]
        public bool monoSpaceFont;

        public float monoSpaceSpacing = 1;

        [Tooltip("Word spacing and spacing for unavailable characters")]
        public float emptySpaceSpacing = 1;

        public float characterSpacing = 1;

        public float TabSpace() => emptySpaceSpacing * 3;

        #endregion Spacing Settings

        [Space]
        [Tooltip("Avoid recursive references")]
        public List<Font> fallbackFonts = new List<Font>();

        [Tooltip("The 3d object with the characters as child object. \nNOT required.")]
        public GameObject modelSource = null;

        #region Kerning

        public bool enableKerning = true;
        public float kerningMultiplier = 1f;

        //unfortunately dictionary isn't serializable //TODO
        public List<KerningPair> kernTable = new List<KerningPair>();

        #endregion Kerning

        #region Font file data

        #region Data from original font file

        [Tooltip("An em is a unit of measurement, relative to the size of the font; therefore, in a typeface set at a font-size of 16px, one em is 16px.")]
        public float unitPerEM = 1; //multiplied by 8

        [Tooltip("Text's character spacing = font's character spacing * text's character spacing")]
        public float lineHeight = 0.1469311f;

        #endregion Data from original font file

        [SerializeField] //It's not serializable though
        private TypeFace _typeFace;

        public TypeFace TypeFace
        {
            get
            {
                if (_typeFace == null)
                    GetTypeFaceFromBytes();

                return _typeFace;
            }
            set
            {
                _typeFace = value;
            }
        }

        public byte[] fontBytes;

        #endregion Font file data

        #region New character creation settings

        public int sizeXYInput = 1;
        public int sizeZInput = 1;
        public float vertexDensityInput = 1;
        public float autoSmoothAngleInput = 30;
        public float averageYValue = 0;

        #endregion New character creation settings

#if UNITY_EDITOR

        /// <summary>
        /// Editor only. Used by inspector.
        /// </summary>
        public bool showCharacterDetailsEditor;

        /// <summary>
        /// Editor only. Used by inspector.
        /// </summary>
        public string beingSearched;

#endif

        #endregion Variable declaration

        /// <summary>
        /// When this returns null, CharacterGenerator script is used to create it on the fly from ttf data
        /// </summary>
        /// <param name="c"></param>
        /// <param name="checkFallBackFonts"></param>
        /// <returns></returns>
        public (Mesh, Character) RetrievePrefabAndCharacter(char c, bool checkFallBackFonts = true)
        {
            //look for character in any form
            for (int i = 0; i < characters.Count; i++)
            {
                if (c == characters[i].character)
                {
                    return (MeshPrefab(i), characters[i]);
                }
            }

            //if no character is found of lower case
            if (useUpperCaseLettersIfLowerCaseIsMissing)
            {
                if (char.IsLower(c))
                {
                    c = char.ToUpper(c);

                    for (int i = 0; i < characters.Count; i++)
                    {
                        if (c == characters[i].character)
                        {
                            return (MeshPrefab(i), characters[i]);
                        }
                    }
                }
            }

            if (checkFallBackFonts)
            {
                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        var missingCharacter = fallbackFonts[i].RetrievePrefabAndCharacter(c, false);
                        if (missingCharacter.Item1 != null)
                        {
                            return (missingCharacter.Item1, missingCharacter.Item2);
                        }
                    }
                }
            }

            return (null, null);
        }

        private Mesh MeshPrefab(int i)
        {
            if (characters[i].prefab)
            {
                if (characters[i].prefab.GetComponent<MeshFilter>())
                {
                    if (characters[i].prefab.GetComponent<MeshFilter>().sharedMesh)
                    {
                        return characters[i].prefab.GetComponent<MeshFilter>().sharedMesh;
                    }
                }
            }
            else if (characters[i].meshPrefab)
            {
                return characters[i].meshPrefab;
            }

            return null;
        }

        public float Spacing(char c)
        {
            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (c == characters[i].character)
                    {
                        return Spacing(characters[i].spacing);
                    }
                }

                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        return fallbackFonts[i].Spacing(c);
                    }
                }

                return MonoSpaceSpacing();
            }
            else //for monospace fonts
            {
                return MonoSpaceSpacing();
            }
        }

        /// <summary>
        /// Spacing with kerning
        /// </summary>
        /// <param name="previousCharacter"></param>
        /// <param name="currentCharacter"></param>
        /// <returns></returns>
        public float Spacing(char previousCharacter, char currentCharacter)
        {
            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (currentCharacter == characters[i].character)
                    {
                        return Spacing(characters[i].spacing) + Kerning(previousCharacter, characters[i]);
                    }
                }

                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        return fallbackFonts[i].Spacing(currentCharacter);
                    }
                }

                return MonoSpaceSpacing();
            }
            else //for monospace fonts
            {
                return MonoSpaceSpacing();
            }
        }
        /// <summary>
        /// Used by getcharacterobject.cs
        /// </summary>
        /// <param name="rawAdvance"></param>
        /// <returns></returns>
        public float Spacing(float rawAdvance) => ConvertedValue(rawAdvance);
        /// <summary>
        /// This is the raw value used by unity after taking font EM into consideration
        /// </summary>
        /// <returns></returns>
        public float MonoSpaceSpacing() => ConvertedValue(monoSpaceSpacing);

        private float Kerning(char previousChar, Character currentChar)
        {
            for (int i = 0; i < kernTable.Count; i++)
            {
                if (kernTable[i].left == previousChar && kernTable[i].right == currentChar.character)
                {
                    return ConvertedValue(kernTable[i].offset * kerningMultiplier);
                }
            }

            return 0;
        }
        public float Kerning(char previousChar, char currentChar)
        {
            for (int i = 0; i < kernTable.Count; i++)
            {
                if (kernTable[i].left == previousChar && kernTable[i].right == currentChar)
                {
                    return ConvertedValue(kernTable[i].offset * kerningMultiplier);
                }
            }

            return 0;
        }


        public float ConvertedValue(float spacing) => (spacing * characterSpacing) / (Mathf.Clamp(unitPerEM, 0.0001f, 1000000) * 8);

        public int KerningReferencesCount(int index)
        {
            if (index >= characters.Count)
                return 0;

            int count = 0;
            char c = characters[index].character;

            for (int i = 0; i < kernTable.Count; i++)
            {
                if (kernTable[i].left == c || kernTable[i].right == c)
                    count++;
            }
            return count;
        }

        //Font creation:

        #region Update Character List start

        public void UpdateCharacterList(GameObject prefab)
        {
            modelSource = prefab;
            UpdateCharacterList();
        }

        public void UpdateCharacterList(bool overwriteOldSet)
        {
            if (overwriteOldSet)
                characters.Clear();

            UpdateCharacterList();
        }

        public void UpdateCharacterList()
        {
            if (modelSource)
            {
                foreach (Transform child in modelSource.transform)
                {
                    AddCharacter(child.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Model source not found on " + name + " couldn't add any characters");
            }

            //TabSpace = emptySpaceSpacing * 3;
        }

        public void AddCharacter(GameObject obj)
        {
            if (!obj)
                return;

            ProcessName(obj.name, out char character, out float spacing);

            if (CharacterAlreadyExists(character))
                return;

            Character newChar = new Character();

            newChar.character = character;
            newChar.spacing = spacing * unitPerEM;
            newChar.prefab = obj;

            characters.Add(newChar);
        }

        private bool CharacterAlreadyExists(char character)
        {
            for (int i = 0; i < characters.Count; i++)
                if (characters[i].character == character)
                    return true;

            return false;
        }

        public void AddCharacter(Mesh mesh)
        {
            Character newChar = new Character();

            if (!mesh)
                return;

            ProcessName(mesh.name, out char character, out float spacing);

            newChar.character = character;
            newChar.spacing = spacing;

            newChar.meshPrefab = mesh;

            characters.Add(newChar);
        }

        private void ProcessName(string name, out char character, out float spacing)
        {
            try
            {
                NewMethod(name, out character, out spacing);
            }
            catch
            {
                //Debug.Log("Old method");
                OldMethod(name, out character, out spacing);
            }
        }

        private void NewMethod(string name, out char character, out float spacing)
        {
            string[] s = name.Split(new char[] { '_', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log(s[0]);
            character = Regex.Unescape("\\u" + s[0]).ToCharArray()[0];
            //Debug.Log(s[1]);
            spacing = GetSpacing(s[1]);
        }

        /// <summary>
        /// Used by the legacy blender font extractor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="character"></param>
        /// <param name="spacing"></param>
        private void OldMethod(string name, out char character, out float spacing)
        {
            if (name.Contains("dot"))
            {
                character = '.';
                spacing = (float)Convert.ToDouble(name.Substring(4));
            }
            else if (name.Contains("forwardSlash"))
            {
                character = '/';
                spacing = GetSpacing(name.Substring(13));
            }
            else if (name.Contains("quotationMark"))
            {
                character = '"';
                spacing = GetSpacing(name.Substring(14));
            }
            else if (name.Contains("multiply"))
            {
                character = '*';
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("colon"))
            {
                character = ':';
                spacing = GetSpacing(name.Substring(6));
            }
            else if (name.Contains("lessThan"))
            {
                character = '<';
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("moreThan"))
            {
                character = '>';
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("questionMark"))
            {
                character = '?';
                spacing = GetSpacing(name.Substring(13));
            }
            else if (name.Contains("slash"))
            {
                character = '/';
                spacing = GetSpacing(name.Substring(6));
            }
            else if (name.Contains("backwardSlash"))
            {
                character = '\\';
                spacing = GetSpacing(name.Substring(14));
            }
            else if (name.Contains("verticalLine"))
            {
                character = '|';
                spacing = GetSpacing(name.Substring(13));
            }
            else
            {
                char[] chars = name.ToCharArray();
                character = chars[0];
                spacing = GetSpacing(name.Substring(2));
            }
            spacing *= 0.81f;
        }

        private float GetSpacing(string numberString)
        {
            if (float.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                return value;
            else
                return 1;
        }

        public void GetMonoSpacingFromAverageCharacterSpacing()
        {
            float spacing = 0;
            for (int i = 0; i < characters.Count; i++)
            {
                spacing += characters[i].spacing;
            }
            monoSpaceSpacing = spacing / characters.Count;
            if (float.IsNaN(monoSpaceSpacing))
                monoSpaceSpacing = 300;
        }

        #endregion Update Character List start

        public void SetFontBytes(byte[] newFontBytes)
        {
            fontBytes = newFontBytes;
            GetTypeFaceFromBytes();
        }

        public void GetTypeFaceFromBytes()
        {
            _typeFace = new TypeFace(fontBytes);
        }
    }
}