using CTTVWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Globalization;

namespace CTTVWebsite.Controllers
{
    [Authorize]
    public class WiptrackingController : Controller
    {
        private readonly ApplicationDbContext _db;
       

        public WiptrackingController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //var objWip = _db.R_WIP_TRACKING_T.FromSqlRaw("SELECT * FROM R_WIP_TRACKING_T WHERE MODEL_NAME='A5025670A'").ToList(); //do'

            var objWip = _db.R_WIP_TRACKING_T.Where(x => x.MODEL_NAME == "A5025670A")
                .Select(wip => new Wiptracking { SERIAL_NUMBER = wip.SERIAL_NUMBER, MODEL_NAME = wip.MODEL_NAME, GROUP_NAME = wip.GROUP_NAME }).Take(100).ToList();
            if (objWip.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objWip);

        }
        public IActionResult QA()
        {

            var objWip = _db.R_WIP_TRACKING_T.Where(x => x.GROUP_NAME != "SHIPPING" && x.GROUP_NAME != "0").OrderBy(x => x.MODEL_NAME)
                .Select(wip => new Wiptracking { SERIAL_NUMBER = wip.SERIAL_NUMBER, MODEL_NAME = wip.MODEL_NAME, GROUP_NAME = wip.GROUP_NAME, PALLET_NO = wip.PALLET_NO, IN_STATION_TIME = wip.IN_STATION_TIME }).ToList();
            if (objWip.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objWip);

        }

        public IActionResult SearchBySN()
        {

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchBySN(string serialNumber,string ConNo,string PO, string Next, string MO,string OBI,string OBIDetail)
        {

            ViewBag.serialNumber = serialNumber;
            ViewBag.ConNo = ConNo;
            ViewBag.PO = PO;
            ViewBag.Next = Next;
            ViewBag.MO = MO;
            ViewBag.OBI = OBI;
            ViewBag.OBIDetail = OBIDetail;
            serialNumber = serialNumber.Replace(System.Environment.NewLine, "','");
            serialNumber = serialNumber.ToUpper();
            serialNumber = "('" + serialNumber + "')";
            serialNumber = serialNumber.Replace(" ", "");
            string query = "";
            string queryPre = $"SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,LINE_NAME,GROUP_NAME,PO_NO,IN_STATION_TIME,PALLET_NO,CONTAINER_NO,GROUP_NEXT,OBI_RECORD,OBI_IN_TIME,CHECK_BOX,OBI_BOX_TIME,CHECK_SN,OBI_SN_TIME FROM (SELECT a.SERIAL_NUMBER, a.MO_NUMBER, a.MODEL_NAME,a.LINE_NAME, Case when a.group_name = 'SYS_REPAIR' and a.next_station = 'N/A' then 'REPAIR_IN' when a.group_name = 'SYS_REPAIR' and a.next_station != 'N/A' then 'REPAIR_OUT' ELSE a.group_name end GROUP_NAME, Case when a.group_name = 'SHIPPING' then a.PO_NO else 'N/A' END PO_NO, a.IN_STATION_TIME, NVL(a.PALLET_NO, 'N/A') PALLET_NO, NVL(a.CONTAINER_NO, 'N/A') CONTAINER_NO";
            query = queryPre;
            if(Next== "Yes")
            {
                query = query + ", case when a.next_station != 'N/A' then a.next_station when a.next_station = 'N/A' and b.group_name is not null and a.error_flag !='1' then b.group_next when a.error_flag = '1' then 'SYS_REPAIR' else 'N/A' end group_next, case when c.OBI_RECORD = 'O1' then 'OBI_YES' else 'OBI_NO' end OBI_RECORD, NVL(TO_CHAR(c.OBI_TIME, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') AS OBI_IN_TIME, NVL(d.group_name, 'N/A') CHECK_BOX, NVL(TO_CHAR(d.in_station_time, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') OBI_BOX_TIME, NVL(e.group_name, 'N/A') CHECK_SN, NVL(TO_CHAR(e.in_station_time, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') OBI_SN_TIME FROM (SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME,LINE_NAME, GROUP_NAME, po_no, IN_STATION_TIME, PALLET_NO, CONTAINER_NO, SPECIAL_ROUTE, next_station,ERROR_FLAG FROM SFISM4.R_WIP_TRACKING_T where serial_number in " + serialNumber + ") a left join (SELECT serial_number, case when group_name = 'OBI_IN' then 'O1' else 'O' end OBI_REcord, max(in_station_time) AS OBI_TIME FROM sfism4.r_sn_detail_t where group_name in ('OBI_IN') group by serial_number,group_name) c on a.serial_number = c.serial_number left join (select serial_number, group_name, in_station_time from sfism4.r_sn_detail_t where group_name in ('LCM_QA_CHKBOX')) d on a.serial_number = d.serial_number left join (select serial_number, group_name, in_station_time from sfism4.r_sn_detail_t where group_name in ('LCM_QA_CHKSN')) e on a.serial_number = e.serial_number left join sfis1.c_route_control_t b on a.special_route = b.route_code and a.group_name = b.group_name and b.group_next not in ('SYS_REPAIR', 'OBI_IN', 'AGIN_ASSY', 'REWORK', 'QA_INLINE_IN', 'OUTSTOCK')) group by SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,LINE_NAME,GROUP_NAME,PO_NO,IN_STATION_TIME,PALLET_NO,CONTAINER_NO,GROUP_NEXT,OBI_RECORD,OBI_IN_TIME,CHECK_BOX,OBI_BOX_TIME,CHECK_SN,OBI_SN_TIME;";
            }
            else
            {
                query = query + ", a.next_station group_next, case when c.OBI_RECORD = 'O1' then 'OBI_YES' else 'OBI_NO' end OBI_RECORD, NVL(TO_CHAR(c.OBI_TIME, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') AS OBI_IN_TIME, NVL(d.group_name, 'N/A') CHECK_BOX, NVL(TO_CHAR(d.in_station_time, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') OBI_BOX_TIME, NVL(e.group_name, 'N/A') CHECK_SN, NVL(TO_CHAR(e.in_station_time, 'MM/DD/YYYY hh24:mi:ss'), 'N/A') OBI_SN_TIME FROM (SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME,LINE_NAME, GROUP_NAME, po_no, IN_STATION_TIME, PALLET_NO, CONTAINER_NO, SPECIAL_ROUTE, next_station FROM SFISM4.R_WIP_TRACKING_T where serial_number in " + serialNumber + ") a left join (SELECT serial_number, case when group_name = 'OBI_IN' then 'O1' else 'O' end OBI_REcord, max(in_station_time) AS OBI_TIME FROM sfism4.r_sn_detail_t where group_name in ('OBI_IN') group by serial_number,group_name) c on a.serial_number = c.serial_number left join (select serial_number, group_name, in_station_time from sfism4.r_sn_detail_t where group_name in ('LCM_QA_CHKBOX')) d on a.serial_number = d.serial_number left join (select serial_number, group_name, in_station_time from sfism4.r_sn_detail_t where group_name in ('LCM_QA_CHKSN')) e on a.serial_number = e.serial_number left join sfis1.c_route_control_t b on a.special_route = b.route_code and a.group_name = b.group_name and b.group_next not in ('SYS_REPAIR', 'OBI_IN', 'AGIN_ASSY', 'REWORK', 'QA_INLINE_IN', 'OUTSTOCK')) group by SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,LINE_NAME,GROUP_NAME,PO_NO,IN_STATION_TIME,PALLET_NO,CONTAINER_NO,GROUP_NEXT,OBI_RECORD,OBI_IN_TIME,CHECK_BOX,OBI_BOX_TIME,CHECK_SN,OBI_SN_TIME;";
            }
            var objResult = _db.R_WIP_TRACKING_T.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            
            return View(objResult);

        }
        public IActionResult SearchByPallet()
        {

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchByPallet(string PalletNo)
        {
            ViewBag.PalletNo = PalletNo;
            PalletNo = PalletNo.Replace(System.Environment.NewLine, "','");
            PalletNo = PalletNo.ToUpper();
            PalletNo = "('" + PalletNo + "')";
            PalletNo = PalletNo.Replace(" ", "");
            string query = $"SELECT SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,PALLET_NO,IN_STATION_TIME,LINE_NAME FROM R_WIP_TRACKING_T WHERE PALLET_NO in {PalletNo}";
            var objResult = _db.palletTrackngs.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }

        public IActionResult SearchInOut()
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
        public IActionResult SearchInOut(string modelname, string linename, string fromDate, string toDate, string shift)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.Linename = linename;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.shift = shift;
            string datex= fromDate.Substring(0,10);
            datex = datex.Replace("/", "");
            datex = new string(datex.ToCharArray().Reverse().ToArray());
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
            string queryShift = "";
            ViewBag.Shift = shift;

            string queryPrefix = $"SELECT MODEL_NAME,LINE_NAME,COUNT(CASE WHEN GROUP_NAME IN ('LCM_LB', 'IQC1') THEN 1 END ) AS INPUT,COUNT( CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT, SHIFT FROM ( SELECT A.MODEL_NAME, A.LINE_NAME, A.SERIAL_NUMBER,  A.GROUP_NAME,CASE WHEN (TIME - TRUNC(TIME)) BETWEEN 8 / 24  AND 20/24 THEN 'Day Shift' ELSE 'Night Shift' END SHIFT FROM (SELECT MODEL_NAME,LINE_NAME, SERIAL_NUMBER, GROUP_NAME, CASE WHEN GROUP_NAME IN ('LCM_LB', 'IQC1') THEN MIN(IN_STATION_TIME) ELSE MAX(IN_STATION_TIME) END TIME FROM SFISM4.R_SN_DETAIL_T WHERE GROUP_NAME IN ('LCM_LB', 'IQC1', 'PACKING4')AND (STATION_NAME LIKE '%LCM_LB%' OR STATION_NAME LIKE '%PACKING4%' OR STATION_NAME LIKE '%IQC1%' ) AND  IN_STATION_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{toDate}', 'dd/mm/yyyy hh24:mi:ss') AND MOD(ERROR_FLAG,3) = 0  ";


            query = queryPrefix;
            if (modelname != "ALL")
            {
                query = query + " AND MODEL_NAME='" + modelname + "' ";
            }

            if (shift == "day")
            {
                queryShift = " AND SHIFT = 'Day Shift' ";
            }
            if (shift == "night")
            {
                queryShift = " AND SHIFT = 'Night Shift' ";
            }

            if (linename != "ALL")
            {
                query = query + " AND LINE_NAME = '"+ linename + "' ";
            }
            else
            {
                query = query + " AND LINE_NAME NOT IN ('OBI','OBI_A01','AG01','TEST','REWORK','DONGLE') ";
            }
            string querySuffix = "GROUP BY MODEL_NAME, LINE_NAME, SERIAL_NUMBER, GROUP_NAME) A ) WHERE LINE_NAME NOT IN('OBI', 'OBI_A01', 'AG01', 'TEST','REWORK')  " + queryShift + "  GROUP BY MODEL_NAME, LINE_NAME, SHIFT";
            query = query + querySuffix;

            
            var objResult = _db.inOuts.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
        public IActionResult SearchHistorySN()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0" ).Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            DateTime fromDate = DateTime.Today.AddDays(-7);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchHistorySN(string modelName,string groupName,string fromDate, string toDate)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            string query = "";
            //AND A.GROUP_NAME NOT IN('SHIPPING')
            string queryPrefix = $"SELECT b.SERIAL_NUMBER,B.MODEL_NAME,B.GROUP_NAME OLD_GROUP_NAME,COUNT(B.IN_STATION_TIME) as  NUM_PASS,B.IN_STATION_TIME,A.GROUP_NAME NOW_GROUP_NAME,A.PALLET_NO,A.IN_STATION_TIME LAST_IN_STATION_TIME FROM sfism4.R_Wip_Tracking_T a, sfism4.R_SN_DETAIL_T b where A.SERIAL_NUMBER=B.SERIAL_NUMBER AND B.GROUP_NAME='{groupName}' AND B.IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ')";
            query = queryPrefix;
            if (modelName != "ALL")
            {
                query = query + " AND a.MODEL_NAME='" + modelName + "' ";
            }
            string querySuffix = " GROUP BY B.IN_STATION_TIME,b.SERIAL_NUMBER,B.MODEL_NAME,B.GROUP_NAME,A.GROUP_NAME,A.PALLET_NO,A.IN_STATION_TIME";
            query = query + querySuffix;
            var objResult = _db.History_serial.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        //public IActionResult QAHOLD()
        //{
        //    var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
        //    var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0" && m!= "SHIPPING").Distinct().OrderBy(m => m).ToList();
        //    ViewBag.ModelName = objModelName;
        //    ViewBag.GroupName = objGroupName;
        //    return View();

        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult QAHOLD(string modelName, string QAjudge,string HoldSTT,string groupName)
        //{
        //    ViewBag.ModelNameSl = modelName;
        //    ViewBag.GroupNameSl = groupName;

        //    ViewBag.QAjudge = QAjudge;
        //    ViewBag.HoldSTT = HoldSTT;
        //    ViewBag.HoldSTT = HoldSTT;

        //    string query = "";
        //    string queryPrefix = $"SELECT serial_number,model_name,group_name,pallet_no,CASE WHEN R_WIP_TRACKING_T.SECTION_FLAG = 1 THEN 'Judged OK' WHEN R_WIP_TRACKING_T.SECTION_FLAG <> 1 THEN 'NOT Judged' END QA_Judge, CASE WHEN( R_WIP_TRACKING_T.ERROR_FLAG < 3 ) THEN 'Unhold' WHEN( R_WIP_TRACKING_T.ERROR_FLAG >= 3 ) THEN 'Holded' END Hold_status FROM sfism4.r_wip_tracking_t where model_name = '{modelName}' and group_name not in ('SHIPPING','0')";

        //    query = queryPrefix;

        //    if (QAjudge == "JugdeOk")
        //    {
        //        query = query + " AND section_flag = 1 ";
        //    }
        //    if (QAjudge == "JudgeFail")
        //    {
        //        query = query + " AND section_flag = 0 ";
        //    }
        //    if (HoldSTT == "Hold")
        //    {
        //        query = query + " AND error_flag >= 3 ";
        //    }
        //    if (HoldSTT == "notHold")
        //    {
        //        query = query + " AND error_flag < 3 ";
        //    }
        //    if (groupName != "ALL")
        //    {
        //        query = query + " AND GROUP_NAME = '" + groupName + "' ";
        //    }



        //    var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m=>m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
        //    var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0" && m != "SHIPPING").Distinct().OrderBy(m => m).ToList();
        //    ViewBag.ModelName = objModelName;
        //    ViewBag.GroupName = objGroupName;
        //    if (query == null)
        //    {
        //        ViewBag.Error = "No Record found";
        //        return View();
                
        //    }
        //    var objResult = _db.QA_HOLD.FromSqlRaw(query).ToList();
        //    if (objResult.Count == 0)
        //    {
        //        ViewBag.Error = "No Record found";
        //    }

        //    return View(objResult);
        //}
        public IActionResult SearchByPoNo()
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
        public IActionResult SearchByPoNo(string DNNumber,string modelName,string fromDate,string toDate)
        {

            ViewBag.ModelNameSl = modelName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
          
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";

            string queryPrefix = $"SELECT a.Serial_Number, a.Model_Name BOM_NO, NVL(b.part_desc,'N/A') Model_Name, a.Group_Name, a.In_Station_Time, a.Pallet_No, a.PO_NO, a.Container_No, a.key_part_no FROM SFISM4.R_WIP_TRACKING_T a, sfis1.c_model_desc_t b WHERE a.GROUP_NAME = 'SHIPPING' and a.model_name=b.model_name AND a.IN_STATION_TIME BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ')";
            
            query = queryPrefix;
            if (modelName != "ALL")
            {
                query = query + " AND a.MODEL_NAME='" + modelName + "' ";
            }

            if (DNNumber != null)
            {
                DNNumber = DNNumber.ToUpper();
                query = query + " AND a.PO_NO = '" + DNNumber + "' ";
            }
            string querySuffix = " ORDER BY a.CONTAINER_NO";
            query = query + querySuffix;
            var objResult = _db.Pono.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            ViewBag.DNNumber = DNNumber;
            return View(objResult);

        }
        public IActionResult SearchGroupName()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchGroupName(string modelName, string groupName)
        {
            ViewBag.ModelNameSl = modelName;
            ViewBag.GroupNameSl = groupName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null && m != "0").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.GroupName = objGroupName;
            string query = $"SELECT SERIAL_NUMBER,MODEL_NAME,GROUP_NAME,IN_STATION_TIME,PALLET_NO,LINE_NAME FROM sfism4.R_WIP_TRACKING_T where MODEL_NAME='{modelName}' and group_name='{groupName}';";
            var objResult = _db.R_WIP_TRACKING_T.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);

        }
        public IActionResult SearchMO()
        {
           
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchMO(string MOnum, string PK4,string LB)
        {
            ViewBag.MO = MOnum;
            ViewBag.packing = PK4;
            ViewBag.LB = LB;
            MOnum = MOnum.ToUpper();
            MOnum = MOnum.Replace(" ", "");
            string query = "";
            string queryPre = $"SELECT a.serial_number, a.model_name, a.mo_number,a.group_name,a.pallet_no,case when b.group_name is null then 'NO' else 'YES' end PACKING4,case when a.group_name = '0' then 'NO' else 'YES' end INPUT FROM SFISM4.R_WIP_TRACKING_T a left join sfism4.r_sn_detail_t b on a.serial_number=b.serial_number and b.group_name='PACKING4' where a.mo_number = '{MOnum}' ";
            query = queryPre;
            if (PK4 == "YES")
            {
                query = query + " AND b.GROUP_NAME is not null ";
            }
            if (PK4 == "NO")
            {
                query = query + " AND b.GROUP_NAME is null ";
            }
            if (LB == "YES")
            {
                query = query + " and a.group_name != '0' ";
            }
            if (LB == "NO")
            {
                query = query + " and a.group_name = '0' ";
            }
            var objResult = _db.mOnos.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult MOSummary()
        {
            DateTime fromDate = DateTime.Today.AddDays(-7);
            DateTime toDate = DateTime.Today;
            ViewBag.fromDate = fromDate.ToString("d/MM/yyyy HH:mm");
            ViewBag.toDate = toDate.ToString("d/MM/yyyy HH:mm");
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MOSummary(string fromDate, string toDate, string MOnum, string modelName)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.MO = MOnum;
            ViewBag.ToDate = toDate;
            ViewBag.ModelNameSl = modelName;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
            string queryPre = $"SELECT a.MO_NUMBER, a.MODEL_NAME,a.TARGET_QTY,a.INPUT_QTY,a.output_qty, a.TURN_OUT_QTY SHIPPING_QTY FROM SFISM4.R_MO_BASE_T a, stock.r_instock_t b where a.mo_number = b.mo_number and a.version_code = '1' and (b.close_flag = '1') and a.model_name in (select model_name from sfis1.c_model_desc_t)  ";
            query = queryPre;
            if (MOnum != null)

            {
                MOnum = MOnum.ToUpper();
                string MO1 = "('" + MOnum + "')";
                MO1 = MO1.Replace(" ", "");
                query = query + " AND a.MO_NUMBER = " + MO1 + "";
            }
            else
            {
                if (modelName != "ALL")
                {
                    query = query + "AND a.MODEL_NAME = '" + modelName + "' and a.MO_schedule_DATE BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ')";
                }
                else
                    query = query + " and a.MO_schedule_DATE BETWEEN TO_DATE('" + fromDate + "', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('" + toDate + "', 'dd/mm/yyyy hh24:mi:ss ')";
            }

            string querysuf = " order by b.close_flag, a.close_flag, a.MO_schedule_DATE ";
            query = query + querysuf;
            var objResult = _db.mosums.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }
            public IActionResult plan()
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
            public IActionResult plan(string modelname, string linename, string fromDate, string toDate, string Shift)
        {
                ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.Shift = Shift;
            ViewBag.ModelNameSl = modelname;
            ViewBag.Linename = linename;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            DateTime endDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy h:mm:ss", CultureInfo.InvariantCulture);
            string shortDate = endDate.ToString("yyyy/MM/dd");
            shortDate = shortDate.Replace("/","");

            DateTime endDate2 = DateTime.ParseExact(toDate, "dd/MM/yyyy h:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDatex = endDate2.AddDays(+1);
            string endDate1 = endDatex.ToString("dd/MM/yyyy HH:mm:ss");
            string shortDate2 = endDate2.ToString("yyyy/MM/dd");
            shortDate2 = shortDate2.Replace("/", "");


            /*string query = $"SELECT a.SERIAL_NUMBER,a.MODEL_NAME,b.GROUP_NAME,a.PALLET_NO,b.IN_STATION_TIME,b.LINE_NAME FROM R_WIP_TRACKING_T a, R_SN_DETAIL_T b WHERE a.SERIAL_NUMBER = b.SERIAL_NUMBER AND a.MODEL_NAME ='{modelname}' AND b.GROUP_NAME ='LCM_FRONT_BEZEL' AND b.LINE_NAME ='{linename}'";*//*AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}') AND TO_DATE('{toDate}')*/
            string query = "";
                string queryPre = $"SELECT d.PLAN_DATE,line_name,e.MODEL_KP MODEL_NAME,d.SHIFT,NVL(d.PLAN_QTY,0) PLAN_QTY,d.OUTPUT,d.Completed_percent, NVL(d.Repair,0) Repair,d.model_name BOM_NO FROM (SELECT NVL(a.PLAN_DATE,'N/A') PLAN_DATE, NVL(NVL(a.line_name,b.line_name),c.line_name) line_name, NVL(b.model_name,a.model_NAME) model_name, NVL(NVL(b.SHIFT,a.SHIFT),c.SHIFT) SHIFT, case when a.model_name!=b.model_name then 0 else a.PLAN_QTY end PLAN_QTY, NVL(b.OUTPUT, '0') OUTPUT, case when a.model_name!=b.model_name then 0 else NVL(round((b.output / a.plan_qty) * 100), '0') end Completed_percent, case when a.model_name!=b.model_name then 0 else c.REPAIR ENd Repair FROM (SELECT MODEL_NAME, LINE_NAME, SHIFT, DATEX, COUNT(CASE WHEN GROUP_NAME = 'PACKING4' THEN 1 END) AS OUTPUT FROM (SELECT MODEL_NAME, LINE_NAME, SERIAL_NUMBER, GROUP_NAME, TO_CHAR(TIME, 'YYYYMMDD') DATEX, CASE WHEN (TIME - TRUNC(TIME)) BETWEEN 0 / 24 AND 11.99973 / 24 THEN 'Day' ELSE 'Night' END SHIFT FROM (SELECT MODEL_NAME, LINE_NAME, SERIAL_NUMBER, GROUP_NAME, (max(IN_STATION_TIME)-8/24) AS TIME FROM SFISM4.R_SN_DETAIL_T WHERE GROUP_NAME IN ('PACKING4') and station_name like 'PACKING4%' AND MOD(ERROR_FLAG, 3) = '0' AND IN_STATION_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{endDate1}', 'dd/mm/yyyy hh24:mi:ss') group by MODEL_NAME, LINE_NAME, SERIAL_NUMBER, GROUP_NAME, in_station_time)) GROUP BY MODEL_NAME, LINE_NAME, SHIFT, DATEX ORDER BY MODEL_NAME) b  full outer join (SELECT PLAN_DATE, line_name, model_name, case when class_name = 'D' then 'Day' else 'Night' end SHIFT, plan_qty FROM kitting.r_assy_line_mo_base_t where plan_date between to_char('{shortDate}') AND to_char('{shortDate2}')) a on a.line_name = b.line_name and a.shift = b.shift and a.PLAN_DATE = b.DATEX left join (SELECT MODEL_NAME, LINE_NAME, SHIFT, COUNT(SERIAL_NUMBER) AS REPAIR, DATEX FROM (SELECT MODEL_NAME, LINE_NAME, SERIAL_NUMBER, TO_CHAR(TIME, 'YYYYMMDD') DATEX, CASE WHEN (TIME - TRUNC(TIME)) BETWEEN 0 / 24 AND 11.99973 / 24 THEN 'Day' ELSE 'Night' END SHIFT FROM (SELECT MODEL_NAME, TEST_LINE LINE_NAME, SERIAL_NUMBER, (max(TEST_TIME)-8/24) AS TIME FROM SFISM4.R_REPAIR_T where TEST_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{endDate1}', 'dd/mm/yyyy hh24:mi:ss') group by MODEL_NAME, TEST_LINE, SERIAL_NUMBER,TEST_TIME)) GROUP BY MODEL_NAME, LINE_NAME, SHIFT,DATEX) c on a.line_name = c.line_name and a.model_name = c.model_name and a.shift = c.shift and a.PLAN_DATE=c.DATEX where a.line_name != 'TV_A07'";
                query = queryPre;
                if (Shift != "ALL")
                {
                
                    query = query + " AND a.shift = '" + Shift + "'";
                    
                }
            if (linename != "ALL")
            {
                query = query + " AND a.line_name = '" + linename + "'";
            }
            if (modelname != "ALL")
            {
                query = query + " AND a.model_name = '" + modelname + "'";
            }

                string querysuf = "  order by a.PLAN_DATE,a.line_name) d left join sfis1.c_model_desc_t e on d.model_name=e.model_name; ";
                query = query + querysuf;
                var objResult = _db.plan.FromSqlRaw(query).ToList();
                if (objResult.Count == 0)
                {
                    ViewBag.Error = "No Record found";
                }
                return View(objResult);

        }
        public IActionResult Over3day()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Over3day(string modelname, string linename)
        {
            ViewBag.ModelNameSl = modelname;
            ViewBag.Linename = linename;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = "";
            string queryPre = $"select line_name, model_name, serial_number, mo_number, group_name, in_station_time FROM sfism4.r_wip_online_t WHERE IN_STATION_TIME < (sysdate-3)";
            query = queryPre;
            if (modelname != "ALL")
            {
                query = query + "  AND model_name = '" + modelname + "'";
            }
            if (linename != "ALL")
            {
                query = query + " AND line_name = '" + linename + "'";
            }
            string querySuf = " order by line_name, model_name, in_station_time desc";
            query = query + querySuf;
            var objResult = _db.r_wip_online_t.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
    }
}