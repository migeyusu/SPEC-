using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SepcReptile
{
   public  struct Pair
    {
        public string Key;
        public string Value;
    }
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
                    var sw = new StreamWriter(Guid.NewGuid() + ".txt");
                    foreach(DataRow dr in dt.Rows)
                    {
                        sw.WriteLine(string.Join(",", dr.ItemArray));
                    }
                    sw.Close();
                    _errorLog.Write(ex.ToString());
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
        //public bool DeleRow(DataTable dt, int index)
        //{
        //    if (datacontroler == null)
        //        return false;
        //    try
        //    {
        //        dt.Rows[index].Delete();
        //        SqlCommandBuilder scb = new SqlCommandBuilder(datacontroler);
        //        datacontroler.Update(dt);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public bool DeleRows(DataTable dt, int[] selects)
        //{
        //    if (datacontroler == null)
        //        return false;
        //    try
        //    {
        //        foreach (int index in selects)
        //            dt.Rows[index].Delete();
        //        SqlCommandBuilder scb = new SqlCommandBuilder(datacontroler);
        //        datacontroler.Update(dt);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public bool MainTableDeleRow(int[] indexes)
        //{
        //    if (datacontroler == null)
        //        return false;
        //    try
        //    {
        //        for (int i = 0; i < indexes.Length; ++i)
        //        {
        //            MainTable.Rows[i].Delete();
        //        }
        //        SqlCommandBuilder scb = new SqlCommandBuilder(datacontroler);
        //        datacontroler.Update(MainTable);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public void MainTableModify(pair[] modified, int index)
        //{
        //    if (datacontroler == null)
        //        return;
        //    DataRow dr = MainTable.Rows[index];
        //    foreach (pair p1 in modified)
        //        dr[p1.key] = p1.value;
        //    SqlCommandBuilder scb = new SqlCommandBuilder(datacontroler);
        //    datacontroler.Update(MainTable);
        //}

        //public void Modify(DataTable dt, pair[] modified, int index)
        //{
        //    if (datacontroler == null)
        //        return;
        //    DataRow dr = dt.Rows[index];
        //    foreach (pair p1 in modified)
        //        dr[p1.key] = p1.value;
        //    SqlCommandBuilder scb = new SqlCommandBuilder(datacontroler);
        //    datacontroler.Update(dt);
        //}

        //public void Find(ref pair[] datas)
        //{
        //    for (int i = 0; i < datas.Length; ++i)
        //        datas[i].value = MainTable.Rows[0][datas[i].key].ToString();
        //}

        //public void Find(string sql, ref pair datas)
        //{
        //    cn.Open();
        //    SqlDataAdapter sda = new SqlDataAdapter(sql, cn);
        //    DataSet ds = new DataSet();
        //    sda.Fill(ds, "tb");
        //    MainTable = ds.Tables["tb"];
        //    cn.Close();
        //    datas.value = MainTable.Rows[0][datas.key].ToString();
        //}

}
