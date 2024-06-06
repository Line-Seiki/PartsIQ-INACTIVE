using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class InspectionCheckpoint
    {
        public long? InspectionItemId { get; set; }
        public long? InspectionId { get; set; }
        public long? CheckpointId { get; set; }
        public int SampleNumber { get; set; }
        public string Specification { get; set; }
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
        public bool IsGood { get; set; }
        public double Measurement { get; set; }
        public double OrigMeasurement { get; set; }
        public string CavityNumber { get; set; }
        public string CCode { get; set; }
        public string MeasurementAttr { get; set; }  
        public long? PartId { get; set; }

    }
    public class MeasurementStats
    {
        public string CCode { get; set; }
        public double MinOrigMeasurement { get; set; }
        public double MaxOrigMeasurement { get; set; }
        public string Specification { get; set; }
        public double? LowerLimit { get; set; }
        public double? UpperLimit { get; set; }
        public string MeasurementAttr { get; set; }
    }

    public class NgRejects
    {
        public double QuantitySamples { get; set; }
        public double NgRejectSamples { get; set; }

        public double RejectPercentage { get; set; }
    }

    public class InspectionProcessor
    {
        public List<MeasurementStats> GetMinMaxOrigMeasurementPerCCode(List<InspectionCheckpoint> inspectionDataList)
        {
            var result = inspectionDataList
                .GroupBy(i => i.CCode)
                .Select(g => new MeasurementStats
                {
                    CCode = g.Key,
                    MinOrigMeasurement = g.Min(i => i.Measurement),
                    MaxOrigMeasurement = g.Max(i => i.Measurement),
                    Specification = g.FirstOrDefault()?.Specification,
                    MeasurementAttr = g.FirstOrDefault()?.MeasurementAttr,
                    UpperLimit = g.FirstOrDefault()?.UpperLimit,
                    LowerLimit = g.FirstOrDefault()?.LowerLimit
                })
                .ToList();

            return result;
        }

        public NgRejects GetMaxNgPercentage(List<InspectionCheckpoint> inspectionDataList)
        {
            var ngRejectsList = new List<NgRejects>();

            // Group the inspection data by InspectionId
            var groupedInspectionData = inspectionDataList.GroupBy(i => i.CCode);

            foreach (var inspectionGroup in groupedInspectionData)
            {
                double quantitySamples = inspectionGroup.Count();
                double ngRejectSamples = inspectionGroup.Count(i => !i.IsGood);
                double rejectPercentage = (ngRejectSamples / quantitySamples) * 100;

                ngRejectsList.Add(new NgRejects
                {
                    QuantitySamples = quantitySamples,
                    NgRejectSamples = ngRejectSamples,
                    RejectPercentage = rejectPercentage
                });
            }

            // Find the entry with the highest reject percentage
            var maxNgRejects = ngRejectsList.OrderByDescending(r => r.RejectPercentage).FirstOrDefault();

            return maxNgRejects;
        }
    }

  

}