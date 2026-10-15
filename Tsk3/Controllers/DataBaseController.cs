using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Tsk3.Models;

namespace Tsk3.Controllers
{
    public class DataBaseController : Controller
    {
        private DataBaseModel GetDbModel()
        {
            return HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;
        }

        public IActionResult Index()
        {
            var db = GetDbModel();
            return View(db.GetNumberOfItems());
        }

        // ОТОБРАЖЕНИЕ ТАБЛИЦЫ
        public IActionResult ShowDataTable(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate, string sortOrder)
        {
            var db = GetDbModel();

            DateTime? actualEndDate = endDate;
            if (endDate.HasValue)
                actualEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var filteredData = db.GetFilteredItems(sensorName, dataType, position, startDate, actualEndDate, sortOrder);

            ViewBag.SensorName = sensorName;
            ViewBag.DataType = dataType;
            ViewBag.Position = position;
            ViewBag.StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.TypeSortParm = sortOrder == "type_asc" ? "type_desc" : "type_asc";
            ViewBag.PosSortParm = sortOrder == "pos_asc" ? "pos_desc" : "pos_asc";
            ViewBag.ValSortParm = sortOrder == "val_asc" ? "val_desc" : "val_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            return View(filteredData);
        }

        // ОТОБРАЖЕНИЕ ГРАФИКА
        public IActionResult ShowChart(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            var db = GetDbModel();

            DateTime? actualEndDate = endDate;
            if (endDate.HasValue)
                actualEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var dataForChart = db.GetFilteredItems(sensorName, dataType, position, startDate, actualEndDate, "date_asc");

            ViewBag.SensorName = sensorName;
            ViewBag.DataType = dataType;
            ViewBag.Position = position;

            ViewBag.StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "";

            return View(dataForChart);
        }

        // ЭКСПОРТ В EXCEL
        public IActionResult SaveToExcelFile(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate, string sortOrder)
        {
            var db = GetDbModel();

            DateTime? actualEndDate = endDate;
            if (endDate.HasValue)
            {
                actualEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            }

            List<DataBaseItem> lstItems = db.GetFilteredItems(sensorName, dataType, position, startDate, actualEndDate, sortOrder);

            Byte[] data = this.GetExcelFileBinaryContent(lstItems);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SensorsData.xlsx");
        }

        // ЭКСПОРТ В XML
        public IActionResult SaveToXmlFile(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate, string sortOrder)
        {
            var db = GetDbModel();

            DateTime? actualEndDate = endDate;
            if (endDate.HasValue)
            {
                actualEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            }

            List<DataBaseItem> lst = db.GetFilteredItems(sensorName, dataType, position, startDate, actualEndDate, sortOrder);

            using (Utf8StringWriter stringWriter = new Utf8StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));

                xmlSerializer.Serialize(stringWriter, lst);

                Byte[] data = Encoding.UTF8.GetBytes(stringWriter.ToString());

                return File(data, "application/xml", "SensorsData.xml");
            }
        }

        // Вспомогательный класс для принудительной кодировки UTF-8 (добавь его внутрь контроллера или в конец файла)
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        // Вспомогательные методы
        private DataTable ConvertToDataTable(List<DataBaseItem> Lst)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(DataBaseItem));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (DataBaseItem item in Lst)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private Byte[] GetExcelFileBinaryContent(List<DataBaseItem> lstItems)
        {
            DataTable dt = this.ConvertToDataTable(lstItems);
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(dt, "Sensors");

                worksheet.Cells(true).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);
                worksheet.Cells(false).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);
                worksheet.Tables.FirstOrDefault().ShowAutoFilter = false;
                worksheet.Columns().AdjustToContents();

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}