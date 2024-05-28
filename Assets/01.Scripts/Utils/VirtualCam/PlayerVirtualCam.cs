namespace VirtualCam
{
    public class PlayerVirtualCam : VirtualCamComponent
    {
        public override void EnterCam()
        {
            var localEuler = transform.localEulerAngles;
            localEuler.y = CameraManager.Instance.LastRotateValue + 5;
            transform.localEulerAngles = localEuler;
            
            InputManager.Instance.CameraInputReader.OnRotateRightEvent += CamRotateRight;
            InputManager.Instance.CameraInputReader.OnRotateLeftEvent += CamRotateLeft;
            
            base.EnterCam();
        }

        public override void ExitCam()
        {
            InputManager.Instance.CameraInputReader.OnRotateRightEvent -= CamRotateRight;
            InputManager.Instance.CameraInputReader.OnRotateLeftEvent -= CamRotateLeft;

            base.ExitCam();
        }
        
        private void CamRotateRight()
        {
            if (SceneControlManager.Instance.CurrentScene.Type != SceneType.Stage)
            {
                return;
            }

            RotateCam(CameraManager.Instance.LastRotateValue - CameraManager.Instance.RotateValue + 5f, CameraManager.Instance.RotateTime);
            LightManager.Instance.RotateDefaultDirectionalLight(CameraManager.Instance.LastRotateValue - (CameraManager.Instance.RotateValue - 15f), CameraManager.Instance.RotateTime);
            CameraManager.Instance.LastRotateValue -= CameraManager.Instance.RotateValue;
        }
        
        private void CamRotateLeft()
        {
            if (SceneControlManager.Instance.CurrentScene.Type != SceneType.Stage)
            {
                return;
            }
            
            RotateCam(CameraManager.Instance.LastRotateValue + CameraManager.Instance.RotateValue - 5f, CameraManager.Instance.RotateTime);
            LightManager.Instance.RotateDefaultDirectionalLight(CameraManager.Instance.LastRotateValue + (CameraManager.Instance.RotateValue + 15f), CameraManager.Instance.RotateTime);
            CameraManager.Instance.LastRotateValue += CameraManager.Instance.RotateValue;
        }
    }
}