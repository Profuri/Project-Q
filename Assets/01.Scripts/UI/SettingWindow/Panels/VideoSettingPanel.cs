using System;
using UnityEngine;

public class VideoSettingPanel : MonoBehaviour
{
    public void GenerateResolutionDropdown(UIButton3D caller)
    {
        UIManager.Instance.Interact3DButton = false;
        
        var dropdown = UIManager.Instance.GenerateUI("DropdownPanel") as UIDropdown;
        dropdown.Title = "RESOLUTION";

        for (var i = 0; i < VideoManager.Instance.Resolutions.Length; i++)
        {
            var resolution = VideoManager.Instance.Resolutions[i];
            var width = resolution.width;
            var height = resolution.height;
            var index = i;

            dropdown.AddOption($"{width} x {height}", () =>
            {
                caller.Text = $"{width}x{height}";
                VideoManager.Instance.SetResolution(index);
                UIManager.Instance.Interact3DButton = true;
            });
        }
        
        var mousePoint = InputManager.Instance.InputReader.mouseScreenPoint;
        dropdown.SetPosition(mousePoint);
    }
    
    public void FullScreenSetting(bool fullScreen)
    {
        VideoManager.Instance.SetFullScreen(fullScreen);
    }
    
    public void GenerateQualityDropdown(UIButton3D caller)
    {
        UIManager.Instance.Interact3DButton = false;
        
        var dropdown = UIManager.Instance.GenerateUI("DropdownPanel") as UIDropdown;
        dropdown.Title = "QUALITY";

        foreach (QualityType quality in Enum.GetValues(typeof(QualityType)))
        {
            var qualityName = quality.ToString();
            
            dropdown.AddOption(qualityName, () =>
            {
                caller.Text = qualityName.ToUpper();
               VideoManager.Instance.SetQuality(quality); 
               UIManager.Instance.Interact3DButton = true;
            });
        }
        
        var mousePoint = InputManager.Instance.InputReader.mouseScreenPoint;
        dropdown.SetPosition(mousePoint);
    }
}