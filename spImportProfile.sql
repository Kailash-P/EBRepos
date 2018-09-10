alter PROC dbo.spImportProfile(@XMLString NVARCHAR(MAX))  
AS  
BEGIN  
 DECLARE @xmlRoot varchar(max)  
 DECLARE @ixml int  
 DECLARE @validateXmlString varchar(max) = ''  
 DECLARE @ValidateXML varchar(max) = ''  
  
 IF ISNULL(@XMLString, '') <> ''  
      BEGIN  
        SET @xmlRoot = SUBSTRING(@XMLString, 0, CHARINDEX('?>', @XMLString) + 2)  
        SET @XMLString = REPLACE(@XMLString, @xmlRoot, '')  
      END  
      EXEC sp_xml_preparedocument @ixml OUTPUT,  
                                  @XMLString  
  
 INSERT INTO ImportProfile  
 (  
	UserName,
	Password,
	ConsumerNo,
	RegionCode,
	Address,
	Email,
	City,
	State,
	ZipCode,
	IsAdmin
 )  
 SELECT  
	UserName,
	Password,
	ConsumerNo,
	RegionCode,
	Address,
	Email,
	City,
	State,
	ZipCode,
	IsAdmin 
 FROM OPENXML(@ixml, '//*', 2)  
   WITH  
      (  
		UserName VARCHAR(MAX),
		Password VARCHAR(MAX),
		ConsumerNo VARCHAR(MAX),
		RegionCode VARCHAR(MAX),
		Address VARCHAR(MAX),
		Email VARCHAR(MAX) ,
		City VARCHAR(MAX),
		State VARCHAR(MAX),
		ZipCode VARCHAR(MAX),
		IsAdmin VARCHAR(MAX) 
   ) AS A  
   WHERE ISNULL(A.UserName,'') <> ''  
  
   IF(SELECT COUNT(ID) FROM ImportProfile) > 0  
   BEGIN  
   EXEC [dbo].[spCheckErrorsForImportEnquiry]  
    
  
   SET @validateXmlString = (SELECT   
           A.ErrorLevel,  
           A.ColumnName,  
           A.ErrorMessage,  
           A.ID AS RowNumber  
          FROM ImportError A             
          FOR XML PATH('ImportError'),Root('ArrayOfImportError'))  
   IF @validateXmlString <> ''  
   BEGIN  
    SET @ValidateXML = '<?xml version="1.0" encoding="utf-8"?>'+ @validateXmlString        
   END  
   ELSE  
   BEGIN  
    EXEC [dbo].[spBulkInsertForImportProfile]  
   END  
   END  

     
   TRUNCATE TABLE ImportProfile  
  
   SELECT @ValidateXML  
END