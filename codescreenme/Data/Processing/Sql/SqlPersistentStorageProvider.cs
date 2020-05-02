using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data.Processing.Sql
{
  public class SqlPersistentStorageProvider : IPersistentStorageProvider
  {
    private string connectionString;
    public SqlPersistentStorageProvider(string connectionString)
    {
      this.connectionString = connectionString;
    }
    public bool AddCodeSession(CodeSession codeSession)
    {
      string insertNewCodeSession = "INSERT INTO CodeSessions (Id, DateCreated, Code, CodeSyntax, CodeHighlights, Owner, UserInControl, Participants)" +
        " VALUES (@Id, @DateCreated, @Code, @CodeSyntax, @CodeHighlights, @Owner, @UserInControl, @Participants)";

      using (var connection = new SqlConnection(this.connectionString))
      {
        try
        {
          SqlCommand command = new SqlCommand(insertNewCodeSession, connection);

          command.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 20);
          command.Parameters.Add("@DateCreated", System.Data.SqlDbType.DateTime2, 7);
          command.Parameters.Add("@Code", System.Data.SqlDbType.NVarChar, -1);
          command.Parameters.Add("@CodeSyntax", System.Data.SqlDbType.NVarChar, 100);
          command.Parameters.Add("@CodeHighlights", System.Data.SqlDbType.NVarChar, -1);
          command.Parameters.Add("@Owner", System.Data.SqlDbType.NVarChar, 100);
          command.Parameters.Add("@UserInControl", System.Data.SqlDbType.NVarChar, 100);
          command.Parameters.Add("@Participants", System.Data.SqlDbType.NVarChar, -1);

          // Add the parameter values.  Validation should have already happened.
          command.Parameters["@Id"].Value = codeSession.Id;
          command.Parameters["@DateCreated"].Value = codeSession.DateCreated;
          command.Parameters["@Code"].Value = codeSession.Code;
          command.Parameters["@CodeSyntax"].Value = codeSession.CodeSyntax;
          command.Parameters["@CodeHighlights"].Value = JsonConvert.SerializeObject(codeSession.CodeHighlights);
          command.Parameters["@Owner"].Value = codeSession.Owner;
          command.Parameters["@UserInControl"].Value = codeSession.UserInControl;
          command.Parameters["@Participants"].Value = JsonConvert.SerializeObject(codeSession.Participants);

          connection.Open();
          command.ExecuteNonQuery();
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }
    }

    public CodeSession GetCodeSession(string id)
    {
      string getOne = "SELECT * FROM CodeSessions WHERE [Id] = @Id";

      using (var connection = new SqlConnection(this.connectionString))
      {
        try
        {
          SqlCommand command = new SqlCommand(getOne, connection);

          command.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 20);

          // Add the parameter values.  Validation should have already happened.
          command.Parameters["@Id"].Value = id;

          connection.Open();
          var reader = command.ExecuteReader();

          if (reader.HasRows)
          {
            reader.Read();

            return new CodeSession()
            {
              Id = reader.GetString(0),
              DateCreated = reader.GetDateTime(1),
              Code = reader.GetString(2),
              CodeSyntax = reader.GetString(3),
              CodeHighlights = JsonConvert.DeserializeObject<IList<CodeCursor>>(reader.GetString(4)),
              Owner = reader.GetString(5),
              UserInControl = reader.GetString(6),
              Participants = JsonConvert.DeserializeObject<IList<string>>(reader.GetString(7))
            };
          }

          return null;
        }
        catch (Exception)
        {
          return null;
        }
      }
    }

    public IEnumerable<CodeSession> GetCodeSessions()
    {
      string readerAll = "SELECT * FROM CodeSessions";
      List<CodeSession> result = new List<CodeSession>();

      using (var connection = new SqlConnection(this.connectionString))
      {
        try
        {
          SqlCommand command = new SqlCommand(readerAll, connection);

          connection.Open();
          var reader = command.ExecuteReader();

          if (reader.HasRows)
          {
            while (reader.Read())
            {
              var cs = new CodeSession()
              {
                Id = reader.GetString(0),
                DateCreated = reader.GetDateTime(1),
                Code = reader.GetString(2),
                CodeSyntax = reader.GetString(3),
                CodeHighlights = JsonConvert.DeserializeObject<IList<CodeCursor>>(reader.GetString(4)),
                Owner = reader.GetString(5),
                UserInControl = reader.GetString(6),
                Participants = JsonConvert.DeserializeObject<IList<string>>(reader.GetString(7))
              };

              result.Add(cs);
            }

            reader.Close();
          }

          return result;
        }
        catch (Exception)
        {
          return Enumerable.Empty<CodeSession>();
        }
      }
    }

    public bool RemoveCodeSession(string id)
    {
      string insertNewCodeSession = "DELETE FROM CodeSessions WHERE [Id] = @Id";

      using (var connection = new SqlConnection(this.connectionString))
      {
        try
        {
          SqlCommand command = new SqlCommand(insertNewCodeSession, connection);

          command.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 20);

          // Add the parameter values.  Validation should have already happened.
          command.Parameters["@Id"].Value = id;

          connection.Open();
          return (command.ExecuteNonQuery() > 0);
        }
        catch (Exception)
        {
          return false;
        }
      }
    }

    public bool UpdateCodeSession(string id, string code, string syntax, IList<CodeCursor> highlights, IList<string> participants, string userInControl)
    {
      string updateSession = "UPDATE CodeSessions SET Code = @Code, " +
        "CodeSyntax = @CodeSyntax, " +
        "CodeHighlights = @CodeHighlights, " +
        "UserInControl = @UserInControl, " +
        "Participants = @Participants " +
        "WHERE Id = @Id";

      using (var connection = new SqlConnection(this.connectionString))
      {
        try
        {
          SqlCommand command = new SqlCommand(updateSession, connection);

          command.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 20);
          
          command.Parameters.Add("@Code", System.Data.SqlDbType.NVarChar, -1);
          command.Parameters.Add("@CodeSyntax", System.Data.SqlDbType.NVarChar, 100);
          command.Parameters.Add("@CodeHighlights", System.Data.SqlDbType.NVarChar, -1);
          
          command.Parameters.Add("@UserInControl", System.Data.SqlDbType.NVarChar, 100);
          command.Parameters.Add("@Participants", System.Data.SqlDbType.NVarChar, -1);

          // Add the parameter values.  Validation should have already happened.
          command.Parameters["@Id"].Value = id;
          
          command.Parameters["@Code"].Value = code;
          command.Parameters["@CodeSyntax"].Value = syntax;
          command.Parameters["@CodeHighlights"].Value = JsonConvert.SerializeObject(highlights);
          
          command.Parameters["@UserInControl"].Value = userInControl;
          command.Parameters["@Participants"].Value = JsonConvert.SerializeObject(participants);

          connection.Open();
          return (command.ExecuteNonQuery() > 0);
        }
        catch (Exception)
        {
          return false;
        }
      }
    }
  }
}
