using CTTVWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Oracle.ManagedDataAccess.Client;

namespace CTTVWebsite.Controllers
{
    public class RepaireController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration configuration;
        public RepaireController(ApplicationDbContext db, IConfiguration iConfig)
        {
            _db = db;
            configuration = iConfig;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RepairSum()
        {
            var query = $"SELECT * FROM (SELECT model_name,line_name, count(case when MOD(ERROR_FLAG,3) = '1' and work_flag in ('0','2') then 1 end) Scan_Error, count(case when work_flag in ('1') then 1 end) Repair_In, count(case when MOD(ERROR_FLAG,3) = '0' and work_flag in ('2') and group_name = 'SYS_REPAIR' then 1 end) Repair_Out FROM SFISM4.R_WIP_TRACKING_T group by model_name,line_name) where Scan_Error != '0' or Repair_In != '0' or Repair_Out != '0'";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<repairsum> repairsums = new List<repairsum>();
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
                        repairsum spsum = new repairsum
                        {
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            LINE_NAME = dr["LINE_NAME"].ToString(),
                            SCAN_ERROR = dr["SCAN_ERROR"].ToString(),
                            REPAIR_IN = dr["REPAIR_IN"].ToString(),
                            REPAIR_OUT = dr["REPAIR_OUT"].ToString()

                            //FirstName = dr["FirstName"].ToString(),
                        };

                        repairsums.Add(spsum);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(repairsums);
        }
        public IActionResult ErrDetail()
        {
            var query = $"SELECT serial_Number, Model_Name, Line_Name, Group_Name, In_Station_Time FROM SFISM4.R_WIP_TRACKING_T WHERE MOD(ERROR_FLAG,3) = '1' and work_flag in ('0','2')";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<repairdetail> repairdetails = new List<repairdetail>();
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
                        repairdetail spdetail = new repairdetail
                        {
                            SERIAL_NUMBER = dr["SERIAL_NUMBER"].ToString(),
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            LINE_NAME = dr["LINE_NAME"].ToString(),
                            GROUP_NAME = dr["GROUP_NAME"].ToString(),
                            IN_STATION_TIME = dr["IN_STATION_TIME"].ToString()

                            //FirstName = dr["FirstName"].ToString(),
                        };

                        repairdetails.Add(spdetail);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(repairdetails);
        }
        public IActionResult RpinDetail()
        {
            var query = $"SELECT serial_Number, Model_Name, Line_Name, Group_Name, In_Station_Time FROM  SFISM4.R_WIP_TRACKING_T WHERE work_flag in ('1')";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<repairdetail> repairdetails = new List<repairdetail>();
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
                        repairdetail spdetail = new repairdetail
                        {
                            SERIAL_NUMBER = dr["SERIAL_NUMBER"].ToString(),
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            LINE_NAME = dr["LINE_NAME"].ToString(),
                            GROUP_NAME = dr["GROUP_NAME"].ToString(),
                            IN_STATION_TIME = dr["IN_STATION_TIME"].ToString()

                            //FirstName = dr["FirstName"].ToString(),
                        };

                        repairdetails.Add(spdetail);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(repairdetails);
        }
        public IActionResult RpoutDetail()
        {
            var query = $"SELECT a.serial_Number, a.Model_Name, a.Line_Name, a.Group_Name, a.In_Station_Time,case when a.next_station != 'N/A' then a.next_station when a.next_station = 'N/A' and b.group_name is not null then b.group_next else 'N/A' end Next_Station FROM SFISM4.R_WIP_TRACKING_T a left join sfis1.c_route_control_t b on a.special_route = b.route_code and a.group_name = b.group_name and b.group_next not in ('SYS_REPAIR', 'OBI_IN', 'AGIN_ASSY', 'REWORK', 'QA_INLINE_IN', 'OUTSTOCK') WHERE MOD(a.ERROR_FLAG,3) = '0' and a.work_flag in ('2') and a.group_name = 'SYS_REPAIR'";
            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<repairdetail> repairdetails = new List<repairdetail>();
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
                        repairdetail spdetail = new repairdetail
                        {
                            SERIAL_NUMBER = dr["SERIAL_NUMBER"].ToString(),
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            LINE_NAME = dr["LINE_NAME"].ToString(),
                            GROUP_NAME = dr["GROUP_NAME"].ToString(),
                            IN_STATION_TIME = dr["IN_STATION_TIME"].ToString(),
                             NEXT_STATION = dr["NEXT_STATION"].ToString()
                            //FirstName = dr["FirstName"].ToString(),
                        };

                        repairdetails.Add(spdetail);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(repairdetails);
        }
        public IActionResult RpInfo()
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult RpInfo(string fromDate, string toDate, string modelName, string lineName)
        {
            var objModelName = _db.R_WIP_TRACKING_T.Select(m => m.MODEL_NAME).Where(m => m != "ITTEST" && m != "TEST001" && m != "TESTREWORK12" && m != "A5034TEST").Distinct().OrderBy(m => m).ToList();
            ViewBag.ModelName = objModelName;
            ViewBag.ModelNameSl = modelName;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.lineName = lineName;
            var query = $"SELECT a.Model_Name,a.test_line Line_Name, a.Serial_Number, a.Test_Time, a.Test_Station, a.Repair_Time,a.Item_Desc,a.Memo,b.Group_Name, b.In_Station_Time from SFISM4.R_REPAIR_T a, SFISM4.R_WIP_TRACKING_T b where a.serial_number=b.serial_number and a.repair_time between to_date('{fromDate}','DD/MM/YYYY HH24:MI:SS') and to_date('{toDate}','DD/MM/YYYY HH24:MI:SS') ";
            if (modelName != "ALL")
            {
                query = query + $"and a.model_name = '{modelName}'";
            }
            if (lineName != "ALL")
            {
                query = query + $"and a.test_line = '{lineName}'";
            }

            var connectString = configuration.GetConnectionString("CustomConnection");
            string result;
            List<rpinfo> rpinfos = new List<rpinfo>();
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
                        rpinfo rpinfo = new rpinfo
                        {
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            LINE_NAME = dr["LINE_NAME"].ToString(),
                            SERIAL_NUMBER = dr["SERIAL_NUMBER"].ToString(),
                            TEST_TIME = dr["TEST_TIME"].ToString(),
                            TEST_STATION = dr["TEST_STATION"].ToString(),
                            ITEM_DESC = dr["ITEM_DESC"].ToString(),
                            MEMO = dr["MEMO"].ToString(),
                            GROUP_NAME = dr["GROUP_NAME"].ToString(),
                            IN_STATION_TIME = dr["IN_STATION_TIME"].ToString(),
                            REPAIR_TIME = dr["REPAIR_TIME"].ToString()
                            //FirstName = dr["FirstName"].ToString(),
                        };

                        rpinfos.Add(rpinfo);
                    }
                    //}
                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);

            }
            return View(rpinfos);
            //var objResult = _db.C_KEYPARTS_MANAGE_T.FromSqlRaw(query).ToList();

            //var connectString = configuration.GetConnectionString("CustomConnection");
            //OracleParameter PartNoParam;
            //OracleParameter ScanRouteParam;
            //OracleParameter TagFlagParam;
            //using (OracleConnection con = new OracleConnection(connectString))
            //{
            //    con.Open();
            //    using (var cmd = con.CreateCommand())
            //    {

            //        //PartNoParam = new OracleParameter("OraPartNo", PartNo);
            //        //ScanRouteParam = new OracleParameter("OraScanRoute", OracleDbType.Varchar2);
            //        //TagFlagParam = new OracleParameter("OraTagFlag", OracleDbType.Int32);


            //        //cmd.Parameters.Add(PartNoParam);
            //        //cmd.Parameters.Add(ScanRouteParam);
            //        //cmd.Parameters.Add(TagFlagParam);
            //        //cmd.BindByName = true;
            //        cmd.CommandText = query2;
            //        objResult2 = int.Parse(cmd.ExecuteScalar().ToString());

            //    }
            //    con.Dispose();
            //    con.Close();
            //    OracleConnection.ClearPool(con);
            //}

            //if (objResult.Count <= 0)
            //{
            //    ScanList = "NOT FOUND";
            //}
            //else
            //{
            //    ScanList = "FOUND";
            //}
            //if (objResult2 <= 0)
            //{
            //    Unique = "NOT FOUND";
            //}
            //else
            //{
            //    Unique = "FOUND";
            //}




            //return Ok(new { ScanList = ScanList, Unique = Unique });
        }
    }
}
