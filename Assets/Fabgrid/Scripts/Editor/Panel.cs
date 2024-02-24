namespace Fabgrid
{
    public class Panel
    {
        public Panel(string name, string stylesheetPath, string visualTreeAssetPath, State state, string buttonIconPath)
        {
            Name = name;
            StylesheetPath = stylesheetPath;
            VisualTreeAssetPath = visualTreeAssetPath;
            State = state;
            ButtonIconPath = buttonIconPath;
        }

        public string Name { get; protected set; }
        public string StylesheetPath { get; protected set; }
        public string VisualTreeAssetPath { get; protected set; }
        public State State { get; protected set; }
        public string ButtonIconPath { get; protected set; }
    }
}
