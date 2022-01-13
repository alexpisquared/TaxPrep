namespace TxnManualEntry2022.Models
{
  public class TxnTypeID // room id
  {
    public TxnTypeID(int iD, string name)
    {
      FloorNum = iD;
      RoomNum = name;
    }

    public int FloorNum { get;  }
    public string RoomNum { get; } = default!;

    public override bool Equals(object? obj) => obj is TxnTypeID tti && FloorNum == tti.FloorNum;

    override public int GetHashCode() => HashCode.Combine(RoomNum, FloorNum);

    public override string ToString() => $":/> {FloorNum} {RoomNum}";

    public static bool operator ==(TxnTypeID? t1, TxnTypeID? t2)
    {
      if (t1 is null && t2 is null) return true;

      return t1 is not null && t1.Equals(t2);
    }
    public static bool operator !=(TxnTypeID? t1, TxnTypeID? t2) { return !(t1 == t2); }

  }
}
