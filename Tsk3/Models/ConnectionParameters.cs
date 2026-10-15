using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tsk3.Models
{
    public class ConnectionParameters
    {
        public String Server {  get; set; }
        public String DbName { get; set; }
        public String MainTable { get; set; }
        public String User {  get; set; }   
        public String Password { get; set; }
        public Int16 Port { get; set; }

        public ConnectionParameters()
        {
            this.DbName = String.Empty;
            this.MainTable = String.Empty;
            this.Port = 0;
            this.User = String.Empty;
            this.Password = String.Empty;
            this.Server = String.Empty;
        }

        public ConnectionParameters(String server, String DBName, String MainTable, String User, String Password, Int16 Port, String ServerName)
        {
            this.Server = ServerName;
            this.DbName = DBName;
            this.MainTable = MainTable;
            this.User = User;
            this.Password = Password;
            this.Port = Port;
        }

        public String ConnectionString
        {
            get 
            {
                return "server=" + this.Server + ";" 
                    + "user=" + this.User + ";" 
                    + "database=" + this.DbName + ";" 
                    + "port=" + this.Port.ToString() + ";" 
                    + "password=" + this.Password;
            }
        }
    }
}
