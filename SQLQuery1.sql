select * from [dbo].[ParentAs]
select * from [dbo].[ChildA1]
select * from [dbo].[ChildA2]
select * from [dbo].[GrandChildA1]

delete from [dbo].[GrandChildA1]
delete from [dbo].[ChildA2]
delete from [dbo].[ChildA1]
delete from [dbo].[ParentAs]


select * from [dbo].[ParentBs]
select * from [dbo].[ChildBs]
select * from [dbo].[GrandChildB1]
select * from [dbo].[GrandChildB2]

delete from [dbo].[GrandChildB2]
delete from [dbo].[GrandChildB1]
delete from [dbo].[ChildBs]
delete from [dbo].[ParentBs]

