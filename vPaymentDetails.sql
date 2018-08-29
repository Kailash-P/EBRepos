alter VIEW dbo.vPaymentDetails AS
SELECT
	tblBillHistory.ID,
	tblConsumer.ConsumerNo,
	tblConsumer.ConsumerName,
	tblLogin.Address,
	tblBillHistory.BillAmount,
	tblBillHistory.UnitsConsumed,
	tblBillHistory.Paid,
	tblBillHistory.BillDate,
	tblLogin.ID AS Login_ID,
	tblConsumer.ID AS Consumer_ID
FROM
tblBillHistory
JOIN tblConsumer ON tblConsumer.ID = tblBillHistory.Consumer_ID
JOIN tblLogin ON tblLogin.ID = tblConsumer.Login_ID