using System.IO;
public class DescSDS_c {
    public static void Init(DescSDS _csv, BinaryReader _br){
        _csv.ID = _br.ReadInt32();
        _csv.desc = _br.ReadString();
    }
}
