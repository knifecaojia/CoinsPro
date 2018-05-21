using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//Backup
		public class Backups
	{
	
      	/// <summary>
		/// id
        /// </summary>
        public virtual int id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// backupType
        /// </summary>
        public virtual string backupType
        {
            get; 
            set; 
        }        
		/// <summary>
		/// dbName
        /// </summary>
        public virtual string dbName
        {
            get; 
            set; 
        }        
		/// <summary>
		/// fileName
        /// </summary>
        public virtual string fileName
        {
            get; 
            set; 
        }        
		/// <summary>
		/// fileSize
        /// </summary>
        public virtual string fileSize
        {
            get; 
            set; 
        }        
		/// <summary>
		/// filePaht
        /// </summary>
        public virtual string filePath
        {
            get; 
            set; 
        }        
		/// <summary>
		/// addTime
        /// </summary>
        public virtual DateTime? addTime
        {
            get; 
            set; 
        }        
		/// <summary>
		/// addManagerID
        /// </summary>
        public virtual int? addManagerID
        {
            get; 
            set; 
        }

        public virtual Manager addManager
        {
            get;
            set;
        }

		/// <summary>
		/// delTime
        /// </summary>
        public virtual DateTime? delTime
        {
            get; 
            set; 
        }        
		/// <summary>
		/// delManagerID
        /// </summary>
        public virtual int delManagerID
        {
            get; 
            set; 
        }
        private Manager _delManager = new Manager();
        public virtual Manager delManager
        {
            get { return _delManager; }
            set 
            {
                if (value != null)
                {
                    //如果给个空对象,则赋予null
                    if (value.user_name == null)
                    {
                        _delManager = null; 
                    }
                    else
                    {
                        _delManager = value;
                    }
                }
            } 
        }

        public virtual string remark { get; set; }
	}
}