
using PartsMysql.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Mysqlx.Crud.Order.Types;
using static System.Collections.Specialized.BitVector32;

namespace LSMTS.Models
{
    public class ModelExtension
    {

    }


    public partial class Permission
    {

        public bool hasPlanning { get; set; }
        public bool hasProduction { get; set; }
        public bool hasAdministration { get; set; }
        public bool hasDashboard { get; set; }
        public bool hasHumanResource { get; set; }
        public bool hasQualityAssurance { get; set; }


    }
    public class MainModule
    {
        public const string Administration = "Administration";
        public const string HumanResource = "Human Resource";
        public const string Production = "Production";
        public const string Planning = "Planning";
        public const string QualityAssurance = "Quality Assurance";
        public const string Dashboard = "Dashboard";

    }

    public class SubModule
    {
        public const string UserManagement = "User Management";
        public const string AttendanceMonitor = "Attendance Monitor";
        public const string ProductionMonitor = "Production Monitor";
        public const string JobScheduling = "Job Scheduling";
        public const string SuppliesMonitor = "Supplies Monitor";
        public const string Main = "Main";
        public const string Supplies = "Supplies";
        public const string PerformanceReview = "Performance Review";
        public const string Planning = "Planning Dashboard";
        public const string Production = "Production Dashboard";
        public const string PlasticInjection = "Plastic Injection";
        public const string Calibration = "Calibration";
    }



    public static class ActionCRUD
    {
        public const string C = "Create";
        public const string R = "Read";
        public const string U = "Update";
        public const string D = "Delete";

    }


    public class ResponseDTO
    {
        public bool Success { get; set; }
        public int EmployeeNumber { get; set; }
        public string Message { get; set; }
        public int? RecordsFound { get; set; }
        public string PartialViewContent { get; set; }

    }
    public class PermissionData
    {
        public string SubModuleName { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

   


}