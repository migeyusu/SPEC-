using System.Collections.Generic;

namespace SepcReptile
{
    /*
     * URL查询-获取网页-正则-选取URL-数据库(包含URL)
     * URL可按照规则生成或在已获取页面里抓取
     * URL和期望的查询结果都在同一个db
     * 
     * 爬虫基类：
     * 1.确定数据列
     * 2.填写正则
     * 3.存入datatable
     */
     

    /// <summary>
    /// 爬虫工作流（基于原生SQL）
    /// </summary>
    public class ReptileWorkFlow
    {
        //public string DataBase { get; set; }
        /// <summary>
        /// 取得带URL的列的datatable
        /// </summary>
        public string UrlSql { get; set; }
        /// <summary>
        /// URL列
        /// </summary>
        public string UrlColumn { get; set; }
        /// <summary>
        /// 储存结果表
        /// </summary>
        public string ValTable { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 结果表有序列
        /// </summary>
        public List<string> ValColumns { get; set; }
        /// <summary>
        /// 起始行id
        /// </summary>
        public int Sn { get; set; }
        public string Sign { get; set; }
    }
}