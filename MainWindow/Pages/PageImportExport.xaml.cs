using ClosedXML.Excel;
using CodeverseWPF.DB;
using CodeverseWPF.Utils;
using ExcelDataReader;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Type = CodeverseWPF.DB.Type;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageImportExport.xaml
    /// </summary>
    public partial class PageImportExport : Page
    {
        IExcelDataReader? edr;
        private readonly string PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public PageImportExport()
        {
            InitializeComponent();
            SPExport.IsEnabled = false;
        }

        private void BtnExelImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "EXCEL Files (*.xlsx)|*.xlsx|EXCEL Files 2003 (*.xls)|*.xls|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() != true)
                    return;

                DbGrig.ItemsSource = readFile(openFileDialog.FileName);
                SPExport.IsEnabled = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private DataView readFile(string fileNames)
        {
            var extension = fileNames.Substring(fileNames.LastIndexOf('.'));

            FileStream stream = File.Open(fileNames, FileMode.Open, FileAccess.Read);

            if (extension == ".xlsx")
                edr = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else if (extension == ".xls")
                edr = ExcelReaderFactory.CreateBinaryReader(stream);

            var conf = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            };

            DataSet dataSet = edr.AsDataSet(conf);
            DataView dtView = dataSet.Tables[0].AsDataView();
            edr.Close();
            return dtView;
        }

        private void ImportFromDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Label? label = ImportFromDB.SelectedItem as Label;
            var table = label.Content.ToString();
            if (table != null)
            {
                try
                {
                    switch (table)
                    {
                        case "Клиенты":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var clients = from p in db.Clients
                                              orderby p.ClientId
                                              select new { Id = p.ClientId, Фамилия = p.Surname, Имя = p.Name, Почта = p.Email, Телефон = p.Phone};
                                DbGrig.ItemsSource = clients.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        case "Бренды":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var brands = from p in db.Brands
                                             orderby p.BrandId
                                             select new { Id = p.BrandId, Название = p.Brand1 };
                                DbGrig.ItemsSource = brands.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        case "Типы деталей":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var types = from p in db.Types
                                            orderby p.TypeId
                                            select new { Id = p.TypeId, Название = p.Type1 };
                                DbGrig.ItemsSource = types.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        case "Устройства":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var devices = from p in db.Devices
                                              orderby p.DeviceId
                                              select new { Id = p.DeviceId, Название = p.Device1, Стоимость = p.Price};
                                DbGrig.ItemsSource = devices.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        case "Детали":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var details = from p in db.ViewDetails
                                              orderby p.DetailId
                                              select new { Id = p.DetailId, Название = p.Detail, Стоимость = p.Price, Бренд = p.Brand, Тип = p.Type, Количество = p.Count };
                                DbGrig.ItemsSource = details.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        case "Сотрудники":
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var employees = from p in db.ViewEmployees
                                                orderby p.EmployeeId
                                                select new { Id = p.EmployeeId, Фамилия = p.Surname, Имя = p.Name, Почта = p.Email, Должность = p.Position };
                                DbGrig.ItemsSource = employees.ToList();
                            }
                            MessageBox.Show("Импорт произошло успешно");
                            break;
                        default:
                            MessageBox.Show("Импорт не удался.");
                            break;
                    }
                    SPExport.IsEnabled = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            DbGrig.ItemsSource = null;
            SPExport.IsEnabled = false;
        }

        private void BtnToDB_Click(object sender, RoutedEventArgs e)
        {
            Label? label = CBTables.SelectedItem as Label;
            if (label != null && label.Content != null)
            {
                string? table = label.Content.ToString();

                string message = ExportToDB(table);

                MessageBox.Show(message);
            }
        }

        private string ExportToDB(string table)
        {
            try
            {
                switch (table)
                {
                    case "Клиенты":
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var clients = db.Clients;
                            foreach (System.Data.DataRowView dr in DbGrig.ItemsSource)
                            {
                                var name = dr["Имя"].ToString();
                                var surName = dr["Фамилия"].ToString();
                                var email = dr["Почта"].ToString();
                                var phone = dr["Телефон"].ToString();

                                if (name != null
                                    && surName != null
                                    && email != null
                                    && phone != null)
                                {
                                    var unique = db.Clients
                                        .FirstOrDefault(p => p.Email == email
                                        || p.Phone == phone);
                                    if (unique != null)
                                    {
                                        MessageBox.Show($"Клиент с почтой - {email} телефоном - {phone} уже есть в системе.");
                                        continue;
                                    }
                                    if (!Validate.IsValidEmail(email))
                                    {
                                        MessageBox.Show($"У клиента {surName} {name} некорректная почта {email}.");
                                        continue;
                                    }

                                    if (!Validate.IsValidPhoneNumber(phone))
                                    {
                                        MessageBox.Show($"У клиента {surName} {name} некорректный номер телефона {phone}.");
                                        continue;
                                    }
                                    Client client = new Client
                                    {
                                        Name = name,
                                        Surname = surName,
                                        Email = email,
                                        Phone = phone
                                    };
                                    clients.Add(client);
                                    db.SaveChanges();
                                }
                            }
                        }

                        return "Новый клиенты успешно добавлены";
                    case "Детали":
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var details = db.Details;

                            foreach (System.Data.DataRowView dr in DbGrig.ItemsSource)
                            {
                                var detailType = int.Parse(dr["Тип детали"].ToString());
                                var brand = int.Parse(dr["Бренд"].ToString());
                                var price = decimal.Parse(dr["Цена"].ToString());
                                var name = dr["Имя"].ToString();
                                var description = dr["Описание"].ToString();
                                var count = int.Parse(dr["Количество"].ToString());
                                if (detailType != null
                                    && brand != null
                                    && price != null
                                    && name != null
                                    && description != null
                                    && count != null)
                                {
                                    var unique = db.Details
                                        .FirstOrDefault(p => p.Detail1 == name);
                                    if (unique != null)
                                    {
                                        var _detail = db.Details
                                            .FirstOrDefault(p =>
                                            p.Detail1.ToLower().Trim() == name.ToLower().Trim());
                                        if(_detail != null)
                                        {
                                            _detail.Count += count;
                                        }
                                    }
                                    else
                                    {
                                        var detail = new Detail
                                        {
                                            Detail1 = name,
                                            Description = description,
                                            TypeId = detailType,
                                            BrandId = brand,
                                            Price = price,
                                            Count = count,
                                        };

                                        details.Add(detail);
                                    }

                                    db.SaveChanges();
                                }
                            }
                        }
                        return "Новые детали успешно добавлены";
                    case "Бренды":
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var brands = db.Brands;
                            foreach (System.Data.DataRowView dr in DbGrig.ItemsSource)
                            {
                                var name = dr["Название"].ToString();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    var unique = db.Brands
                                        .FirstOrDefault(p => p.Brand1 == name);
                                    if (unique != null)
                                    {
                                        MessageBox.Show($"Бренд {name} уже есть в системе.");
                                        continue;
                                    }

                                    Brand brand = new Brand()
                                    {
                                        Brand1 = name,
                                    };
                                    brands.Add(brand);
                                    db.SaveChanges();
                                }
                            }
                        }
                        return "Новые бренды успешно добавлены";
                    case "Типы деталей":
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var types = db.Types;
                            foreach (System.Data.DataRowView dr in DbGrig.ItemsSource)
                            {
                                var name = dr["Название"].ToString();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    var unique = db.Types
                                        .FirstOrDefault(p => p.Type1 == name);
                                    if (unique != null)
                                    {
                                        MessageBox.Show($"Тип {name} уже есть в системе.");
                                        continue;
                                    }
                                    Type type = new Type()
                                    {
                                        Type1 = name,
                                    };
                                    types.Add(type);
                                    db.SaveChanges();
                                }
                            }
                        }
                        return "Новые типы деталей успешно добавлены";
                    case "Устройства":
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var devices = db.Devices;
                            foreach (System.Data.DataRowView dr in DbGrig.ItemsSource)
                            {
                                var name = dr["Название"].ToString();
                                var price = int.Parse(dr["Цена"].ToString());

                                if (!string.IsNullOrEmpty(name)
                                    && price != null)
                                {
                                    var unique = db.Devices
                                        .FirstOrDefault(p => p.Device1 == name);
                                    if (unique != null)
                                    {
                                        MessageBox.Show($"Тип {name} уже есть в системе.");
                                        continue;
                                    }
                                    Device device = new Device()
                                    {
                                        Device1 = name,
                                        Price = price,
                                    };
                                    devices.Add(device);
                                    db.SaveChanges();
                                }
                            }
                        }
                        return "Новые устройства успешно добавлены";
                    default:
                        return "Не удалось экспортировать данные в базу данных.";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "Не удалось экспортировать данные в базу данных.";
        }

        private void ToJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filename = TBDocumentName.Text;
                if (string.IsNullOrEmpty(filename))
                {
                    TBDocumentName.BorderBrush = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("Пожалуйста, Заполните поле название документа.");
                }
                else
                {
                    filename = $"{PATH}\\{filename}.json";
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

                    foreach (var item in DbGrig.Items)
                    {
                        var row = new Dictionary<string, object>();

                        foreach (var column in DbGrig.Columns)
                        {
                            var cellContent = column.GetCellContent(item);
                            var cellValue = (cellContent as TextBlock)?.Text;

                            row.Add(column.Header.ToString(), cellValue);
                        }

                        data.Add(row);
                    }
                    var fileExists = File.Exists(filename);
                    if (!fileExists)
                    {
                        File.CreateText(PATH + "data.json").Dispose();
                    }
                    else
                    {
                        if (MessageBox.Show("Данный файл уже существует. Заменить его?",
                        "Заменить файл",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.No) return;
                    }
                    using (StreamWriter writer = File.CreateText(filename))
                    {
                        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                        writer.Write(json);
                    }
                    MessageBox.Show($"Файл {TBDocumentName.Text} был успешно создан по пути {filename}");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportDataGridToExcel(DbGrig);
        }

        public void ExportDataGridToExcel(DataGrid dataGrid)
        {
            if (dataGrid.Items.Count == 0)
            {
                MessageBox.Show("В таблице нет значений.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Файлы |*.xlsx";
            saveFileDialog.Title = "Сохранить как файл Excel";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Sheet1");

                        // Adding the headers
                        for (int i = 0; i < dataGrid.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = (string)dataGrid.Columns[i].Header;
                        }

                        // Adding the rows
                        for (int i = 0; i < dataGrid.Items.Count; i++)
                        {
                            for (int j = 0; j < dataGrid.Columns.Count; j++)
                            {
                                var cellValue = GetCellValue(dataGrid, dataGrid.Items[i], j);
                                worksheet.Cell(i + 2, j + 1).Value = (string)cellValue;
                            }
                        }

                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Импорт в Excel файл произошел успешно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private object GetCellValue(DataGrid dataGrid, object dataItem, int columnIndex)
        {
            var cellContent = dataGrid.Columns[columnIndex].GetCellContent(dataItem) as TextBlock;
            return cellContent?.Text ?? string.Empty;
        }
    }
}
