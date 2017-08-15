using UnityEditor;
using System.IO;
using UnityEngine;
using FinalWar;

public class CreateRandomDat
{
    [MenuItem("Create random.dat/Do")]
    public static void Start()
    {
        ConfigDictionary.Instance.LoadLocalConfig(Path.Combine(Application.streamingAssetsPath, "local.xml"));

        FileInfo fi = new FileInfo(Path.Combine(ConfigDictionary.Instance.random_path, "random.dat"));

        if (fi.Exists)
        {
            fi.Delete();
        }

        using (FileStream fs = fi.Create())
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                BattleRandomPool.Save(bw);
            }
        }
    }
}
