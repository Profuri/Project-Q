using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    [System.Serializable]
    public class ModuleContainer
    {
        /// <summary>
        /// If you are updating/changing modules runtime, remember to call UpdateVariableHolders() to update the variable holders length. Otherwise, it won't work.
        /// </summary>
        public Module module;
       
        public VariableHolder[] variableHolders;

        public void UpdateVariableHolders()
        {
            if (module == null) return;

            if (module.variableHolders == null) return;

            if (variableHolders == null || variableHolders.Length != module.variableHolders.Length)
            {
                variableHolders = new VariableHolder[module.variableHolders.Length];
                for (int k = 0; k < variableHolders.Length; k++)
                {
                    if (k < module.variableHolders.Length)
                    {
                        variableHolders[k] = module.variableHolders[k];
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class VariableHolder 
    {
        public string variableName;
        public ModuleVariableType type;
        //[HideInInspector]
        public float floatValue;
        //[HideInInspector]
        public int intValue;
        //[HideInInspector]
        public bool boolValue;
        //[HideInInspector]
        public string stringValue;
        //[HideInInspector]
        public Vector2 vector2Value;
        //[HideInInspector]
        public Vector3 vector3Value;
        //[HideInInspector]
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        //[HideInInspector]
        public GameObject gameObjectValue;
        //[HideInInspector]
        public PhysicMaterial physicMaterialValue;
        //[HideInInspector]
        public string hideIf;
        //[HideInInspector]
        public string tooltip = string.Empty;
    }

    [System.Serializable]
    public enum ModuleVariableType
    {
        @float,
        @int,
        @bool,
        @string,
        vector2,
        vector3,
        animationCurve,
        gameObject,
        physicMaterial
    }
}