using CTTVWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CTTVWebsite.Controllers
{
    [Authorize]
    public class HoldController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HoldController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult HoldUnhold()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HoldUnhold(string HoldNo,string HoldSTT)
        {

            string query = "";
            
            if (HoldNo == null)
            {
                if(HoldSTT == "Hold")
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO where hold_no is not null ORDER BY HOLD_DAYS DESC";
                }
                else if(HoldSTT == "Unhold")
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO where unhold_no is not null ORDER BY HOLD_DAYS DESC";
                }
                else
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO ORDER BY HOLD_DAYS DESC";
                }
            }
            else if (HoldNo != null)
            {
                HoldNo = HoldNo.ToUpper();
                if (HoldSTT == "Hold")
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO where HOLD_NO is not null AND a.TABLE_NO='{HoldNo}' ORDER BY HOLD_DAYS DESC";
                }
                else if (HoldSTT == "Unhold")
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO where UNHOLD_NO is not null AND a.TABLE_NO='{HoldNo}' ORDER BY HOLD_DAYS DESC";
                }
                else
                {
                    query = $"SELECT a.TABLE_NO,a.APPLY_REASON,a.TOTAL_COUNT,a.APPLY_TIME,TRUNC(sysdate - a.APPLY_TIME) HOLD_DAYS,b.UNHOLD_NO, c.HOLD_NO FROM SFISM4.R_APPLY_TABLE_T a LEFT JOIN(SELECT table_no, count(distinct(serial_number)) UNHOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '0' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) b ON a.TABLE_NO = b.TABLE_NO LEFT JOIN(SELECT TABLE_NO, count(distinct(SERIAL_NUMBER)) HOLD_NO FROM sfism4.R_APPLY_SN_LIST_T WHERE STATUS = '3' AND GROUP_NAME NOT IN ('SHIPPING') GROUP BY TABLE_NO) c ON a.TABLE_NO = c.TABLE_NO where a.TABLE_NO='{HoldNo}' ORDER BY HOLD_DAYS DESC";
                }
            }
            var objResult = _db.R_APPLY_TABLE_T1.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            ViewBag.HoldNo = HoldNo;
            ViewBag.HoldSTT = HoldSTT; 
            return View(objResult);
        }
        public IActionResult SearchHoldSTT()
        {

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchHoldSTT(string serialNumber)
        {
            ViewBag.serialNumber = serialNumber;
            serialNumber = serialNumber.Replace(System.Environment.NewLine, "','");
            serialNumber = serialNumber.ToUpper();
            serialNumber = "('" + serialNumber + "')";
            serialNumber = serialNumber.Replace(" ", "");
            string query = $"SELECT a.serial_number, a.model_name,a.table_no Hold_ID,b.apply_reason HOLD_REASON,a.STOP_GROUP  FROM sfism4.R_APPLY_sn_list_T a,SFISM4.R_APPLY_TABLE_T b  where a.table_no = b.table_no and a.status = '3' and a.serial_number in {serialNumber} order by a.serial_number; ";
            var objResult = _db.holdstts.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult SearchOneHoldSTT()
        {

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchOneHoldSTT(string serialNumber,string preSerial,string subSerial)
        {
            ViewBag.serialNumber = serialNumber;
            ViewBag.Pre = preSerial;
            ViewBag.Sub = subSerial;
            if (serialNumber == null && preSerial != null && subSerial != null)
            {
                preSerial = preSerial.Substring(1, 8);
                subSerial = subSerial.Substring(3, 7);
                serialNumber = preSerial + "$" + subSerial;
            }
            //serialNumber = "('" + serialNumber + "')";
            //serialNumber = serialNumber.Replace(" ", "");
            string query = $"SELECT a.serial_number, a.model_name,a.table_no Hold_ID,b.apply_reason HOLD_REASON FROM sfism4.R_APPLY_sn_list_T a,SFISM4.R_APPLY_TABLE_T b  where a.table_no = b.table_no and a.status = '3' and a.serial_number = '{serialNumber}'";
            var objResult = _db.holdstts.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }

            return View(objResult);

        }
        public IActionResult HoldModel()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HoldModel(string modelname)
        {
            ViewBag.ModelNameSl = modelname;
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            string query = "";

            string queryPrefix = $"SELECT a.serial_number,a.model_name,a.hold_no,b.apply_reason,c.group_name,c.in_station_time,c.pallet_no FROM ( SELECT serial_number,model_name,table_no hold_no FROM sfism4.R_APPLY_sn_list_T where status = '3') a left join (select table_no,apply_reason from  SFISM4.R_APPLY_TABLE_T) b on a.hold_no=b.table_no left join (SELECT serial_number,group_name,in_station_time,pallet_no FROM SFISM4.R_WIP_TRACKING_T) c on a.serial_number=c.serial_number";
            query = queryPrefix;
          
            if (modelname != "ALL")
            {
                query = query + " WHERE a.MODEL_NAME LIKE '" + '%' + modelname + '%' + "' ";
            }

            var objResult = _db.holdModels.FromSqlRaw(query).ToList();
            if (objResult.Count == 0)
            {
                ViewBag.Error = "No Record found";
            }
            return View(objResult);
        }

    }
}
