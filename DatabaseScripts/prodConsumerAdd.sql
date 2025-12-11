USE [IDecide]
GO

INSERT INTO [dbo].[Consumers]
           ([Id]
           ,[Name]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[ContactEmail]
           ,[ContactNumber]
           ,[ContactPerson]
           ,[EntraId])
     VALUES
           (
		   '212088C6-1BC3-44EF-A513-3401E98B4358',
		   'ldsprodidecide-fapp-uai',
		   'System',
		   getdate(),
		   'System',
		   getdate(),
		   'david.hayes17@nhs.net',
		   '07974226084',
		   'David Hayes',
		   'b29a85d8-1fa6-4bed-b545-e73c4923cca2'
		   )
GO


