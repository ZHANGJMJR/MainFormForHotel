using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormForHotel
{
    using Oracle.ManagedDataAccess.Client;
    using MySql.Data.MySqlClient;
    using System;
    using System.Threading.Tasks;
    using System.Data;

    public class Wind
    {
        private static readonly string oracleConnStr = "User Id=sys;Password=Orcl$1mph0ny;Data Source=172.16.139.12:1521/mcrspos;DBA Privilege=SYSDBA;";
 
        private static readonly string mysqlConnStr = "Server=localhost;Port=3306;Database=hotel;User=root;Password=123456;";
        // 172.16.139.12:1521/mcrspos
        public static async Task SyncOracleToMySQL()
        {
            //using (OracleConnection oracleConn = new OracleConnection(Wind.oracleConnStr))
            //using (MySqlConnection mysqlConn = new MySqlConnection(Wind.mysqlConnStr))
            //{
            //    oracleConn.Open();
            //    mysqlConn.Open();
            //    DateTime now = DateTime.Now;
            //    DateTime currentDate = now.Date;
            //    // currentDate.ToString("yyyy-MM-dd");
            //    string currentDateStr = "2024-07-01";
            //    //string query = "SELECT * FROM guest_check_hist WHERE gch.openbusinessdate >= TO_DATE('2025-02-26', 'YYYY-MM-DD') AND gch.closebusinessdate <= TO_DATE('2025-02-26', 'YYYY-MM-DD')";

            //    string queryStr = $@"SELECT gch.guestCheckID as guestCheckID, gch.openBusinessDate as busDate,
            //            gch.locationID as locationid, gch.revenuecenterid as revenuecenterid,
            //         gch.checknum AS checkNum, gch.opendatetime AS openDateTime, gch.checktotal AS checkTotal,
            //         gch.numitems AS numItems, e.firstname AS firstName,  e.lastname AS lastName, 
            //        '_HIST' AS isHist FROM guest_check_hist gch  LEFT OUTER JOIN employee e 
            //         ON gch.employeeid=e.employeeid WHERE gch.organizationID =10260 
            //            AND gch.locationID = 2041 AND gch.openbusinessdate >= to_date('{currentDateStr}', 'YYYY-MM-DD')
            //                AND gch.closebusinessdate <= to_date('{currentDateStr}', 'YYYY-MM-DD')
            //        UNION ALL 
            //        SELECT gch.guestCheckID as guestCheckID, gch.openBusinessDate as busDate, 
            //        gch.locationID as locationid, gch.revenuecenterid as revenuecenterid, 
            //        gch.checknum AS checkNum, gch.opendatetime AS openDateTime, gch.checktotal AS checkTotal,
            //        gch.numitems AS numItems, e.firstname AS firstName, e.lastname AS lastName, 
            //        ' ' AS isHist FROM guest_check gch  LEFT OUTER JOIN employee e 
            //           ON gch.employeeid=e.employeeid WHERE gch.organizationID =10260 
            //        AND gch.locationID = 2041 AND gch.openbusinessdate >= to_date('{currentDateStr}', 'YYYY-MM-DD')
            //         AND gch.closebusinessdate <= to_date('{currentDateStr}', 'YYYY-MM-DD') ";

            //    using (OracleCommand oracleCmd = new OracleCommand(queryStr, oracleConn))
            //    using (OracleDataReader reader =  oracleCmd.ExecuteReader())
            //    {
            //        using (MySqlCommand mysqlCmd = mysqlConn.CreateCommand())
            //        {


            //            //mysqlCmd.CommandText = "INSERT INTO guestcheck " +
            //            //    "(guestCheckID, busDate,locationid ,revenuecenterid,checkNum,checkTotal,numItems,firstName,lastName)" +
            //            //    "  VALUES " +
            //            //    "(@guestCheckID, @busDate,@locationid ,@revenuecenterid,@checkNum,@checkTotal,@numItems,@firstName,@lastName);";

            //            //mysqlCmd.Parameters.Add("@guestCheckID", MySqlDbType.Int32);
            //            //mysqlCmd.Parameters.Add("@busDate", MySqlDbType.Date);
            //            //mysqlCmd.Parameters.Add("@locationid", MySqlDbType.Int32);
            //            //mysqlCmd.Parameters.Add("@revenuecenterid", MySqlDbType.Int32);
            //            //mysqlCmd.Parameters.Add("@checkNum", MySqlDbType.Int32);
            //            //mysqlCmd.Parameters.Add("@checkTotal", MySqlDbType.Float);
            //            //mysqlCmd.Parameters.Add("@numItems", MySqlDbType.Int32);
            //            //mysqlCmd.Parameters.Add("@firstName", MySqlDbType.VarChar); 
            //            //mysqlCmd.Parameters.Add("@lastName", MySqlDbType.VarChar);
            //            // busDate, @busDate,

            //            //mysqlCmd.CommandText = @"
            //            //       INSERT INTO guestcheck (  guestCheckID, 
            //            //             locationid,   revenuecenterid,  checkNum,  checkTotal, 
            //            //           numItems,  firstName,   lastName   ) 
            //            //       VALUES ( @guestCheckID,     @locationid,   @revenuecenterid,   @checkNum, 
            //            //           @checkTotal,   @numItems,   @firstName,     @lastName  );";
            //            mysqlCmd.CommandText = @"INSERT INTO guestcheck (  guestcheckid  ) 
            //                   VALUES ( @guestcheckid   );";
            //            while (reader.Read())
            //            {
            //                //Console.WriteLine(reader["guestCheckID"] + reader.RowSize.ToString() +"当前日期（使用DateTime.Today）: " + reader["guestCheckID"]);
            //                //ii_tmp++;
            //                //mysqlCmd.Parameters["@guestcheckid"].Value = reader["guestCheckID"];
            //                ////mysqlCmd.Parameters["@busDate"].Value = reader["busDate"];
            //                //mysqlCmd.Parameters["@locationid"].Value = reader["locationid"];
            //                //mysqlCmd.Parameters["@revenuecenterid"].Value = reader["revenuecenterid"];
            //                //mysqlCmd.Parameters["@checkNum"].Value = reader["checkNum"];
            //                //mysqlCmd.Parameters["@checkTotal"].Value = reader["checkTotal"];
            //                //mysqlCmd.Parameters["@numItems"].Value = reader["numItems"];
            //                //mysqlCmd.Parameters["@firstName"].Value = reader["firstName"];
            //                //mysqlCmd.Parameters["@lastName"].Value = reader["lastName"];

            //                //为参数赋值
            //                mysqlCmd.Parameters.AddWithValue("@guestcheckid", MySqlDbType.Int64);
            //                ////mysqlCmd.Parameters.Add("@busDate", MySqlDbType.Date);
            //                //mysqlCmd.Parameters.AddWithValue("@locationid", MySqlDbType.Int64);
            //                //mysqlCmd.Parameters.AddWithValue("@revenuecenterid", MySqlDbType.Int64);
            //                //mysqlCmd.Parameters.AddWithValue("@checkNum", MySqlDbType.Int64);
            //                //mysqlCmd.Parameters.AddWithValue("@checkTotal", MySqlDbType.Decimal);
            //                //mysqlCmd.Parameters.AddWithValue("@numItems", MySqlDbType.Int64);
            //                //mysqlCmd.Parameters.AddWithValue("@firstName", MySqlDbType.VarChar);
            //                //mysqlCmd.Parameters.AddWithValue("@lastName", MySqlDbType.VarChar);

            //                mysqlCmd.Parameters["@guestcheckid"].Value = reader["guestCheckID"]; 
            //                // 替换为实际的guestCheckID值
            //                                                                                     //mysqlCmd.Parameters.AddWithValue("@busDate", reader["busDate"]); // 替换为实际的busDate值，注意日期格式
            //                                                                                     //mysqlCmd.Parameters["@locationid"].Value = reader["locationid"]; // 替换为实际的locationid值
            //                                                                                     //mysqlCmd.Parameters["@revenuecenterid"].Value = reader["revenuecenterid"]; // 替换为实际的revenuecenterid值
            //                                                                                     //mysqlCmd.Parameters["@checkNum"].Value = reader["checkNum"]; // 替换为实际的checkNum值
            //                                                                                     //mysqlCmd.Parameters["@checkTotal"].Value = reader["checkTotal"]; // 替换为实际的checkTotal值
            //                                                                                     //mysqlCmd.Parameters["@numItems"].Value = reader["numItems"]; // 替换为实际的numItems值
            //                                                                                     //mysqlCmd.Parameters["@firstName"].Value = reader["firstName"]; // 替换为实际的firstName值
            //                                                                                     //mysqlCmd.Parameters["@lastName"].Value = reader["lastName"]; // 替换为实际的lastName值

            //                 mysqlCmd.ExecuteNonQuery();
        }
    }
    }

 