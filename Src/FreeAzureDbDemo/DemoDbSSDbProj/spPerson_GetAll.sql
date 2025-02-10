CREATE PROCEDURE [dbo].[spPerson_GetAll]
AS
  SELECT [Id], [FirstName], [LastName] 
  FROM [dbo].[Person]
RETURN 0
