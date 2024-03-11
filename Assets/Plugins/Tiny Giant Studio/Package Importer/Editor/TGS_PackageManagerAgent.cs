using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TinyGiantStudio.PackageImporter
{
    /// <summary>
    /// Handles talking to unity for information
    /// </summary>
    internal class TGS_PackageManagerAgent
    {
        //The request needed to get packages list
        private static ListRequest Request;

        //List of installed packages
        private static PackageCollection packages;

        //To check if still featching packages list
        private static bool isChecking;




        //Initialiization called after every domain reload on unity editor
        //[InitializeOnLoadMethod] //TODO: Seems unnecessary
        public static void Check()
        {
            //Requests for new list allowing for offline search
            Request = Client.List(offlineMode: true);

            //Adds callback in the Editor Application's update event
            EditorApplication.update += FetchList;
        }


        static void FetchList()
        {
            isChecking = true;

            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    packages = Request.Result;
                else if (Request.Status >= StatusCode.Failure)
                    Debug.LogWarning("Could not check for packages: " + Request.Error.message);

                //Marks as isChecking to false
                isChecking = false;

                //Remove callback from the Editor Application's update event
                EditorApplication.update -= FetchList;
            }
        }

        //To check if a package with the packagename parameter exists in the installed packages list
        public static bool ContainsPackage(string packageName)
        {
            //if packages list is null and not checking for packages
            if (packages == null && !isChecking)
            {
                //Request for package checking
                Check();

                //End function
                return false;
            }

            if (packages == null)
                return false;

            if (isChecking)
                return false;

            //Iterate through installed packages list
            foreach (var package in packages)
            {
                if (package == null)
                    continue;

                if (string.IsNullOrEmpty(package.name))
                    continue;

                //If package found with given name
                if (package.name == packageName)
                {
                    //End function and return that package exists
                    return true;
                }
            }

            //End function
            return false;
        }

        //To get the package if a package with the packagename parameter exists in the installed packages list
        public static UnityEditor.PackageManager.PackageInfo GetPackage(string packageName)
        {
            //if packages list is null and not checking for packages
            if (packages == null && !isChecking)
            {
                //Request for package checking
                Check();

                //End function
                return null;
            }

            if (isChecking)
                return null;

            //Iterate through installed packages list
            foreach (var package in packages)
            {
                //If package found with given name
                if (package.name == packageName)
                {
                    //End function and return that package
                    return package;
                }
            }

            return null;
        }
    }
}
