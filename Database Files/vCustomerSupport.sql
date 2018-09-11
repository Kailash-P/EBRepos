ALTER VIEW dbo.vCustomerSupport AS
SELECT
	Email,
	Consumer_ID,
	Concern,	
	IsResolved,
	ResolvedMessage,
	RaisedDate,
	ResolvedDate,
	(CAST(tblConsumer.ConsumerNo AS VARCHAR) + ' - ' + tblConsumer.ConsumerName) AS ConsumerName,
	tblLogin.EmailId AS ConsumerEmail,
	tblCustomerSupport.ID
FROM
tblCustomerSupport
JOIN tblConsumer ON tblConsumer.ID = tblCustomerSupport.Consumer_ID
JOIN tblLogin ON tblLogin.ID = tblConsumer.Login_ID