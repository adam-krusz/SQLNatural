using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

public class DatabaseHelper
{
    private string connectionString;

    public DatabaseHelper(string dbFilePath)
    {
        connectionString = $"Data Source={dbFilePath};Version=3;";
    }

    public List<string> RetrieveStructure()
    {
        List<string> structure = new List<string>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            DataTable tablesSchema = connection.GetSchema("Tables");

            foreach (DataRow tableRow in tablesSchema.Rows)
            {
                string tableName = (string)tableRow["TABLE_NAME"];
                string tableStructure = RetrieveTableStructure(connection, tableName);
                structure.Add(tableStructure);
            }

            connection.Close();
        }

        return structure;
    }
    



    private string RetrieveTableStructure(SQLiteConnection connection, string tableName)
    {
        string structure = "";

        using (SQLiteCommand command = new SQLiteCommand($"SELECT sql FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    structure = reader.GetString(0);
                }
            }
        }

        return structure;
    }



    public DataSet ExecuteQuery(string query)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    return dataSet;
                }
            }
        }
    }
    public void DisplayQueryResultsOld(DataSet dataSet)
    {
        foreach (DataTable table in dataSet.Tables)
        {
            Console.WriteLine($"Table: {table.TableName}");
            Console.WriteLine("---------------------------------------");

            foreach (DataColumn column in table.Columns)
            {
                Console.Write($"{column.ColumnName}\t");
            }

            Console.WriteLine("\n---------------------------------------");

            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item}\t");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
public static void DisplayQueryResults(DataSet dataSet)
{
    int bufferWidth = Console.BufferWidth; // Get the console buffer width

    foreach (DataTable table in dataSet.Tables)
    {
        // Find the maximum width for each column
        int[] columnWidths = new int[table.Columns.Count];
        foreach (DataColumn column in table.Columns)
        {
            int columnIndex = column.Ordinal;
            int maxColumnWidth = column.ColumnName.Length;

            foreach (DataRow row in table.Rows)
            {
                string? value = row[columnIndex].ToString();
                int cellWidth = value?.Length ?? 0;
                if (cellWidth > maxColumnWidth)
                    maxColumnWidth = cellWidth;

            }

            columnWidths[columnIndex] = maxColumnWidth;
        }

        // Display table header
        Console.WriteLine($"Table: {table.TableName}");
        Console.WriteLine("---------------------------------------");

        // Display column names
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string columnName = table.Columns[i].ColumnName;
            Console.Write($"{columnName.PadRight(columnWidths[i])}\t");
        }

        Console.WriteLine("\n---------------------------------------");

        // Display rows
        foreach (DataRow row in table.Rows)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string value = row[i]?.ToString() ?? "";
                int columnWidth = Math.Min(columnWidths[i], bufferWidth - Console.CursorLeft);
                Console.Write($"{value.PadRight(columnWidth)}\t");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }
}




    

    public void DisplayQueryResults(string query)
    {
        DataSet result = ExecuteQuery(query);
        DisplayQueryResults(result);
    }

    //method returning table Artists



}