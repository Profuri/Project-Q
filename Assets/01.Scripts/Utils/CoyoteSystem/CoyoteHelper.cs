using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coyote
{
    private double _coyoteTime;
    public bool IsTrue;
    public Coyote(double time)
    {
        _coyoteTime = Time.time + time;
    }

    public void CheckIsTrue(float currentTime)
    {
        IsTrue = _coyoteTime <= currentTime;
    }
}

public class CoyoteHelper
{
    public Dictionary<int,Coyote> CoyoteDictionary { get; private set; }
    private MonoBehaviour parent;

    public CoyoteHelper(MonoBehaviour parent)
    {
        CoyoteDictionary = new Dictionary<int, Coyote>();
        this.parent = parent;

        parent.StartCoroutine(CheckIsTrue());
    }

    private IEnumerator CheckIsTrue()
    {
        while(true)
        {
            if(CoyoteDictionary.Count > 0)
            {
                foreach(var kvp in CoyoteDictionary)
                {
                    kvp.Value.CheckIsTrue(Time.time);
                }
                yield return null;
            }
        }
    }

    public bool AddCoyote(int guid, float time)
    {
        bool returnValue = false;
        if(!CoyoteDictionary.ContainsKey(guid))
        {
            Coyote coyote = new Coyote(time);
            CoyoteDictionary.Add(guid, coyote);
            returnValue = coyote.IsTrue;
        }
        else
        {
            returnValue = CoyoteDictionary[guid].IsTrue;
        }

        if(returnValue)
        {
            CoyoteDictionary.Remove(guid);
        }
        return returnValue;
    }
}
