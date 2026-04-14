using Microsoft.Data.SqlClient;
using System.Data;

namespace IMDBImport;

public class BulkInserter : IInserter
{
    // For titles, vi skal lave en række for hver title med TConst,
    // TitleType, PrimaryTitle, OriginalTitle, IsAdult, StartYear, EndYear og RuntimeMinutes.
    public void InsertTitles(List<Title_Model> titles, SqlConnection conn)
    {
        var table = new DataTable();
        table.Columns.Add("TConst", typeof(int));
        table.Columns.Add("TitleType", typeof(string));
        table.Columns.Add("PrimaryTitle", typeof(string));
        table.Columns.Add("OriginalTitle", typeof(string));
        table.Columns.Add("IsAdult", typeof(bool));
        table.Columns.Add("StartYear", typeof(int));
        table.Columns.Add("EndYear", typeof(int));
        table.Columns.Add("RuntimeMinutes", typeof(int));

        foreach (var t in titles)
        {
            var row = table.NewRow();
            row["TConst"] = t.TConst;
            row["TitleType"] = t.TitleType ?? "";
            row["PrimaryTitle"] = t.PrimaryTitle?.Length > 500 ? t.PrimaryTitle[..500] : t.PrimaryTitle ?? "";
            row["OriginalTitle"] = t.OriginalTitle?.Length > 500 ? t.OriginalTitle[..500] : t.OriginalTitle ?? "";
            row["IsAdult"] = t.IsAdult;
            row["StartYear"] = (object?)t.StartYear ?? DBNull.Value;
            row["EndYear"] = (object?)t.EndYear ?? DBNull.Value;
            row["RuntimeMinutes"] = (object?)t.RuntimeMinutes ?? DBNull.Value;
            table.Rows.Add(row);
        }

        using var bulk = new SqlBulkCopy(conn);
        bulk.DestinationTableName = "Titles";
        bulk.BulkCopyTimeout = 0;
        bulk.ColumnMappings.Add("TConst", "TConst");
        bulk.ColumnMappings.Add("TitleType", "TitleType");
        bulk.ColumnMappings.Add("PrimaryTitle", "PrimaryTitle");
        bulk.ColumnMappings.Add("OriginalTitle", "OriginalTitle");
        bulk.ColumnMappings.Add("IsAdult", "IsAdult");
        bulk.ColumnMappings.Add("StartYear", "StartYear");
        bulk.ColumnMappings.Add("EndYear", "EndYear");
        bulk.ColumnMappings.Add("RuntimeMinutes", "RuntimeMinutes");
        bulk.WriteToServer(table);

        Console.WriteLine($"Inserted {titles.Count} titles.");
    }
    // For persons,vi skal lave en række for hver person med NConst,
    // PrimaryName, BirthYear og DeathYear.
    public void InsertPersons(List<Person_Model> persons, SqlConnection conn)
    {
        var table = new DataTable();
        table.Columns.Add("NConst", typeof(int));
        table.Columns.Add("PrimaryName", typeof(string));
        table.Columns.Add("BirthYear", typeof(int));
        table.Columns.Add("DeathYear", typeof(int));

        foreach (var p in persons)
        {
            var row = table.NewRow();
            row["NConst"] = p.NConst;
            row["PrimaryName"] = p.PrimaryName?.Length > 300 ? p.PrimaryName[..300] : p.PrimaryName ?? "";
            row["BirthYear"] = (object?)p.BirthYear ?? DBNull.Value;
            row["DeathYear"] = (object?)p.DeathYear ?? DBNull.Value;
            table.Rows.Add(row);
        }

        using var bulk = new SqlBulkCopy(conn);
        bulk.DestinationTableName = "Persons";
        bulk.BulkCopyTimeout = 0;
        bulk.ColumnMappings.Add("NConst", "NConst");
        bulk.ColumnMappings.Add("PrimaryName", "PrimaryName");
        bulk.ColumnMappings.Add("BirthYear", "BirthYear");
        bulk.ColumnMappings.Add("DeathYear", "DeathYear");
        bulk.WriteToServer(table);

        Console.WriteLine($"Inserted {persons.Count} persons.");
    }
    //crew er en liste af Crew_Model, hvor hver Crew_Model indeholder
    //en TConst og lister af NConst for henholdsvis directors og writers.
    //Vi skal derfor lave en række for hver kombination af TConst,
    //NConst og Role (director/writer).
    public void InsertCrew(List<Crew_Model> crew, SqlConnection conn)
    {
        var table = new DataTable();
        table.Columns.Add("TConst", typeof(int));
        table.Columns.Add("NConst", typeof(int));
        table.Columns.Add("Role", typeof(string));

        foreach (var c in crew)
        {
            foreach (int director in c.Directors)
            {
                var row = table.NewRow();
                row["TConst"] = c.TConst;
                row["NConst"] = director;
                row["Role"] = "director";
                table.Rows.Add(row);
            }
            foreach (int writer in c.Writers)
            {
                var row = table.NewRow();
                row["TConst"] = c.TConst;
                row["NConst"] = writer;
                row["Role"] = "writer";
                table.Rows.Add(row);
            }
        }

        using var bulk = new SqlBulkCopy(conn);
        bulk.DestinationTableName = "Crew";
        bulk.BulkCopyTimeout = 0;
        bulk.ColumnMappings.Add("TConst", "TConst");
        bulk.ColumnMappings.Add("NConst", "NConst");
        bulk.ColumnMappings.Add("Role", "Role");
        bulk.WriteToServer(table);

        Console.WriteLine($"Inserted {table.Rows.Count} crew rows.");
    }
}