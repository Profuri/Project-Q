using System;
using Unity.VisualScripting;
using UnityEngine;

public class VideoSettingPanel : WindowPanel
{
    [SerializeField] private UIButton3D _resolutionBtn;
    [SerializeField] private UIButton3D _qualityBtn;

    [SerializeField] private UIYesNoButton3D _fullScreenYesBtn;
    [SerializeField] private UIYesNoButton3D _fullScreenNoBtn;

    private void OnDisable()
    {
        ReleasePanel();
    }
    
    public void GenerateResolutionDropdown()
    {
        UIManager.Instance.Interact3DButton = false;
        
        var dropdown = UIManager.Instance.GenerateUI("DropdownPanel") as UIDropdown;
        dropdown.Title = "RESOLUTION";

        for (var i = 0; i < VideoManager.Instance.ResolutionList.Count; i++)
        {
            var resolution = VideoManager.Instance.ResolutionList[i];
            var width = resolution.width;
            var height = resolution.height;
            var index = i;

            dropdown.AddOption($"{width} x {height}", () =>
            {
                _resolutionBtn.Text = $"{width}x{height}";
                VideoManager.Instance.SetResolution(index);
                VideoManager.Instance.SetFullScreen(Screen.fullScreen);
                UIManager.Instance.Interact3DButton = true;
            });
        }
        
        var mousePoint = InputManager.Instance.UIInputReader.mouseScreenPoint;
        var mousePercent = new Vector2(mousePoint.x / Screen.width, mousePoint.y / Screen.height);

        var canvasRect = ((RectTransform)UIManager.Instance.MainCanvas2D.transform).rect;
        var canvasPos = new Vector2(canvasRect.width * mousePercent.x, canvasRect.height * mousePercent.y);
        
        dropdown.SetPosition(canvasPos);
    }
    
    public void FullScreenSetting(bool fullScreen)
    {
        VideoManager.Instance.SetFullScreen(fullScreen);
    }
    
    public void GenerateQualityDropdown()
    {
        UIManager.Instance.Interact3DButton = false;
        
        var dropdown = UIManager.Instance.GenerateUI("DropdownPanel") as UIDropdown;
        dropdown.Title = "QUALITY";

        foreach (QualityType quality in Enum.GetValues(typeof(QualityType)))
        {
            var qualityName = quality.ToString();
            
            dropdown.AddOption(qualityName, () =>
            {
                _qualityBtn.Text = qualityName.ToUpper();
               VideoManager.Instance.SetQuality(quality); 
               UIManager.Instance.Interact3DButton = true;
            });
        }
        
        var mousePoint = InputManager.Instance.UIInputReader.mouseScreenPoint;
        dropdown.SetPosition(mousePoint);
    }

    public override void LoadPanel()
    {
        base.LoadPanel();
        DataManager.Instance.LoadData(VideoManager.Instance);
        SettingUI(DataManager.sSaveData);
    }

    private void SettingUI(SaveData saveData)
    {
        //Resolution
        Resolution resolution;
        try
        { 
            resolution = VideoManager.Instance.ResolutionList[(int)saveData.ResolutionIndex];
        }
        catch
        {
            var resolutionList = VideoManager.Instance.ResolutionList;
            
            resolution = resolutionList[resolutionList.Count - 1];
            string resolutionText = $"{resolution.width} X {resolution.height}";
            _resolutionBtn.Text = resolutionText;
        }
        
        //Qaulity
        QualityType qualityType = saveData.DefaultQuality;
        _qualityBtn.Text = qualityType.ToString().ToUpper();
        
        //FullScreen
        _fullScreenYesBtn.SettingActive(saveData.DefaultFullScreen);
        _fullScreenNoBtn.SettingActive(!saveData.DefaultFullScreen);
    }
    
    
    
    public override void ReleasePanel()
    {
        base.ReleasePanel();
        DataManager.Instance.LoadData(VideoManager.Instance);
        SettingUI(DataManager.sSaveData);
    }

    public void SaveDatas()
    {
        SoundManager.Instance.PlaySFX("UIApply", false);
        DataManager.Instance.SaveData(VideoManager.Instance);
    }
}