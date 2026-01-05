
truncate table auditlog;

truncate table cart;
truncate table inventory_count;
delete from inventory;
dbcc checkident('inventory', reseed, 0 );

truncate table order_item;
delete from [order];
dbcc checkident('[order]', reseed, 0 );

truncate table user_session;
truncate table customerdetails;