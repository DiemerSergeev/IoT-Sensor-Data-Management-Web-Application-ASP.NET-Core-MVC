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
        // Вспомогательный метод для получения сервиса
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
        public IActionResult ShowDataTable(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            var db = GetDbModel();

            // Получаем отфильтрованные данные
            var filteredData = db.GetFilteredItems(sensorName, dataType, position, startDate, endDate);

            // Сохраняем параметры фильтра для формы
            ViewBag.SensorName = sensorName;
            ViewBag.DataType = dataType;
            ViewBag.Position = position;

            // Форматируем дату специально для HTML5 input type="datetime-local"
            ViewBag.StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-ddTHH:mm") : "";

            return View(filteredData);
        }

        // ОТОБРАЖЕНИЕ ГРАФИКА
        public IActionResult ShowChart(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            var db = GetDbModel();

            var dataForChart = db.GetFilteredItems(sensorName, dataType, position, startDate, endDate);

            ViewBag.SensorName = sensorName;
            ViewBag.DataType = dataType;
            ViewBag.Position = position;
            ViewBag.StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-ddTHH:mm") : "";
            ViewBag.EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-ddTHH:mm") : "";

            return View(dataForChart);
        }

        // ЭКСПОРТ В EXCEL (С поддержкой фильтров, если параметры переданы в URL)
        public IActionResult SaveToExcelFile(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            var db = GetDbModel();
            // Используем GetFilteredItems, чтобы скачать именно то, что искали (или всё, если фильтры пусты)
            List<DataBaseItem> lstItems = db.GetFilteredItems(sensorName, dataType, position, startDate, endDate);

            Byte[] data = this.GetExcelFileBinaryContent(lstItems);
            return File(data, "application/xlsx", "SensorsData.xlsx");
        }

        // ЭКСПОРТ В XML
        public IActionResult SaveToXmlFile(string sensorName, string dataType, string position, DateTime? startDate, DateTime? endDate)
        {
            var db = GetDbModel();
            List<DataBaseItem> lst = db.GetFilteredItems(sensorName, dataType, position, startDate, endDate);

            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));
                xmlSerializer.Serialize(stringWriter, lst);
                Byte[] data = Encoding.Unicode.GetBytes(stringWriter.ToString());
                return File(data, "application/xml", "SensorsData.xml");
            }
        }

        // Вспомогательные методы для Excel
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

                // Стилизация (как в методичке)
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