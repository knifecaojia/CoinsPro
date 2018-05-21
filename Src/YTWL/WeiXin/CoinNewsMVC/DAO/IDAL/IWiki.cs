﻿using NHibernate;
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
	/// 接口层D_Wiki
	/// </summary>
	public interface IWiki
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Wiki model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Wiki model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Wiki model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Wiki Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Wiki> LoadAll();
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		IList<Wiki> GetList(List<SearchTemplate> st, List<Order> order);
		
		/// <summary>
		/// 获得总条数
		/// </summary>
		int GetCount(List<SearchTemplate> st);
		#endregion  成员方法
	} 
}