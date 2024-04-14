using System;
using System.Collections;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : BaseManager<TimelineManager>
{
    [SerializeField] private TimelineClipSetList _timelineClipSetList;

    private PlayableDirector _currentDirector;

    public bool IsPlay
    {
        get
        {
            if (_currentDirector == null)
            {
                Debug.LogError("[TimelineManager] The clip in play does not exist");
                return false;
            }

            return _currentDirector.time < _currentDirector.duration;
        }
    }
    
    public override void StartManager()
    {
        _currentDirector = null;
    }

    public void ShowTimeline(PlayableDirector director, TimelineType type, Action onComplete = null)
    {
        _currentDirector = director;
        _currentDirector.playableAsset = _timelineClipSetList.GetAsset(type);

        CoroutineManager.Instance.StartSafeCoroutine(GetInstanceID(), PlayRoutine(onComplete));
    }

    public void CancelSkip()
    {
        if (_currentDirector == null)
        {
            Debug.LogError("[TimelineManager] The clip in play does not exist");
            return;
        }
        
        SetDirectorSpeed(1f);
    }

    public void Skip()
    {
        if (_currentDirector == null)
        {
            Debug.LogError("[TimelineManager] The clip in play does not exist");
            return;
        }
        
        SetDirectorSpeed(2f);
    }

    private void SetDirectorSpeed(float speed)
    {
        _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {
        _currentDirector.Play();
        yield return new WaitUntil(() => IsPlay);
        _currentDirector = null;
        onComplete?.Invoke();
    }
}