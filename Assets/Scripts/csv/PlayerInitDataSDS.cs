using FinalWar;

public partial class PlayerInitDataSDS : CsvBase, IPlayerInitDataSDS
{
    public int deckCardsNum;
    public int addCardsNum;
    public int addMoney;
    public int defaultHandCardsNum;
    public int defaultMoney;

    public int GetDeckCardsNum()
    {
        return deckCardsNum;
    }

    public int GetAddCardsNum()
    {
        return addCardsNum;
    }

    public int GetAddMoney()
    {
        return addMoney;
    }

    public int GetDefaultHandCardsNum()
    {
        return defaultHandCardsNum;
    }

    public int GetDefaultMoney()
    {
        return defaultMoney;
    }
}
