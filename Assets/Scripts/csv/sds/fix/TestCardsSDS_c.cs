using System.IO;
public class TestCardsSDS_c {
    public static void Init(TestCardsSDS _csv, BinaryReader _br){
        _csv.ID = _br.ReadInt32();
        int lengthcards = _br.ReadInt32();
        _csv.cards = new int[lengthcards];
        for(int i = 0 ; i < lengthcards ; i++){
            _csv.cards[i] = _br.ReadInt32();
        }
    }
}
