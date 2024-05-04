using EtlTestProject.DTO;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
namespace EtlTestProject.DataMapping;

public class DataMappingService
{
    public static DataTable GetDataTabletFromCSVFile(string csv_file_path)
    {
        DataTable csvData = new DataTable();
        List<SampleDate> dataWithDuplicates = new List<SampleDate>();
        try
        {
            using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
            {
                string[] columnsFilter =
                {
                    "tpep_pickup_datetime",
                    "tpep_dropoff_datetime",
                    "passenger_count",
                    "trip_distance",
                    "store_and_fwd_flag",
                    "PULocationID",
                    "DOLocationID",
                    "fare_amount",
                    "tip_amount"
                };

                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                List<string> colFields = csvReader.ReadFields().ToList<string>();
                List<int> indexesOfColumns = new List<int>();


                foreach(string col in columnsFilter)
                {
                    indexesOfColumns.Add(colFields.IndexOf(col));
                }
                //визначаю типи колонок
                csvData.Columns.Add(new DataColumn("tpep_pickup_datetime", typeof(DateTime)));
                csvData.Columns.Add(new DataColumn("tpep_dropoff_datetime", typeof(DateTime)));
                csvData.Columns.Add(new DataColumn("passenger_count", typeof(int)));
                csvData.Columns.Add(new DataColumn("trip_distance", typeof(decimal)));
                csvData.Columns.Add(new DataColumn("store_and_fwd_flag", typeof(string)));
                csvData.Columns.Add(new DataColumn("PULocationID", typeof(int)));
                csvData.Columns.Add(new DataColumn("DOLocationID", typeof(int)));
                csvData.Columns.Add(new DataColumn("fare_amount", typeof(decimal)));
                csvData.Columns.Add(new DataColumn("tip_amount", typeof(decimal)));

                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();

                    List<string> filteredFieldData = new List<string>();
                    //шукаю потрібні колонки і фільтрую лишні дані
                    foreach (int numberColumn in indexesOfColumns)
                    {
                        filteredFieldData.Add(fieldData[numberColumn]);
                    }

                    int.TryParse(filteredFieldData[2] , out int i3);
                    //формую масив потрібних даних для подальшої обробки
                    dataWithDuplicates.Add(new SampleDate
                    (
                        DateTime.ParseExact(filteredFieldData[0], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).ToUniversalTime(),
                        DateTime.ParseExact(filteredFieldData[1], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).ToUniversalTime(),
                        i3,
                        decimal.Parse(filteredFieldData[3].Replace(".", ",")),
                        (filteredFieldData[4].Trim() == "Y" ? "YES" : "No"),
                        int.Parse(filteredFieldData[5]),
                        int.Parse(filteredFieldData[6]),
                        decimal.Parse(filteredFieldData[7].Replace(".", ",")),
                        decimal.Parse(filteredFieldData[8].Replace(".", ","))
                    ));
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        //пошук дублікатів
        var duplicates = dataWithDuplicates
            .GroupBy(x => new { x.Tpep_pickup_datetime, x.Tpep_dropoff_datetime, x.Passenger_count })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g).ToList();
        ;
        //зберігаю дублікати в csv 
        SaveDuplicatesToCsv(duplicates);
        //пошук унікальних значень для запису в бд
        var distinctData = dataWithDuplicates
            .GroupBy(x => new { x.Tpep_pickup_datetime, x.Tpep_dropoff_datetime, x.Passenger_count })
            .Select(g => g.First()).ToList();

        foreach(var distinctVal in distinctData)
        {
            csvData.Rows.Add(
                             distinctVal.Tpep_pickup_datetime,
                             distinctVal.Tpep_dropoff_datetime,
                             distinctVal.Passenger_count,
                             distinctVal.Trip_distance,
                             distinctVal.Store_and_fwd_flag,
                             distinctVal.PULocationID,
                             distinctVal.DOLocationID,
                             distinctVal.Fare_amount,
                             distinctVal.Tip_amount
                            );
        }

        return csvData;
    }

    public static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
    {
        try
        {
            using (SqlConnection dbConnection = new SqlConnection("Server=DESKTOP-H7QHPK1;Database=ETLTestProject;Trusted_Connection=true;TrustServerCertificate=True;"))
            {
                dbConnection.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "SampleData";
                    foreach (var column in csvFileData.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());
                    s.WriteToServer(csvFileData);
                }
            }
        }
        catch(Exception)
        {
            throw;
        }
    }

    private static void SaveDuplicatesToCsv(List<SampleDate> data)
    {
        using (StreamWriter writer = new StreamWriter("duplicates.csv"))
        {
            writer.WriteLine("Tpep_pickup_datetime,Tpep_dropoff_datetime,Passenger_count,Trip_distance,Store_and_fwd_flag,PULocationID,DOLocationID,Fare_amount,Tip_amount");

            foreach (var duplicate in data)
            {
                writer.WriteLine($"{duplicate.Tpep_pickup_datetime},{duplicate.Tpep_dropoff_datetime},{duplicate.Passenger_count},{duplicate.Trip_distance},{duplicate.Store_and_fwd_flag},{duplicate.PULocationID},{duplicate.DOLocationID},{duplicate.Fare_amount},{duplicate.Tip_amount}");
            }
        }
    }
} 