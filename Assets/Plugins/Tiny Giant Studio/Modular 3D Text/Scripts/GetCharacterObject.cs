using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

using TinyGiantStudio.Layout;
using TinyGiantStudio.Text.FontCreation;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// To-do: Check if it is possible to pass null as previous char, this will allow me to use one method, instead of two to get mesh layout and object
    /// To-do: Test out alternatives to using statics
    /// </summary>

    public class GetCharacterObject
    {
        private static readonly float unitConverted = 0.1f; //an arbitrary value

        #region Public methods

        /// <summary>
        /// This is for the first character only
        /// Used by text to get character mesh + layout info when single mesh is turned on.
        /// The other way to get character is using GetObject(char c, Modular3DText text); for multiple objects
        /// </summary>
        /// <param name="currentChar">The character</param>
        /// <param name="text">The Modular3DText</param>
        /// <returns></returns>
        public static MeshLayout GetMeshLayout(char currentChar, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            if (!IsSpecialSymbol(currentChar))
                return ProcessNormalCharacter(currentChar, text, meshPostProcess); //stuff with mesh
            else
                return ProcessSpecialCharacter(currentChar, text); //stuff like spaces and tabs, newlines
        }

        /// <summary>
        /// Used by text to get character mesh + layout info when single mesh is turned on.
        /// The other way to get character is using GetObject(char c, Modular3DText text); for multiple objects
        /// </summary>
        /// <param name="currentChar">The character</param>
        /// <param name="text">The Modular3DText</param>
        /// <returns></returns>
        public static MeshLayout GetMeshLayout(char previousChar, char currentChar, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            if (!IsSpecialSymbol(currentChar))
                return ProcessNormalCharacter(previousChar, currentChar, text, meshPostProcess); //stuff with mesh
            else
                return ProcessSpecialCharacter(currentChar, text); //stuff like spaces and tabs, newlines
        }

        /// <summary>
        /// When single mesh is turned off
        /// </summary>
        /// <param name="c"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameObject GetObject(char c, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            Font font = text.Font;

            GameObject obj = new GameObject();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, "Update text");
#endif
            LayoutElement layoutElement = obj.AddComponent<LayoutElement>();

            if (!IsSpecialSymbol(c))
                obj = ProcessNormalCharacter(c, text, obj, layoutElement, meshPostProcess);
            else
                obj = ProcessSpecialCharacter(c, font, text, layoutElement, obj);

            obj.SetActive(false);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckLeftOver(obj, text);

                obj.AddComponent<DelayCallCharacterCleanUp>().text = text;
            }
            else
            {
                if (text.hideLettersInHierarchyInPlayMode)
                    obj.hideFlags = HideFlags.HideInHierarchy;
            }

            text._allCharacterObjectList.Add(obj);

            EditorUtility.SetDirty(obj); //without this, unity won't save the character
#endif

            return obj;
        }

        /// <summary>
        /// When single mesh is turned off
        /// </summary>
        /// <param name="c"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameObject GetObject(char previousChar, char currentChar, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            Font font = text.Font;
            GameObject obj = new GameObject();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, "Update text");
#endif
            LayoutElement layoutElement = obj.AddComponent<LayoutElement>();

            if (!IsSpecialSymbol(currentChar))
                obj = ProcessNormalCharacter(previousChar, currentChar, text, obj, layoutElement, meshPostProcess);
            else
                obj = ProcessSpecialCharacter(currentChar, font, text, layoutElement, obj); //todo previous char

            obj.SetActive(false);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckLeftOver(obj, text);

                obj.AddComponent<DelayCallCharacterCleanUp>().text = text;
            }
            else
            {
                if (text.hideLettersInHierarchyInPlayMode)
                    obj.hideFlags = HideFlags.HideInHierarchy;
            }

            text._allCharacterObjectList.Add(obj);

            EditorUtility.SetDirty(obj); //without this, unity won't save the character
#endif

            return obj;
        }

        #endregion Public methods

        private static MeshLayout ProcessNormalCharacter(char c, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            bool AutoLetterSize = text.AutoLetterSize;
            MeshLayout meshLayout = new MeshLayout();

            //returns mesh and mtext_Character
            var meshPrefab = text.Font.RetrievePrefabAndCharacter(c);

            bool useAutoSizeAnyway = false;
            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                meshLayout.mesh = MeshPostProcessing.PostProcess(meshPrefab.Item1, meshPostProcess);

                if (!AutoLetterSize)
                {
                    if (meshPrefab.Item2 != null) //TODO: why it can be nulled
                    {
                        meshLayout.xOffset = meshPrefab.Item2.xOffset;
                        meshLayout.yOffset = meshPrefab.Item2.yOffset;
                        meshLayout.zOffset = meshPrefab.Item2.zOffset;
                    }

                    meshLayout.width = text.Font.Spacing(c) * text.FontSize.x;
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from ttf data
            else
            {
                CharacterGenerator creator = new CharacterGenerator();

                var mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, c);
                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                meshLayout.mesh = mesh;
                text.generatedMeshes.Add(mesh);

                if (!AutoLetterSize)
                {
                    if (!text.Font.monoSpaceFont)
                    {
                        if (creator.GlyphExists(c))
                            meshLayout.width = text.Font.Spacing(creator.GetCharacterAdvance(creator.Index(c))) * text.FontSize.x;
                        else
                            useAutoSizeAnyway = true;
                    }
                    else
                        meshLayout.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    //meshLayout.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(c))) * text.FontSize.x;
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize || useAutoSizeAnyway)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(meshLayout.mesh);

                meshLayout.xOffset = bounds.center.x;
                meshLayout.yOffset = bounds.center.y;
                meshLayout.zOffset = bounds.center.z;

                meshLayout.width = bounds.size.x * text.FontSize.x;
                meshLayout.height = text.FontSize.y * unitConverted;
            }

            return meshLayout;
        }

        private static MeshLayout ProcessNormalCharacter(char previousChar, char currentChar, Modular3DText text, MeshPostProcess meshPostProcess)
        {
            bool AutoLetterSize = text.AutoLetterSize; //to-do: naming
            MeshLayout meshLayout = new MeshLayout();

            //returns mesh and mtext_Character
            var meshPrefab = text.Font.RetrievePrefabAndCharacter(currentChar);

            bool useAutoSizeAnyway = false;

            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                meshLayout.mesh = MeshPostProcessing.PostProcess(meshPrefab.Item1, meshPostProcess);

                if (!AutoLetterSize)
                {
                    if (meshPrefab.Item2 != null) //TODO: why it can be nulled
                    {
                        meshLayout.xOffset = meshPrefab.Item2.xOffset - (text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;
                        meshLayout.yOffset = meshPrefab.Item2.yOffset;
                        meshLayout.zOffset = meshPrefab.Item2.zOffset;
                    }

                    meshLayout.width = text.Font.Spacing(previousChar, currentChar) * text.FontSize.x;
                    //meshLayout.width = text.Font.Spacing(currentChar) * text.FontSize.x; //old, not kerning supported.
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from TTF data
            else
            {
                CharacterGenerator creator = new CharacterGenerator();

                var mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, currentChar);
                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                meshLayout.mesh = mesh;
                text.generatedMeshes.Add(mesh);

                if (!AutoLetterSize)
                {
                    meshLayout.xOffset = -(text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;

                    if (!text.Font.monoSpaceFont)
                    {
                        if (creator.GlyphExists(currentChar))
                            meshLayout.width = (text.Font.Spacing(creator.GetCharacterAdvance(creator.Index(currentChar))) + text.Font.Kerning(previousChar, currentChar)) * text.FontSize.x;
                        else
                            useAutoSizeAnyway = true;
                    }
                    else
                        meshLayout.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    //meshLayout.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(c))) * text.FontSize.x;
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize || useAutoSizeAnyway)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(meshLayout.mesh);

                meshLayout.xOffset = bounds.center.x - (text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;
                meshLayout.yOffset = bounds.center.y;
                meshLayout.zOffset = bounds.center.z;

                meshLayout.width = (bounds.size.x + text.Font.Kerning(previousChar, currentChar)) * text.FontSize.x;
                //meshLayout.width = bounds.size.x * text.FontSize.x;
                meshLayout.height = text.FontSize.y * unitConverted;
            }

            return meshLayout;
        }

        private static GameObject ProcessNormalCharacter(char c, Modular3DText text, GameObject obj, LayoutElement layoutElement, MeshPostProcess meshPostProcess)
        {
            bool AutoLetterSize = text.AutoLetterSize;
            obj.name = c.ToString();

            var meshPrefab = text.Font.RetrievePrefabAndCharacter(c);
            obj.AddComponent<MeshFilter>();

            bool useAutoSizeAnyway = false;
            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                //obj.GetComponent<MeshFilter>().sharedMesh = meshPrefab.Item1;
                Mesh mesh = meshPrefab.Item1;
                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;

//#if UNITY_EDITOR
//                if (Application.isPlaying)
//                    EditorApplication.delayCall += () => SetParent(text, obj); //why delay
//#endif

                obj.SetActive(false);

                if (!AutoLetterSize)
                {
                    layoutElement.xOffset = meshPrefab.Item2.xOffset;
                    layoutElement.yOffset = meshPrefab.Item2.yOffset;
                    layoutElement.zOffset = meshPrefab.Item2.zOffset;

                    layoutElement.width = text.Font.Spacing(c) * text.FontSize.x;
                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from ttf data
            else
            {
                CharacterGenerator creator = new CharacterGenerator();
                Mesh mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, c);
                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;
                text.generatedMeshes.Add(mesh);

                if (!AutoLetterSize)
                {
                    if (!text.Font.monoSpaceFont)
                    {
                        if (creator.GlyphExists(c))
                            layoutElement.width = text.Font.Spacing(creator.GetCharacterAdvance(creator.Index(c))) * text.FontSize.x;
                        else
                            useAutoSizeAnyway = true;
                    }
                    else
                        layoutElement.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize || useAutoSizeAnyway)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(obj.GetComponent<MeshFilter>().sharedMesh);

                layoutElement.xOffset = bounds.center.x;
                layoutElement.yOffset = bounds.center.y;
                layoutElement.zOffset = bounds.center.z;

                layoutElement.width = bounds.size.x * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
                layoutElement.autoCalculateSize = true;
            }

            return obj;
        }

        private static GameObject ProcessNormalCharacter(char previousChar, char currentChar, Modular3DText text, GameObject obj, LayoutElement layoutElement, MeshPostProcess meshPostProcess)
        {
            bool AutoLetterSize = text.AutoLetterSize; //to-do: naming
            obj.name = currentChar.ToString();

            var meshPrefab = text.Font.RetrievePrefabAndCharacter(currentChar);
            obj.AddComponent<MeshFilter>();

            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                Mesh mesh = meshPrefab.Item1;
                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;

//#if UNITY_EDITOR
//                if (Application.isPlaying)
//                    EditorApplication.delayCall += () => SetParent(text, obj); //why delay
//#endif
                obj.SetActive(false);

                if (!AutoLetterSize)
                {
                    layoutElement.xOffset = meshPrefab.Item2.xOffset - (text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;
                    //layoutElement.xOffset = meshPrefab.Item2.xOffset;
                    layoutElement.yOffset = meshPrefab.Item2.yOffset;
                    layoutElement.zOffset = meshPrefab.Item2.zOffset;

                    layoutElement.width = text.Font.Spacing(previousChar, currentChar) * text.FontSize.x;
                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from TTF data
            else
            {
                CharacterGenerator creator = new CharacterGenerator();
                Mesh mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, currentChar);

                mesh = MeshPostProcessing.PostProcess(mesh, meshPostProcess);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;
                text.generatedMeshes.Add(mesh);

                if (!AutoLetterSize)
                {
                    layoutElement.xOffset = -(text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;
                    if (!text.Font.monoSpaceFont)
                        layoutElement.width = text.Font.Spacing(creator.GetCharacterAdvance(creator.Index(currentChar))) * text.FontSize.x;
                    else
                        layoutElement.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(obj.GetComponent<MeshFilter>().sharedMesh);

                layoutElement.xOffset = bounds.center.x - (text.Font.Kerning(previousChar, currentChar) * text.FontSize.x) / 2;
                layoutElement.yOffset = bounds.center.y;
                layoutElement.zOffset = bounds.center.z;

                layoutElement.width = (bounds.size.x + text.Font.Kerning(previousChar, currentChar)) * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
                layoutElement.autoCalculateSize = false;
            }

            return obj;
        }

        //private static void SetParent(Modular3DText text, GameObject obj)
        //{
        //    if (text)
        //    {
        //        if (obj)
        //            obj.transform.SetParent(text.transform);
        //    }
        //}

        private static bool IsSpecialSymbol(char c)
        {
            if (c == '\t')
                return true;
            if (c == '\n')
                return true;
            if (char.IsWhiteSpace(c))
                return true;
            return false;
        }

        private static MeshLayout ProcessSpecialCharacter(char c, Modular3DText text)
        {
            MeshLayout layoutElement = new MeshLayout();

            if (c == '\n')
            {
                layoutElement.lineBreak = true;
            }
            else if (c == '\t')
            {
                layoutElement.width = text.Font.ConvertedValue(text.Font.TabSpace()) * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            else if (char.IsWhiteSpace(c))
            {
                layoutElement.space = true;
                layoutElement.width = text.Font.ConvertedValue(text.Font.emptySpaceSpacing) * text.WordSpacing * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            return layoutElement;
        }

        private static GameObject ProcessSpecialCharacter(char c, Font font, Modular3DText text, LayoutElement layoutElement, GameObject obj)
        {
            if (c == '\n')
            {
                obj.name = "New Line";
                layoutElement.lineBreak = true;
            }
            else if (c == '\t')
            {
                obj.name = "Tab";
                layoutElement.width = text.Font.ConvertedValue(font.TabSpace()) * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            else if (char.IsWhiteSpace(c))
            {
                obj.name = "Space";
                layoutElement.space = true;
                layoutElement.width = text.Font.ConvertedValue(text.Font.emptySpaceSpacing) * text.WordSpacing * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }

            return obj;
        }

#if UNITY_EDITOR

        private static void CheckLeftOver(GameObject obj, Modular3DText text)
        {
            if (text)
                return;

            obj.hideFlags = HideFlags.None;

            EditorApplication.delayCall += () =>
            {
                try { Object.DestroyImmediate(obj); }
                catch { }
            };
        }

#endif
    }
}