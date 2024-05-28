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
                "WHERE [DateFinished] >= @StartDate AND [DateFinished] <= @EndDate ORDER BY [DateFinished] DESC",
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
                sqlQuery += " AND LotNumber = @LotNumber";
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

    }
}