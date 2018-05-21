using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//Customer
        public class Customer
     {
         public virtual int Id { get; set; }
         public virtual int Version { get; set; }
         public virtual string FirstName { get; set; }
         public virtual string LastName { get; set; }
         public virtual double AverageRating { get; set; }
         public virtual int? Points { get; set; }
         public virtual bool HasGoldStatus { get; set; }
         public virtual DateTime MemberSince { get; set; }
         public virtual CustomerCreditRating CreditRating { get; set; }
         public virtual string Street { get; set; }
         public virtual string City { get; set; }
         public virtual string Province { get; set; }
         public virtual string Country { get; set; }
     }
 
     public enum CustomerCreditRating
     {
         Excellent, VeryVeryGood, VeryGood, Good, Neutral, Poor, Terrible
     }
}