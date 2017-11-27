using System.IO;
using UnityEngine;
using UnityEditor;

public static class CopyAssets
{
    [MenuItem("Copy Assets/Do")]
    public static void Start()
    {
        ResourceLoader.LoadConfigLocal();

        DirectoryInfo di = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "ai"));

        if (di.Exists)
        {
            di.Delete(true);
        }

        di.Create();

        string path = Path.Combine(ConfigDictionary.Instance.ai_path, "ai_action.xml");

        string path2 = Path.Combine(di.FullName, "ai_action.xml");

        File.Copy(path, path2);

        path = Path.Combine(ConfigDictionary.Instance.ai_path, "ai_summon.xml");

        path2 = Path.Combine(di.FullName, "ai_summon.xml");

        File.Copy(path, path2);

        di = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "map"));

        if (di.Exists)
        {
            di.Delete(true);
        }

        di.Create();

        DirectoryInfo di2 = new DirectoryInfo(ConfigDictionary.Instance.map_path);

        for (int i = 0; i < di2.GetFiles().Length; i++)
        {
            FileInfo fi = di2.GetFiles()[i];

            if (fi.Extension == ".map")
            {
                File.Copy(fi.FullName, Path.Combine(di.FullName, fi.Name));
            }
        }
    }
}
