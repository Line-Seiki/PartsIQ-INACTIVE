using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PartsMySql.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using MySql.Data.EntityFramework;
using PartsMysql.Models;
using System.Data.SqlClient;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics;
using System.Web.Helpers;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Collections;
using System.Web.Mvc;
using OfficeOpenXml.Export.HtmlExport.StyleCollectors.StyleContracts;

namespace PartsMysql.DataAccess
{
 
    public class PartsIQEntities : DbContext
    {
        public DbSet<Inspection> Inspection { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
        }
        public List<InspectionSummary> GetInspectionSummarry()
        {
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;

            Console.WriteLine(startDate);

            // Query the SQL view using raw SQL
            return this.Database.SqlQuery<InspectionSummary>(
                "SELECT TOP (1000)[InspectionId], [Decision], [Evaluator], [Inspector], [ControlNumber], [DateFinished], " +
                "[PartCode], [PartName], [LotNumber], [LotQuantity], [DrNumber], [Time], [Comments], [InspectorComments], " +
                "[NcrId], [SupplierId], [SupplierName], [SupplierInCharge]" +
                "FROM [PartsIQ].[dbo].[InspectionSummaryView] " +
                "WHERE [Decision] != 'Pending' AND [DateFinished] >= @StartDate AND [DateFinished] <= @EndDate ORDER BY [DateFinished] DESC",
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            ).ToList();
        }

        public List <Suppliers> GetSuppliers()
        {
            return this.Database.SqlQuery<Suppliers>(
            "SELECT TOP (1000) Id,InCharge,Name,Version FROM [PartsIQ].[dbo].[SupplierDetailsView] " +
            "ORDER BY CAST(Name AS NVARCHAR(MAX))"
                ).ToList();
        }
        public List<SupplierPerformance> GetSuppliersPerformance(long SupplierId, DateTime DateFrom, DateTime DateTo, string PartCode)
        {
            // Query the SQL view using raw SQL
            string sqlQuery = "SELECT Decision, DateFinished, PartName, PartsCode, LotNumber, SupplierId, SupplierName, NcrNumber " +
                              "FROM [PartsIQ].[dbo].[InspectionDetailsView] " +
                              "WHERE SupplierId = @SupplierId AND DateFinished >= @DateFrom AND DateFinished <= @DateTo";

            List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@SupplierId", SupplierId),
                        new SqlParameter("@DateFrom", DateFrom),
                        new SqlParameter("@DateTo", DateTo)
                    };

            if (!string.IsNullOrEmpty(PartCode))
            {
                sqlQuery += " AND PartsCode = @PartsCode";
                parameters.Add(new SqlParameter("@PartsCode", PartCode));
            }

            sqlQuery += " ORDER BY DateFinished DESC";

            return this.Database.SqlQuery<SupplierPerformance>(sqlQuery, parameters.ToArray()).ToList();
        }

        public List <InspectionInfo> GetInspectionDetailsPerId(long Id)
        {
            return this.Database.SqlQuery<InspectionInfo>(
            "SELECT InspectionId, Time, Decision, InspectionStart, InspectionEnd, Comments, Inspector, Evaluator, PartName, PartModel, PartsCode, ControlNumber, DocNumber, LotNumber, LotQuantity, SampleSize, SupplierName, " +
            " CavityCount,DrNumber, DocNumber, DateDelivered, Temperature, Humidity " +
            "FROM [PartsIQ].[dbo].[InspectionDetailsView] " +
            "WHERE InspectionId = @InspectionId " +
            "ORDER BY InspectionEnd DESC",
            new SqlParameter("@InspectionId", Id)
            ).ToList();
        }

        public List<InspectionInfoPerId> GetInspectionInfoPerId(long Id)
        {

            return this.Database.SqlQuery<InspectionInfoPerId>(
            "SELECT InspectionId, Comments, DeliveryId, PartName, CheckpointId, CCode, LowerLimit, UpperLimit, WithInvalid, Specification, ChIsActive, Tool " +
            "FROM [PartsIQ].[dbo].[InspectionInfo] " +
            "WHERE InspectionId = @InspectionId AND ChIsActive != 0 " +
            "ORDER BY InspectionId",
            new SqlParameter("@InspectionId", Id)
            ).ToList();
        }

        public List<InspectionCheckpoint> GetInspectionCheckpoints(long inspectionId, long checkpointId)
        {

            return this.Database.SqlQuery<InspectionCheckpoint>(
            "SELECT LowerLimit, UpperLimit, CCode, Specification, InspectionItemId, InspectionId, CheckpointId, IsGood, Measurement, OrigMeasurement, SampleNumber, CavityNumber, MeasurementAttr " +
            "FROM [PartsIQ].[dbo].[InspectionInfoCheckpoint] " +
            "WHERE CheckpointId = @CheckpointId  AND InspectionId = @InspectionId " +
            "ORDER BY CheckpointId ASC",
            new SqlParameter("@InspectionId", inspectionId),
             new SqlParameter("@CheckpointId", checkpointId)
            ).ToList();
        }
        public List<InspectionCheckpoint> GetInspectionCheckpointsPerId (long partId)
        {

            return this.Database.SqlQuery<InspectionCheckpoint>(
            "SELECT c.id as CheckpointId, c.code as CCode, c.specification as Specification " +
            "from [PARTSIQ].[gemini_pa]..[checkpoint] as c " +
            "where c.Part_ID = @PartId AND c.isActive != 0" +
            "ORDER BY CAST(c.code AS VARCHAR) ASC",
            new SqlParameter("@PartId", partId)
            ).ToList();
        }
        public List<InspectionSummary> InspectionSummaryFilter(string PartsCode, string LotNumber, DateTime DateFrom, DateTime DateTo, string DrNumber, int SupplierId)
        {

            string sqlQuery = "SELECT TOP (1000)[InspectionId], [Decision], [Evaluator], [Inspector], [ControlNumber], [DateFinished], " +
                "[PartCode], [PartName], [LotNumber], [LotQuantity], [DrNumber], [Time], [Comments], [InspectorComments], " +
                "[NcrId], [SupplierId], [SupplierName]" +
                "FROM [PartsIQ].[dbo].[InspectionSummaryView] " +
                "WHERE [DateFinished] >= @StartDate AND [DateFinished] <= @EndDate ";
            List<SqlParameter> parameters = new List<SqlParameter> {
                new SqlParameter("@StartDate", DateFrom),
                new SqlParameter("@EndDate", DateTo), 
            };
            if (!string.IsNullOrEmpty(PartsCode))
            {
                sqlQuery += " AND PartCode = @PartsCode";
                parameters.Add(new SqlParameter("@PartsCode", PartsCode));
            }
            if (!string.IsNullOrEmpty(LotNumber))
            {
                sqlQuery += " AND Cast (LotNumber as NVARCHAR(MAX)) = @LotNumber";
                parameters.Add(new SqlParameter("@LotNumber", LotNumber));
            }
            if (!string.IsNullOrEmpty(DrNumber))
            {
                sqlQuery += " AND DrNumber = @DrNumber";
                parameters.Add(new SqlParameter("@DrNumber", DrNumber));
            }
            if (SupplierId > 0)
            {
                sqlQuery += " AND SupplierId = @SupplierId";
                parameters.Add(new SqlParameter("@SupplierId", SupplierId));
            }

            sqlQuery += " ORDER BY [DateFinished] DESC";

            Console.WriteLine(sqlQuery);    

            return this.Database.SqlQuery<InspectionSummary>(sqlQuery, parameters.ToArray()).ToList();
        }

        public List<Part> GetPartsCode()
        {
            return this.Database.SqlQuery<Part>(
                "SELECT [PartId], [PartCode], [DocNumber], [PartModel], " +
                "[PartName], [PartType], [PartFileAttachment] FROM [PartsIQ].[dbo].[PartDetails] ORDER BY CONVERT(nvarchar(MAX), [PartCode])"
            ).ToList();
        }
        public List<InspectionInfo> GetSupplierPerPart(int id)
        {
            try
            {
                return this.Database.SqlQuery<InspectionInfo>(
                    "SELECT TOP (50) [InspectionId], [Decision], [Evaluator], [Inspector], [ControlNumber], [DateFinished], " +
                    "[PartsCode], [PartId], [PartName], [PartModel], [LotNumber], [LotQuantity], [DrNumber], [Time], [Comments], [InspectorComments], " +
                    "[NcrId], [SupplierId], [SupplierName], [CavityId], [CavityNumber] " +
                    "FROM [PartsIQ].[dbo].[PartPerformanceView] " +
                    "WHERE PartId = @PartId",
                    new SqlParameter("@PartId", id)
                ).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine(ex.Message);
                // Optionally rethrow or handle the exception as needed
                return new List<InspectionInfo>();
            }
        }

        public List<InspectionStatistics> GetInspectionStatistics(DateTime? fromDateIs = null, DateTime? toDateIs = null, string inspectorIs = null, string minMaxIs = null, long? partsCodeIs = null)
        {
            try
            {
                Debug.WriteLine(inspectorIs);
                // Initialize the base query
                var sqlQuery =
                    "SELECT InspectionId, Decision, DateDelivered, ControlNumber, DateFinished, LotNumber, LotQuantity, SampleSize, InspectionDuration, EvaluationDuration, InspectionStart, InspectionEnd, Comments, InspectorComments, " +
                    "Decision, DrNumber, NcrId, Evaluator, Inspector, PartName, PartCode, PartId, SupplierId, SupplierName, SupplierInCharge " +
                    "FROM [PartsIQ].[dbo].[InspectionStatisticsView] " +
                    "WHERE (@fromDateIs IS NULL OR InspectionEnd >= @fromDateIs) " +
                    "AND (@toDateIs IS NULL OR InspectionEnd < DATEADD( day, 1, @toDateIs)) ";

                List<SqlParameter> parameters = new List<SqlParameter> {
                new SqlParameter("@fromDateIs", fromDateIs),
                new SqlParameter("@toDateIs", toDateIs)
    };

                if (inspectorIs != null && inspectorIs != "null")

                {
                    Debug.WriteLine($"{inspectorIs} is not supported.");
                    sqlQuery += " AND Inspector = @Inspector";
                    parameters.Add(new SqlParameter("@Inspector", inspectorIs));

            }
                if (partsCodeIs !=null && partsCodeIs >= 0)
                {
                    sqlQuery += " AND PartId = @PartId";
                    parameters.Add(new SqlParameter("@PartId", partsCodeIs));
                }
                sqlQuery += " ORDER BY [DateDelivered] DESC";
                Debug.WriteLine(sqlQuery);
                return this.Database.SqlQuery<InspectionStatistics>(sqlQuery, parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} Error");
                return new List<InspectionStatistics>();
            }
        }

        public List <User> GetUsers()
        {
            try
            {
                var query = @"
                SELECT TOP (1000)
                    [UserId],
                    [UserEmail],
                    [UserName],
                    [UserUsername],
                    [UserPassword],
                    [IsActive],
                    [UserRole]
                FROM [PartsIQ].[dbo].[UserPermissionView] Where IsActive != 0";

                // Execute the query and return the results
                return this.Database.SqlQuery<User>(query).ToList();
            }
            catch (Exception ex){
                Debug.WriteLine(ex.Message);
                return new List<User>();
            }
        }
        [HttpPost]
        public List<Logs> GetLogsList(DateTime? fromDate = null, DateTime? toDate = null, long? userId = null, string logEvent = null)
        {
            try
            {
                var sqlQuery =
            "SELECT TOP(1000) [LogId],[UserId] ,[PartId] ,[Event],[DateLog],[LogComment],[UserName] FROM [PartsIQ].[dbo].[LogsListView] " +
            "WHERE (@fromDate IS NULL OR DateLog >= @fromDate) " +
            "AND (@toDate IS NULL OR DateLog < DATEADD( day, 1, @toDate)) ";
                List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@fromDate", fromDate),
                new SqlParameter("@toDate", toDate)
            };
                if (userId != null && userId > 0)
                {
                    sqlQuery += " AND UserId = @userId";
                    parameters.Add(new SqlParameter("userId", userId));
                }
                if (logEvent != null)
                {
                    sqlQuery += " AND Event = @logEvent";
                    parameters.Add(new SqlParameter("logEvent", logEvent));
                }
                sqlQuery += " ORDER BY [DateLog] DESC";
                Debug.WriteLine(sqlQuery);

                return this.Database.SqlQuery<Logs>(sqlQuery, parameters.ToArray()).ToList();
            }
            catch (Exception ex ) { Debug.WriteLine(ex.Message); return new List<Logs>(); }

                
        }

        public List<NcrDetails> GetNcrList (long? partId = null, long? supplierId = null)
        {
            try
            {
                var query = "SELECT TOP(1000) [NcrId] ,[IsCompleted], [InspectionId] ,[NcrNumber] ,[DateInspected],[ControlNumber],[LotNumber],[InspectionComment], " +
             "[DrNumber] ,[DateEnded],[PartId], [PartCode],[PartName],[PartModel], [SupplierId], [SupplierName],[SupplierInCharge], [Decision] " +
             "FROM[PartsIQ].[dbo].[NcrInfoView] WHERE 1 = 1";

                List<SqlParameter> parameters = new List<SqlParameter>();

                if (partId != null && partId > 0)
                {
                    query += " AND PartId = @PartId ";
                    parameters.Add(new SqlParameter("@PartId", partId));
                }
                if (supplierId != null && supplierId > 0)
                {
                    query += " AND SupplierId = @SupplierId ";
                    parameters.Add(new SqlParameter("@SupplierId", supplierId));
                }
                query += "Order BY DateInspected DESC";
                Debug.WriteLine(query);
                return this.Database.SqlQuery<NcrDetails>(query, parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<NcrDetails>();
            }
        }


        public List<InspectionSummary> GetPartQualityPerformance(DateTime? fromDate = null, DateTime? toDate = null, long? partId = null, long? supplierId = null)
        {
            try
            {
                Debug.WriteLine(supplierId);
                var query = new StringBuilder("SELECT * FROM [PartsIQ].[dbo].[GetPartQualityPerformanceByDateRange](@StartDate, @EndDate) WHERE 1 = 1 ");
                List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("@StartDate", fromDate ?? (object)DBNull.Value),
            new SqlParameter("@EndDate", toDate ?? (object)DBNull.Value)
        };

                if (supplierId != null)
                {
                    query.Append("AND SupplierId = @SupplierId ");
                    parameters.Add(new SqlParameter("@SupplierId", supplierId));
                }
                if (partId != null)
                {
                    query.Append("And PartId = @PartId");
                    parameters.Add(new SqlParameter("@PartId", partId));
                        
                }
                // Add more filters if needed similarly

                Debug.WriteLine(query.ToString());
                return this.Database.SqlQuery<InspectionSummary>(query.ToString(), parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<InspectionSummary>();
            }
        }


    }
}