namespace ManagingSystem
{
    public interface IManager
    {
        /// <summary>
        /// Start setting manager when call "Start"
        /// </summary>
        public void StartManager();
    
        /// <summary>
        /// Updating manager when call "Update"
        /// </summary>
        public void UpdateManager();
    }
}