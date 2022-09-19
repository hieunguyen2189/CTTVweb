using CTTVWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Oracle.ManagedDataAccess.Client;

namespace CTTVWebsite.Controllers
{
    [Authorize]
    public class SnDetailController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration configuration;
        public SnDetailController(ApplicationDbContext db, IConfiguration iConfig)
        {
            _db = db;
            configuration = iConfig;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchDetailedIn()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            //DateTime fromDate = DateTime.Today.AddDays(-7);
            //DateTime toDate = DateTime.Today;
            //ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            //ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            if (objModelName.Count == 0)
            {
                ViewBag.Error = "0 Record found";
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchDetailedIn(string modelName, string fromDate, string toDate)
        {

            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = "";
            string modelNamex = modelName;
            string fromDatex = fromDate;
            string toDatex = toDate;
            string queryPrefix = "";
            if (modelName == "A5029913A" || modelName == "A5043480A" || modelName == "A5043483A")
            {
                queryPrefix = $" SELECT SERIAL_NUMBER, MODEL_NAME, GROUP_NAME, LINE_NAME, MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME,  MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME, COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'IQC1' and R_SN_DETAIL_T.STATION_NAME LIKE 'IQC1%'  AND R_SN_DETAIL_T.SERIAL_NUMBER IN( SELECT DISTINCT(SERIAL_NUMBER) FROM SFISM4.R_SN_DETAIL_T WHERE MODEL_NAME = '" + modelNamex + "' AND GROUP_NAME LIKE 'IQC1' AND R_SN_DETAIL_T.STATION_NAME LIKE 'IQC1%' AND IN_STATION_TIME BETWEEN TO_DATE( '" + fromDatex + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE( '" + toDatex + "', 'dd/mm/yyyy hh24:mi:ss' ))";
            }
            else
            {
                queryPrefix = $" SELECT SERIAL_NUMBER, MODEL_NAME, GROUP_NAME, LINE_NAME, MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME,  MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME, COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'LCM_LB' and R_SN_DETAIL_T.STATION_NAME LIKE 'LCM_LB%'  AND R_SN_DETAIL_T.SERIAL_NUMBER IN( SELECT DISTINCT(SERIAL_NUMBER) FROM SFISM4.R_SN_DETAIL_T WHERE MODEL_NAME = '" + modelNamex + "' AND GROUP_NAME LIKE 'LCM_LB' AND R_SN_DETAIL_T.STATION_NAME LIKE 'LCM_LB%' AND IN_STATION_TIME BETWEEN TO_DATE( '" + fromDatex + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE( '" + toDatex + "', 'dd/mm/yyyy hh24:mi:ss' ))";
            }  
            
            string querySuffix = " GROUP BY MODEL_NAME,GROUP_NAME,LINE_NAME,SERIAL_NUMBER";

            query = queryPrefix;
          

            query = query + querySuffix;
            //if (modelName== "A5029913A")
            //{
            //    query = $"SELECT  SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,LINE_NAME,MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME, MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME,COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'IQC1' AND IN_STATION_TIME BETWEEN TO_DATE( '{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND  TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss' ) GROUP BY MODEL_NAME,GROUP_NAME,LINE_NAME,SERIAL_NUMBER";
              
            //}
            //else
            //{
            //    query = $"SELECT SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,LINE_NAME,MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME, MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME,COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'LCM_LB' AND IN_STATION_TIME BETWEEN TO_DATE( '{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND  TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss' )  GROUP BY MODEL_NAME,GROUP_NAME,LINE_NAME,SERIAL_NUMBER";
               
            //}
            var objResult = _db.R_SN_DETAIL_T.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "0 Record found";
            }
            ViewBag.modelNameSl = modelName;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            return View(objResult);

        }
        public IActionResult SearchDetailedOut()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchDetailedOut(string modelName, string fromDate, string toDate)
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            //string query = $"SELECT SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,LINE_NAME,MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME, MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME,COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'PACKING4' AND IN_STATION_TIME BETWEEN TO_DATE( '{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND  TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss') GROUP BY MODEL_NAME,GROUP_NAME,LINE_NAME,SERIAL_NUMBER";
            string query = $"SELECT  SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,LINE_NAME, MIN(R_SN_DETAIL_T.IN_STATION_TIME) FIRST_TIME,  MAX(R_SN_DETAIL_T.IN_STATION_TIME) LAST_TIME, COUNT(sfism4.R_SN_DETAIL_T.IN_STATION_TIME) NUM FROM SFISM4.R_SN_DETAIL_T R_SN_DETAIL_T WHERE R_SN_DETAIL_T.MODEL_NAME = '{modelName}' AND R_SN_DETAIL_T.GROUP_NAME LIKE 'PACKING4' and R_SN_DETAIL_T.STATION_NAME LIKE 'PACKING4%' AND IN_STATION_TIME BETWEEN TO_DATE( '{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss' ) AND R_SN_DETAIL_T.SERIAL_NUMBER IN( SELECT DISTINCT(SERIAL_NUMBER) FROM SFISM4.R_SN_DETAIL_T WHERE MODEL_NAME = '{modelName}' AND GROUP_NAME LIKE 'PACKING4' AND STATION_NAME LIKE 'PACKING4%' AND IN_STATION_TIME BETWEEN TO_DATE( '{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss' )) AND MOD(ERROR_FLAG,3) = 0  GROUP BY MODEL_NAME,GROUP_NAME,LINE_NAME,SERIAL_NUMBER; ";
            var objResult = _db.R_SN_DETAIL_T.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "0 Record found";
            }
            ViewBag.modelNameSl = modelName;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            return View(objResult);
        }
        public IActionResult SearchInOutTotal()
        {
            DateTime fromDate = DateTime.Today.AddDays(-3);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchInOutTotal(string modelname, string fromDate, string toDate)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
            string IQC1 = "";
            if (modelname == "A5029913A" || modelname == "ALL" || modelname == "A5043480A" || modelname == "A5043483A")
            {
                IQC1 = ",'IQC1'";
            }

            string queryPrefix = $"SELECT MODEL_NAME, COUNT(CASE WHEN GROUP_NAME IN ('LCM_LB'" + IQC1 + ") THEN 1 END) AS INPUT, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM(SELECT MODEL_NAME, SERIAL_NUMBER, GROUP_NAME, CASE WHEN GROUP_NAME IN ('PACKING4') THEN MAX(IN_STATION_TIME) ELSE MIN(IN_STATION_TIME) END LAST_IN_STATION_TIME FROM SFISM4.R_SN_DETAIL_T  WHERE GROUP_NAME IN ('LCM_LB','IQC1','PACKING4') AND (STATION_NAME LIKE '%LCM_LB%' or STATION_NAME LIKE '%PACKING4%' or STATION_NAME LIKE '%IQC1%') AND MOD(ERROR_FLAG,3) = 0 GROUP BY MODEL_NAME, SERIAL_NUMBER, GROUP_NAME) WHERE LAST_IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ') ";
            string querySuffix = " GROUP BY MODEL_NAME";
            query = queryPrefix;
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME='" + modelname + "' ";
            }

            query = query + querySuffix;


            var objResult = _db.inOutTotals.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        //public IActionResult CountInOut()
        //{
        //    var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Distinct().OrderBy(m => m).ToList();
        //    ViewBag.ModelName = objModelName;
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CountInOut(string modelName, string fromDate, string toDate)
        //{
        //    string queryModel = $"SELECT DISTINCT MODEL_NAME FROM R_WIP_TRACKING_T";
        //    var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Distinct().ToList();
        //    ViewBag.ModelName = objModelName;
        //    string query;
        //    if (modelName == "A5029913A")
        //    {
        //        query = $"SELECT MODEL_NAME,COUNT(CASE WHEN GROUP_NAME = 'IQC1' THEN 1 END) AS INPUT, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM(SELECT MODEL_NAME, SERIAL_NUMBER, GROUP_NAME, MAX(IN_STATION_TIME) LAST_IN_STATION_TIME FROM SFISM4.R_SN_DETAIL_T GROUP BY MODEL_NAME, SERIAL_NUMBER, GROUP_NAME) WHERE LAST_IN_STATION_TIME BETWEEN(TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss')) AND(TO_DATE('{toDate}', 'dd/mm/yyyy hh24:mi:ss')) and model_name = '{modelName}' GROUP BY MODEL_NAME";
        //    }
        //    else
        //    {
        //        query = $"SELECT MODEL_NAME,COUNT(CASE WHEN GROUP_NAME = 'LCM_LB' THEN 1 END) AS INPUT, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM(SELECT MODEL_NAME, SERIAL_NUMBER, GROUP_NAME, MAX(IN_STATION_TIME) LAST_IN_STATION_TIME FROM SFISM4.R_SN_DETAIL_T GROUP BY MODEL_NAME, SERIAL_NUMBER, GROUP_NAME) WHERE LAST_IN_STATION_TIME BETWEEN(TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss')) AND(TO_DATE('{toDate}', 'dd/mm/yyyy hh24:mi:ss')) and model_name = '{modelName}' GROUP BY MODEL_NAME";
        //    }
        //    var objResult = _db.inOuts.FromSqlRaw(query).ToList();
        //    if (objResult.Count == 0)
        //    {
        //        ViewBag.Error = "No Record found";
        //    }

        //    return View(objResult);
        //}
        public IActionResult SearchModelNotShip()
        { 
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null).Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchModelNotShip(string modelName,string PK4,string groupName,string LB, string HoldSTT, string QAjudge)
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null).Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            ViewBag.QAjudge = QAjudge;
            ViewBag.HoldSTT = HoldSTT;
            ViewBag.packing = PK4;
            ViewBag.LB = LB;
            string query = "";
            string queryPrefix = $"SELECT  a.serial_number,a.line_name,a.group_name Group_name_now,a.model_name,a.MO_NUMBER, case when B.IN_STATION_TIME is not null then TO_CHAR(B.IN_STATION_TIME, 'MM/DD/YYYY hh24:mi:ss') else 'N/A' end Last_Packing4_time, NVL(TO_CHAR(A.IN_STATION_TIME, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') Last_In_station_time, a.pallet_no, CASE WHEN a.SECTION_FLAG = 1 THEN 'Judged OK' ELSE 'NOT Judged' END QA_Judge, CASE WHEN(a.ERROR_FLAG < 3) THEN  'Unhold'  ELSE 'Holded' END Hold_status  FROM sfism4.r_wip_tracking_t a left join (select serial_number,max(in_station_time) in_station_time from sfism4.r_sn_detail_t where group_name = 'PACKING4' and group_name != 'SHIPPING'  group by serial_number) b on a.serial_number = b.serial_number where a.group_name != 'SHIPPING' ";
            string querySuffix = "group by a.serial_number,a.line_name,a.model_name, a.group_name, a.pallet_no, a.section_flag, b.in_station_time, a.error_flag, a.in_station_time,a.MO_NUMBER";
            query = queryPrefix;
            if (PK4 == "YES")
            {
                query = query + " and B.IN_STATION_TIME is not null ";
            }
            if (modelName != "ALL")
            {
                query = query + " AND  a.model_name ='" + modelName + "' ";
            }
            if (PK4 == "NO")
            {
                query = query + " and B.IN_STATION_TIME is null ";
            }
            if(groupName != "ALL")
            {
                query = query + " AND a.GROUP_NAME ='" + groupName + "' ";
            }

            if (QAjudge == "JugdeOk")
            {
                query = query + " AND a.section_flag = 1 ";
            }
            if (QAjudge == "JudgeFail")
            {
                query = query + " AND a.section_flag = 0 ";
            }
            if (HoldSTT == "Hold")
            {
                query = query + " AND a.error_flag >= 3 ";
            }
            if (HoldSTT == "notHold")
            {
                query = query + " AND a.error_flag < 3 ";
            }
            if (LB == "YES")
            {
                query = query + " and a.group_name != '0' ";
            }
            if (LB == "NO")
            {
                query = query + " and a.group_name = '0' ";
            }
            query = query + querySuffix;
            var objResult = _db.Modelnotship.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "0 Record found";
            }
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            return View(objResult);
        }
        public IActionResult InOutProduction()
        {
            DateTime fromDate = DateTime.Today.AddDays(-3);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InOutProduction(string modelname, string fromDate, string toDate)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
            string IQC1 = "";
            if (modelname == "A5029913A" || modelname == "ALL" || modelname == "A5043480A" || modelname == "A5043483A")
            {
                IQC1 = ",'IQC1'";
            }

          //  string queryPrefix = $"SELECT MODEL_NAME,LINE_NAME, COUNT(CASE WHEN GROUP_NAME IN ('LCM_LB'" + IQC1 + ") THEN 1 END) AS INPUT, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM(SELECT MODEL_NAME,LINE_NAME, SERIAL_NUMBER, GROUP_NAME, MIN(IN_STATION_TIME) LAST_IN_STATION_TIME FROM SFISM4.R_SN_DETAIL_T  WHERE GROUP_NAME IN ('LCM_LB','IQC1','PACKING4') AND (STATION_NAME LIKE '%LCM_LB%' or STATION_NAME LIKE '%PACKING4%' or STATION_NAME LIKE '%IQC1%') AND MOD(ERROR_FLAG,3) = 0 GROUP BY MODEL_NAME,LINE_NAME, SERIAL_NUMBER, GROUP_NAME) WHERE LAST_IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ') AND LINE_NAME != 'TV_A07'";
            string queryPrefix = $"SELECT MODEL_NAME, COUNT(CASE WHEN GROUP_NAME IN ('LCM_LB'" + IQC1 + ") THEN 1 END) AS INPUT, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM(SELECT MODEL_NAME, SERIAL_NUMBER, GROUP_NAME, MIN(IN_STATION_TIME) LAST_IN_STATION_TIME FROM SFISM4.R_SN_DETAIL_T  WHERE GROUP_NAME IN ('LCM_LB','IQC1','PACKING4') AND (STATION_NAME LIKE '%LCM_LB%' or STATION_NAME LIKE '%PACKING4%' or STATION_NAME LIKE '%IQC1%') AND MOD(ERROR_FLAG,3) = 0 GROUP BY MODEL_NAME, SERIAL_NUMBER, GROUP_NAME) WHERE LAST_IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ') ";
            //string querySuffix = " GROUP BY MODEL_NAME,LINE_NAME";
            string querySuffix = " GROUP BY MODEL_NAME";
            query = queryPrefix;
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME='" + modelname + "' ";
            }
            //if(lineName!="ALL")
            //{
            //    query = query + " AND LINE_NAME='" + lineName + "' ";
            //}
            query = query + querySuffix;


            var objResult = _db.inoutPros.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult FactoryOutput()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST" && m != "TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FactoryOutput(string modelname, string serialNumber, string palletNo, string groupName)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.SN = serialNumber;
            ViewBag.pallet = palletNo;
            ViewBag.groupName = groupName;



            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST" && m != "TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";

            string queryPrefix = $"SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, GROUP_NAME, IN_STATION_TIME INSTOCK_TIME, PALLET_NO, CASE WHEN SECTION_FLAG = 1 THEN 'Judged OK' WHEN SECTION_FLAG <> 1 THEN 'NOT Judged' END QA_Judge, CASE WHEN (ERROR_FLAG < 3) THEN 'Unhold' WHEN (ERROR_FLAG >= 3) THEN 'Holded' END Hold_status FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('INSTOCK', 'CTTVOUT', 'DEEPCIN', 'PACKING4', 'OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_OUT', 'OBI_TV_CHK1', 'AGIN_ASSY', 'AGOUT_ASSY', 'OBI_CIT_CHK', 'OBI_INT_CHK', 'OBI_TV_OUT', 'SHIPPING_LIST', 'OUTSTOCK') and (serial_number not like '%TEST%' or model_name not in ('TEST') or mo_number not in ('ITS001')) ";
            string querySuffix = " ORDER BY in_station_time desc";
            query = queryPrefix;
            if (serialNumber != null)
            {
                serialNumber = serialNumber.ToUpper();
                query = query + " AND SERIAL_NUMBER LIKE '" + '%' + serialNumber + '%' + "' ";
            }
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME LIKE '" + '%' + modelname + '%' + "' ";
            }
            if (palletNo != null)
            {
                palletNo = palletNo.ToUpper();
                query = query + " AND PALLET_NO LIKE '" + '%' + palletNo + '%' + "' ";
            }
            if (groupName != "ALL")
            {
                query = query + " AND GROUP_NAME LIKE '" + '%' + groupName + '%' + "' ";
            }

            query = query + querySuffix;


            var objResult = _db.ProOutputs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult OutSummary()
        {
            var query = $"SELECT NVL(NVL(a.model_name, d.model_name), g.model_name) MODEL_NAME, NVL(b.OK, '0') CTTV_OK, NVL(c.N_OK, '0') CTTV_NOK, NVL(a.TOTAL, '0') CTTV_TOTAL, NVL(e.DEEPC_OK, '0') DEEPC_OK, NVL(f.DEEPC_NOK, '0') DEEPC_NOK, NVL(d.DEEPC_Total, '0') DEEPC_Total, NVL(h.CTTVOUT_OK, '0') CTTVOUT_OK, NVL(i.CTTVOUT_NOK, '0') CTTVOUT_NOK, NVL(g.CTTVOUT_TOTAL, '0') CTTVOUT_TOTAL FROM (SELECT MODEL_NAME, Count(SERIAL_NUMBER) Total FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('INSTOCK', 'PACKING4', 'OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_OUT', 'OBI_TV_CHK1', 'AGIN_ASSY', 'AGOUT_ASSY', 'OBI_CIT_CHK', 'OBI_INT_CHK', 'OBI_TV_OUT', 'SHIPPING_LIST', 'OUTSTOCK') and model_name not like '%TEST%' group by model_name) a left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) OK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('INSTOCK', 'PACKING4', 'OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_OUT', 'OBI_TV_CHK1', 'AGIN_ASSY', 'AGOUT_ASSY', 'OBI_CIT_CHK', 'OBI_INT_CHK', 'OBI_TV_OUT', 'SHIPPING_LIST', 'OUTSTOCK') and SECTION_FLAG = '1' and error_flag = '0' group by model_name order by model_name) b on a.model_name = b.model_name left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) N_OK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('INSTOCK', 'PACKING4', 'OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_OUT', 'OBI_TV_CHK1', 'AGIN_ASSY', 'AGOUT_ASSY', 'OBI_CIT_CHK', 'OBI_INT_CHK', 'OBI_TV_OUT', 'SHIPPING_LIST', 'OUTSTOCK') and (SECTION_FLAG != '1' or error_flag != '0') group by model_name) c on a.model_name = c.model_name full join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) DEEPC_Total FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('DEEPCIN') group by model_name) d on a.model_name = d.model_name left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) DEEPC_OK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('DEEPCIN') and SECTION_FLAG = '1' and error_flag = '0' group by model_name) e on d.model_name = e.model_name left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) DEEPC_NOK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('DEEPCIN') and (SECTION_FLAG != '1' or error_flag != '0') group by model_name) f on d.model_name = f.model_name full join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) CTTVOUT_TOTAL FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('CTTVOUT') group by model_name) g on a.model_name = g.model_name left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) CTTVOUT_OK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('CTTVOUT') and SECTION_FLAG = '1' AND error_flag = '0' group by model_name) h on g.model_name = h.model_name left join (SELECT MODEL_NAME, Count(SERIAL_NUMBER) CTTVOUT_NOK FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME in ('CTTVOUT') and (SECTION_FLAG != '1' or error_flag != '0') group by model_name) i on g.model_name = i.model_name order by a.model_name";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<OutSum> outList = new List<OutSum>();
            using (OracleConnection con = new OracleConnection(connectString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = query;
                    var reader = cmd.ExecuteReader();
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        OutSum out1 = new OutSum
                        {
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            CTTV_OK = dr["CTTV_OK"].ToString(),
                            CTTV_NOK = dr["CTTV_NOK"].ToString(),
                            CTTV_TOTAL = dr["CTTV_TOTAL"].ToString(),
                            DEEPC_OK = dr["DEEPC_OK"].ToString(),
                            DEEPC_NOK = dr["DEEPC_NOK"].ToString(),
                            DEEPC_TOTAL = dr["DEEPC_TOTAL"].ToString(),
                            CTTVOUT_OK = dr["CTTVOUT_OK"].ToString(),
                            CTTVOUT_NOK = dr["CTTVOUT_NOK"].ToString(),
                            CTTVOUT_TOTAL = dr["CTTVOUT_TOTAL"].ToString()

                        };

                        outList.Add(out1);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(outList);
        }
    }

}
