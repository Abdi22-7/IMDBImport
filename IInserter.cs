using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using IMDBImport;

namespace IMDBImport;

public interface IInserter
{
    void InsertTitles(List<Title_Model> titles, SqlConnection conn);
    void InsertPersons(List<Person_Model> persons, SqlConnection conn);
    void InsertCrew(List<Crew_Model> crew, SqlConnection conn);
}
