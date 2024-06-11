using System;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Core
{
    public static class Define
    {
        private static Camera _mainCam;
        public static Camera MainCam
        {
            get
            {
                if (_mainCam == null)
                {
                    _mainCam = Camera.main;
                }

                return _mainCam;
            }
        }
    }

    public static class MessageUtil
    {
        public static void CallMessageEvent(EventMarker eventMarker)
        {
            switch (eventMarker.name)
            {
                case "camDampingChange":
                    CamDampingChangeHandle(eventMarker);
                    break;
                case "camFollowTargetChange":
                    CamFollowTargetChangeHandle(eventMarker);
                    break;
                case "camOffsetChange":
                    CamOffsetChangeHandle(eventMarker);
                    break;
                case "camSizeChange":
                    CamSizeChangeHandle(eventMarker);
                    break;
                case "setTutorialProps":
                    SetTutorialPropsHandle(eventMarker);
                    break;
                case "PlayVideo":
                    PlayVideo(eventMarker);
                    break;
                case "StopVideo":
                    StopVideo();
                    break;
            }
        }

        private static void SetTutorialPropsHandle(EventMarker eventMarker)
        {
            var propsName = eventMarker.parameters[0];
            var enable = eventMarker.parameters[1].ToLower() == "true"; 
            var propsController = StageManager.Instance.CurrentStage.transform.Find($"{propsName}Props")
                .GetComponent<TutorialPropController>();

            if (enable)
            {
                propsController.ShowProps();
            }
            else
            {
                propsController.UnShowProps();
            }
        }

        private static void CamDampingChangeHandle(EventMarker eventMarker)
        {
            var xDamping = Convert.ToSingle(eventMarker.parameters[0]);
            var yDamping = Convert.ToSingle(eventMarker.parameters[1]);
            var zDamping = Convert.ToSingle(eventMarker.parameters[2]);
            CameraManager.Instance.ActiveVCam.SetDamping(new Vector3(xDamping, yDamping, zDamping));
        }

        private static void CamFollowTargetChangeHandle(EventMarker eventMarker)
        {
            var targetName = eventMarker.parameters[0];
            CameraManager.Instance.ActiveVCam.SetFollowTarget(GameObject.Find(targetName).transform);
        }

        private static void CamOffsetChangeHandle(EventMarker eventMarker)
        {
            var offsetX = Convert.ToSingle(eventMarker.parameters[0]);
            var offsetY = Convert.ToSingle(eventMarker.parameters[1]);
            var offsetZ = Convert.ToSingle(eventMarker.parameters[2]);
            var offset = new Vector3(offsetX, offsetY, offsetZ);
            CameraManager.Instance.ActiveVCam.SetOffset(offset);
        }

        private static void CamSizeChangeHandle(EventMarker eventMarker)
        {
            var targetSize = Convert.ToSingle(eventMarker.parameters[0]);
            var time = Convert.ToSingle(eventMarker.parameters[1]);
            CameraManager.Instance.ActiveVCam.Zoom(targetSize, time);
        }

        private static void PlayVideo(EventMarker eventMarker)
        {
            var clipName = eventMarker.parameters[0];
            var storyData = StoryManager.Instance.CurrentPlayStoryData;

            if (storyData == null)
            {
                return;
            }

            var clip = storyData.videoClips.Find(clip => clip.name == clipName);
            StoryManager.Instance.PlayMessageVideo(clip);
        }

        private static void StopVideo()
        {
            StoryManager.Instance.StopMessageVideo();
        }
    }
}