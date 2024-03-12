using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text.FontCreation
{
    public class MText_FontExporter
    {
        public void CreateFontFile(string prefabPath, string fontName, CharacterGenerator fontCreator, byte[] fontData)
        {
            Font newFont = ScriptableObject.CreateInstance<Font>();
            newFont.fontBytes = fontData;
            newFont.averageYValue = fontCreator.averageYValue;
            newFont.GetMonoSpacingFromAverageCharacterSpacing();
            GameObject fontSet = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;


#if UNITY_EDITOR
            {
                ModelImporter importer = ModelImporter.GetAtPath(prefabPath) as ModelImporter;

                importer.isReadable = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.importAnimation = false;

                importer.SaveAndReimport();
            }
#endif


            newFont.UpdateCharacterList(fontSet);
            for (int i = 0; i < newFont.characters.Count; i++)
            {
                int characterIndex = fontCreator.Index(newFont.characters[i].character);
                newFont.characters[i].glyphIndex = characterIndex;
                newFont.characters[i].spacing = Mathf.Abs(fontCreator.GetCharacterAdvance(characterIndex));
                newFont.characters[i].leftSideBearing = Mathf.Abs(fontCreator.GetCharacterLeftSideBearing(characterIndex));
            }

            newFont.characters = newFont.characters.OrderBy(p => p.glyphIndex).ToList();

            if (fontCreator.KerningSupported())
                newFont = GetKerning(fontCreator, newFont);
            //else
            //    Debug.Log("Kerning not supported");

            PassFontInformations(fontCreator, newFont);

            string scriptableObjectSaveLocation = EditorUtility.SaveFilePanel("Save font location", "", fontName, "asset");
            scriptableObjectSaveLocation = FileUtil.GetProjectRelativePath(scriptableObjectSaveLocation);
            AssetDatabase.CreateAsset(newFont, scriptableObjectSaveLocation);
            AssetDatabase.SaveAssets();



        }

        private Font GetKerning(CharacterGenerator fontCreator, Font newFont)
        {
            fontCreator.GetKerningInfo(out List<ushort> lefts, out List<ushort> rights, out List<short> offsets);


            for (int i = 0; i < lefts.Count; i++)
            {
                char leftChar = fontCreator.Character(lefts[i]);
                char rightChar = fontCreator.Character(rights[i]);
                //Debug.Log(leftChar + " " + rightChar);
                float offset = offsets[0];
                newFont.kernTable.Add(new KerningPair(leftChar, rightChar, offset));
            }

            return newFont;
        }

        private void PassFontInformations(CharacterGenerator fontCreator, Font newFont)
        {
            float unitsPerEM = 1;
            float originalLineHeight = 0;
            float whiteSpaceSpacing = 1;

            fontCreator.GetTypeFaceInformation(ref originalLineHeight, ref unitsPerEM, ref whiteSpaceSpacing);

            newFont.unitPerEM = unitsPerEM;
            newFont.lineHeight = originalLineHeight;
            //newFont.lineHeight = (originalLineHeight / (unitsPerEM / 1000)) * 0.00175f; //Here, 1000 is the default EM. No specific reason
            newFont.emptySpaceSpacing = whiteSpaceSpacing;
        }
    }
}
