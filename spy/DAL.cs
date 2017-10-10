using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace spy
{
   public  struct pair
    {
        public string key;
        public string value;
    }
    //Data Source=.\SQLEXPRESS;AttachDbFilename=C:\工程\TMS\TMS\bin\Release\DataBase\teachmanage.mdf;Integrated Security=True;User ID=admin;Connect Timeout=30;User Instance=True
    public class SqlWorkUnit
    {
        SqlConnection cn;
        Dictionary<DataTable, SqlDataAdapter> UpdateMapper;
        TextLog errorLog;

        public SqlWorkUnit(string datapath)
        {
            errorLog = new TextLog(System.AppDomain.CurrentDomain.BaseDirectory + "mssql.txt");
            string ConnectParam = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + datapath + ";Integrated Security=True;User Instance=True";//(LocalDB)\MSSQLLocalDB
            cn = new SqlConnection(ConnectParam);
            UpdateMapper = new Dictionary<DataTable, SqlDataAdapter>();
        }

        public SqlWorkUnit(string datapath, string sqlmodel)
        {
            errorLog = new TextLog(System.AppDomain.CurrentDomain.BaseDirectory + "mssql.txt");
            string ConnectParam = @"Data Source=" + sqlmodel + ";AttachDbFilename=" + datapath + ";Integrated Security=True;User Instance=True";//(LocalDB)\MSSQLLocalDB
            cn = new SqlConnection(ConnectParam);
            UpdateMapper = new Dictionary<DataTable, SqlDataAdapter>();
        }

        public DataTable ExuSQLDataTable(string sql, bool persisted = true)
        {
            cn.Open();
            DataTable dt = new DataTable();  
            SqlDataAdapter sda = new SqlDataAdapter(sql, cn);
            sda.Fill(dt);
            if (persisted)
            {
                UpdateMapper.Add(dt, sda);
            }
            cn.Close();
            return dt;
        }

        public bool ExuSQL(string sql)
        {
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.ExecuteNonQuery();
                cn.Close();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(DataTable dt)
        {
            if (UpdateMapper.ContainsKey(dt))
            {
                SqlCommandBuilder scb = new SqlCommandBuilder(UpdateMapper[dt]);
                UpdateMapper[dt].Update(dt);
            }
        }

        public void Save(DataTable dt,string tablename,Dictionary<string,string> mapping)
        {
            using (SqlBulkCopy copy = new SqlBulkCopy(cn.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
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
            using (SqlBulkCopy copy = new SqlBulkCopy(cn.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
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
                    StreamWriter sw = new StreamWriter(Guid.NewGuid() + ".txt");
                    foreach(DataRow dr in dt.Rows)
                    {
                        sw.WriteLine(string.Join(",", dr.ItemArray));
                    }
                    sw.Close();
                    errorLog.Write(ex.ToString());
                }
            }
        }

        public void Clear()
        {
            List<DataTable> list = UpdateMapper.Keys.ToList();
            foreach(DataTable dt in list)
            {
                UpdateMapper[dt].Dispose();
            }
        }


        ~SqlWorkUnit()
        {
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
