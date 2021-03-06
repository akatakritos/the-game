/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [Id]
      ,[PlayerName]
      ,[Timestamp]
      ,[Badges]
      ,[Effects]
      ,[Title]
      ,[Points]
      ,[ItemsGained]
      ,[ItemsUsed]
  FROM [dbo].[PointLog] order by id desc


  SELECT
   cast(Timestamp as smalldatetime)  Timestamp
  ,avg(Points)  Value
 from PointLog
 group by cast(Timestamp as smalldatetime)
 order by cast(Timestamp as smalldatetime)