CREATE PROCEDURE [dbo].[spCheckErrorsForImportEnquiry]  
AS  
BEGIN  
  
  TRUNCATE TABLE ImportError  
  
  ------------------------------------- BEGIN CHECK MAX LENGTH VALIDATIONS ---------------------------------------------  
  
  DECLARE @MaxLength int  
  
  -- Max Length Validation for CustomerName  
  
  SELECT  
    @MaxLength = c.max_length 
  FROM sys.columns c  
  INNER JOIN sys.types t  
    ON c.user_type_id = t.user_type_id  
  WHERE c.object_id = OBJECT_ID('ImportProfile')  
  AND c.name = 'UserName'  
  
  INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'UserName',  
      'Length of ' + UserName + ' exceeds possible size ' + CAST(@MaxLength AS varchar)  
    FROM ImportProfile  
    WHERE LEN(UserName) > @MaxLength  
  
  SELECT  
    @MaxLength = c.max_length   
  FROM sys.columns c  
  INNER JOIN sys.types t  
    ON c.user_type_id = t.user_type_id  
  WHERE c.object_id = OBJECT_ID('ImportProfile')  
  AND c.name = 'Password'  
  
  INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'Password',  
      'Length of ' + Password + ' exceeds possible size ' + CAST(@MaxLength AS varchar)  
    FROM ImportProfile  
    WHERE LEN(Password) > @MaxLength  
  
  
  SELECT  
    @MaxLength = c.max_length
  FROM sys.columns c  
  INNER JOIN sys.types t  
    ON c.user_type_id = t.user_type_id  
  WHERE c.object_id = OBJECT_ID('ImportProfile')  
  AND c.name = 'Email'  
  
  INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'Email',  
      'Length of ' + Email + ' exceeds possible size ' + CAST(@MaxLength AS varchar)  
    FROM ImportProfile  
    WHERE LEN(Email) > @MaxLength  
  
  SELECT  
    @MaxLength = c.max_length 
  FROM sys.columns c  
  INNER JOIN sys.types t  
    ON c.user_type_id = t.user_type_id  
  WHERE c.object_id = OBJECT_ID('ImportProfile')  
  AND c.name = 'City'  
  
  INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'City',  
      'Length of ' + City + ' exceeds possible size ' + CAST(@MaxLength AS varchar)  
    FROM ImportProfile  
    WHERE LEN(City) > @MaxLength  
  
  
  SELECT  
    @MaxLength = c.max_length
  FROM sys.columns c  
  INNER JOIN sys.types t  
    ON c.user_type_id = t.user_type_id  
  WHERE c.object_id = OBJECT_ID('ImportProfile')  
  AND c.name = 'State'  
  
  INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'State',  
      'Length of ' + State + ' exceeds possible size ' + CAST(@MaxLength AS varchar)  
    FROM ImportProfile  
    WHERE LEN(State) > @MaxLength 


  
------------------------------------- END CHECK MAX LENGTH VALIDATIONS ---------------------------------------------  
  
------------------------------------- BEGIN CHECK MANDATORY VALIDATIONS ---------------------------------------------  
  
  INSERT INTO ImportError(ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'UserName',  
      'UserName is Mandatory'  
    FROM ImportProfile  
    WHERE LTRIM(RTRIM(ISNULL(UserName, ''))) = ''  

	INSERT INTO ImportError(ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'Password',  
      'Password is Mandatory'  
    FROM ImportProfile  
    WHERE LTRIM(RTRIM(ISNULL(Password, ''))) = ''  

	INSERT INTO ImportError(ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'ConsumerNo',  
      'ConsumerNo is Mandatory'  
    FROM ImportProfile  
    WHERE LTRIM(RTRIM(ISNULL(ConsumerNo, ''))) = ''  
  
  DECLARE @id INT =0

  SET @id = (    SELECT  top 1
      ID
    FROM ImportProfile  
    WHERE ISNUMERIC(ISNULL(ZipCode, 1)) = 0  
    AND ISNULL(ZipCode, '') <> ''  )
  
  IF @id > 0
  BEGIN
   INSERT INTO ImportError (ID, ErrorLevel, ColumnName, ErrorMessage)  
    SELECT  
      ID,  
      'Error',  
      'ZipCode',  
      'ZipCode ' + ZipCode + ' should be an integer value'  
    FROM ImportProfile  
    WHERE ISNUMERIC(ISNULL(ZipCode, 1)) = 0  
    AND ISNULL(ZipCode, '') <> ''  
  END
  
   Insert into ImportError (ID, ErrorLevel, ColumnName, ErrorMessage) 
   Select ID, 'Error', 'IsAdmin', 'IsAdmin should be an boolean value (either TRUE or FALSE)' 
   From ImportProfile
    where Not (IsNull(UPPER(IsAdmin), 'TRUE') = 'TRUE' OR IsNull(UPPER(IsAdmin), 'FALSE') = 'FALSE')

	Insert into ImportError (ID, ErrorLevel, ColumnName, ErrorMessage) 
	 SELECT
                ID,
                'Error',
                'UserName',
                'UserName is duplicated in the Excel'
            FROM ImportProfile
            WHERE (LTRIM(RTRIM(ISNULL(UserName, '')))) IN (SELECT
               (LTRIM(RTRIM(ISNULL(UserName, '')))) 
            FROM ImportProfile
            GROUP BY (LTRIM(RTRIM(ISNULL(UserName, '')))) 
            HAVING COUNT(UserName) > 1) 

	Insert into ImportError (ID, ErrorLevel, ColumnName, ErrorMessage) 
	 SELECT
                ID,
                'Error',
                'ConsumerNo',
                'ConsumerNo is duplicated in the Excel'
            FROM ImportProfile
            WHERE (LTRIM(RTRIM(ISNULL(ConsumerNo, '')))) IN (SELECT
               (LTRIM(RTRIM(ISNULL(ConsumerNo, '')))) 
            FROM ImportProfile
            GROUP BY (LTRIM(RTRIM(ISNULL(ConsumerNo, '')))) 
            HAVING COUNT(ConsumerNo) > 1) 

	Insert into ImportError (ID, ErrorLevel, ColumnName, ErrorMessage) 
	 SELECT
                ImportProfile.ID,
                'Error',
                'UserName',
                'UserName ' + ImportProfile.UserName + ' already exists in the database.'
            FROM ImportProfile
			JOIN tblLogin ON tblLogin.UserName = ImportProfile.UserName

	Insert into ImportError (ID, ErrorLevel, ColumnName, ErrorMessage) 
	 SELECT
                ImportProfile.ID,
                'Error',
                'ConsumerNo',
                'ConsumerNo ' + ImportProfile.ConsumerNo + ' already exists in the database.'
            FROM ImportProfile
            JOIN tblConsumer ON tblConsumer.ConsumerNo = ImportProfile.ConsumerNo

------------------------------------- END CHECK MANDATORY VALIDATIONS ---------------------------------------------  
END