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
		   'ldsdevidecide-fapp-uai',
		   'System',
		   getdate(),
		   'System',
		   getdate(),
		   'david.hayes17@nhs.net',
		   '07974226084',
		   'David Hayes',
		   'e7c55ef7-67a9-4378-b69a-4b4bb2be88e0'
		   )
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
		   '212088C6-1BC3-44EF-A513-3401E98B4357',
		   'David Hayes',
		   'System',
		   getdate(),
		   'System',
		   getdate(),
		   'david.hayes17@nhs.net',
		   '07974226084',
		   'David Hayes',
		   '48eed1a1-c919-48fd-ac06-7d2348c1d3bc'
		   )
