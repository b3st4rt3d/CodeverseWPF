using CodeverseWPF.DB;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ScottPlot;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageReports.xaml
    /// </summary>
    public partial class PageReports : Page
    {
        public PageReports()
        {
            InitializeComponent();
            InitializeComponent();
            DateTime date = DateTime.Today;
            DpStartDate.SelectedDate = new DateTime(date.Year, date.Month, 1);
            DPEndDate.SelectedDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        private void BtnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            var startDate = DpStartDate.SelectedDate;
            var endDate = DPEndDate.SelectedDate;
            var selectedLabel = CBReport.SelectedItem as System.Windows.Controls.Label;
            if (selectedLabel != null
                && startDate != null
                && endDate != null)
            {
                string reportName = selectedLabel.Content.ToString();
                try
                {
                    CreateReport(reportName, (DateTime)startDate, (DateTime)endDate);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
            {
                MessageBox.Show("Заполните все поля.");
            }
        }

        private void CreateReport(string reportName, DateTime startDate, DateTime endDate)
        {
            var plot = PlotReports.Plot;
            plot.Clear();

            if (plot != null)
            {
                if (reportName == "Заказы мастеров")
                {
                    List<Tick> labels = new List<Tick>();
                    double index = 1;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var employeeOrderCounts = db.ReportEmployeeOrderCounts
                            .Where(p =>
                            p.Date >= DateOnly.FromDateTime(startDate)
                            && p.Date <= DateOnly.FromDateTime(endDate));

                        foreach (ReportEmployeeOrderCount employeeOrderCount in employeeOrderCounts)
                        {
                            if (employeeOrderCount.Count != null
                                && employeeOrderCount.Surname != null
                                && employeeOrderCount.Date != null)
                            {
                                DateTime date = DateTime.Parse(employeeOrderCount.Date.ToString());
                                plot.Add.Bar(position: index, value: (double)employeeOrderCount.Count);
                                labels.Add(new(index, date.ToLongDateString()
                                    + "\n"
                                    + employeeOrderCount.Surname
                                    + employeeOrderCount.Name));
                                index++;
                            }

                        }
                    }
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels.ToArray());
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Дата и мастера";
                    plot.Axes.Left.Label.Text = "Количество заказов";
                }
                else if (reportName == "Выполненные заказы")
                {
                    List<Tick> labels = new List<Tick>();
                    double index = 1;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var completedOrders = from p in db.ReportCompletedOrders
                                        where (p.Date >= startDate && p.Date <= endDate)
                                        select p;

                        foreach (var order in completedOrders)
                        {
                            if (order != null)
                            {
                                plot.Add.Bar(position: index, value: (double)order.Count);
                                labels.Add(new(index, order.Date.ToString()));
                                index++;
                            }
                        }
                    }
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels.ToArray());
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Дата";
                    plot.Axes.Left.Label.Text = "Количество заказов";
                }
                else if (reportName == "Обороты мастерской")
                {
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);
                    double targetTurnover = 300000;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var turnover = db.ReportTurnovers
                            .Where(p =>
                            p.Дата >= startDate
                            && p.Дата <= endDate)
                            .Sum(p => p.Оборот);
                        if (turnover != null)
                        {
                            List<PieSlice> slices = new()
                                {
                                    new PieSlice((double)turnover, ScottPlot.Colors.Aquamarine, "Оборот"),
                                    new PieSlice(targetTurnover, ScottPlot.Colors.AliceBlue, "Цель на месяц"),
                                };
                            var pie = plot.Add.Pie(slices);
                            pie.LineStyle.Color = ScottPlot.Colors.Transparent;
                            plot.ShowLegend();
                            plot.Axes.Bottom.Label.Text = $"Оборот за период составляет {turnover / (turnover + (decimal)targetTurnover) * 100:00.0}% от цели на месяц ({turnover} Руб.)";
                        }
                    }
                }
                else if (reportName == "Обороты по дням")
                {
                    List<DateTime> timeLine = new List<DateTime>();
                    List<double> values = new List<double>();
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var turnovers = db.ReportTurnovers
                            .Where(p =>
                            p.Дата >= startDate
                            && p.Дата <= endDate);

                        foreach (var turnover in turnovers)
                        {
                            if (turnover.Оборот != null
                                && turnover.Дата != null)
                            {
                                DateTime date = (DateTime)turnover.Дата;
                                values.Add((double)turnover.Оборот);
                                timeLine.Add(date);
                            }
                        }
                    }
                    plot.Add.Scatter(timeLine.ToArray(), values.ToArray());
                    plot.Axes.DateTimeTicksBottom();
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Дата";
                    plot.Axes.Left.Label.Text = "Обороты";
                }
                else if(reportName == "Популярные компьютеры")
                {
                    List<Tick> labels = new List<Tick>();
                    double index = 1;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var popularDevices = from p in db.ReportPopularDevices
                            where (p.Date >= DateOnly.FromDateTime(startDate) && p.Date <=DateOnly.FromDateTime(endDate))
                            group p by p.Device into g
                            select new { Device = g.Key, Count = g.Sum(p => (int)p.Count) };

                        foreach (var popularDevice in popularDevices)
                        {
                            if (popularDevice != null)
                            {
                                plot.Add.Bar(position: index, value: (double)popularDevice.Count);
                                labels.Add(new(index, popularDevice.Device));
                                index++;
                            }
                        }
                    }
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels.ToArray());
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Компьютер";
                    plot.Axes.Left.Label.Text = "Количество покупок";
                }
                else if (reportName == "Популярные услуги")
                {
                    List<Tick> labels = new List<Tick>();
                    double index = 1;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var popularServices = from p in db.ReportPopularServices
                                             where (p.Date >= DateOnly.FromDateTime(startDate) && p.Date <= DateOnly.FromDateTime(endDate))
                                             group p by p.Service into g
                                             select new { Service = g.Key, Count = g.Sum(p => (int)p.Count) };

                        foreach (var popularService in popularServices)
                        {
                            if (popularService != null)
                            {
                                plot.Add.Bar(position: index, value: (double)popularService.Count);
                                labels.Add(new(index, popularService.Service));
                                index++;
                            }
                        }
                    }
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels.ToArray());
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Услуга";
                    plot.Axes.Left.Label.Text = "Количество покупок";
                }
                else if (reportName == "Популярные детали")
                {
                    List<Tick> labels = new List<Tick>();
                    double index = 1;
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var popularDetails = from p in db.ReportPopularDetails
                                             where (p.Date >= DateOnly.FromDateTime(startDate) && p.Date <= DateOnly.FromDateTime(endDate))
                                             group p by p.Detail into g
                                             select new { Detail = g.Key, Count = g.Sum(p => (int)p.Count)};

                        foreach (var popularDetail in popularDetails)
                        {
                            if (popularDetail != null)
                            {
                                plot.Add.Bar(position: index, value: (double)popularDetail.Count);
                                labels.Add(new(index, popularDetail.Detail));
                                index++;
                            }
                        }
                    }
                    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels.ToArray());
                    plot.Axes.Bottom.MajorTickStyle.Length = 0;
                    plot.Axes.Bottom.Label.Text = "Деталь";
                    plot.Axes.Left.Label.Text = "Количество покупок";
                }
                else
                {
                    MessageBox.Show("Данного отчета не существует.");
                }
                plot.Axes.Margins(bottom: 0);
                plot.Axes.AutoScale();
                PlotReports.Refresh();
            }
        }

        public static void ExportStackPanelToPdf(StackPanel stackPanel)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.Title = "Save a PDF File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                try
                {
                    // Создаем изображение из StackPanel
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                        (int)stackPanel.ActualWidth,
                        (int)stackPanel.ActualHeight,
                        96d,
                        96d,
                        System.Windows.Media.PixelFormats.Pbgra32);

                    renderBitmap.Render(stackPanel);

                    // Кодируем изображение в формате PNG
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (MemoryStream stream = new MemoryStream())
                    {
                        encoder.Save(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        using (var bitmap = new Bitmap(stream))
                        {
                            PdfDocument document = new PdfDocument();
                            PdfPage page = document.AddPage();
                            XGraphics gfx = XGraphics.FromPdfPage(page);
                            XImage xImage = XImage.FromStream(stream);

                            double ratioX = page.Width / xImage.PixelWidth;
                            double ratioY = page.Height / xImage.PixelHeight;
                            double ratio = Math.Min(ratioX, ratioY);

                            double scaledWidth = xImage.PixelWidth * ratio;
                            double scaledHeight = xImage.PixelHeight * ratio;

                            gfx.DrawImage(xImage, 0, 0, scaledWidth, scaledHeight);
                            document.Save(saveFileDialog.FileName);
                        }
                    }

                    MessageBox.Show("Отчет сохранен в файл PDF.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void BtnToPdf_Click(object sender, RoutedEventArgs e)
        {
            ExportStackPanelToPdf(SPPlot);
        }
    }
}