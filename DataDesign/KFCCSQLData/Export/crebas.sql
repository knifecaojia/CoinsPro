/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     2018/6/14 9:22:55                            */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('T_SmsLog')
            and   type = 'U')
   drop table T_SmsLog
go

/*==============================================================*/
/* Table: T_SmsLog                                              */
/*==============================================================*/
create table T_SmsLog (
   ID                   int                  identity,
   Phone                varchar(20)          null,
   Code                 varchar(10)          null,
   SendTime             datetime             null,
   Status               smallint             null,
   constraint PK_T_SMSLOG primary key (ID)
)
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('T_SmsLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Status')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'T_SmsLog', 'column', 'Status'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '发送状态 null || 0= 未发送 1=发送成功 2=发送失败',
   'user', @CurrentUser, 'table', 'T_SmsLog', 'column', 'Status'
go

