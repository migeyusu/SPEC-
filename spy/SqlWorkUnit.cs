using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SepcReptile
{
    //Data Source=.\SQLEXPRESS;AttachDbFilename=C:\工程\TMS\TMS\bin\Release\DataBase\teachmanage.mdf;Integrated Security=True;User ID=admin;Connect Timeout=30;User Instance=True
    public class SqlWorkUnit
    {
        private readonly SqlConnection _cn;
        private readonly Dictionary<DataTable, SqlDataAdapter> _updateMapper;
        private readonly TextLog _errorLog;

        public SqlWorkUnit(string datapath)
        {
            _errorLog = new TextLog(System.AppDomain.CurrentDomain.BaseDirectory + "mssql.txt");
            var connectParam = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + datapath + ";Integrated Security=True;User Instance=True";//(LocalDB)\MSSQLLocalDB
            _cn = new SqlConnection(connectParam);
            _updateMapper = new Dictionary<DataTable, SqlDataAdapter>();
        }

        public SqlWorkUnit(string datapath, string sqlmodel)
        {
            _errorLog = new TextLog(System.AppDomain.CurrentDomain.BaseDirectory + "mssql.txt");
            var connectParam = @"Data Source=" + sqlmodel + ";AttachDbFilename=" + datapath + ";Integrated Security=True;User Instance=True";//(LocalDB)\MSSQLLocalDB
            _cn = new SqlConnection(connectParam);
            _updateMapper = new Dictionary<DataTable, SqlDataAdapter>();
        }

        public DataTable ExuSqlDataTable(string sql, bool persisted = true)
        {
            _cn.Open();
            var dt = new DataTable();  
            var sda = new SqlDataAdapter(sql, _cn);
            sda.Fill(dt);
            if (persisted)
            {
                _updateMapper.Add(dt, sda);
            }
            _cn.Close();
            return dt;
        }

        public bool ExuSql(string sql)
        {
            _cn.Open();
            var cmd = new SqlCommand(sql, _cn);
            cmd.ExecuteNonQuery();
            _cn.Close();
            return true;
        }

        public void Update(DataTable dt)
        {
            if (_updateMapper.ContainsKey(dt)) {
                new SqlCommandBuilder(_updateMapper[dt]);
                _updateMapper[dt].Update(dt);
            }
        }   

        public void Save(DataTable dt,string tablename,Dictionary<string,string> mapping)
        {
            using (var copy = new SqlBulkCopy(_cn.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                copy.DestinationTableName = tablename;
                foreach (var x in mapping.Keys)
                {

                    copy.ColumnMappings.Add(x, mapping[x]);
                }
                copy.WriteToServer(dt);
            }
        }

        public void Save(DataTable dt, string tablename, List<string> mapping)
        {
            using (var copy = new SqlBulkCopy(_cn.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                copy.DestinationTableName = tablename;
                foreach (var x in mapping)
                {
                    copy.ColumnMappings.Add(x, x);
                }
                try
                {
                    copy.WriteToServer(dt);
                    copy.Close();
                }
                catch(Exception ex)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (DataRow dr in dt.Rows)
                        stringBuilder.Append(string.Join(",", dr.ItemArray));
                    _errorLog.Write( ex.ToString());
                }
            }
        }

        public void Clear()
        {
            var list = _updateMapper.Keys.ToList();
            foreach(var dt in list)
            {
                _updateMapper[dt].Dispose();
            }
        }
    }
}
