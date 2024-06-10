using OfficeOpenXml;
using PartsMysql.DataAccess;
using PartsMysql.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Diagnostics;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.IO.Ports;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;
using OfficeOpenXml.Style;
using MySqlX.XDevAPI.Common;
using Microsoft.IdentityModel.Tokens;
using System.IO.Packaging;
using PartsMySql.Models;
using Org.BouncyCastle.Utilities;
using System.Text;

namespace PartsMysql.Controllers
{
    public class PartsIqController : Controller
    {
        // GET: PartsIq
        private PartsIQEntities dbMysql = new PartsIQEntities();
        public ActionResult Index()
        {
            return View();
        }
        //Controller for Returning Report Json
        public ActionResult GetInspectionSummary()
        {
            using (var dbContext = new PartsIQEntities())
            {
                var inspectionDetails = dbContext.GetInspectionSummarry();
                return Json(inspectionDetails, JsonRequestBehavior.AllowGet);
            }

        }
        //Controller for Returning Inspection Summary per Inspection Id
        public ActionResult GetInspectionDetailsPerId(long id)
        {
            using (var dbContext = new PartsIQEntities())
            {
                var inspectionDetailsPerId = dbContext.GetInspectionDetailsPerId(id);
                return Json(inspectionDetailsPerId, JsonRequestBehavior.AllowGet);
            }
        }
        //Controller for InspectionInfoPerId

        public ActionResult GetInspectionInfoPerId(long id)
        {
            using (var dbContext = new PartsIQEntities())
            {
                var inspectionInfoPerId = dbContext.GetInspectionInfoPerId(id);
                return Json(inspectionInfoPerId, JsonRequestBehavior.AllowGet);
            }
        }

        //Controller for InspectionInfoCheckpoints

        public ActionResult GetInspectionCheckpoints(long inspectionId, long checkpointId)
        {
            using (var dbContext = new PartsIQEntities())
            {
                var checkpointInfo = dbContext.GetInspectionCheckpoints(inspectionId, checkpointId);
                return Json(checkpointInfo, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: Suppliers Details
        public ActionResult GetSuppliers()
        {

            using (var dbContext = new PartsIQEntities())
            {
                var supplierDetails = dbContext.GetSuppliers();
                return Json(supplierDetails, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetUsers()
        {
            using (var dbContext = new PartsIQEntities())
            {
                var users = dbContext.GetUsers();
                return Json(users, JsonRequestBehavior.AllowGet);
            }

        }

        //POST: Get Supplier Performance
        [HttpPost]
        public ActionResult SupplierPerformance(FormCollection formData)
        {
            try
            {
                // Extract data from the FormData object
                int SupplierId;
                if (!Int32.TryParse(formData["SupplierId"], out SupplierId))
                {
                    throw new ArgumentException("Invalid SupplierId");
                }

                DateTime DateFrom;
                if (!DateTime.TryParse(formData["DateFrom"], out DateFrom))
                {
                    throw new ArgumentException("Invalid DateFrom");
                }

                DateTime DateTo;
                if (!DateTime.TryParse(formData["DateTo"], out DateTo))
                {
                    throw new ArgumentException("Invalid DateTo");
                }

                string PartCode = formData["PartCode"] ?? ""; // Handle null PartCode

                // Query the database with the extracted data
                using (var dbContext = new PartsIQEntities())
                {
                    var supplierDetails = dbContext.GetSuppliersPerformance(SupplierId, DateFrom, DateTo, PartCode);
                    return Json(supplierDetails, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                // Return a JSON response with the error message
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //POST: Get Inspection Summary filter
        [HttpPost]
        public ActionResult InspectionSummaryFilter(FormCollection formData)
        {
            string PartsCode = (formData["partsCodeF"]) ?? "";
            string LotNumber = (formData["lotNumberF"]) ?? "";
            DateTime DateFrom;
            DateTime.TryParse(formData["fromDateF"], out DateFrom); // Handle date parsing
            DateTime DateTo;
            DateTime.TryParse(formData["toDateF"], out DateTo); // Handle date parsing
            DateTo = DateTo.AddDays(1);
            string DrNumber = formData["drNumberF"] ?? ""; // Handle null PartCode
            int SupplierId = (formData["supplierF"] != "null") ? Int32.Parse(formData["supplierF"]) : 0;

            // Query the database with the extracted data
            using (var dbContext = new PartsIQEntities())
            {
                var supplierDetails
                    = dbContext.InspectionSummaryFilter(PartsCode, LotNumber, DateFrom, DateTo, DrNumber, SupplierId);
                return Json(supplierDetails, JsonRequestBehavior.AllowGet);
            }
        }


        //GET: Parts Parts Code
        public ActionResult PartsCode()
        {
            using (var dbContext = new PartsIQEntities())
            {
                var partsCode = dbContext.GetPartsCode();
                return Json(partsCode, JsonRequestBehavior.AllowGet);
            }
        }
        //GET All Suppliers associated to a Part
        public JsonResult SupplierForPart(int partId)
        {
            Console.Write(partId);
            using (var dbContext = new PartsIQEntities())
            {
                var suppliers = dbContext.GetSupplierPerPart(partId);
                var checkPoints = dbContext.GetInspectionCheckpointsPerId((long)partId);
                var data = new
                {
                    Suppliers = suppliers,
                    CheckPoints = checkPoints
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        //GET: PartsIq/
        //
        private string ExcelColumnIndexToName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static string GetNextColumn(string column, int hops, bool increment)
        {
            char symbol = '-';
            if (increment)
            {
                symbol = '+';
            }

            if (column.Length == 1)
            {
                char c = column[0];
                c = (char)(c + (symbol == '+' ? hops : -hops)); // Increment or decrement the character value by the number of hops
                return c.ToString();
            }
            else
            {
                char lastChar = column[column.Length - 1]; // Get the last character of the column string
                char nextChar = (char)(lastChar + (symbol == '+' ? hops : -hops)); // Increment or decrement the character value by the number of hops to get the next character
                return column.Substring(0, column.Length - 1) + nextChar; // Concatenate the next character to the original column string
            }
        }

        public string FormatSpecification(string specification)
        {
            if (specification.Contains('�'))
            {
                return specification.Replace('�', '±');
            }
            else
            {
                return specification;
            }
        }

        public string FormatDate(string inputDate)
        {
            DateTime date;
            if (DateTime.TryParseExact(inputDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date) ||
       DateTime.TryParseExact(inputDate, "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
            {
                // Format the DateTime object to the desired string format "MMM dd, yyyy"
                string formattedDate = date.ToString("MMM dd, yyyy");
                return formattedDate; // Output: Jun 28, 2024
            }
            else
            {
                // Handle the case where the input date is not valid
                return "Invalid date format";
            }
        }

        public string FormatDuration(long ms)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(ms);
            string formatDuration = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                                             timeSpan.Hours,
                                             timeSpan.Minutes,
                                             timeSpan.Seconds,
                                             timeSpan.Milliseconds);
            return formatDuration;
        }



        [HttpPost]
        public ActionResult ExportSupplierPerformance(FormCollection formData)
        {
            try
            {
                DateTime date;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string templatePath = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/SupplierPerformance.xlsx");
                FileInfo templateFile = new FileInfo(templatePath);
                var jsonData = formData["partData"] ?? "";
                var accept = formData["accept"] ?? "";
                var sAccept = formData["sAccept"] ?? "";
                var reject = formData["reject"] ?? "";
                var sortOut = formData["sortOut"] ?? "";
                var total = formData["total"] ?? "";
                var toDate = formData["toDate"] ?? "";
                var fromDate = formData["fromDate"] ?? "";
                var supplier = formData["supplier"] ?? "";
                SupplierPerformanceGroup supplierPerformance = JsonConvert.DeserializeObject<SupplierPerformanceGroup>(jsonData);
                if (DateTime.TryParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    // Format the DateTime object to the desired string format "MM, dd, yyyy"
                    string formattedDate = date.ToString("MM, dd, yyyy");
                    Console.WriteLine(formattedDate); // Output: 05, 28, 2024
                }
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    var worksheet = package.Workbook.Worksheets["sheet1"];
                    worksheet.Cells["C6"].Value = supplier;
                    worksheet.Cells["D7"].Value = $"{FormatDate(fromDate)} to {FormatDate(toDate)}";
                    worksheet.Cells["F9"].Value = accept;
                    worksheet.Cells["F10"].Value = sAccept;
                    worksheet.Cells["F11"].Value = sortOut;
                    worksheet.Cells["F12"].Value = reject;
                    worksheet.Cells["F14"].Value = total;

                    // Write headers

                    int row = 16;

                    void WriteGroup(string decisionHeader, DecisionGroup group)
                    {
                        if (group == null)
                        {
                            Debug.WriteLine($"{decisionHeader} group is null");
                            return;
                        }

                        // Write section header
                        worksheet.Cells[row, 1].Value = $"Parts Supplied with {decisionHeader} Decision";
                        worksheet.Cells[row, 1, row, 3].Style.Font.Bold = true; // Make header bold
                        row++;

                        // Write column headers
                        worksheet.Cells[row, 2, row, 5].Merge = true; // Merge cells from column 2 to column 5
                        worksheet.Cells[row, 2].Value = "Part Code"; // Set the value in the merged cell
                        worksheet.Cells[row, 2, row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text
                        worksheet.Cells[row, 2, row, 5].Style.Font.Bold = true; // Apply bold
                        worksheet.Cells[row, 2, row, 5].Style.Font.Italic = true;


                        worksheet.Cells[row, 6, row, 9].Merge = true; // Adjusted to not overlap with previous merge
                        worksheet.Cells[row, 6].Value = "Part Name"; // Set the value in the merged cell
                        worksheet.Cells[row, 6, row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text
                        worksheet.Cells[row, 6, row, 9].Style.Font.Bold = true; // Apply bold
                        worksheet.Cells[row, 6, row, 9].Style.Font.Italic = true;

                        worksheet.Cells[row, 10, row, 13].Merge = true; // Adjusted to not overlap with previous merge
                        worksheet.Cells[row, 10].Value = "Number of NCR Issued"; // Set the value in the merged cell
                        worksheet.Cells[row, 10, row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text
                        worksheet.Cells[row, 10, row, 13].Style.Font.Bold = true; // Apply bold
                        worksheet.Cells[row, 10, row, 13].Style.Font.Italic = true;

                        row++; // Move to the next row


                        // Write data rows
                        foreach (var item in group)
                        {
                            worksheet.Cells[row, 2, row, 5].Merge = true;
                            worksheet.Cells[row, 2].Value = item.Key;
                            worksheet.Cells[row, 2, row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text

                            worksheet.Cells[row, 6, row, 9].Merge = true;
                            worksheet.Cells[row, 6].Value = item.Value.PartName;
                            worksheet.Cells[row, 6, row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text

                            worksheet.Cells[row, 10, row, 13].Merge = true; // Adjusted to not overlap with previous merge
                            worksheet.Cells[row, 10].Value = item.Value.NcrCount;
                            worksheet.Cells[row, 10, row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center the text

                            row++;
                        }

                        // Add an empty row after each group for better readability
                        row++;
                    }



                    WriteGroup("Accept", supplierPerformance.Accept);
                    WriteGroup("Special Accept", supplierPerformance.SpecialAccept);
                    WriteGroup("Sort Out", supplierPerformance.SortOut);
                    WriteGroup("Reject", supplierPerformance.Reject);

                    row += 5;

                    worksheet.Cells[row, 2].Value = "Note: This is a system-generated report, no signature required";
                    worksheet.Cells[row, 2].Style.Font.Italic = true;
                    // Save the Excel package to a memory stream
                    MemoryStream stream = new MemoryStream();
                    package.SaveAs(stream);

                    // Return the Excel file
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SupplierPerformance.xlsx");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
        public ActionResult ExportToExcel(long inspectionId)
        {
            try
            {
                using (var dbContext = new PartsIQEntities())
                {
                    var inspectionDetailsPerId = dbContext.GetInspectionDetailsPerId(inspectionId).ToList();
                    var inspectionItems = dbContext.GetInspectionInfoPerId(inspectionId).ToList();
                    // Assuming GetInspectionDetailsPerId returns IEnumerable

                    // For logging, you can use libraries like log4net
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial if applicable
                    string templatePath = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/Inspection.xlsx");

                    // Load the template into memory
                    FileInfo templateFile = new FileInfo(templatePath);
                    Debug.WriteLine(templateFile.FullName);
                    string inspectionDecision = "";
                    using (ExcelPackage package = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"]; // Modify "Sheet1" as per your sheet name
                        foreach (var detail in inspectionDetailsPerId)
                        {
                            inspectionDecision = detail.Decision;
                            worksheet.Cells["I" + 2].Value = detail.ControlNumber;
                            worksheet.Cells["C" + 7].Value = detail.SupplierName; // Adjust property accordingly
                            worksheet.Cells["C" + 8].Value = detail.PartModel;
                            worksheet.Cells["C" + 9].Value = detail.PartName;
                            worksheet.Cells["C" + 10].Value = detail.PartsCode;
                            worksheet.Cells["C" + 11].Value = detail.DrNumber;

                            worksheet.Cells["H" + 7].Value = detail.LotNumber;
                            worksheet.Cells["H" + 8].Value = detail.LotQuantity;
                            worksheet.Cells["H" + 9].Value = detail.SampleSize;
                            worksheet.Cells["H" + 10].Value = detail.DateDelivered?.ToString("MMMM dd, yyyy");
                            worksheet.Cells["H" + 11].Value = detail.DocNumber;

                            worksheet.Cells["I" + 5].Value = detail.InspectionStart?.ToString("MMMM dd, yyyy");
                            worksheet.Cells["J" + 5].Value = detail.InspectionEnd?.ToString("MMMM dd, yyyy");

                            worksheet.Cells["D" + 13].Value = $"{detail.Temperature} \u2103";

                            worksheet.Cells["I" + 13].Value = $"{detail.Humidity}%";
                            switch (inspectionDecision)
                            {
                                case "Accept":
                                    // Delete the contents of cell C46
                                    worksheet.Cells["C46"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["C46"].Clear();

                                    // Insert the symbol into cell C46
                                    worksheet.Cells["C46"].Value = "\U0001F5F7";
                                    break;
                                case "Special Accept":
                                    // Delete the contents of cell C47
                                    worksheet.Cells["C47"].Clear();
                                    // Insert the symbol into cell C47
                                    worksheet.Cells["C47"].Value = "\U0001F5F7";
                                    break;
                                case "Reject":
                                    // Delete the contents of cell C48
                                    worksheet.Cells["C48"].Clear();
                                    // Insert the symbol into cell C48
                                    worksheet.Cells["C48"].Value = "\U0001F5F7";
                                    break;
                                case "Sort out":
                                    // Delete the contents of cell C49
                                    worksheet.Cells["C49"].Clear();
                                    // Insert the symbol into cell C49
                                    worksheet.Cells["C49"].Value = "\U0001F5F7";
                                    break;
                            }
                            worksheet.Cells["A46"].Value = detail.Comments;
                            worksheet.Cells["E48"].Value = detail.Inspector;

                        }
                        string templateRange = "A1:J50";

                        int columnCount = inspectionItems.Count;
                        int templateCopiesNeeded = (columnCount - 1) / 8 + 1;
                        int templateWidth = 10; // Number of columns covered by the template
                        List<string> destinationCell1 = new List<string>
                    {
                        "C"
                    };
                        for (int i = 0; i < templateCopiesNeeded; i++)
                        {
                            // Calculate the column index for the destination cell
                            int destinationColumnIndex = i * templateWidth + 11; // 11 is the column index of column K

                            // Convert the column index to the corresponding Excel column letter
                            string destinationColumnLetter = ExcelColumnIndexToName(destinationColumnIndex);

                            // Construct the destination cell address
                            string destinationCell = $"{destinationColumnLetter}1";
                            destinationCell1.Add(destinationColumnLetter);

                            // Use the destination cell address to paste the copied template format
                            // Copy format from the template range to the destination cells
                            worksheet.Cells[templateRange].Copy(worksheet.Cells[destinationCell]);

                        }
                        int cellDivisionCounter = 1;
                        int listCounter = 1;
                        string inspectionCell = "C";
                        string cavityCell = "B";
                        foreach (var detail in inspectionItems)
                        {
                            var cavity = "";
                            //char inspectionCell = destinationCell1[listCounter];
                            double minChk = double.MaxValue;
                            double maxChk = 0;

                            int CheckpointCell = 22;
                            worksheet.Cells[inspectionCell.ToString() + 16].Value = detail.CCode;
                            worksheet.Cells[inspectionCell.ToString() + 17].Value = FormatSpecification(detail.Specification);
                            if (detail.UpperLimit > 0)
                            {
                                worksheet.Cells[inspectionCell.ToString() + 18].Value = detail.UpperLimit;

                            }
                            else
                            {
                                worksheet.Cells[inspectionCell.ToString() + 18].Value = "N/A";
                            }
                            if (detail.UpperLimit > 0)
                            {
                                worksheet.Cells[inspectionCell.ToString() + 19].Value = detail.LowerLimit;

                            }
                            else
                            {
                                worksheet.Cells[inspectionCell.ToString() + 19].Value = "N/A";
                            }
                            worksheet.Cells[inspectionCell.ToString() + 20].Value = detail.Tool;
                            worksheet.Cells[inspectionCell.ToString() + 44].Value = detail.WithInvalid != 0 ? "NG" : "G";

                            var InspectionCheckpoints = dbContext.GetInspectionCheckpoints(inspectionId, (long)detail.CheckpointId);

                            foreach (var checkPoint in InspectionCheckpoints)
                            {
                                if (checkPoint.Measurement < minChk)
                                {
                                    minChk = checkPoint.Measurement;
                                }

                                if (checkPoint.Measurement > maxChk)
                                {
                                    maxChk = checkPoint.Measurement;
                                }

                                if ((checkPoint.MeasurementAttr.Length > 0))
                                {
                                    worksheet.Cells[inspectionCell.ToString() + CheckpointCell].Value = checkPoint.MeasurementAttr;
                                }
                                else
                                {
                                    worksheet.Cells[inspectionCell.ToString() + CheckpointCell].Value = checkPoint.Measurement;
                                }

                                cavity = checkPoint.CavityNumber;
                                worksheet.Cells[cavityCell + CheckpointCell].Value = cavity;

                                Debug.WriteLine(checkPoint.IsGood);
                                CheckpointCell++;
                            }
                            //Ternary Operator that is not strongly type is having a error for this version
                            if (maxChk > 0)
                            {
                                worksheet.Cells[inspectionCell.ToString() + 43].Value = maxChk;
                            }
                            else
                            {
                                worksheet.Cells[inspectionCell.ToString() + 43].Value = "N/A";
                            }
                            if (minChk > 0)
                            {
                                worksheet.Cells[inspectionCell.ToString() + 42].Value = minChk;
                            }
                            else
                            {
                                worksheet.Cells[inspectionCell.ToString() + 42].Value = "N/A";
                            }




                            inspectionCell = GetNextColumn(inspectionCell, 1, true);

                            if (cellDivisionCounter == 8)
                            {
                                inspectionCell = destinationCell1[listCounter];
                                Debug.WriteLine($"Inspection decision {inspectionDecision} and cell is {inspectionCell}");


                                cavityCell = GetNextColumn(inspectionCell, 1, true);
                                Debug.WriteLine($"{cavityCell} is the Cavity Cell");
                                inspectionCell = GetNextColumn(inspectionCell, 2, true);

                                listCounter++;
                                cellDivisionCounter = 0;
                            }
                            cellDivisionCounter++;
                        }

                        // Prepare response to send to the browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=YourExportedFile.xlsx");

                        // Write the file to the Response.OutputStream
                        Response.BinaryWrite(package.GetAsByteArray());
                        Response.Flush(); // Flush the response to ensure all content is sent
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        //This Controller is for Exporting Part Performance Charts [XbarChart || xChart, R (Ichart) Chart || Mr Chart, and Cavity Reltaion Chart. 
        public ActionResult ExportPartPerformance(FormCollection formData)
        {
            try
            {
                // Ensure EPPlus can operate under a non-commercial license
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //Loading of Template
                string templatePath = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/PartPerformance.xlsx");
                FileInfo templateFile = new FileInfo(templatePath);

                var jsonData = (formData["jsonData"]) ?? "";
                var partDataLimit = (formData["partData"]) ?? "";
                string imageX = (formData["imageX"]) ?? "";
                string imageMr = (formData["imageMr"]) ?? "";
                string imageR = (formData["imageR"]) ?? "";
                string action = (formData["action"]) ?? "";
                Debug.WriteLine(templateFile);
                var partDataList = JsonConvert.DeserializeObject<List<PerformanceData>>(partDataLimit);
                var partData = JsonConvert.DeserializeObject<ImageDataModel>(jsonData);

                byte[] imageBytesX = !string.IsNullOrEmpty(imageX) ? Convert.FromBase64String(imageX.Split(',')[1]) : null;
                byte[] imageBytesMr = !string.IsNullOrEmpty(imageMr) ? Convert.FromBase64String(imageMr.Split(',')[1]) : null;
                byte[] imageBytesR = !string.IsNullOrEmpty(imageR) ? Convert.FromBase64String(imageR.Split(',')[1]) : null;

                string sheet1Title = partData.HeaderTitle != "1" ? "Part Performance x Chart" : "Part Performance xBar Chart";
                string sheet2Title = partData.HeaderTitle != "1" ? "Part Performance MR Chart" : "Part Performance R Chart";

                using (var package = new ExcelPackage(templateFile))
                {
                    var worksheetTemplate = package.Workbook.Worksheets["sheet"];
                    if (worksheetTemplate == null)
                    {
                        return new HttpStatusCodeResult(404, "Worksheet not found.");
                    }
                    //For Both xBar || X chart and Mr || R(I) Chart
                    if (action == "xBar")
                    {
                        // Create and populate the first sheet for xBar or xChart
                        var worksheetX = package.Workbook.Worksheets.Add(sheet1Title, worksheetTemplate);
                        AddImageToWorksheet(worksheetX, imageBytesX, "ChartImage");
                        AddTitleToWorksheet(worksheetX, sheet1Title);
                        PopulateWorksheetData(worksheetX, partData, partDataList, false);

                        // Create and populate the second sheet for MR or R Chart
                        var worksheetMr = package.Workbook.Worksheets.Add(sheet2Title, worksheetTemplate);
                        AddImageToWorksheet(worksheetMr, imageBytesMr, "ChartImageMr");
                        AddTitleToWorksheet(worksheetMr, sheet2Title);
                        PopulateWorksheetData(worksheetMr, partData, partDataList, true);
                    }
                    //For Cavity Relation Chart
                    else if (action == "rChart")
                    {
                        // Create and populate the sheet for Relation Chart
                        var worksheetR = package.Workbook.Worksheets.Add("Cavity Relation Chart", worksheetTemplate);
                        AddImageToWorksheet(worksheetR, imageBytesR, "ChartImageR");
                        AddTitleToWorksheet(worksheetR, "Cavity Relation Chart");
                        PopulateWorksheetData(worksheetR, partData, partDataList, true); // Adjust as needed for relation chart data
                    }

                    // Remove the template worksheet
                    package.Workbook.Worksheets.Delete(worksheetTemplate);

                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "chart.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately in a real-world scenario
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
        //Attaching the Chart Image in Workbook
        private void AddImageToWorksheet(ExcelWorksheet worksheet, byte[] imageBytes, string imageName)
        {
            if (imageBytes != null)
            {
                using (var stream = new MemoryStream(imageBytes))
                {
                    var picture = worksheet.Drawings.AddPicture(imageName, stream);
                    //(B10 - M33)
                    picture.SetPosition(9, 0, 1, 0); // Position the image (row 9, column 1)
                }
            }
        }
        //Adding Title on Worksheets
        private void AddTitleToWorksheet(ExcelWorksheet worksheet, string title)
        {
            worksheet.Cells["B1"].Value = title;
            var titleCell = worksheet.Cells["B1"];
            titleCell.Style.Font.Size = 18; // Set font size
            titleCell.Style.Font.Name = "Arial"; // Set font name
            titleCell.Style.Font.Bold = true; // Make the text bold
            titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }
        //Plotting Common Part Details
        private void PopulateWorksheetData(ExcelWorksheet worksheet, ImageDataModel partData, List<PerformanceData> partDataList, bool isMrOrRChart)
        {
            worksheet.Cells["D5"].Value = partData.PartCode;
            worksheet.Cells["D6"].Value = partData.PartName;
            worksheet.Cells["D7"].Value = partData.Checkpoint;
            worksheet.Cells["K5"].Value = partData.Supplier;
            worksheet.Cells["K6"].Value = partData.FromDate.ToShortDateString();
            worksheet.Cells["L6"].Value = partData.ToDate.ToShortDateString();
            worksheet.Cells["K7"].Value = partData.Cavity;
            worksheet.Cells["E8"].Value = partData.LowerLimit;
            worksheet.Cells["L8"].Value = partData.UpperLimit;

            int rowStart = 11;
            foreach (var part in partDataList)
            {
                worksheet.Cells[$"O{rowStart}"].Value = part.Cavity;
                if (isMrOrRChart)
                {
                    worksheet.Cells[$"P{rowStart}"].Value = part.MrChart.Lcl?.ToString() ?? "N/A";
                    worksheet.Cells[$"Q{rowStart}"].Value = part.MrChart.Ucl?.ToString() ?? "N/A";
                }
                else
                {
                    worksheet.Cells[$"P{rowStart}"].Value = part.IChart.Lcl?.ToString() ?? "N/A";
                    worksheet.Cells[$"Q{rowStart}"].Value = part.IChart.Ucl?.ToString() ?? "N/A";
                }
                rowStart++;
            }
        }

        private object GetNonconformityFn(long inspectionId, int isNg)
        {
            Debug.WriteLine(inspectionId);
            List<InspectionInfoPerId> inspectionList = new List<InspectionInfoPerId>();
            List<InspectionCheckpoint> inspectionCheckpoint = new List<InspectionCheckpoint>();

            using (var dbContext = new PartsIQEntities())
            {
                var inspectionInfoPerId = dbContext.GetInspectionInfoPerId(inspectionId);

                // Check if inspectionInfoPerId is null or empty
                if (inspectionInfoPerId != null)
                {
                    foreach (var inspection in inspectionInfoPerId)
                    {
                        if (inspection.WithInvalid == 1)
                        {
                            inspectionList.Add(inspection);
                        }
                        else
                        {

                        }
                    }

                    foreach (var inspection in inspectionList)
                    {
                        var res = dbContext.GetInspectionCheckpoints(inspectionId, (long)inspection.CheckpointId);

                        // Since GetInspectionCheckpoints returns a list, filter and add non-good checkpoints
                        if (res != null)
                        {
                            inspectionCheckpoint.AddRange(res);
                        }
                    }
                }
            }

            // Return both inspectionList and inspectionCheckpointsNg as needed
            var result = new
            {
                Inspections = inspectionList,
                Checkpoints = inspectionCheckpoint
            };

            return result;
        }
        //Get NCR 
        public ActionResult GetNonconformityReport(long inspectionId)
        {
            try
            {
                dynamic result = GetNonconformityFn(inspectionId, 1);
                var ngCheckpoint = result.Checkpoints;
                var processor = new InspectionProcessor();
                var maxNgReject = processor.GetMaxNgPercentage(ngCheckpoint);

                var returnObj = new
                {
                    Checkpoints = ngCheckpoint,
                    RejectPercentage = maxNgReject,
                };

                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ExportNonConformityReport(FormCollection formData)
        {
            try
            {
                var inspectionInfo = formData["formData"] ?? "";
                var remarks = formData["nRemarks"] ?? "";
                var controlNumber = formData["nControl"] ?? "";
                InspectionInfo inspection = JsonConvert.DeserializeObject<InspectionInfo>(inspectionInfo);
                Debug.WriteLine(inspection);
                var inspectionId = inspection.InspectionId;

                dynamic result = GetNonconformityFn(inspectionId, 0);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string template = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/NonConformity.xlsx");

                FileInfo templatefile = new FileInfo(template);
                if (!templatefile.Exists)
                {
                    return Json("Error, no template file is found", JsonRequestBehavior.AllowGet);
                }
                using (ExcelPackage package = new ExcelPackage(templatefile))
                {
                    var worksheet = package.Workbook.Worksheets["sheet1"];
                    DateTime now = DateTime.Now;
                    worksheet.Cells["U3"].Value = now.ToString("MMM dd, yyyy");
                    worksheet.Cells["F6"].Value = inspection.SupplierName;
                    worksheet.Cells["F7"].Value = inspection.SupplierInCharge;
                    worksheet.Cells["O6"].Value = inspection.LotNumber;
                    worksheet.Cells["U6"].Value = inspection.PartName;
                    worksheet.Cells["F8"].Value = inspection.DrNumber;
                    worksheet.Cells["O8"].Value = inspection.LotQuantity;
                    worksheet.Cells["U8"].Value = inspection.PartsCode;
                    worksheet.Cells["U1"].Value = controlNumber;
                    worksheet.Cells["C36"].Value = remarks;
                    var checkpointNg = result.Checkpoints;
                    var processor = new InspectionProcessor();
                    var measurementStats = processor.GetMinMaxOrigMeasurementPerCCode(checkpointNg);
                    var maxNgReject = processor.GetMaxNgPercentage(checkpointNg);
                    worksheet.Cells["F9"].Value = maxNgReject.QuantitySamples;
                    worksheet.Cells["K9"].Value = maxNgReject.NgRejectSamples;
                    worksheet.Cells["Q9"].Value = maxNgReject.RejectPercentage / 100;
                    worksheet.Cells["G43"].Value = now.AddDays(14).ToString("MMM dd, yyyy");
                    Debug.WriteLine($"Quantity Samples {maxNgReject.QuantitySamples} {maxNgReject.NgRejectSamples} {maxNgReject.RejectPercentage}");
                    var rowCount = 14;

                    foreach (var stat in measurementStats)
                    {
                        Debug.WriteLine($"CCode: {stat.CCode}, MinOrigMeasurement: {stat.MinOrigMeasurement}, MaxOrigMeasurement: {stat.MaxOrigMeasurement}, Specification: {stat.Specification}");
                        worksheet.Cells["B" + rowCount].Value = stat.CCode;
                        worksheet.Cells["E" + rowCount].Value = FormatSpecification(stat.Specification);
                        var cellK = worksheet.Cells["K" + rowCount];
                        cellK.Value = $"Max={stat.MaxOrigMeasurement}, Min={stat.MinOrigMeasurement}";

                        worksheet.Cells["K" + rowCount].Value = (stat.MinOrigMeasurement > 0 ? $"Min={stat.MinOrigMeasurement}" : string.Empty);
                        worksheet.Cells["N" + rowCount].Value = (stat.MaxOrigMeasurement > 0 ? $"Max={stat.MaxOrigMeasurement}" : string.Empty);



                        // Check if the measurements are out of limits and style accordingly

                        if (stat.MaxOrigMeasurement > stat.UpperLimit)
                        {
                            worksheet.Cells["N" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                        if (stat.MinOrigMeasurement < stat.LowerLimit)
                        {
                            worksheet.Cells["K" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        rowCount++;  // Increment the rowCount to move to the next row for the next stat
                    }
                    var view = worksheet.View;
                    // Clear any existing page breaks
                    view.PageBreakView = false;
                    worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "chart.xlsx");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
 

        [HttpPost]
        public ActionResult ExportInspectionStats(FormCollection formCollection)
        {
            try
            {
                DateTime startDate = DateTime.Now.AddYears(-1);
                DateTime endDate = DateTime.Now;
                DateTime? fromDateIs = startDate;
                DateTime? toDateIs = endDate;
                var partCode = formCollection["partsCodeIs"];

                if (formCollection != null)
                {
                    if (!string.IsNullOrEmpty(formCollection["fromDateIs"]) && formCollection["fromDateIs"] != "null" && DateTime.TryParse(formCollection["fromDateIs"], out DateTime parsedFromDate))
                    {
                        fromDateIs = parsedFromDate;
                    }

                    // Handle toDateIs
                    if (!string.IsNullOrEmpty(formCollection["toDateIs"]) && formCollection["toDateIs"] != "null" && DateTime.TryParse(formCollection["toDateIs"], out DateTime parsedToDate))
                    {
                        toDateIs = parsedToDate;
                    }
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string template = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/InspectionStats.xlsx");
                FileInfo templatefile = new FileInfo(template);
                List<InspectionStatistics> inspectionStats = GetInspectionStatisticsFn(formCollection);
                var summaryObj = GetInspectionStatsMinMax(inspectionStats);

                Debug.WriteLine(inspectionStats);

                if (!templatefile.Exists)
                {
                    return Json("Error, no template file is found", JsonRequestBehavior.AllowGet);
                }
                using (ExcelPackage package = new ExcelPackage(templatefile))
                {
                    var worksheet = package.Workbook.Worksheets["Inspection Statistics"];
                    worksheet.Cells["C3"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null" ? "All" : inspectionStats.FirstOrDefault().PartCode;
                    worksheet.Cells["C4"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null" ? "" : inspectionStats.FirstOrDefault().PartName;
                    worksheet.Cells["C5"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null" ? "" : inspectionStats.FirstOrDefault().Inspector;
                    worksheet.Cells["C7"].Value = summaryObj.TotalPartsInspected;
                    worksheet.Cells["C8"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null"
                                                                                    ? ""
                                                                                    : summaryObj.MaxInspectionTime.ToString(@"hh\:mm\:ss\.fff");
                    worksheet.Cells["C9"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null"
                                                                                   ? ""
                                                                                   : summaryObj.MinInspectionTime.ToString(@"hh\:mm\:ss\.fff");

                    worksheet.Cells["C10"].Value = string.IsNullOrEmpty(partCode) || partCode.ToLower() == "null"
                                                                                    ? ""
                                                                                    : summaryObj.AverageInspectionTime.ToString(@"hh\:mm\:ss\.fff");

                    worksheet.Cells["H3"].Value = FormatDate(fromDateIs.ToString());
                    worksheet.Cells["H4"].Value = FormatDate(toDateIs.ToString());

                    int numRow = 13;
                    foreach (var inspection in inspectionStats)
                    {
                        DateTime dateDelivered = inspection.DateDelivered ?? DateTime.Now;
                        worksheet.Cells["B" + numRow].Value = dateDelivered.ToString("MMM dd, yyyy"); ;
                        worksheet.Cells["C" + numRow].Value = inspection.DrNumber;
                        worksheet.Cells["D" + numRow].Value = inspection.PartCode;
                        worksheet.Cells["E" + numRow].Value = inspection.PartName;
                        worksheet.Cells["F" + numRow].Value = inspection.LotNumber;
                        DateTime dateFrom = inspection.InspectionStart ?? DateTime.Now;
                        DateTime dateTo = inspection.InspectionEnd ?? DateTime.Now;
                        worksheet.Cells["G" + numRow].Value = dateFrom.ToString("MM, dd yy h:mm:ss tt");
                        worksheet.Cells["H" + numRow].Value = dateTo.ToString("MM, dd yy h:mm:ss tt");
                        worksheet.Cells["I" + numRow].Value = FormatDuration((long)inspection.InspectionDuration);
                        worksheet.Cells["J" + numRow].Value = FormatDuration((long)inspection.EvaluationDuration);
                        worksheet.Cells["K" + numRow].Value = inspection.Inspector;
                        worksheet.Cells["L" + numRow].Value = inspection.Decision;
                        numRow++;
                    }
                    var range = worksheet.Cells[$"B13:L{numRow}"];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "chart.xlsx");
                }
            }
            catch (Exception ex)
            {
                return Json($"Error occured {ex}");
            }


        }

        private InspectionStatsSummary GetInspectionStatsMinMax(List<InspectionStatistics> stats)
        {
            try
            {
                var InspectionSummary = new InspectionStatsSummary();
                var summary = InspectionSummary.Calculate(stats);
                return summary;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error message: {ex.Message}");
            }
        }

        public ActionResult GetInspectionStatistics(FormCollection formCollection = null)
        {
            try
            {
                List<InspectionStatistics> stats = GetInspectionStatisticsFn(formCollection);
                return new LargeJsonResult { Data = stats, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            catch (Exception ex)
            {
                return Json($"Error {ex}", JsonRequestBehavior.AllowGet);
            }

        }
        public List<InspectionStatistics> GetInspectionStatisticsFn(FormCollection formData = null)
        {
            try
            {
                DateTime startDate = DateTime.Now.AddYears(-1);
                DateTime endDate = DateTime.Now;
                DateTime? fromDateIs = startDate;
                DateTime? toDateIs = endDate;
                long? partsCodeIs = null;
                string minMaxIs = null;
                string inspectorIs = null;

                if (formData != null)
                {
                    if (!string.IsNullOrEmpty(formData["fromDateIs"]) && formData["fromDateIs"] != "null" && DateTime.TryParse(formData["fromDateIs"], out DateTime parsedFromDate))
                    {
                        fromDateIs = parsedFromDate;
                    }

                    // Handle toDateIs
                    if (!string.IsNullOrEmpty(formData["toDateIs"]) && formData["toDateIs"] != "null" && DateTime.TryParse(formData["toDateIs"], out DateTime parsedToDate))
                    {
                        toDateIs = parsedToDate;
                    }


                    if (!string.IsNullOrEmpty(formData["inspectorIs"]) && formData["inspectorIs"] != "null")
                    {
                        inspectorIs = formData["inspectorIs"];
                    }


                    // Handle partsCodeIs

                    if (!string.IsNullOrEmpty(formData["partsCodeIs"]) && formData["partsCodeIs"] != "null" && long.TryParse(formData["partsCodeIs"], out long parsedPartsCode))
                    {
                        partsCodeIs = parsedPartsCode;
                    }


                    minMaxIs = formData["minMaxIs"];
                    inspectorIs = formData["inspectorIs"];
                }


                using (var dbContext = new PartsIQEntities())
                {
                    var res = dbContext.GetInspectionStatistics(fromDateIs, toDateIs, inspectorIs, minMaxIs, partsCodeIs);


                    return res;
                }
            }
            catch (Exception ex)
            {
                return new List<InspectionStatistics>();
            }


        }


        public ActionResult GetLogsList(FormCollection formCollection)
        {
            try
            {
                var res = GetLogsListFn(formCollection);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json($"Error {ex} occured. Contact Administrator", JsonRequestBehavior.AllowGet);
            }
        }
        public List<Logs> GetLogsListFn(FormCollection formData = null)
        {
            try
            {
                DateTime startDate = DateTime.Now.AddYears(-1);
                DateTime endDate = DateTime.Now;
                DateTime? fromDateIs = startDate;
                DateTime? toDateIs = endDate;
                long? userId = null;
                string logEvent = null;
                if (formData != null)
                {
                    if (!string.IsNullOrEmpty(formData["fromDateLl"]) && formData["fromDateLl"] != "null" && DateTime.TryParse(formData["fromDateLl"], out DateTime parsedFromDate))
                    {
                        fromDateIs = parsedFromDate;
                    }

                    // Handle toDateIs
                    if (!string.IsNullOrEmpty(formData["toDateLl"]) && formData["toDateLl"] != "null" && DateTime.TryParse(formData["toDateLl"], out DateTime parsedToDate))
                    {
                        toDateIs = parsedToDate;
                    }

                    if (!string.IsNullOrEmpty(formData["eventLl"]) && formData["eventLl"] != "null")
                    {
                        logEvent = formData["eventLl"];
                    }
                    if (!string.IsNullOrEmpty(formData["userLl"]) && formData["userLl"] != "null" && long.TryParse(formData["UserLl"], out long userLl))
                    {
                        userId = userLl;
                    }
                }
                using (var dbContext = new PartsIQEntities())
                {
                    var logsList = dbContext.GetLogsList(fromDateIs, toDateIs, userId, logEvent);
                    return logsList;

                }
            }
            catch (Exception ex)
            {
                return new List<Logs>();
            }

        }

        public ActionResult ExportLogsList(FormCollection formCollection)
        {
            try
            {
                var result = GetLogsListFn(formCollection);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("sheet1");
                    worksheet.Cells["A1"].Value = "Date Log";
                    worksheet.Cells["B1"].Value = "User";
                    worksheet.Cells["C1"].Value = "Event";
                    worksheet.Cells["D1"].Value = "Event Details";

                    int row = 2;
                    foreach (var logs in result)
                    {
                        DateTime dateLog = logs.DateLog ?? DateTime.Now;
                        worksheet.Cells["A" + row].Value = dateLog.ToString("MM, dd yy h:mm:ss tt");
                        worksheet.Cells["B" + row].Value = logs.UserName;
                        worksheet.Cells["C" + row].Value = logs.Event;
                        worksheet.Cells["D" + row].Value = logs.LogComment;
                        row++;

                    }
                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "logs.xlsx");
                }
            }
            catch (Exception ex)
            {
                return Json($"Error {ex.Message}", JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetNcrList(FormCollection formCollection)
        {
            try
            {
                var ncrList = GetNcrListFn(formCollection);
                return Json(ncrList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json($"Error {e.Message} occured.", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ExportNcrList(FormCollection formCollection)
        {
            try
            {
                // Fetch the NCR list based on the form collection
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string template = Server.MapPath("~/assets/excelFiles/Templates/PartsIq/NcrReport.xlsx");
                FileInfo templatefile = new FileInfo(template);
                if (!templatefile.Exists)
                {
                    return Json("Error, no template file is found", JsonRequestBehavior.AllowGet);
                }
                var ncrList = GetNcrListFn(formCollection);
                using (ExcelPackage package = new ExcelPackage(templatefile))
                {
                    DateTime dateNow = DateTime.Now;
                    var worksheet = package.Workbook.Worksheets["sheet1"];
                    int row = 11;
                    worksheet.Cells["C" + 6].Value = dateNow.ToString("MMM dd, yyyy");
                    worksheet.Cells["C" + 7].Value = ncrList.FirstOrDefault().SupplierName;
                    foreach (var inspection in ncrList)
                    {
                        DateTime dateInspected = inspection.DateEnded ?? DateTime.Now;
                       
                       
                        worksheet.Cells["B" + row].Value = inspection.NcrNumber;
                        worksheet.Cells["C" + row].Value = dateInspected.ToString("MMM dd, yyyy");
                        worksheet.Cells["D" + row].Value = inspection.DrNumber;
                        worksheet.Cells["E" + row].Value = inspection.PartCode;
                        worksheet.Cells["F" + row].Value = inspection.PartName;

                        dynamic result =  GetNonconformityFn((long)inspection.InspectionId, 0);                  
                        //List<InspectionCheckpoint> checkpoints = res.Checkpoints as List<InspectionCheckpoint>;
                        //foreach (var checkpoint in checkpoints)
                        //{


                        //    worksheet.Cells["G" + row].Value = checkpoint.Specification;
                        //    worksheet.Cells["H" + row].Value = checkpoint.MeasurementAttr;
                        //    break;
                        //}
                        var checkpointNg = result.Checkpoints;
                        var processor = new InspectionProcessor();
                        var measurementStats = processor.GetMinMaxOrigMeasurementPerCCode(checkpointNg);

                        var descriptionBuilder = new StringBuilder();
                        var actualDimensionBuilder = new StringBuilder();

                        foreach (var stat in measurementStats)
                        {
                            descriptionBuilder.Append(FormatSpecification(stat.Specification) + " ");

                            if (!String.IsNullOrEmpty(stat.MeasurementAttr) && !String.IsNullOrWhiteSpace(stat.MeasurementAttr))
                            {
                                actualDimensionBuilder.Append(stat.MeasurementAttr + " ");
                            }
                            else
                            {
                                var minMeasurement = stat.MinOrigMeasurement > 0 ? $"Min={stat.MinOrigMeasurement}" : string.Empty;
                                var maxMeasurement = stat.MaxOrigMeasurement > 0 ? $"Max={stat.MaxOrigMeasurement}" : string.Empty;

                                // Add space between min and max if both are present
                                if (!string.IsNullOrEmpty(minMeasurement) && !string.IsNullOrEmpty(maxMeasurement))
                                {
                                    actualDimensionBuilder.Append(minMeasurement + " " + maxMeasurement + " ");
                                }
                                else
                                {
                                    actualDimensionBuilder.Append(minMeasurement + maxMeasurement + " ");
                                }
                            }
                        }

                        string description = descriptionBuilder.ToString().Trim();
                        string actualDimension = actualDimensionBuilder.ToString().Trim();
                        Debug.Write(actualDimension);
                        worksheet.Cells["G" + row].Value = description;
                        worksheet.Cells["H" + row].Value = actualDimension;
                        worksheet.Cells["I" + row].Value = inspection.Decision;
                        row++;

                    }
                
                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "logs.xlsx");
                }
            }
                
            
            catch (Exception ex)
            {
                // Log the exception (not shown here) and return a user-friendly error message
                return Json(new { error = $"An error {ex} occurred while processing your request. Please try again later." }, JsonRequestBehavior.AllowGet);
            }
        }
        private List<NcrDetails> GetNcrListFn(FormCollection formData = null)
        {
            try
            {
                long? partIdN = null;
                long? supplierN = null;
                if (formData != null)
                {
                    if (!string.IsNullOrEmpty(formData["partCodeN"]) && long.TryParse(formData["partCodeN"], out long partCode))
                    {
                        partIdN = partCode;
                    }

                    // Handle toDateIs
                    if (!string.IsNullOrEmpty(formData["supplierN"]) && formData["supplierN"] != "null" && long.TryParse(formData["supplierN"], out long supplier))    
                    {
                        supplierN = supplier;
                    }
                }
                using (dbMysql = new PartsIQEntities())
                {
                    var ncrList = dbMysql.GetNcrList(partIdN, supplierN);
                    return ncrList;
                }
            }
            catch (Exception ex)
            {
                return new List<NcrDetails> ();
            }
        }

        public ActionResult GetPartQualityPerformance(FormCollection formData = null)
        {
            var res = GetQualityPerformanceFn(formData);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public List<InspectionSummary> GetQualityPerformanceFn(FormCollection formData = null)
        {
            try
            {
                DateTime startDate = DateTime.Now.AddYears(-1);
                DateTime endDate = DateTime.Now;
                DateTime? fromDate = startDate;
                DateTime? toDate = endDate;
                long? supplierId = null;
                long? partId = null;
                if (formData != null)
                {
                    if (!string.IsNullOrEmpty(formData["fromDatPartQuality"]) && formData["fromDatPartQuality"] != "null" && DateTime.TryParse(formData["fromDatPartQuality"], out DateTime parsedFromDate))
                    {
                        fromDate = parsedFromDate;
                    }
                    if (!string.IsNullOrEmpty(formData["supplierQualityPerf"]) && formData["supplierQualityPerf"] != "null" && long.TryParse(formData["supplierQualityPerf"], out long supplierIdP))
                    {
                        supplierId = supplierIdP;
                    }
                    if (!string.IsNullOrEmpty(formData["partQualityPerf"]) && formData["partQualityPerf"] != "null" && long.TryParse(formData["partQualityPerf"], out long partIdP))
                    {
                        partId = partIdP;
                    } 

                    // Handle toDateIs
                    if (!string.IsNullOrEmpty(formData["toDatPartQuality"]) && formData["toDatPartQuality"] != "null" && DateTime.TryParse(formData["toDatPartQuality"], out DateTime parsedToDate))
                    {
                        toDate = parsedToDate;
                    }

                    //if (!string.IsNullOrEmpty(formData["eventLl"]) && formData["eventLl"] != "null")
                    //{
                    //    logEvent = formData["eventLl"];
                    //}
                    //if (!string.IsNullOrEmpty(formData["userLl"]) && formData["userLl"] != "null" && long.TryParse(formData["UserLl"], out long userLl))
                    //{
                    //    userId = userLl;
                    //}
                }
                var res = dbMysql.GetPartQualityPerformance(fromDate, toDate, partId, supplierId);
                return res;
               
            }
            catch (Exception ex)
            {
                return new List<InspectionSummary> ();
            }
        }
    }
}
