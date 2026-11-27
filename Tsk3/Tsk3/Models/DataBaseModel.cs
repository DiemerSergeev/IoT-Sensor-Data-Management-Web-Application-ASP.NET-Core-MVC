using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsk3.Models
{
    public class DataBaseModel
    {
        private ConnectionParameters connParameters;
        public DataBaseModel(ConnectionParameters ConnParams)
        {
            this.connParameters = ConnParams;
        }
        public List<DataBaseItem> GetItems()
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();
            using (MySqlConnection connection = new MySqlConnection(this.connParameters.ConnectionString))
            {
                connection.Open();
                String selectSqlCmd = "SELECT * FROM " + this.connParameters.MainTable;
                MySqlCommand cmd = new MySqlCommand(selectSqlCmd, connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();
                        item.Id = Convert.ToInt32(reader["Id"]);
                        item.SensorName = Convert.ToString(reader["SensorName"]);
                        item.DataType = Convert.ToString(reader["DataType"]);
                        item.Position = Convert.ToString(reader["Position"]);
                        item.Value = Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);
                        resultList.Add(item);
                    }
                }
            }
            return resultList;
        }
        public Int32 GetNumberOfItems()
        {
            Int32 numOfItems = 0;
            using(MySqlConnection connection = new MySqlConnection(this.connParameters.ConnectionString))
            {
                connection.Open();
                String selectSqlCmd = "SELECT COUNT(*) FROM " + this.connParameters.MainTable;
                MySqlCommand cmd = new MySqlCommand( selectSqlCmd, connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numOfItems = Convert.ToInt32(reader[0]);
                    }
                }

            }
            return numOfItems;
        }
        public List<DataBaseItem> GetSensorItems(String SensorName, String DataType, String Position)
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();
            using (MySqlConnection connection =
            new MySqlConnection(this.connParameters.ConnectionString))
            {
                connection.Open();
                String selectSqlCmd = "SELECT * FROM " +
                this.connParameters.MainTable +
                " WHERE (SensorName = '" + SensorName + "')"
                + " AND (DataType = '" + DataType + "')"
                + " AND (Position = '" + Position + "');";
                MySqlCommand cmd = new MySqlCommand(selectSqlCmd,
                connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();
                        item.Id = Convert.ToInt32(reader["ID"]);
                        item.SensorName =
                        Convert.ToString(reader["SensorName"]);
                        item.DataType =
                        Convert.ToString(reader["DataType"]);
                        item.Position =
                        Convert.ToString(reader["Position"]);
                        item.Value =
                        Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);
                        resultList.Add(item);
                    }
                }
            }
            return resultList;
        }
        public List<DataBaseItem> GetFilteredItems(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();

            using (MySqlConnection connection = new MySqlConnection(this.connParameters.ConnectionString))
            {
                connection.Open();

                // 1. Формируем базовый запрос
                // "WHERE 1=1" — это трюк, чтобы дальше можно было просто писать "AND ..."
                StringBuilder sql = new StringBuilder("SELECT * FROM " + this.connParameters.MainTable + " WHERE 1=1");

                // 2. Динамически добавляем условия, если параметры не пустые
                if (!string.IsNullOrEmpty(sensorName))
                {
                    sql.Append(" AND SensorName = @sensorName");
                }

                if (!string.IsNullOrEmpty(dataType))
                {
                    sql.Append(" AND DataType = @dataType");
                }

                if (!string.IsNullOrEmpty(position))
                {
                    sql.Append(" AND Position = @position");
                }

                if (startDate.HasValue)
                {
                    sql.Append(" AND Date >= @startDate");
                }

                if (endDate.HasValue)
                {
                    // Чтобы включить весь конец дня, можно добавить время 23:59:59 или просто сравнить строго
                    sql.Append(" AND Date <= @endDate");
                }

                // Опционально: сортировка
                sql.Append(" ORDER BY Date DESC");

                // 3. Создаем команду
                MySqlCommand cmd = new MySqlCommand(sql.ToString(), connection);

                // 4. БЕЗОПАСНО передаем значения через параметры
                // Драйвер сам поймет, что это строка или дата, и защитит от кавычек и инъекций
                if (!string.IsNullOrEmpty(sensorName)) cmd.Parameters.AddWithValue("@sensorName", sensorName);
                if (!string.IsNullOrEmpty(dataType)) cmd.Parameters.AddWithValue("@dataType", dataType);
                if (!string.IsNullOrEmpty(position)) cmd.Parameters.AddWithValue("@position", position);

                if (startDate.HasValue)
                    cmd.Parameters.AddWithValue("@startDate", startDate.Value); // Передаем DateTime напрямую

                if (endDate.HasValue)
                    cmd.Parameters.AddWithValue("@endDate", endDate.Value);

                // 5. Выполняем запрос
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();
                        // Безопасное чтение с проверкой на null (если вдруг в БД есть NULL)
                        item.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        item.SensorName = reader["SensorName"]?.ToString();
                        item.DataType = reader["DataType"]?.ToString();
                        item.Position = reader["Position"]?.ToString();
                        item.Value = reader["Value"]?.ToString();
                        item.Date = reader["Date"]?.ToString(); // Или reader.GetDateTime("Date")

                        resultList.Add(item);
                    }
                }
            }
            return resultList;
        }

    }
}

