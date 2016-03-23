using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace XMLEditor
{
    public class SQLDatabase
    {
        // Create the sqlite database according to the DBName
        public void createDatabase(string DBName, string path)
        {            
            try
            {
                DBName = DBName + ".db";
                string temp = Path.Combine("AppData", DBName);
                if (!File.Exists(Path.Combine(path, temp)))         // if file is not exists
                {
                    SQLiteConnection.CreateFile(DBName);
                    string s = Path.Combine(path, DBName);
                    File.Move(s, Path.Combine(path, temp));
                }
            }
            catch(Exception ex)
            {
               
            }
        }

        // Create the connection for specific database
        public SQLiteConnection createDBConnection(string path, SQLiteConnection m_dbConnection)
        {
            m_dbConnection = new SQLiteConnection("Data Source=" + path + ";Version=3;" + "PRAGMA AUTO_VACUUM = true");
            return m_dbConnection;
        }

        public void addData(string category, string module, string moduleID, string functionName, SQLiteCommand command)
        {
            command.Parameters.AddWithValue("@category", category);
            command.Parameters.AddWithValue("@module", module);
            command.Parameters.AddWithValue("@moduleID", moduleID);
            command.Parameters.AddWithValue("@functionName", functionName);
            command.ExecuteNonQuery();
        }

        // Open the connection for specific database
        public void connectToDB(SQLiteConnection connection)
        {
            connection.Open();
        }

        // Create the database (One time only)
        public void createDataTable(SQLiteConnection connection)
        {
            string daily = "create table if not exists dailyExpenses (category text, module text, moduleID text, functionName text)";
            SQLiteCommand command = new SQLiteCommand(daily, connection);
            command.ExecuteNonQuery();
        }

        public bool isRecordExists(SQLiteConnection connection, string itemToSearch)
        {
            string temp = "'" + itemToSearch + "'";
            string sql = "SELECT count(*) from TestCases where command like " + temp;
            SQLiteCommand sqlCommand = new SQLiteCommand(sql, connection);

            int count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            if (count == 0)
                return false;
            else
                return true;
        }


        public void updateSpecificDataInDB(SQLiteCommand command, string itemToSearch, string Type, string Description, double Fares)
        {
            command.Parameters.AddWithValue("@itemToSearch", itemToSearch);
            command.Parameters.AddWithValue("@Type", Type);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@Fares", Fares);
  
            command.ExecuteNonQuery();
        }

        public void closeConnection(SQLiteConnection m_dbConnection)
        {
            m_dbConnection.Close();
        }

        public void deleteSpecificFromDB(SQLiteCommand command, string itemToSearch)
        {
            command.Parameters.AddWithValue("@itemToSearch", itemToSearch);
            command.ExecuteNonQuery();
        }
    }
}
