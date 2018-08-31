/* ==============================================================================
* 类型名称：DapperDbProvider  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/5 14:39:15
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using DES.DbCaches.DbEntities;
using DES.DbCaches.Entities;
using DES.DbCaches.Interfaces;
using Dapper;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace DES.DbCaches.Implementeds
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperDbProvider : IDbProvider, IDbHExchangeLoad, IVehicleBaseInfoLoad, IDbBYDQBaseInfoLoad
    {
        public event Action<string> InfoEvent;
        public event Action<string> WarnEvent;
        protected DbProviderFactory DbFactory { get; set; }
        protected string ConnectionString { get; set; }

        private const string connectionStringName = "forward";

        public DapperDbProvider()
        {
            var item = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (item == null)
                throw new ConfigurationErrorsException(string.Format("ConnectionStringNameInvalid:{0}", connectionStringName));

            if (string.IsNullOrEmpty(item.ProviderName))
                throw new ConfigurationErrorsException("connectionString.ProviderName");

            ConnectionString = item.ConnectionString;
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ConfigurationErrorsException("ConnectionString");

            try
            {
                DbFactory = DbProviderFactories.GetFactory(item.ProviderName);

            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(item.ProviderName + " Provider name invalid for" + connectionStringName, ex);
            }
        }

        public IDbConnection GetDbConnection()
        {
            var connectString = DbFactory.CreateConnection();
            if (connectString != null)
                connectString.ConnectionString = ConnectionString;
            return connectString;
        }

        public DbConnection CreateOracleConnection()
        {
            var connection = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["incom"].ToString());
            connection.Open();
            return connection;
        }

        public List<ForwadVehicleEntity> QueryForwardVehicles()
        {
            const string sql =
                    "select vfc.VEHICLEID,vfs.SEQ,vfs.URL,vfs.PORT from vw_incom_forwardingcar vfc inner join vw_incom_forwardingserver vfs  on vfs.SEQ = vfc.ForwardingServerSEQ where vfs.FLAG = 1 and vfc.Flag = 1";
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<ForwadVehicleEntity>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'vw_incom_forwardingcar',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<HisotoryDbcmapEntity> QueryHistoryDbcmap(IEnumerable<string> md5List)
        {
            const string format = "select md5code,protocolid,MAPSERIALIZE from history_dbcmap where flag=1 and md5code in ({0})";
            var sql = string.Empty;
            var md5codes = md5List.Select(find => string.Format("'{0}'", find)).ToList();
            var result = new List<HisotoryDbcmapEntity>();
            using (var connection = GetDbConnection())
            {
                try
                {
                    while (md5codes.Count != 0)
                    {
                        // 单次读取50行
                        var count = md5codes.Count < 50 ? md5codes.Count : 50;
                        var md5Condition = string.Join(",", md5codes.Take(count));
                        md5codes.RemoveRange(0, count);
                        sql = string.Format(format, md5Condition);
                        result.AddRange(connection.Query<HisotoryDbcmapEntity>(sql).ToList());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'QueryHistoryDbcmap',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }


        public List<Faultmap> QueryFaultMap()
        {
            const string sql =
                "SELECT  PROTOCOLID, PROTOCOLFAULT, FAULTCODE, PROTOCOLFAULTLEVEL FROM FORWARD_FAULTMAP  WHERE  FLAG=1";
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<Faultmap>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'QueryFaultMap',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<VehicleBaseInfo> LoadAccessVehicleInfo()
        {
            List<VehicleBaseInfo> lst = new List<VehicleBaseInfo>();
            const string sql =
                "select C.TERMINALCODE Terminalcode,a.VEHICLEID Vinno,a.accessinforid Accessinfoid from accessvechile a left join VW_FORWARD_CAR_BASE@platform C on C.VEHICLEID = a.vehicleid where a.flag=0 ";
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<VehicleBaseInfo>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'LoadAccessVehicleInfo',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<VehicleBaseInfo> LoadAllVehicleInfo()
        {
            const string sql =
                "SELECT  VINNO, LICENSEPLATE FROM VW_VEHICLEBASEINFO";
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<VehicleBaseInfo>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'LoadAllVehicleInfo',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public VehicleBaseInfo LoadAllVehicleInfo(string vinno)
        {
            var sql =
                string.Format("SELECT  VINNO, LICENSEPLATE FROM VW_VEHICLEBASEINFO WHERE  VINNO='{0}'", vinno);
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<VehicleBaseInfo>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'LoadAllVehicleInfo(string vinno)',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指令集合
        /// </summary>
        /// <returns></returns>
        public List<DispatchInstructionDownEntity> GetOrderList(string state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from ");
            sb.Append("(select * from X_BYDDATADOWNLOAD where state = '" + state + "' and flag = 1 order by Time1");
            sb.Append(")t where rownum <= 100");
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<DispatchInstructionDownEntity>(sb.ToString()).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'GetOrderList',sql:{0}", sb.ToString()), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 获取UDS指令集合
        /// </summary>
        /// <returns></returns>
        public List<RemoteDebugDownEntity> GetUDSOderList(string state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from ");
            sb.Append("(select * from x_bydremotedebugdownload where state = '" + state + "' and flag = 1 order by CREATETIME");
            sb.Append(")t where rownum <= 100");
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<RemoteDebugDownEntity>(sb.ToString()).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'GetUDSOderList',sql:{0}", sb.ToString()), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 分页查询3006功能类型数据
        /// </summary>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<DataMonitorEntity> GetPageDataMonitorDataList(string startNum, string endNum, string condition)
        {
            //string sql =
            //    string.Format("select t.* from bydqdatamonitorbaseinfo t,(select rowid k, rownum rn,to_date('1970-01-01 08:00:00','yyyy-mm-dd hh24:mi:ss')+ timestamp/1000/24/60/60 as time from bydqdatamonitorbaseinfo t where uuid ='{0}' order by time) b where t.rowid=b.k and b.rn between {1} and {2}", condition, startNum, endNum);
            string sql =
                string.Format("select b.* from bydqdatamonitorbaseinfo b,(select rowid as k, to_date('1970-01-01 08:00:00','yyyy-mm-dd hh24:mi:ss')+ timestamp/1000/24/60/60 as time ,row_number() over(order by time) as row_number from bydqdatamonitorbaseinfo where uuid ='{0}' ) t where b.rowid = t.k and t.row_number between {1} and {2}", condition, startNum, endNum);
            using (var connection = CreateOracleConnection())
            {
                try
                {
                    return connection.Query<DataMonitorEntity>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'GetPageDataMonitorDataList(string startNum, string endNum, string condition)',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 分页查询3006功能类型数据
        /// </summary>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<RemoteDebugUPEntity> GetPageRemoteDebugDataList(string startNum, string endNum, string condition)
        {
            //string sql =
            //    string.Format("select t.* from bydqdatamonitorbaseinfo t,(select rowid k, rownum rn,to_date('1970-01-01 08:00:00','yyyy-mm-dd hh24:mi:ss')+ timestamp/1000/24/60/60 as time from bydqdatamonitorbaseinfo t where uuid ='{0}' order by time) b where t.rowid=b.k and b.rn between {1} and {2}", condition, startNum, endNum);
            string sql =
                string.Format("select b.* from BYDQREMOTEDEBUGBASEINFO b,(select rowid as k,row_number() over(order by TIME) as row_number from BYDQREMOTEDEBUGBASEINFO where uuid ='{0}' ) t where b.rowid = t.k and t.row_number between {1} and {2}", condition, startNum, endNum);
            using (var connection = CreateOracleConnection())
            {
                try
                {
                    return connection.Query<RemoteDebugUPEntity>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'GetPageRemoteDebugDataList(string startNum, string endNum, string condition)',sql:{0}", sql), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 获取3006功能类型数据
        /// </summary>
        /// <returns></returns>
        public List<DataMonitorEntity> GetDataMonitorDataList(string UUID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from ");
            sb.Append("bydqdatamonitorbaseinfo ");
            sb.Append("where UUID ='" + UUID + "'");
            using (var connection = GetDbConnection())
            {
                try
                {
                    return connection.Query<DataMonitorEntity>(sb.ToString()).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("query 'GetDataMonitorDataList(string UUID)',sql:{0}", sb.ToString()), ex);
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 更新3006类型数据下载状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateMonitorDataState(string uuid, string state, string errorMsg, string Path)
        {
            int row = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("update X_BYDDATADOWNLOAD ");
            sb.Append("set state = '" + state + "',MESSAGE = '" + errorMsg + "',PATH = " + Path);
            sb.Append(" where uuid = '" + uuid + "'");
            using (var connection = GetDbConnection())
            {
                connection.Open();
                try
                {
                    row = connection.Execute(sb.ToString());
                }
                catch (Exception ex)
                {
                    WarnEvent(string.Format("错误信息{0}", ex.Message));
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }

                }
                return row != 0;
            }
        }

        /// <summary>
        /// 更新7002类型数据下载状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateeRemoteDebugDataState(string uuid, string state, string errorMsg)
        {
            int row = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("update x_bydremotedebugdownload ");
            sb.Append("set state = '" + state + "',MESSAGE = '" + errorMsg + "'");
            sb.Append(" where uuid = '" + uuid + "'");
            using (var connection = GetDbConnection())
            {
                connection.Open();
                try
                {
                    row = connection.Execute(sb.ToString());
                }
                catch (Exception ex)
                {
                    WarnEvent(string.Format("错误信息{0}", ex.Message));
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }

                }
                return row != 0;
            }
        }

        /// <summary>
        /// 插入7002下行指令数据段（byte[]存blob）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool UpdateeRemoteDebugData(byte[] buffer)
        {
            int row = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("update x_bydremotedebugdownload ");
            sb.Append("set DIAGNOSTICDATALIST = :buffer where Flag =1");
            using (var connection = GetDbConnection())
            {
                connection.Open();
                try
                {

                    var transaction = connection.BeginTransaction();
                    try
                    {
                        row = connection.Execute(sb.ToString(), new { buffer }, transaction);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    WarnEvent(string.Format("错误信息{0}", ex.Message));
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }

                }
                return row != 0;
            }
        }

        public bool UpdateRemoteDebugUpBaseInfos(List<RemoteDebugUPJsonResultEntity> jsonResultEntity)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int row = 0;
            StringBuilder sb =new StringBuilder();
            sb.Append("update x_bydremotedebugdownload ");
            sb.Append("set JSONRESULT = :JsonResult where uuid =:uuid");
            using (var connection = GetDbConnection())
            {
                connection.Open();
                try
                {

                    var transaction = connection.BeginTransaction();
                    try
                    {
                        row = connection.Execute(sb.ToString(), jsonResultEntity, transaction);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    transaction.Commit();
                    
                }
                catch (Exception ex)
                {
                    WarnEvent(string.Format("错误信息{0}", ex.Message));
                }
                finally
                {
                    sw.Stop();
                    InfoEvent(string.Format("JsonResult数据入库共花费{0}ms.", sw.Elapsed.TotalMilliseconds));
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
                return row != 0;
            }
        }

        public bool SaveDataMonitorBaseInfos(List<DataMonitorEntity> list)
        {
            const string strSql = @"INSERT INTO BYDQDataMonitorBaseInfo (Encryption,Type,CMD,FunctionNumber,ResponseSign," +
                                  "FunctionStatus,UniqueIdentity,ProductType,CloudProductCode,TimeStamp,Time,UUID," +
                                  "SignType,DbCNT,DbSEQ,CanMessageData) " +
                                  "VALUES(:Encryption,:Type,:CMD,:FunctionNumber,:ResponseSign," +
                                  ":FunctionStatus,:UniqueIdentity,:ProductType,:CloudProductCode,:TimeStamp,:Time,:UUID," +
                                  ":SignType,:DbCNT,:DbSEQ,:CanMessageData) ";
            return ExecuteTransaction(strSql, list);
        }



        public bool SaveEventTriggerBaseInfos(List<EventTriggerEntity> list)
        {
            const string strSql = @"INSERT INTO BYDQEVENTTRIGGERBASEINFO (Encryption,Type,CMD,FunctionNumber,ResponseSign," +
                                  "FunctionStatus,UniqueIdentity,ProductType,CloudProductCode,TimeStamp,Time,UUID," +
                                  "DbDT,EventCode) " +
                                  "VALUES(:Encryption,:Type,:CMD,:FunctionNumber,:ResponseSign," +
                                  ":FunctionStatus,:UniqueIdentity,:ProductType,:CloudProductCode,:TimeStamp,:Time,:UUID," +
                                  ":DbDT,:EventCode) ";
            return ExecuteTransaction(strSql, list);
        }

        public bool SaveRemoteDebugUPBaseInfos(List<RemoteDebugUPEntity> list)
        {
            const string strSql = "INSERT INTO BYDQREMOTEDEBUGBASEINFO (Encryption,Type,CMD,FunctionNumber,ResponseSign," +
                                  "FunctionStatus,UniqueIdentity,ProductType,CloudProductCode,TimeStamp,Time,UUID,CNT,SEQ,DiagnosticResult,DiagnosticData)" +
                                  "VALUES(:Encryption,:Type,:CMD,:FunctionNumber,:ResponseSign," +
                                  ":FunctionStatus,:UniqueIdentity,:ProductType,:CloudProductCode,:TimeStamp,:Time,:UUID," +
                                  ":CNT,:SEQ,:DiagnosticResult,:DiagnosticData)";
            return ExecuteTransaction(strSql, list);
        }


        public bool ExecuteTransaction<T>(string strSql, List<T> list)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var row = 0;

            using (var conn = CreateOracleConnection())
            {
                try
                {
                    //conn.Open();
                    var transaction = conn.BeginTransaction();
                    try
                    {
                        row = conn.Execute(strSql, list, transaction);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    WarnEvent(string.Format("错误信息{0}", ex.Message));
                }
                finally
                {
                    sw.Stop();
                    InfoEvent(string.Format("数据入库共花费{0}ms.", sw.Elapsed.TotalMilliseconds));
                    conn.Close();
                }
            }

            return row != 0;
        }

    }
}
