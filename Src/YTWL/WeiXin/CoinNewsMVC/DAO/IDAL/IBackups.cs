using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using System.Linq.Expressions;
using NHibernate.Criterion;
using Common;
namespace DAO.IDAL {
	/// <summary>
	/// 接口层D_Backup
	/// </summary>
	public interface IBackups
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Backups model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Backups model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Backups model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Backups Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Backups> LoadAll();
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        IList<Domain.Backups> GetList(List<SearchTemplate> st, List<Order> order);

        int GetCount(List<SearchTemplate> st);


        void BackupDB(string dbName, string filePath, int backType);

        string GetDbName();
		#endregion  成员方法
	} 
}