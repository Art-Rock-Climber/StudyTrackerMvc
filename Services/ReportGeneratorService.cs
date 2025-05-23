using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using stTrackerMVC.ViewModels;
using stTrackerMVC.Models;

namespace stTrackerMVC.Services
{
    public class ReportGeneratorService
    {
        public byte[] GenerateExcelReport(IEnumerable<CourseTask> tasks, string currentUserId, List<UserTask> userTasks)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Задания курса");

            // Заголовки
            worksheet.Cell(1, 1).Value = "Название";
            worksheet.Cell(1, 2).Value = "Описание";
            worksheet.Cell(1, 3).Value = "Дедлайн";
            worksheet.Cell(1, 4).Value = "Статус";
            worksheet.Cell(1, 5).Value = "Дата выполнения";

            // Данные
            int row = 2;
            foreach (var task in tasks)
            {
                var userTask = userTasks.FirstOrDefault(ut => ut.TaskId == task.Id);
                var status = userTask?.Status ?? CourseTaskStatus.NotStarted;
                var completedDate = userTask?.CompletedDate;

                worksheet.Cell(row, 1).Value = task.Title;
                worksheet.Cell(row, 2).Value = task.Description;
                worksheet.Cell(row, 3).Value = task.Deadline.ToString("g");
                worksheet.Cell(row, 4).Value = GetStatusName(status);
                worksheet.Cell(row, 5).Value = completedDate?.ToString("g") ?? "-";

                // Подсветка просроченных заданий
                if (status != CourseTaskStatus.Completed && task.Deadline < DateTime.Now)
                {
                    worksheet.Range(row, 1, row, 5).Style.Fill.BackgroundColor = XLColor.LightSalmon;
                }

                row++;
            }

            // Автоподбор ширины столбцов
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateWordReport(IEnumerable<CourseTask> tasks, string currentUserId, List<UserTask> userTasks)
        {
            using var stream = new MemoryStream();
            using var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);

            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Заголовок
            var title = body.AppendChild(new Paragraph(
                new Run(
                    new Text("Отчет по заданиям курса")
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }))
            );
            title.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );

            // Создаем таблицу
            var table = new Table();

            // Настройки таблицы
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct } // 100% ширины
            );
            table.AppendChild(tableProperties);

            // Ширины столбцов (в 1/50 процента)
            int[] columnWidths = { 1500, 2000, 1000, 1000, 1200 }; // Сумма ~5000 (100%)

            // Заголовки таблицы
            var tableHeader = new TableRow();
            var headers = new[] { "Название", "Описание", "Дедлайн", "Статус", "Дата выполнения" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = new TableCell(new Paragraph(
                    new Run(
                        new Text(headers[i])
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        }))
                );

                cell.TableCellProperties = new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = columnWidths[i].ToString() }
                );

                tableHeader.Append(cell);
            }
            table.Append(tableHeader);

            // Добавляем данные
            foreach (var task in tasks)
            {
                var userTask = userTasks.FirstOrDefault(ut => ut.TaskId == task.Id);
                var status = userTask?.Status ?? CourseTaskStatus.NotStarted;
                var completedDate = userTask?.CompletedDate;

                var row = new TableRow();

                var cells = new[]
                {
                    task.Title,
                    task.Description,
                    task.Deadline.ToString("g"),
                    GetStatusName(status),
                    completedDate?.ToString("g") ?? "-"
                };

                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = new TableCell(new Paragraph(
                        new Run(
                            new Text(cells[i])
                            {
                                Space = SpaceProcessingModeValues.Preserve
                            }))
                    );

                    cell.TableCellProperties = new TableCellProperties(
                        new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = columnWidths[i].ToString() }
                    );

                    row.Append(cell);
                }

                table.Append(row);
            }

            body.Append(table);
            document.Save();
            return stream.ToArray();
        }

        public byte[] GenerateOverdueExcelReport(List<CourseTask> tasks, List<UserTask> userTasks)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Просроченные задания");

            // Заголовки
            var headers = new[]
            {
                "Студент", "Email", "Курс", "Задание",
                "Дедлайн", "Статус", "Дней просрочки"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Данные
            int row = 2;
            foreach (var task in tasks)
            {
                var taskUserTasks = userTasks.Where(ut => ut.TaskId == task.Id).ToList();

                if (taskUserTasks.Any())
                {
                    foreach (var userTask in taskUserTasks)
                    {
                        worksheet.Cell(row, 1).Value = userTask.User?.LastName ?? "N/A";
                        worksheet.Cell(row, 2).Value = userTask.User?.Email ?? "N/A";
                        worksheet.Cell(row, 3).Value = task.Course?.Name ?? "N/A";
                        worksheet.Cell(row, 4).Value = task.Title;
                        worksheet.Cell(row, 5).Value = task.Deadline.ToString("g");
                        worksheet.Cell(row, 6).Value = GetStatusName(userTask.Status);
                        worksheet.Cell(row, 7).Value = (DateTime.Now - task.Deadline).Days;
                        row++;
                    }
                }
                else
                {
                    // Задания без UserTask (студенты еще не начали)
                    worksheet.Cell(row, 1).Value = "Все студенты";
                    worksheet.Cell(row, 2).Value = "-";
                    worksheet.Cell(row, 3).Value = task.Course?.Name ?? "N/A";
                    worksheet.Cell(row, 4).Value = task.Title;
                    worksheet.Cell(row, 5).Value = task.Deadline.ToString("g");
                    worksheet.Cell(row, 6).Value = "Не начато";
                    worksheet.Cell(row, 7).Value = (DateTime.Now - task.Deadline).Days;
                    row++;
                }
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        private string GetStatusName(CourseTaskStatus status)
        {
            return status switch
            {
                CourseTaskStatus.NotStarted => "Не начато",
                CourseTaskStatus.InProgress => "В процессе",
                CourseTaskStatus.Completed => "Завершено",
                _ => status.ToString()
            };
        }
    }
}
