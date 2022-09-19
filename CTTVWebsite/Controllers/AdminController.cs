using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.Data.OracleClient;
using CTTVWebsite.Models;

namespace CTTVWebsite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration configuration;
   
        public AdminController(ApplicationDbContext db, IConfiguration iConfig)
        {
            _db = db;
            configuration = iConfig;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckScanList()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult CheckScanList(string ScanRoute, int KP_LENGTH, string PartNo,  string COM_STR1, string Position1, string COM_STR2, string Position2)
        {
            string ScanList;
            string Unique;

            var Position1Delim = Position1.Split('-');
            var STR_START1 = Position1Delim[0];
            var STR_END1 = Int32.Parse(Position1Delim[1]) - Int32.Parse(Position1Delim[0]) + 1;
            
            string STR_START2 = "0";
            string STR_END2 = "0";


            //var query = $"select KEY_PART_NO FROM  SFIS1.C_KEYPARTS_MANAGE_T WHERE COM_STR1 = '{COM_STR1}' and KP_LENGTH='{KP_LENGTH}' AND STR_START1='{STR_START1}' AND STR_END1='{STR_END1}' ";
            var query = $"SELECT KEY_PART_NO, CASE WHEN KEY_PART_NO = '{PartNo}' AND COM_STR1 = '{COM_STR1}' THEN 'RULE1 Match|Keypart Match' WHEN KEY_PART_NO <> '{PartNo}' AND COM_STR1 = '{COM_STR1}' THEN 'RULE1 Match|Keypart not Match' WHEN KEY_PART_NO = '{PartNo}' AND COM_STR2 = '{COM_STR1}' THEN 'RULE2 Match|Keypart Match' WHEN KEY_PART_NO <> '{PartNo}' AND COM_STR2 = '{COM_STR1}' THEN 'RULE2 Match|Keypart Not Match' END AS CHECK_RESULT FROM SFIS1.C_KEYPARTS_MANAGE_T WHERE KP_LENGTH = {KP_LENGTH} AND ( ( STR_START1 = {STR_START1} AND STR_END1 = {STR_END1} AND COM_STR1 = '{COM_STR1}' ) OR ( STR_START2 = {STR_START1} AND STR_END2 = {STR_END1} AND COM_STR2 = '{COM_STR1}' ) ) UNION ALL SELECT KEY_PART_NO, 'RULES SIMILAR|SIMILAR RULE EXIST' AS CHECK_RESULT FROM SFIS1.C_KEYPARTS_MANAGE_T WHERE KP_LENGTH = {KP_LENGTH} AND ( ( COM_STR1 IS NOT NULL AND LENGTH(COM_STR1) <> LENGTH('{COM_STR1}') AND ( 1 = STR_START1 + INSTR(COM_STR1, '{COM_STR1}') -1 OR 1 = STR_START1 - INSTR('{COM_STR1}', COM_STR1) + 1 ) ) OR ( COM_STR2 IS NOT NULL AND LENGTH(COM_STR2) <> LENGTH('{COM_STR1}') AND ( 1 = STR_START2 + INSTR(COM_STR2, '{COM_STR1}') -1 OR 1 = STR_START2 - INSTR('{COM_STR1}', COM_STR2) + 1 ) ) );";
            
            //if (Position2 != null && COM_STR2 != null)
            //{
            //    var Position2Delim = Position2.Split('-');
            //    STR_START2 = Position2Delim[0];
            //    var calStr2 = Position2Delim[1];
            //    STR_END2 = calStr2.ToString();
            //    query = query + $" AND STR_START2 = '{STR_START2}' AND STR_END2 = '{STR_END2}' AND COM_STR2 = '{COM_STR2}'";
            //}
            
            var objResult = _db.C_KEYPARTS_MANAGE_T.FromSqlRaw(query).ToList();

            string TagFlag;
            if(STR_END1 == KP_LENGTH)
            {
                TagFlag = "0";
            }  
            else
            {
                TagFlag = "1";
            }   
            
            var query2 = $"SELECT count(*) FROM C_KEYPARTS_TAG_T a, C_KEYPARTS_DESC_T b where a.kp_name = b.kp_name and b.key_part_no = '{PartNo}' AND a.TAG_GROUP='{ScanRoute}' AND a.TAG_FLAG = '{TagFlag}' ";
            //var query2 = $"SELECT count(*) C_KEYPARTS_TAG_T where key_part_no = '{PartNo}' AND TAG_GROUP = '{ScanRoute}' AND TAG_FLAG = '{TagFlag}' ";
            int objResult2 = 0;
            string RULE_WARNING = "FALSE";
            var connectString = configuration.GetConnectionString("CustomConnection");
            //OracleParameter PartNoParam;
            //OracleParameter ScanRouteParam;
            //OracleParameter TagFlagParam;
            List<KeyPartTag> kpNameString = new List<KeyPartTag>();
            using (OracleConnection con = new OracleConnection(connectString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    
                    //PartNoParam = new OracleParameter("OraPartNo", PartNo);
                    //ScanRouteParam = new OracleParameter("OraScanRoute", OracleDbType.Varchar2);
                    //TagFlagParam = new OracleParameter("OraTagFlag", OracleDbType.Int32);


                    //cmd.Parameters.Add(PartNoParam);
                    //cmd.Parameters.Add(ScanRouteParam);
                    //cmd.Parameters.Add(TagFlagParam);
                    //cmd.BindByName = true;
                    cmd.CommandText = query2;
                    objResult2 = int.Parse(cmd.ExecuteScalar().ToString());

                   
                    if (objResult2 <= 0)
                    {
                        Unique = "NOT FOUND";
                        var query3 = $"SELECT KP_NAME FROM C_KEYPARTS_DESC_T WHERE key_part_no = '{PartNo}'";
                        using (OracleConnection con2 = new OracleConnection(connectString))
                        {
                            con2.Open();
                            using (var cmd2 = con.CreateCommand())
                            {

                                //PartNoParam = new OracleParameter("OraPartNo", PartNo);
                                //ScanRouteParam = new OracleParameter("OraScanRoute", OracleDbType.Varchar2);
                                //TagFlagParam = new OracleParameter("OraTagFlag", OracleDbType.Int32);


                                //cmd.Parameters.Add(PartNoParam);
                                //cmd.Parameters.Add(ScanRouteParam);
                                //cmd.Parameters.Add(TagFlagParam);
                                //cmd.BindByName = true;
                                cmd2.CommandText = query3;
                                var dr = cmd2.ExecuteReader();
                                while (dr.Read())
                                {
                                    KeyPartTag sto = new KeyPartTag
                                    {
                                        KP_NAME = dr["KP_NAME"].ToString(),
                                        TAG_FLAG = TagFlag,
                                        TAG_GROUP = ScanRoute
                                    };

                                    kpNameString.Add(sto);
                                }

                            }
                            con2.Dispose();
                            con2.Close();
                            OracleConnection.ClearPool(con2);
                        }
                    }
                    

                }
                con.Dispose();
                con.Close();
                OracleConnection.ClearPool(con);
            }



            

            if (objResult.Count <= 0)
            {
                ScanList = "NOT FOUND";
            }
            else
            {
                ScanList = "FOUND";
            }
            if (objResult2 <= 0)
            {
                Unique = "NOT FOUND";
            }
            else
            {
                Unique = "FOUND";
            }

            return Ok(new { ScanList = Newtonsoft.Json.JsonConvert.SerializeObject(objResult), Unique = Unique, KP_NAME = Newtonsoft.Json.JsonConvert.SerializeObject(kpNameString) });
        }

        public IActionResult DataTester()
        {
            return View();
        }
    }
}