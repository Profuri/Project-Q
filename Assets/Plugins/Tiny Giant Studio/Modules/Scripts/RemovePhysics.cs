/// Created by Ferdowsur Asif @ Tiny Giant Studios


using System.Collections;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    /// <summary>
    /// Variable Holders:
    /// 0 - Delay - Float
    /// </summary>
    [CreateAssetMenu(menuName = "Tiny Giant Studio/Modular 3D Text/Modules/Remove Physics")]
    public class RemovePhysics : Module
    {
        public override IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders)
        {
            yield return new WaitForSeconds(variableHolders[0].floatValue);
            if (obj)
            {
                if (obj.GetComponent<BoxCollider>())
                {
                    Destroy(obj.GetComponent<Rigidbody>());
                }
                if (obj.GetComponent<Rigidbody>())
                {
                    Destroy(obj.GetComponent<Rigidbody>());
                }
            }
        }
        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            return null;
        }
    }
}
