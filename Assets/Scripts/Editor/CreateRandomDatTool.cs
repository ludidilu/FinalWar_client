using UnityEditor;
using System.IO;
using FinalWar;

public class CreateRandomDatTool
{
    [MenuItem("Create random.dat/Do")]
    public static void Start()
    {
        ResourceLoader.LoadConfigLocal();

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
