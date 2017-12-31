using System.Collections.Generic;

namespace Reptile
{
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
        /// <summary>
        /// 执行成功URL行标志
        /// </summary>
        public string Sign { get; set; }
    }
}