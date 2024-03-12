using System.Collections;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    /// <summary>
    /// Variable holders:
    /// Index 0: Delay | float
    /// Index 1: Duration | float
    /// Index 2: Grow from default scale | bool
    /// Index 3: Grow from | vector3
    /// Index 4: Grow to original | bool
    /// Index 5: Grow to | vector3
    /// Index 6: Scale curve | Animation curve
    /// </summary>
    [CreateAssetMenu(menuName = "Tiny Giant Studio/Modular 3D Text/Modules/Change Scale")]
    public class ScaleChange : Module
    {
        public override IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders)
        {
            if (obj != null && variableHolders != null)
            {
                if (variableHolders.Length >= 6)
                {
                    Transform transform = obj.transform;

                    Vector3 startScale = variableHolders[3].vector3Value; //Grow from
                    if (variableHolders[2].boolValue) //Grow from default scale
                        startScale = transform.localScale;

                    Vector3 targetScale = variableHolders[5].vector3Value; //Grow to
                    if (variableHolders[4].boolValue) //Grow to default scale
                        targetScale = transform.localScale;

                    float timer = 0;
                    float duration = variableHolders[1].floatValue;
                    AnimationCurve animationCurve = variableHolders[6].animationCurve;

                    yield return new WaitForSeconds(variableHolders[0].floatValue); // Delay before starting
                    while (timer < duration)
                    {
                        if (!transform)
                            break;

                        float perc = timer / duration;
                        transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, animationCurve.Evaluate(perc));
                        timer += Time.deltaTime;

                        yield return null;
                    }
                    if (transform)
                        transform.localScale = targetScale;
                }
            }
        }

        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            if (variableHolders == null)
                return null;

            string warning = string.Empty;
            if (variableHolders != null)
            {
                if (variableHolders.Length > 1)
                {
                    try
                    {
                        if (variableHolders[1].floatValue <= 0) // duration
                        {
                            warning += AddWarning("Invalid duration input.", warning);
                        }
                        if (variableHolders[2].boolValue && variableHolders[4].boolValue)
                        {
                            warning += AddWarning("Changing scale from default to default. No change detected.", warning);
                        }
                        if ((!variableHolders[2].boolValue && !variableHolders[4].boolValue) && variableHolders[3].vector3Value == variableHolders[5].vector3Value)
                        {
                            warning += AddWarning("No change to scale detected.", warning);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return warning;
        }
    }
}