namespace IMDBImport;

public class Crew_Model
{
    public int TConst { get; set; }
    public List<int> Directors { get; set; } = new List<int>();
    public List<int> Writers { get; set; } = new List<int>();

    public Crew_Model(string[] crewInfo)
    {
        TConst = int.Parse(crewInfo[0].Substring(2));
        //Directors
        if (crewInfo[1] != @"\N")
        {
            foreach (string d in crewInfo[1].Split(','))
                if (int.TryParse(d.Replace("nm", ""), out int id))
                    Directors.Add(id);
        }
        //Writers
        if (crewInfo[2] != @"\N")
        {
            foreach (string w in crewInfo[2].Split(','))
                if (int.TryParse(w.Replace("nm", ""), out int id))
                    Writers.Add(id);
        }
    }
}
