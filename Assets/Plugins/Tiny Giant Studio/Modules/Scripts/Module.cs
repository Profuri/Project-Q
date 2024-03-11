using System.Collections;
using UnityEngine;


namespace TinyGiantStudio.Modules
{
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/modules")]
    public abstract class Module : ModuleCore
    {
        public abstract IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders);
    }
}
