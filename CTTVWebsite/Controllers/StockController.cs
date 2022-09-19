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
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration configuration;
        public StockController(ApplicationDbContext db, IConfiguration iConfig)
        {
            _db = db;
            configuration = iConfig;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult InstockInven()
        {

            string query = $" SELECT NVL(NVL(a.model_name,b.model_name),c.model_name)  MODEL_NAME, NVL(a.CTTV_WH_SN_Qty,0) CTTV_WH_SN_Qty,NVL(a.CTTV_WH_Pallet_Qty,0) CTTV_WH_Pallet_Qty,NVL(b.CTTVOUT_WH_SN_QTY,0) CTTVOUT_WH_SN_QTY,NVL(b.CTTVOUT_PALLET_QTY,0) CTTVOUT_PALLET_QTY,NVL(c.DEEPC_WH_SN_Qty,0) DEEPC_WH_SN_Qty,NVL(c.DEEPC_WH_Pallet_Qty,0) DEEPC_WH_Pallet_Qty FROM (SELECT MODEL_NAME ,COUNT(DISTINCT SHIPPING_SN) CTTV_WH_SN_Qty ,COUNT(DISTINCT PALLET_NO) CTTV_WH_Pallet_Qty FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME = 'INSTOCK' GROUP BY MODEL_NAME) a full join (SELECT model_name,COUNT(DISTINCT SERIAL_NUMBER) CTTVOUT_WH_SN_Qty,COUNT(DISTINCT PALLET_NO) CTTVOUT_Pallet_Qty FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME = 'CTTVOUT' GROUP BY MODEL_NAME) b on a.model_name = b.model_name full join (SELECT model_name,COUNT(DISTINCT SERIAL_NUMBER) DEEPC_WH_SN_Qty,COUNT(DISTINCT PALLET_NO) DEEPC_WH_Pallet_Qty FROM SFISM4.R_WIP_TRACKING_T WHERE GROUP_NAME = 'DEEPCIN' GROUP BY MODEL_NAME) c on a.model_name = c.model_name or b.model_name=c.model_name;";
            var objResult = _db.stockInvens.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult OBIInven()
        {

            string query = $"SELECT * FROM ( SELECT MODEL_NAME,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_IN') THEN 1 END ) AS OBI_IN, COUNT(CASE WHEN GROUP_NAME IN ( 'LCM_QA_CHKBOX') THEN 1 END ) AS LCM_QA_CHKBOX,COUNT(CASE WHEN GROUP_NAME IN ( 'LCM_QA_CHKSN') THEN 1 END ) AS LCM_QA_CHKSN  , COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_TV_CHK1') THEN 1 END ) AS OBI_TV_CHK1, COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_TV_OUT') THEN 1 END ) AS OBI_TV_OUT,COUNT(CASE WHEN GROUP_NAME IN ( 'AGIN_ASSY') THEN 1 END ) AS AGIN_ASSY,COUNT(CASE WHEN GROUP_NAME IN ( 'AGOUT_ASSY') THEN 1 END ) AS AGOUT_ASSY,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_INT_CHK') THEN 1 END ) AS OBI_INT_CHK,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_OUT') THEN 1 END ) AS OBI_OUT FROM sfism4.r_wip_tracking_t group by model_name)  where OBI_IN != '0' or LCM_QA_CHKBOX != '0' or LCM_QA_CHKSN != '0'or OBI_TV_CHK1 != '0' or OBI_TV_OUT != '0' or OBI_OUT != '0' or AGIN_ASSY != '0' or AGOUT_ASSY != '0' or OBI_INT_CHK != '0' ORDER BY MODEL_NAME;";
            var objResult = _db.OBIs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult SearchInstock()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchInstock(string modelname, string serialNumber, string palletNo, string WHcode, string Bincode,string groupName)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.SN = serialNumber;
            ViewBag.pallet = palletNo;
            ViewBag.WHcode = WHcode;
            ViewBag.Bincode = Bincode;
            ViewBag.groupName = groupName;



            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";

            string queryPrefix = $"SELECT a.SERIAL_NUMBER,  a.MODEL_NAME, a.GROUP_NAME, a.IN_STATION_TIME INSTOCK_TIME, a.PALLET_NO, b.Cangku WH_CODE, NVL(b.POSITION_NO, 'N/A') BIN_CODE, CASE WHEN a.SECTION_FLAG = 1 THEN 'Judged OK' WHEN a.SECTION_FLAG <> 1 THEN 'NOT Judged' END QA_Judge, CASE WHEN(a.ERROR_FLAG< 3 ) THEN 'Unhold' WHEN(a.ERROR_FLAG >= 3) THEN 'Holded' END Hold_status FROM SFISM4.R_WIP_TRACKING_T a left join  SFISM4.R_INOUT_STOCK_T b on a.serial_number = b.serial_number WHERE a.GROUP_NAME in  ('INSTOCK','CTTVOUT','DEEPCIN')";
            string querySuffix = " ORDER BY a.in_station_time desc";
            query = queryPrefix;
            if (serialNumber != null)
            {
                serialNumber = serialNumber.ToUpper();
                query = query + " AND a.SERIAL_NUMBER LIKE '" + '%' + serialNumber + '%' + "' ";
            }
            if (modelname != "ALL")
            {
                query = query + " AND a.MODEL_NAME LIKE '" + '%' + modelname + '%' + "' ";
            }
            if (palletNo != null)
            {
                palletNo = palletNo.ToUpper();
                query = query + " AND a.PALLET_NO LIKE '" + '%' + palletNo + '%' + "' ";
            }
            if (WHcode != null)
            {
                string WHcode_low = WHcode.ToLower();
                WHcode = WHcode.ToUpper();
                query = query + " AND b.CANGKU in " + "('" + WHcode + "','" + WHcode_low + "')";
            }
            if (Bincode != null)
            {
                Bincode = Bincode.ToUpper();
                query = query + " AND B.POSITION_NO LIKE '" + '%' + Bincode + '%' + "' ";
            }
            if (groupName!="ALL")
            {
                query = query + " AND a.GROUP_NAME LIKE '" + '%' + groupName + '%' + "' ";
            }

            query = query + querySuffix;


            var objResult = _db.Instocksearch.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult OBIInOut()
        {
            DateTime fromDate = DateTime.Today.AddDays(-3);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OBIInOut(string modelname, string fromDate, string toDate, string shift)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.shift = shift;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = "";
            string queryShift = "";
            ViewBag.Shift = shift;

            string queryPrefix = $"SELECT MODEL_NAME,COUNT( CASE WHEN GROUP_NAME IN('OBI_IN') THEN 1 END) AS OBI_IN, COUNT( CASE WHEN GROUP_NAME = 'OBI_OUT' THEN 1 END) AS OBI_OUT, SHIFT FROM ( SELECT A.MODEL_NAME, A.SERIAL_NUMBER,A.GROUP_NAME,CASE WHEN (TIME - TRUNC(TIME)) BETWEEN 8 / 24  AND 19.99973/24 THEN 'Day Shift' ELSE 'Night Shift' END SHIFT FROM (SELECT MODEL_NAME, SERIAL_NUMBER, GROUP_NAME, IN_STATION_TIME AS TIME FROM SFISM4.R_SN_DETAIL_T WHERE GROUP_NAME IN ('OBI_IN','OBI_OUT') AND  IN_STATION_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{toDate}', 'dd/mm/yyyy hh24:mi:ss') ";


            query = queryPrefix;
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME ='" + modelname + "' ";
            }

            if (shift == "day")
            {
                queryShift = " WHERE SHIFT = 'Day Shift' ";
            }
            if (shift == "night")
            {
                queryShift = " WHERE  SHIFT = 'Night Shift' ";
            }
            string querySuffix = ") A ) " + queryShift + "  GROUP BY MODEL_NAME, SHIFT";
            query = query + querySuffix;


            var objResult = _db.inOutObi.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult DetailOBI()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailOBI(string modelName, string groupName, string fromDate, string toDate)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            string query = $"SELECT MODEL_NAME, SERIAL_NUMBER, MIN(IN_STATION_TIME) FIRST_DATE_TIME, MAX(IN_STATION_TIME) LAST_DATE_TIME, COUNT(IN_STATION_TIME) PASSED_TIME,EMP_NO FROM  SFISM4.R_SN_DETAIL_T   WHERE MODEL_NAME = '{modelName}'  AND GROUP_NAME = '{groupName}' AND IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ') GROUP BY MODEL_NAME, SERIAL_NUMBER,EMP_NO;";

            var objResult = _db.OBI_QA_detail.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        public IActionResult QAInOut()
        {
            DateTime fromDate = DateTime.Today.AddDays(-3);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QAInOut(string modelname, string linename, string fromDate, string toDate, string shift)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.Linename = linename;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.shift = shift;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
            string queryShift = "";
            ViewBag.Shift = shift;

            string queryPrefix = $"SELECT MODEL_NAME,LINE_NAME,COUNT(CASE WHEN GROUP_NAME = 'QA_INLINE_IN' THEN 1 END ) AS INPUT,COUNT( CASE WHEN GROUP_NAME= 'QA_INLINE_OUT' THEN 1 END) AS OUTPUT, SHIFT FROM ( SELECT A.MODEL_NAME, A.LINE_NAME, A.SERIAL_NUMBER,  A.GROUP_NAME,CASE WHEN (TIME - TRUNC(TIME)) BETWEEN 8 / 24  AND 19.99973/24 THEN 'Day Shift' ELSE 'Night Shift' END SHIFT FROM (SELECT MODEL_NAME,LINE_NAME, SERIAL_NUMBER, GROUP_NAME, IN_STATION_TIME AS TIME FROM SFISM4.R_SN_DETAIL_T WHERE GROUP_NAME IN ('QA_INLINE_IN','QA_INLINE_OUT') AND  IN_STATION_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{toDate}', 'dd/mm/yyyy hh24:mi:ss') ";


            query = queryPrefix;
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME='" + modelname + "' ";
            }

            if (shift == "day")
            {
                queryShift = " WHERE SHIFT = 'Day Shift' ";
            }
            if (shift == "night")
            {
                queryShift = "  WHERE SHIFT = 'Night Shift' ";
            }

            if (linename != "ALL")
            {
                query = query + " AND LINE_NAME = '" + linename + "' ";
            }
            string querySuffix = ") A ) " + queryShift + "  GROUP BY MODEL_NAME, LINE_NAME, SHIFT ORDER BY MODEL_NAME";
            query = query + querySuffix;


            var objResult = _db.inOuts.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult DetailQA()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailQA(string modelName, string groupName, string fromDate, string toDate)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            string query = $"SELECT MODEL_NAME, SERIAL_NUMBER, MIN(IN_STATION_TIME) FIRST_DATE_TIME, MAX(IN_STATION_TIME) LAST_DATE_TIME, COUNT(IN_STATION_TIME) PASSED_TIME,EMP_NO FROM  SFISM4.R_SN_DETAIL_T   WHERE MODEL_NAME = '{modelName}'  AND GROUP_NAME = '{groupName}' AND IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ') GROUP BY MODEL_NAME, SERIAL_NUMBER,EMP_NO;";

            var objResult = _db.OBI_QA_detail.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        public IActionResult OBIStock()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OBIStock(string modelName, string groupName)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = $"SELECT SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,PALLET_NO,IN_STATION_TIME,EMP_NO FROM SFISM4.R_WIP_TRACKING_T";

            if (groupName == "ALL")
            {
                query = query + " WHERE GROUP_NAME IN ('OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_TV_CHK1', 'OBI_TV_OUT', 'AGIN_ASSY','AGOUT_ASSY', 'OBI_INT_CHK', 'OBI_OUT')";

            }
            else
            {
                query = query + " WHERE GROUP_NAME ='" + groupName + "' ";
            }
            if (modelName != "ALL")
            {
                query = query + " AND MODEL_NAME='" + modelName + "' ";
            }
            var objResult = _db.OBI_EMPs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        public IActionResult OPTracking()
        {
            DateTime fromDate = DateTime.Today.AddDays(-3);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OPTracking(string Empno, string fromDate, string toDate)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.EmpNo = Empno;
            string query = "";
            string queryPrefix = $"SELECT * FROM ( SELECT EMP_NO,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_IN') THEN 1 END ) AS OBI_IN, COUNT(CASE WHEN GROUP_NAME IN ( 'LCM_QA_CHKBOX') THEN 1 END ) AS LCM_QA_CHKBOX,COUNT(CASE WHEN GROUP_NAME IN ( 'LCM_QA_CHKSN') THEN 1 END ) AS LCM_QA_CHKSN  , COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_TV_CHK1') THEN 1 END ) AS OBI_TV_CHK1, COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_TV_OUT') THEN 1 END ) AS OBI_TV_OUT,COUNT(CASE WHEN GROUP_NAME IN ( 'AGIN_ASSY') THEN 1 END ) AS AGIN_ASSY,COUNT(CASE WHEN GROUP_NAME IN ( 'AGOUT_ASSY') THEN 1 END ) AS AGOUT_ASSY,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_INT_CHK') THEN 1 END ) AS OBI_INT_CHK,COUNT(CASE WHEN GROUP_NAME IN ( 'OBI_OUT') THEN 1 END ) AS OBI_OUT FROM sfism4.r_sn_detail_t where in_station_time between to_date('{fromDate}','DD/MM/YYYY HH24:MI:SS') and to_date('{toDate}','DD/MM/YYYY HH24:MI:SS') ";
            query = queryPrefix;
            if (Empno != null)
            {
                query = query + " and emp_no = '" + Empno + "' ";
            }

            string querySuffix = "group by EMP_NO)  where OBI_IN != '0' or LCM_QA_CHKBOX != '0' or LCM_QA_CHKSN != '0'or OBI_TV_CHK1 != '0'or OBI_TV_OUT != '0' or AGIN_ASSY != '0' or AGOUT_ASSY != '0' or OBI_INT_CHK != '0' ORDER BY EMP_NO;";
            query = query + querySuffix;
            var objResult = _db.OBI_EMPTs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult OBIDetail()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OBIDetail(string Empno, string fromDate, string toDate)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.EmpNo = Empno;
            string query = "";
            string queryPrefix = $"SELECT Emp_no, Model_Name, Serial_Number, Group_Name, Pallet_No, In_Station_time FROM sfism4.r_sn_detail_t where in_station_time between to_date('{fromDate}','DD/MM/YYYY HH24:MI:SS') and to_date('{toDate}','DD/MM/YYYY HH24:MI:SS')";
            query = queryPrefix;
            if (Empno != null)
            {
                query = query + " AND EMP_NO = '" + Empno + "' ";
            }
            string querySuffix = " AND group_name in ('OBI_IN', 'LCM_QA_CHKBOX', 'LCM_QA_CHKSN', 'OBI_TV_CHK1', 'OBI_TV_OUT', 'AGIN_ASSY','AGOUT_ASSY', 'OBI_INT_CHK', 'OBI_OUT');";
            query = query + querySuffix;
            var objResult = _db.OBI_EMPs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        public IActionResult FacSum()
        {

            string query = $"SELECT model_name, COUNT(SERIAL_NUMBER) AS TOTAL_PLAN,COUNT(CASE WHEN GROUP_NAME IN ('0') THEN 1 END ) AS NOT_PRODUCE,COUNT(CASE WHEN GROUP_NAME NOT IN ('SHIPPING','0') THEN 1 END ) AS IN_FACTORY,COUNT(CASE WHEN GROUP_NAME = 'SHIPPING' THEN 1 END) AS SHIPPING FROM SFISM4.R_WIP_TRACKING_T where model_name not in ('ITTEST','TEST001','TESTREWORK12','A5034TEST','7A5034904A','2Q014CU00-600-G','2Q014CV00-600-G') group by model_name order by model_name;";
            var objResult = _db.FacSums.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }


        public IActionResult BINSum()
        {



            var query = $"SELECT a.Position_No BIN_Code, a.WH_Code Warehouse_Code, a.MODEL_NAME Model_Type, case when (a.POSITION_LENGTH!='0' and a.pallet_length!='0') then FLOOR(a.position_length / a.pallet_length) else 0 end Max_Pallet_Qty, NVL(b.num,'0') NUM FROM SFIS1.C_STOCK_POSITION_T a left join (select position_no,count(distinct(pallet_no)) num from SFISM4.R_INOUT_STOCK_T where in_flag='1' group by position_no) b on a.position_no = b.position_no order by NUM desc";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<Stock> stockList = new List<Stock>();
            using (OracleConnection con = new OracleConnection(connectString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    //PartNoParam = new OracleParameter("OraPartNo", PartNo);
                    //ScanRouteParam = new OracleParameter("OraScanRoute", ScanRoute);
                    //TagFlagParam = new OracleParameter("OraTagFlag", OracleDbType.Decimal);
                    //TagFlagParam.Value = 1;
                    //cmd.Parameters.Add(PartNoParam);
                    //cmd.Parameters.Add(ScanRouteParam);
                    //cmd.Parameters.Add(TagFlagParam);
                    cmd.CommandText = query;
                    var reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    objResult2++;
                    //}
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine("BIN_CODE: {0} | WAREHOUSE_CODE: {1}, | MODEL_TYPE: {2},  | MAX_PALLET_QTY: {3}  | NUM: {4} ", reader.GetString(0), reader.GetString(1), reader.GetString(2)),reader.GetString(3),reader.GetString(4);
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Stock sto = new Stock
                        {
                            BIN_CODE = dr["BIN_CODE"].ToString(),
                            WAREHOUSE_CODE = dr["WAREHOUSE_CODE"].ToString(),
                            MODEL_TYPE = dr["MODEL_TYPE"].ToString(),
                            MAX_PALLET_QTY = dr["MAX_PALLET_QTY"].ToString(),
                            NUM = dr["NUM"].ToString()

                            //FirstName = dr["FirstName"].ToString(),
                        };

                        stockList.Add(sto);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(stockList);
        }
        public IActionResult Bincode()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Bincode(string modelName, string binCode)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.binCode = binCode;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = "";
            string queryPrefix = $"SELECT Pallet_No, Model_Name,CANGKU WH_Code,NVL(POSITION_NO,'N/A') BIN_Code,min(WORK_TIME) WORK_TIME, count(serial_number) Qty_Pallet FROM SFISM4.R_INOUT_STOCK_T WHERE IN_FLAG = '1'";
            query = queryPrefix;
            if (modelName != "ALL")
            {
                query = query + " AND MODEL_NAME = '" + modelName + "' ";
            }
            if (binCode != null)
            {
                query = query + " AND POSITION_NO = '" + binCode + "' ";
            }
            string querySuffix = "  group by Pallet_No, Model_Name,CANGKU,POSITION_NO;";
            query = query + querySuffix;
            var objResult = _db.bincodes.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
    }
}
