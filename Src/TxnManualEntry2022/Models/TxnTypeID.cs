namespace TxnManualEntry2022.Models
{
  public class TxnTypeID // room id
  {
    public TxnTypeID(int iD, string name)
    {
      ID = iD;
      Name = name;
    }

    public int ID { get;  }
    public string Name { get; } = default!;

    public override bool Equals(object? obj) => obj is TxnTypeID tti && ID == tti.ID;

    override public int GetHashCode() => HashCode.Combine(Name, ID);

    public override string ToString() => $":/> {ID} {Name}";

    public static bool operator ==(TxnTypeID? t1, TxnTypeID? t2)
    {
      if (t1 is null && t2 is null) return true;

      return t1 is not null && t1.Equals(t2);
    }
    public static bool operator !=(TxnTypeID? t1, TxnTypeID? t2) { return !(t1 == t2); }

  }
}
