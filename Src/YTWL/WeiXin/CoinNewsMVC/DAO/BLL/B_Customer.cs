using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAO.DAL;
using DAO.IDAL;
using Common;
using DAO.Cache;
namespace DAO.BLL {
	public partial class B_Customer
	{
		private readonly ICustomer dal;
		public B_Customer()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Customer();
            }
            else
            {
                dal = new D_Customer();
            }
        }
		
		#region  Method

		#endregion
   
	}
}