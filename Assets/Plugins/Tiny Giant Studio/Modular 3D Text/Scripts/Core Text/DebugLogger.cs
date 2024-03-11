using UnityEngine;

namespace TinyGiantStudio.Text
{
    public class DebugLogger
    {
        private readonly Modular3DText text; //assigned by constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myText"></param>
        public DebugLogger(Modular3DText myText)
        {
            text = myText;
        }

        private bool LoggingTurnedOn()
        {
            if (!text.debugLogs) return false;

            if (Application.isPlaying && !text.runTimeLogging) return false;

            if (!Application.isPlaying && !text.editorTimeLogging) return false;

            return true;
        }

        public void LogToDeleteCharacters(int toDeleteCount)
        {
            if (LoggingTurnedOn() && text.logDeletedCharacters)
                Debug.Log("To delete : <color=green>" + toDeleteCount + "</color> chars on " + text.gameObject, text);
        }

        public void LogDeletedCharacters(string name)
        {
            if (LoggingTurnedOn() && text.logDeletedCharacters)
                Debug.Log("Destroy object is being called on  <color=green>" + name + "</color> for being unused character object", text);
        }

        public void LogTextUpdate(string oldText, string newText)
        {
            if (LoggingTurnedOn() && text.logTextUpdates)
                Debug.Log("Old Text is \"<color=green>" + oldText + "</color>\"" + " new Text is \"<color=green>" + newText + "</color>\"", text.gameObject);
        }

        public void LogFontUpdate(Font oldFont, Font newFont)
        {
            if (LoggingTurnedOn() && text.logFontUpdates)
                Debug.Log("Old Font is \"<color=green>" + oldFont.name + "</color>\"" + " new Font is \"<color=green>" + newFont.name + "</color>\"", text.gameObject);
        }

        public void LogMaterialUpdate(Material oldFont, Material newFont)
        {
            if (LoggingTurnedOn() && text.logMaterialUpdates)
                Debug.Log("Old Material is \"<color=green>" + oldFont.name + "</color>\"" + " new Material is \"<color=green>" + newFont.name + "</color>\"", text.gameObject);
        }

        /// <summary>
        /// Checking if 
        /// </summary>
        /// <param name="oldFont"></param>
        /// <param name="newFont"></param>
        public void LogSingleMeshStatus(bool usingSingleMesh)
        {
            if (LoggingTurnedOn() && text.logSingleMeshStatus)
            {
                bool hasChildObjects = text.transform.childCount > 0;
                bool hasMeshFilter = text.GetComponent<MeshFilter>() != null;
                Debug.Log(text.gameObject.name + " is using single mesh : <color=white>" + usingSingleMesh + "</color>  has child objects : <color=white>" + hasChildObjects + "</color> has mesh filter :" + hasMeshFilter);
            }
        }
    }
}