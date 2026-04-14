namespace IMDBImport;

public class Person_Model
{
    public int NConst { get; set; }
    public string PrimaryName { get; set; }
    public int? BirthYear { get; set; }
    public int? DeathYear { get; set; }

    public Person_Model(string[] info)
    {
        NConst = int.Parse(info[0].Substring(2));
        PrimaryName = info[1];
        BirthYear = ParseNullableInt(info[2]);
        DeathYear = ParseNullableInt(info[3]);
    }

    private int? ParseNullableInt(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        return null;
    }

    public override string ToString()
    {
        return NConst + " - " + PrimaryName;
    }
}
