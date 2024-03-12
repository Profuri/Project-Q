using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// The purpose of this script is to update all instance of a font.
    /// Used by editor script to update scene objects
    /// </summary>
    public class FontInstanceUpdater
    {
        //Called after fields are changed in inspector
        public void ApplyFontChangesToTheScene(Font targetFont)
        {
            List<GameObject> allObjectInScene = GetAllObjectsOnlyInScene();
            List<Modular3DText> texts = new List<Modular3DText>();
            for (int i = 0; i < allObjectInScene.Count; i++)
            {
                if (allObjectInScene[i].GetComponent<Modular3DText>())
                    texts.Add(allObjectInScene[i].GetComponent<Modular3DText>());
            }

            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].Font == targetFont)
                {
                    texts[i].CleanUpdateText();
                }
            }
        }

        List<GameObject> GetAllObjectsOnlyInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                    objectsInScene.Add(go);
            }

            return objectsInScene;
        }
    }
}