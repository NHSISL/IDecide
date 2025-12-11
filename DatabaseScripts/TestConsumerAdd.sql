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
		   'ldstestidecide-fapp-uai',
		   'System',
		   getdate(),
		   'System',
		   getdate(),
		   'david.hayes17@nhs.net',
		   '07974226084',
		   'David Hayes',
		   '9379cc97-5258-425e-ab4e-106a3dc0d5f8'
		   )
GO


