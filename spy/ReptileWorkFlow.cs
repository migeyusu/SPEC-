using System.Collections.Generic;

namespace SepcReptile
{
    /*
     * URL��ѯ-��ȡ��ҳ-����-ѡȡURL-���ݿ�(����URL)
     * URL�ɰ��չ������ɻ����ѻ�ȡҳ����ץȡ
     * URL�������Ĳ�ѯ�������ͬһ��db
     * 
     * ������ࣺ
     * 1.ȷ��������
     * 2.��д����
     * 3.����datatable
     */
     

    /// <summary>
    /// ���湤����������ԭ��SQL��
    /// </summary>
    public class ReptileWorkFlow
    {
        //public string DataBase { get; set; }
        /// <summary>
        /// ȡ�ô�URL���е�datatable
        /// </summary>
        public string UrlSql { get; set; }
        /// <summary>
        /// URL��
        /// </summary>
        public string UrlColumn { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string ValTable { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// �����������
        /// </summary>
        public List<string> ValColumns { get; set; }
        /// <summary>
        /// ��ʼ��id
        /// </summary>
        public int Sn { get; set; }
        public string Sign { get; set; }
    }
}