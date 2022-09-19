using CTTVWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;

namespace CTTVWebsite.Controllers
{
    [Authorize]
    public class KeypartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public KeypartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public string? SerialNumber { get; private set; }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult SearchKpNo()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SearchKpNo(string KpNo, string fromDate, string toDate)
        //{
        //    string query = $"SELECT a.SERIAL_NUMBER,a.KEY_PART_NO,a.KEY_PART_SN,a.MODEL_NAME,b.GROUP_NAME,a.WORK_TIME,b.PALLET_NO FROM R_WIP_KEYPARTS_T a, R_WIP_TRACKING_T b where a.SERIAL_NUMBER=b.SERIAL_NUMBER and a.key_part_no='{KpNo}'";
        //    var objResult = _db.WiptrackingWipkeypart.FromSqlRaw(query).ToList();
        //    if (objResult.Count == 0)
        //    {
        //        ViewBag.Error = "No Record found";
        //    }
        //    return View(objResult);
        //}

        public IActionResult SearchKpNoDate()
        {
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null).Distinct().OrderBy(m => m).ToList();
            ViewBag.GroupName = objGroupName;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchKpNoDate(string KpNo, string fromDate, string toDate,string groupName)
        {
            var objGroupName = _db.R_WIP_TRACKING_T.Select(m => m.GROUP_NAME).Where(m => m != null).Distinct().OrderBy(m => m).ToList();
            ViewBag.GroupName = objGroupName;
            KpNo = KpNo.ToUpper();
           // string query = $"SELECT a.SERIAL_NUMBER,a.KEY_PART_NO,a.KEY_PART_SN,a.MODEL_NAME,b.GROUP_NAME,a.WORK_TIME,a.group_name work_group,b.PALLET_NO,c.line_name FROM R_WIP_KEYPARTS_T a, R_WIP_TRACKING_T b,(SELECT serial_number,line_name FROM  sfism4.r_sn_detail_t order by in_station_time desc FETCH NEXT 1 ROWS ONLY) c where a.SERIAL_NUMBER=b.SERIAL_NUMBER and a.serial_number=c.serial_number and a.key_part_no='{KpNo}'"
                 string query = $"SELECT a.SERIAL_NUMBER, a.KEY_PART_NO, a.KEY_PART_SN, a.MODEL_NAME, b.GROUP_NAME, a.WORK_TIME, a.group_name work_group, b.PALLET_NO FROM sfism4.R_WIP_KEYPARTS_T a, sfism4.R_WIP_TRACKING_T b where a.SERIAL_NUMBER = b.SERIAL_NUMBER and a.key_part_no='{KpNo}'";
            if (groupName != "ALL")
            {
                query = query + " AND a.GROUP_NAME ='" + groupName + "' ";
            }
            if (fromDate != null && toDate != null)
            {
                query = query + $" AND a.WORK_TIME BETWEEN TO_DATE('{fromDate}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE( '{toDate}', 'dd/mm/yyyy hh24:mi:ss' )";
            }
            var objResult = _db.WiptrackingWipkeypart.FromSqlRaw(query).ToList();

            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            ViewBag.Kpno = KpNo;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.GroupNameSl = groupName;
            return View(objResult);
        }

        public IActionResult SearchKpSn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchKpSn(string KpSn,string OBI,string QA)
        {
            ViewBag.KpSn = KpSn;
            ViewBag.OBI = OBI;
            ViewBag.QA = QA;
            KpSn = KpSn.Replace(System.Environment.NewLine, "','");
            KpSn = KpSn.ToUpper();
            KpSn = "('" + KpSn + "')";
            //KpSn = KpSn.Replace(" ", "");
           // string query = $"SELECT a.SERIAL_NUMBER,a.KEY_PART_NO,a.KEY_PART_SN,d.GROUP_NAME,d.in_station_time,NVL(d.container_no,'N/A') container_no,a.MODEL_NAME,a.WORK_TIME,case when b.OBI_RECORD = 'O1' then 'OBI_YES' else 'OBI_NO' end OBI_RECORD, case when c.QA_RECORD = 'Q1' then 'QA_YES' else 'QA_NO' end QA_RECORD,NVL(TO_CHAR(b.OBI_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS OBI_TIME,NVL(TO_CHAR(c.QA_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS QA_TIME,D.PALLET_NO FROM SFISM4.R_WIP_KEYPARTS_T a left join ( SELECT serial_number, case when group_name = 'OBI_IN' then 'O1' else 'O' end OBI_REcord,  max(in_station_time) as OBI_TIME FROM sfism4.r_sn_detail_t where group_name in ('OBI_IN') group by serial_number,group_name) b on a.serial_number = b.serial_number left join (SELECT serial_number, case when group_name = 'QA_INLINE_IN' then 'Q1' else 'Q' end QA_REcord,  max(in_station_time) as QA_TIME FROM sfism4.r_sn_detail_t where group_name in ('QA_INLINE_IN') group by serial_number,group_name) c on a.serial_number = c.serial_number left join sfism4.r_wip_tracking_t d on a.serial_number = d.serial_number  where a.KEY_PART_SN in {KpSn}";
            string query = $"SELECT a.SERIAL_NUMBER,a.KEY_PART_NO,a.KEY_PART_SN,d.GROUP_NAME,d.in_station_time,NVL(d.container_no,'N/A') container_no,a.MODEL_NAME,a.WORK_TIME,case when b.OBI_RECORD = 'O1' then 'OBI_YES' else 'OBI_NO' end OBI_RECORD, case when c.QA_RECORD = 'Q1' then 'QA_YES' else 'QA_NO' end QA_RECORD,NVL(TO_CHAR(b.OBI_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS OBI_TIME,NVL(TO_CHAR(c.QA_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS QA_TIME,D.PALLET_NO FROM (SELECT SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN, MODEL_NAME, max(WORK_TIME) WORK_TIME FROM SFISM4.R_WIP_KEYPARTS_T group by SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN, MODEL_NAME) a left join ( SELECT serial_number, case when group_name = 'OBI_IN' then 'O1' else 'O' end OBI_REcord,  max(in_station_time) as OBI_TIME FROM sfism4.r_sn_detail_t where group_name in ('OBI_IN') group by serial_number,group_name) b on a.serial_number = b.serial_number left join (SELECT serial_number, case when group_name = 'QA_INLINE_IN' then 'Q1' else 'Q' end QA_REcord,  max(in_station_time) as QA_TIME FROM sfism4.r_sn_detail_t where group_name in ('QA_INLINE_IN') group by serial_number,group_name) c on a.serial_number = c.serial_number left join sfism4.r_wip_tracking_t d on a.serial_number = d.serial_number  where a.KEY_PART_SN in {KpSn}";
            var objResult = _db.KpTvSn.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            
            return View(objResult);
        }
        public IActionResult SearchOldKPSn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchOldKpSn(string KpSn)
        {
            ViewBag.KpSn = KpSn;
            KpSn = KpSn.Replace(System.Environment.NewLine, "','");
            KpSn = KpSn.ToUpper();
            KpSn = "('" + KpSn + "')";
            KpSn = KpSn.Replace(" ", "");
            string query = $"SELECT SERIAL_NO, OLD_KP_SN, OLD_KP_NO, CHANGE_TIME  FROM SFISM4.R_CHANGE_KP_T WHERE OLD_KP_SN IN {KpSn}";
            var objResult = _db.OldKP.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);
        }
        public IActionResult SearchKpTV()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchKpTV(string TVSn,string KpNo, string OBI, string QA)
        {
            ViewBag.TVSn = TVSn;
            ViewBag.KpNo = KpNo;
            ViewBag.OBI = OBI;
            ViewBag.QA = QA;
            TVSn = TVSn.Replace(System.Environment.NewLine, "','");
            TVSn = TVSn.ToUpper();
            KpNo = KpNo.ToUpper();
            TVSn = "('" + TVSn + "')";
            TVSn = TVSn.Replace(" ", "");
            string query = $"SELECT a.SERIAL_NUMBER,a.KEY_PART_NO,a.KEY_PART_SN,d.GROUP_NAME,d.in_station_time,a.MODEL_NAME,a.WORK_TIME,case when b.OBI_RECORD = 'O1' then 'OBI_YES' else 'OBI_NO' end OBI_RECORD, case when c.QA_RECORD = 'Q1' then 'QA_YES' else 'QA_NO' end QA_RECORD,NVL(TO_CHAR(b.OBI_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS OBI_TIME,NVL(TO_CHAR(c.QA_TIME,'MM/DD/YYYY hh:mi:ss AM'),'N/A') AS QA_TIME,D.PALLET_NO FROM SFISM4.R_WIP_KEYPARTS_T a left join ( SELECT serial_number, case when group_name = 'OBI_IN' then 'O1' else 'O' end OBI_REcord, in_station_time as OBI_TIME FROM sfism4.r_sn_detail_t where group_name in ('OBI_IN')) b on a.serial_number = b.serial_number left join (SELECT serial_number, case when group_name = 'QA_INLINE_IN' then 'Q1' else 'Q' end QA_REcord, in_station_time as QA_TIME FROM sfism4.r_sn_detail_t where group_name in ('QA_INLINE_IN')) c on a.serial_number = c.serial_number left join sfism4.r_wip_tracking_t d on a.serial_number = d.serial_number  where a.serial_number in {TVSn} and a.key_part_no='{KpNo}'";
            var objResult = _db.KpTvSn.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);
        }
    }
}
