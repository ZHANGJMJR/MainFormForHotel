using System;
using System.Data;
using System.Transactions;
using Dapper;
using log4net;
using log4net.Config;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.IO;

public class GuestCheckHist
{
    public long guestcheckid { get; set; }
    public DateTime busdate { get; set; }
    public long locationid { get; set; }
    public long revenuecenterid { get; set; }
    public long checkNum { get; set; }
    public DateTime openDateTime { get; set; }
    public decimal checkTotal { get; set; }
    public long numItems { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
}

public class GuestCheckDetails
{
    public DateTime transTime { get; set; }
    public long serviceRoundNum { get; set; }
    public long lineNum { get; set; }
    public long guestCheckLineItemID { get; set; }
    public int detailType { get; set; }
    public string itemName { get; set; }
    public string itemName2 { get; set; }
    public string itemchname { get; set; }
    public string rvcName { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string reasonVoidText { get; set; }
    public string returnText { get; set; }
    public long recordID { get; set; }

    public decimal salesTotal { get; set; }
    public int salesCount { get; set;   }
    public string salesCountDivisor { get; set;  }

    public long locationID { get;set; }
    public int doNotShow { get;set;      }
    public long guestCheckID { get; set; }
}


public class Dlt
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static readonly string oracleConnStr = "User Id=sys;Password=Orcl$1mph0ny;Data Source=172.16.139.12:1521/mcrspos;DBA Privilege=SYSDBA;";
    private static readonly string mysqlConnStr = "Server=localhost;Port=3309;Database=hotel;User=root;Password=123456;";

    static Dlt()
    {
        XmlConfigurator.Configure(new FileInfo("log.config"));
    }

    public void SyncData()
    {
        log.Info("=== 开始同步数据 ===");
        try
        {
            using (var oracleConn = new OracleConnection(oracleConnStr))
            using (var mysqlConn = new MySqlConnection(mysqlConnStr))
            {
                try
                {
                    oracleConn.Open();
                    log.Info("Oracle 数据库连接成功");

                    mysqlConn.Open();
                    log.Info("MySQL 数据库连接成功");

                    string currentDateStr = "2024-07-01";
                    string queryStr = @"SELECT gch.guestCheckID as guestCheckID, gch.openBusinessDate as busDate,
                   gch.locationID as locationid, gch.revenuecenterid as revenuecenterid,
                gch.checknum AS checkNum, 
                TO_DATE(TO_CHAR(gch.opendatetime, 'YYYY-MM-DD HH24:MI:SS'),'YYYY-MM-DD HH24:MI:SS') AS openDateTime, 
                gch.checktotal AS checkTotal,
                gch.numitems AS numItems, e.firstname AS firstName, e.lastname AS lastName 
                FROM guest_check_hist gch  
                LEFT JOIN employee e ON gch.employeeid=e.employeeid 
                WHERE gch.organizationID =10260 
                   AND gch.locationID = 2041 
                   AND gch.openbusinessdate >= TO_DATE(:currentDateStr, 'YYYY-MM-DD')
                   AND gch.closebusinessdate <= TO_DATE(:currentDateStr, 'YYYY-MM-DD')";

                    var records = oracleConn.Query<GuestCheckHist>(queryStr, new { currentDateStr });

                    if (records == null || records.AsList().Count == 0)
                    {
                        log.Warn("未查询到数据");
                        return;
                    }

                    log.Info($"从 Oracle 获取 {records.AsList().Count} 条记录");

                    string insertQuery = @"INSERT INTO guestcheck 
                  (guestCheckID, busDate, locationid, revenuecenterid, checkNum, openDateTime, checkTotal, numItems, firstName, lastName) 
                  VALUES 
                  (@guestCheckID, @busDate, @locationid, @revenuecenterid, @checkNum, @openDateTime, @checkTotal, @numItems, @firstName, @lastName);";

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            int insertedRows = mysqlConn.Execute(insertQuery, records);
                            log.Info($"成功插入 {insertedRows} 条数据到 MySQL");

                            transaction.Complete();
                        }
                        catch (Exception ex)
                        {
                            log.Error("插入 MySQL 数据失败: " + ex.Message, ex);
                        }
                    }

                    


                   // var guestCheckIDs = records.Select(x => x.guestcheckid).ToList();
                    var guestCheckIDs = string.Join(",", records.Select(x => x.guestcheckid).ToList());
                    string detailQuery = @"SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       MENU_ITEM.menuItemName1Master AS itemName,
       STTEXT.stringtext AS itemchname,
       rcs.name AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.genFlag1 = 1 THEN 'Return'
           ELSE 'blank' END AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       GUEST_CHECK_LINE_ITEM_HIST.lineTotal AS salesTotal,
       GUEST_CHECK_LINE_ITEM_HIST.lineCount AS salesCount,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.denominator > 0
               THEN CONCAT('/ ', GUEST_CHECK_LINE_ITEM_HIST.denominator)
           ELSE ''
           END AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM GUEST_CHECK_LINE_ITEM_HIST left join REVENUE_CENTER
on REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
left join Revenue_Center_String RCS on REVENUE_CENTER.Revenuecenterid=RCS.Revenuecenterid  and rcs.poslanguageid=3
left join EMPLOYEE
on  EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
left join mENU_ITEM
on mENU_ITEM.menuItemID = GUEST_CHECK_LINE_ITEM_HIST.recordID
left join MCRSPOSDB.menu_item_master MIM on MIM.objectnumber = mENU_ITEM.menuitemposref
left join MCRSPOSDB.string_table STTEXT on MIM.nameid = STTEXT.stringnumberid and (STTEXT.langid = 2)
where
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ))
  and mENU_ITEM.menuitemposref not in (19999997,19999998,19999999)
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       ((GUEST_CHECK_LINE_ITEM_HIST.lineNum*10)+1) AS lineNum,
       0 AS guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       REASON_CODE.name AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       'Reason' AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       0 AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     REASON_CODE  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN
         GUEST_CHECK_LINE_ITEM_HIST
         ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
         ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
         ON REASON_CODE.reasonCodeID = GUEST_CHECK_LINE_ITEM_HIST.reasonCodeID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ))
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       ((GUEST_CHECK_LINE_ITEM_HIST.lineNum*10)+1) AS lineNum,
       0 AS guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       REASON_CODE.name AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       'Reason' AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       0 AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     REASON_CODE  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN
         GUEST_CHECK_LINE_ITEM_HIST
         ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
         ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
         ON REASON_CODE.reasonCodeID = GUEST_CHECK_LINE_ITEM_HIST.reasonCodeID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.genFlag1 = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ))
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       DISCOUNT.nameMaster AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       GUEST_CHECK_LINE_ITEM_HIST.lineTotal AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     DISCOUNT  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
ON DISCOUNT.discountID = GUEST_CHECK_LINE_ITEM_HIST.recordID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 2) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ))
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       SERVICE_CHARGE.nameMaster AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       GUEST_CHECK_LINE_ITEM_HIST.lineTotal AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     SERVICE_CHARGE  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
ON SERVICE_CHARGE.serviceChargeID = GUEST_CHECK_LINE_ITEM_HIST.recordID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 3) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
UNION
SELECT GUEST_CHECK_HIST.openDateTime  AS transDatetime,
       100 AS serviceRoundNum,
       NULL AS lineNum,
       NULL AS guestCheckLineItemID,
       NULL AS detailType,
       SERVICE_CHARGE.nameMaster AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       'blank' AS reasonVoidText,
       'blank' AS returnText,
       NULL AS recordID,
       GUEST_CHECK_HIST.autoServiceChargeTotal AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_HIST.guestCheckID
FROM     EMPLOYEE RIGHT OUTER JOIN
         SERVICE_CHARGE   RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN
         GUEST_CHECK_HIST
         ON GUEST_CHECK_HIST.revenueCenterID = REVENUE_CENTER.revenueCenterID
         ON  SERVICE_CHARGE.serviceChargePosRef = REVENUE_CENTER.autoServiceChargePosref  AND
             SERVICE_CHARGE.locationID = REVENUE_CENTER.locationID
         ON GUEST_CHECK_HIST.employeeID = EMPLOYEE.employeeID
WHERE GUEST_CHECK_HIST.organizationID =10260 AND
    GUEST_CHECK_HIST.locationID =2041 AND
    GUEST_CHECK_HIST.autoServiceChargeTotal > 0 AND
    GUEST_CHECK_HIST.guestCheckID in ( :guestCheckIDs )
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       0 AS guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       REASON_CODE.name AS itemName,
       chr('') AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       'Reason' AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       0 AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     REASON_CODE  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN
         GUEST_CHECK_LINE_ITEM_HIST
         ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
         ON EMPLOYEE.employeeID = GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID
         ON REASON_CODE.reasonCodeID = GUEST_CHECK_LINE_ITEM_HIST.reasonCodeID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 3) AND
    (GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       TENDER_MEDIA.nameMaster AS itemName,
       TO_NCHAR(TMS.name) AS itemchname,
       RCS.name  AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       GUEST_CHECK_LINE_ITEM_HIST.lineTotal AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     TENDER_MEDIA  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
left join Revenue_Center_String RCS on REVENUE_CENTER.Revenuecenterid=RCS.Revenuecenterid  and rcs.poslanguageid=3
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
ON TENDER_MEDIA.tenderMediaID = GUEST_CHECK_LINE_ITEM_HIST.recordID
RIGHT JOIN  TENDER_MEDIA_string TMS on TMS.TENDERMEDIAID=TENDER_MEDIA.tenderMediaID and TMS.poslanguageid=3
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 4) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (TENDER_MEDIA.typeMaster = 1) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       GUEST_CHECK_LINE_ITEM_HIST.referenceInfo AS itemName,
       TO_NCHAR(GUEST_CHECK_LINE_ITEM_HIST.referenceInfo) AS itemchname,
       RCS.name  AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       0 AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
left join Revenue_Center_String RCS on REVENUE_CENTER.Revenuecenterid=RCS.Revenuecenterid  and rcs.poslanguageid=3
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 5) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
UNION
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       CASE
           WHEN TENDER_MEDIA.hideAcctNum = 1
               THEN  CONCAT('xxxx-', SUBSTR(TRIM(GUEST_CHECK_LINE_ITEM_HIST.referenceInfo), -4, 4))
           ELSE referenceInfo
           END itemName,
       CASE
           WHEN TENDER_MEDIA.hideAcctNum = 1
               THEN TO_NCHAR( CONCAT('xxxx-', SUBSTR(TRIM(GUEST_CHECK_LINE_ITEM_HIST.referenceInfo), -4, 4)))
           ELSE TO_NCHAR(referenceInfo)
           END AS itemchname,
       rcs.name AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID,
       0 AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     TENDER_MEDIA  RIGHT OUTER JOIN
         EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
left join Revenue_Center_String RCS on REVENUE_CENTER.Revenuecenterid=RCS.Revenuecenterid  and rcs.poslanguageid=3
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
ON TENDER_MEDIA.tenderMediaID = GUEST_CHECK_LINE_ITEM_HIST.recordID
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 6) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
union
SELECT GUEST_CHECK_LINE_ITEM_HIST.transDatetime AS transTime,
       GUEST_CHECK_LINE_ITEM_HIST.serviceRoundNum,
       (GUEST_CHECK_LINE_ITEM_HIST.lineNum*10) AS lineNum,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckLineItemID,
       GUEST_CHECK_LINE_ITEM_HIST.detailType,
       GUEST_CHECK_LINE_ITEM_HIST.referenceInfo AS itemName,
       TO_NCHAR(GUEST_CHECK_LINE_ITEM_HIST.referenceInfo) AS itemchname,
       REVENUE_CENTER.nameMaster AS rvcName,
       EMPLOYEE.firstName,
       EMPLOYEE.lastName,
       CASE
           WHEN GUEST_CHECK_LINE_ITEM_HIST.voidFlag = 1 THEN 'Void'
           ELSE 'blank' END AS reasonVoidText,
       'blank' AS returnText,
       GUEST_CHECK_LINE_ITEM_HIST.recordID AS RECORDID,
       GUEST_CHECK_LINE_ITEM_HIST.lineTotal AS salesTotal,
       0 AS salesCount,
       '' AS salesCountDivisor,
       GUEST_CHECK_LINE_ITEM_HIST.guestCheckID
FROM     EMPLOYEE RIGHT OUTER JOIN
         REVENUE_CENTER  RIGHT OUTER JOIN GUEST_CHECK_LINE_ITEM_HIST
ON REVENUE_CENTER.revenueCenterID = GUEST_CHECK_LINE_ITEM_HIST.revenueCenterID
ON EMPLOYEE.employeeID = NVL(GUEST_CHECK_LINE_ITEM_HIST.managerEmployeeID,GUEST_CHECK_LINE_ITEM_HIST.transEmployeeID)
WHERE (GUEST_CHECK_LINE_ITEM_HIST.organizationID =10260) AND
    (GUEST_CHECK_LINE_ITEM_HIST.locationID =2041) AND
    (GUEST_CHECK_LINE_ITEM_HIST.detailType = 7) AND
    (GUEST_CHECK_LINE_ITEM_HIST.doNotShow IS NULL OR GUEST_CHECK_LINE_ITEM_HIST.doNotShow = 0) AND
    (GUEST_CHECK_LINE_ITEM_HIST.guestCheckID in ( :guestCheckIDs ) )
ORDER BY guestCheckID,2, 3, 4;";
                    //var detailRecords = oracleConn.Query<GuestCheckDetails>(detailQuery, [1029263, 1029255]).ToList();
                    var detailRecords = oracleConn.Query<GuestCheckDetails>(detailQuery, new { guestCheckIDs }).ToList();
                    if (!detailRecords.Any())
                    {
                        log.Warn("从表未查询到数据");
                        return;
                    }
                    log.Info($"从 Oracle 获取 {detailRecords.Count} 条从表记录");
                    string detailInsertQuery = @"
                INSERT INTO guestcheckdetails 
                  (transTime, serviceRoundNum, lineNum, guestCheckLineItemID, detailType,itemName,itemName2,
                   itemchname, rvcName,firstName,lastName,reasonVoidText,returnText,recordID,salesTotal,
                    salesCount,salesCountDivisor,guestCheckID) 
                VALUES 
                  (@transTime, @serviceRoundNum, @lineNum, @guestCheckLineItemID, @detailType,@itemName,@itemName2,
                   @itemchname, @rvcName,@firstName,@lastName,@reasonVoidText,@returnText,@recordID,@salesTotal,
                    @salesCount,@salesCountDivisor,@guestCheckID);";

                    using (var transaction = new TransactionScope())
                    {
                        mysqlConn.Execute(detailInsertQuery, detailRecords);
                        transaction.Complete();
                    }

                    log.Info("从表数据同步完成");
                }
                catch (OracleException ex)
                {
                    log.Error("Oracle 数据库查询失败: " + ex.Message, ex);
                }
                catch (MySqlException ex)
                {
                    log.Error("MySQL 数据库连接失败: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    log.Error("同步数据过程中发生未知错误: " + ex.Message, ex);
                }
                finally
                {
                    if (oracleConn.State == ConnectionState.Open)
                    {
                        oracleConn.Close();
                        log.Info("Oracle 数据库连接关闭");
                    }
                    if (mysqlConn.State == ConnectionState.Open)
                    {
                        mysqlConn.Close();
                        log.Info("MySQL 数据库连接关闭");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            log.Error(e);
            return;
        }
        log.Info("=== 数据同步完成 ===");
    }
}
