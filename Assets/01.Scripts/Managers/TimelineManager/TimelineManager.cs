using System;
using System.Collections;
using System.Collections.Generic;
using InputControl;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineQueueInfo
{
    public readonly PlayableDirector Director;
    public readonly TimelineType AssetType;
    public readonly bool SkipOnStart;
    public readonly Action Callback;

    public TimelineQueueInfo(PlayableDirector director, TimelineType assetType, bool skipOnStart, Action callback)
    {
        Director = director;
        AssetType = assetType;
        SkipOnStart = skipOnStart;
        Callback = callback;
    }
}

public class TimelineManager : BaseManager<TimelineManager>
{
    [SerializeField] private TimelineClipSetList _timelineClipSetList;
    [SerializeField] private float _skipOffset;

    [SerializeField] private float _speedMultiply;

    private Queue<TimelineQueueInfo> _timelineQueue;
    
    private PlayableDirector _currentDirector;
    private TimelineQueueInfo _currentPlayQueueInfo;

    public bool IsPlay
    {
        get
        {
            if (_currentDirector is null || !_currentDirector.playableGraph.IsValid())
            {
                return false;
            }

            return _currentDirector.playableGraph.GetRootPlayable(0).GetTime() <= _currentDirector.duration;
        }
    }
    
    public event Action AllTimelineEnd;

    public override void Init()
    {
        base.Init();
        _timelineQueue = new Queue<TimelineQueueInfo>();
    }

    public override void StartManager()
    {
        _currentDirector = null;
    }

    public void ShowTimeline(PlayableDirector director, TimelineType type, bool skipOnStart = false, Action onComplete = null)
    {
        _timelineQueue.Enqueue(new TimelineQueueInfo(director, type, skipOnStart, onComplete));
        while (skipOnStart && !_timelineQueue.Peek().SkipOnStart)
        {
            _timelineQueue.Enqueue(_timelineQueue.Dequeue());
        }

        // first timeline start
        if (!IsPlay)
        {
            InputManager.Instance.TimelineInputReader.OnSpeedUpEvent += SpeedUpHandle;
            InputManager.Instance.TimelineInputReader.CancelSpeedUpEvent += CancelSpeedUpHandle;
            PlayNextQueue();
        }
    }

    private void SetDirectorSpeed(float speed)
    {
        _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
    }

    private void PlayNextQueue()
    {
        if (_timelineQueue.Count <= 0)
        {
            return;
        }
        
        _currentPlayQueueInfo = _timelineQueue.Dequeue();

        _currentDirector = _currentPlayQueueInfo.Director;
        _currentDirector.playableAsset = _timelineClipSetList.GetAsset(_currentPlayQueueInfo.AssetType);

        StartSafeCoroutine("TimelinePlayRoutine", PlayRoutine(_currentPlayQueueInfo.Callback));

        if (_currentPlayQueueInfo.SkipOnStart)
        {
            _currentDirector.playableGraph.GetRootPlayable(0).SetTime(_currentDirector.duration - _skipOffset);
        }
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {
        _currentDirector.Play();
        yield return new WaitUntil(() => !IsPlay);
        _currentDirector.Stop();
        _currentDirector = null;

        onComplete?.Invoke();
        
        // complete last clip
        if (_timelineQueue.Count <= 0)
        {
            StoryManager.Instance.StartStoryIfCan(StoryAppearType.CUTSCENE_END, _currentPlayQueueInfo.AssetType);
            AllTimelineEnd?.Invoke();
            AllTimelineEnd = null;
            InputManager.Instance.TimelineInputReader.OnSpeedUpEvent -= SpeedUpHandle;
            InputManager.Instance.TimelineInputReader.CancelSpeedUpEvent -= CancelSpeedUpHandle;
            VolumeManager.Instance.SetVolume(VolumeType.Default, 0.1f);
        }
        else
        {
            PlayNextQueue();
        }
    }

    private void SpeedUpHandle()
    {
        VolumeManager.Instance.SetVolume(VolumeType.RetroSkip, 0.2f);
        SetDirectorSpeed(_speedMultiply);
    }

    private void CancelSpeedUpHandle()
    {
        VolumeManager.Instance.SetVolume(VolumeType.Default, 0.1f);
        SetDirectorSpeed(1f);
    }
}