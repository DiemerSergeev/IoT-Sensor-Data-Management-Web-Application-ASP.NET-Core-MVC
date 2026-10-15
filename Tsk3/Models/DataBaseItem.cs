using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tsk3.Models
{
    // ВАЖНО: Должно быть public для XmlSerializer
    public class DataBaseItem
    {
        public int Id { get; set; }
        public string SensorName { get; set; }
        public string DataType { get; set; }
        public string Position { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }

        // XmlSerializer нужен пустой конструктор (он создается автоматически, если нет других конструкторов)
    }
}