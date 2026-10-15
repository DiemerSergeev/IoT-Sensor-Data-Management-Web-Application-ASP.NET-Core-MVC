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

        public List<DataBaseItem> GetFilteredItems(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate, string sortOrder)
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();

            using (MySqlConnection connection = new MySqlConnection(this.connParameters.ConnectionString))
            {
                connection.Open();

                StringBuilder sql = new StringBuilder("SELECT * FROM " + this.connParameters.MainTable + " WHERE 1=1");

                if (!string.IsNullOrEmpty(sensorName)) sql.Append(" AND SensorName = @sensorName");
                if (!string.IsNullOrEmpty(dataType)) sql.Append(" AND DataType = @dataType");
                if (!string.IsNullOrEmpty(position)) sql.Append(" AND Position = @position");
                if (startDate.HasValue) sql.Append(" AND Date >= @startDate");
                if (endDate.HasValue) sql.Append(" AND Date <= @endDate");

                switch (sortOrder)
                {
                    case "id_asc":
                        sql.Append(" ORDER BY Id ASC");
                        break;
                    case "id_desc":
                        sql.Append(" ORDER BY Id DESC");
                        break;
                    case "name_asc":
                        sql.Append(" ORDER BY SensorName ASC");
                        break;
                    case "name_desc":
                        sql.Append(" ORDER BY SensorName DESC");
                        break;
                    case "type_asc":
                        sql.Append(" ORDER BY DataType ASC");
                        break;
                    case "type_desc":
                        sql.Append(" ORDER BY DataType DESC");
                        break;
                    case "pos_asc":
                        sql.Append(" ORDER BY Position ASC");
                        break;
                    case "pos_desc":
                        sql.Append(" ORDER BY Position DESC");
                        break;
                    case "val_asc":
                        sql.Append(" ORDER BY Value ASC");
                        break;
                    case "val_desc":
                        sql.Append(" ORDER BY Value DESC");
                        break;
                    case "date_asc":
                        sql.Append(" ORDER BY Date ASC");
                        break;
                    case "date_desc":
                        sql.Append(" ORDER BY Date DESC");
                        break;
                    default:
                        sql.Append(" ORDER BY Id DESC");
                        break;
                }

                MySqlCommand cmd = new MySqlCommand(sql.ToString(), connection);

                if (!string.IsNullOrEmpty(sensorName)) cmd.Parameters.AddWithValue("@sensorName", sensorName);
                if (!string.IsNullOrEmpty(dataType)) cmd.Parameters.AddWithValue("@dataType", dataType);
                if (!string.IsNullOrEmpty(position)) cmd.Parameters.AddWithValue("@position", position);
                if (startDate.HasValue) cmd.Parameters.AddWithValue("@startDate", startDate.Value);
                if (endDate.HasValue) cmd.Parameters.AddWithValue("@endDate", endDate.Value);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();
                        item.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        item.SensorName = reader["SensorName"]?.ToString();
                        item.DataType = reader["DataType"]?.ToString();
                        item.Position = reader["Position"]?.ToString();
                        item.Value = reader["Value"]?.ToString();
                        item.Date = reader["Date"]?.ToString();
                        resultList.Add(item);
                    }
                }
            }
            return resultList;
        }
    }

    }


