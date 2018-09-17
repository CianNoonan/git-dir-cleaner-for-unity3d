using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class GitDirCleaner : MonoBehaviour
{
    const string GitFolderInternalPath = "/hooks/post-merge";
    const string SuccessString = " Git-Hooks-successfully-applied ";

    //Unity calls the static constructor when the engine opens
    static GitDirCleaner()
    {
        var projectPath = new DirectoryInfo(Application.dataPath).Parent;
        var hookFilePath = projectPath.Parent.GetFiles("post-merge", SearchOption.AllDirectories); //Find file anywhere in the repo
        var ClassPath = projectPath.GetFiles("GitDirCleaner.cs");
        var classImporter = AssetImporter.GetAtPath(ClassPath[0].FullName);

        if (classImporter.userData.Contains(SuccessString)) return;

        var running = true;
        do
        {
            if(projectPath == null)
            {
                Debug.LogError(".git folder cannot be found! Git Hooks cannot be auto applied");
                running = false;
            }

            var childDir = projectPath.GetDirectories(".git", SearchOption.TopDirectoryOnly);
            if (childDir.Length != 0)
            {
                File.Copy(hookFilePath[0].FullName, childDir[0].FullName + GitFolderInternalPath, true);
                running = false;
                Debug.Log(SuccessString);
                classImporter.userData = classImporter.userData + SuccessString;
            }
            else
            {
                //Continue one path up
                projectPath = projectPath.Parent;
            }
        } while (running);
    }
}
