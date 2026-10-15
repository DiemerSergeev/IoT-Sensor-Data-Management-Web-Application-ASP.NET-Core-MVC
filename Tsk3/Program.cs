using Tsk3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Регистрация сервиса базы данных
// Мы регистрируем DataBaseModel как Transient (создается каждый раз при запросе)
// и инициализируем его параметрами подключения
builder.Services.AddTransient<DataBaseModel>(provider =>
{
    // Настройки подключения. 
    // В реальном проекте их лучше вынести в appsettings.json, но для курсовой можно так.
    ConnectionParameters connParams = new ConnectionParameters(
        server: "192.168.1.57",      // Или IP вашего сервера MySQL
        DBName: "SENSOR_DB",
        MainTable: "SENSOR_DATA_TABLE",
        User: "reader",           // Пользователь, которого вы создали
        Password: "reader_passwd", // Пароль
        Port: 3306,
        ServerName: "192.168.1.57"
    );
    return new DataBaseModel(connParams);
});

var app = builder.Build();

// --- ГЛОБАЛЬНАЯ НАСТРОЙКА ФОРМАТОВ (РЕШЕНИЕ ОШИБКИ В LINUX) ---
var cultureInfo = new System.Globalization.CultureInfo("en-US");
// Разделитель дробной части - точка (важно для координат и графиков)
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
// Формат даты для привязки данных
cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// --------------------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); // Можно отключить для локальных тестов
}

// Отключаем принудительный HTTPS, чтобы не было ошибок на Linux без сертификатов
// app.UseHttpsRedirection(); 

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();