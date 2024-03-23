using UnityEngine;

public class VideoSettingPanel : MonoBehaviour
{
    public void GenerateResolutionDropdown(UIButton3D caller)
    {
        var dropdown = UIManager.Instance.GenerateUI("DropdownPanel") as UIDropdown;
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
            });
        }
    }
    
    public void GenerateQualityDropdown(UIButton3D caller)
    {
        
    }
}