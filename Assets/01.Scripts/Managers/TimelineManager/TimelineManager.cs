using System;
using System.Collections;
using System.Linq;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineManager : BaseManager<TimelineManager>
{
    [SerializeField] private TimelineClipSetList _timelineClipSetList;
    [SerializeField] private float _skipOffset;

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

            return _currentDirector.time <= _currentDirector.duration;
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
    
    [ContextMenu("Skip")]
    public void Skip()
    {
        if (_currentDirector == null)
        {
            Debug.LogError("[TimelineManager] The clip in play does not exist");
            return;
        }

        _currentDirector.time = _currentDirector.playableGraph.GetRootPlayable(0).GetDuration() - _skipOffset;

        // SetDirectorSpeed(2f);
    }

    private void SetDirectorSpeed(float speed)
    {
        _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {
        // InputManager.Instance.SetEnableInputAll(false);
        _currentDirector.Play();
        yield return new WaitUntil(() => !IsPlay);
        _currentDirector = null;
        // InputManager.Instance.SetEnableInputAll(true);
        onComplete?.Invoke();
    }
}