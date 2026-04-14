using Microsoft.Data.SqlClient;
using IMDBImport;
using System.Diagnostics;

try
{
    Console.WriteLine("IMDB Import");
    Stopwatch stopwatch = Stopwatch.StartNew();
    IInserter inserter = new BulkInserter();
    SqlConnection sqlConn = new SqlConnection(
        "Server=LAPTOP-I11RFB6M;Database=IMDB;Integrated Security=True;" +
        "Trusted_Connection=True;TrustServerCertificate=True;");

    //TITLES 
    List<Title_Model> movies = new List<Title_Model>();
    foreach (string movie in File.ReadLines(@"C:\Users\Abdim\Desktop\DATABASE4Sem\title.basics.tsv").Skip(1))
    {
        string[] parts = movie.Split('\t');
        if (parts.Length == 9)
            movies.Add(new Title_Model(parts));
    }
    Console.WriteLine("Læst titles fra fil: " + stopwatch.ElapsedMilliseconds + "ms");
    stopwatch.Restart();
    sqlConn.Open();
    inserter.InsertTitles(movies, sqlConn);
    sqlConn.Close();
    Console.WriteLine("Indsat titles i DB: " + stopwatch.ElapsedMilliseconds + "ms");


    //PERSONS
    Console.WriteLine("Starter import af persons...");
    stopwatch.Restart();
    List<Person_Model> persons = new List<Person_Model>();
    foreach (string person in File.ReadLines(@"C:\Users\Abdim\Desktop\DATABASE4Sem\name.basics.tsv").Skip(1))
    {
        string[] parts = person.Split('\t');
        if (parts.Length >= 4)
            persons.Add(new Person_Model(parts));
    }
    Console.WriteLine("Læst persons fra fil: " + stopwatch.ElapsedMilliseconds + "ms");
    stopwatch.Restart();
    sqlConn.Open();
    inserter.InsertPersons(persons, sqlConn);
    sqlConn.Close();
    Console.WriteLine("Indsat persons i DB: " + stopwatch.ElapsedMilliseconds + "ms");


    // CREW
    Console.WriteLine("Starter import af crew...");
    stopwatch.Restart();
    List<Crew_Model> crew = new List<Crew_Model>();
    foreach (string line in File.ReadLines(@"C:\Users\Abdim\Desktop\DATABASE4Sem\title.crew.tsv").Skip(1))
    {
        string[] parts = line.Split('\t');
        if (parts.Length >= 3)
            crew.Add(new Crew_Model(parts));
    }
    Console.WriteLine("crew fra fil: " + stopwatch.ElapsedMilliseconds + "ms");
    stopwatch.Restart();
    sqlConn.Open();
    inserter.InsertCrew(crew, sqlConn);
    sqlConn.Close();
    Console.WriteLine("Indsat crew i DB: " + stopwatch.ElapsedMilliseconds + "ms");
}
catch (Exception ex)
{
    Console.WriteLine("FEJL: " + ex.Message);
    Console.WriteLine(ex.StackTrace);
}

Console.WriteLine("Tryk en tast for at afslutte...");
Console.ReadKey();