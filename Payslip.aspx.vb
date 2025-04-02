Imports System.Data
Imports HelperClass
Partial Class Payslip
    Inherits System.Web.UI.Page
    Dim vSQL As String = ""
    Public Earnings As String = ""
	Public Deduction As String = ""
	Public EarningsHisto As String = ""
	Public DeductionHisto As String = ""

	Private Sub Payslip_Load(sender As Object, e As EventArgs) Handles Me.Load
		If Session("EmpId") = "" Then
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Session expired.'); window.close();", True)
		End If

		If Not IsPostBack Then
			'batch=8212020174728&pDate=08/15/2020&empcd=2019-0170

			Dim PrivateToken As String = getEncryptedCode256("@neTw03456Sev3n" & Request.Item("Token"))
			GetEmployeeInfo(PrivateToken)

			'Response.Write(PrivateToken)

		End If
	End Sub
	Public Function getEncryptedCode256(ByVal inputString As String) As String

		Dim Hash As Byte() = New System.Security.Cryptography.SHA256Managed().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(inputString))
		Dim outputString As New System.Text.StringBuilder()

		For i As Integer = 0 To Hash.Length - 1
			outputString.Append(Hash(i).ToString("X2"))
		Next

		Return outputString.ToString()

	End Function
	Private Sub GetEmployeeInfo(PrivateToken As String)

		Dim c As New SqlClient.SqlConnection
		Dim cm As New SqlClient.SqlCommand
		Dim rs As SqlClient.SqlDataReader
		Dim BatchNo As String = ""
		Dim Paydate As Date
		Dim EmpCd As String = ""
		Dim TotalEarningsHisto As Decimal = 0
		Dim TotalDeductionHisto As Decimal = 0

		Dim EarningTempAmt As Decimal = 0
		Dim EarningHistoAmt As Decimal = 0
		Dim EarningHistoTaxableAmt As Decimal = 0
		Dim EarningHistoTempAmt As String = ""
		Dim Ctr As Integer = 0
		Dim IsRemove As Integer = 0

		c.ConnectionString = ConnStr

		Try
			c.Open()
			cm.Connection = c
		Catch ex As SqlClient.SqlException
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Database connection error.');", True)
			Exit Sub
		End Try

		vSQL = "select BatchNo,EmpCode,PayDate,IsActive,CreatedBy,DateCreated " _
			& "from tblPayslipToken " _
			& "where PrivateToken='" & PrivateToken & "'"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader

		If rs.Read Then
			BatchNo = rs("BatchNo")
			Paydate = rs("PayDate")
			EmpCd = rs("EmpCode")
		End If
		rs.Close()

		vSQL = "select EmployeeCode, FullName, MonthlyRate, AddressRegistered, EmploymentTypeId, PayGroupId, TINNo, LocationId, " _
			& "(select Name from tblLocations where id=LocationId) as EmpLocation, " _
			& "(select Name from tblPayGroup where id=PayGroupId) as PayGroup, " _
			& "(select TaxPercent*100 from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as TaxPercent, " _
			& "(select IsNonVat from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as IsNonVat, " _
			& "(select VatPercent from tblEmployeeTaxRef a where a.EmployeeCode=b.EmployeeCode) as VatPercent, " _
			& "(select Name from tblDivision a where DivisionId=id) as DivName " _
			& "from tblEmployees b where EmployeeCode='" & EmpCd & "'"

		cm.CommandText = vSQL
		rs = cm.ExecuteReader

		Do While rs.Read
			LblVendorCode.InnerText = rs("EmployeeCode")
			LblName.InnerText = rs("FullName")
			LblTin.InnerText = IIf(Not IsDBNull(rs("TINNo")), rs("TINNo"), "")
			LblVatReg.InnerText = IIf(Not IsDBNull(rs("IsNonVat")), rs("IsNonVat"), "")
			LblAddress.InnerText = IIf(Not IsDBNull(rs("AddressRegistered")), rs("AddressRegistered"), "")
			LblStatus.InnerText = IIf(Not IsDBNull(rs("PayGroup")), rs("PayGroup"), "")
			LblDiv.InnerText = IIf(Not IsDBNull(rs("EmpLocation")), rs("EmpLocation"), "")
			LblGroup.InnerText = IIf(Not IsDBNull(rs("DivName")), rs("DivName"), "")
			LblMRate.InnerText = Format(CDec(rs("MonthlyRate")), "#,###,##0.00")
			LblWHTax.InnerText = rs("TaxPercent") & "%"
			LblPayDate.InnerText = Format(CDate(Paydate), "MM/dd/yyyy")
		Loop
		rs.Close()

		vSQL = "select TotalEarning, TotalDeduction, TotalTaxable, InputVAT, GrandTotal, WHTax, NetPay, " _
			& "(select Amount from tblPayInstruction a where a.BatchNo=b.BatchNo and a.EmpCode='" & Session("EmpId") & "' and PayElement='BASIC' ) as BasicPay " _
			& "from tblPayrollSummary b " _
			& "where EmpCode='" & Session("EmpId") & "' and BatchNo='" & BatchNo & "'"

		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		If rs.Read Then
			LblBasic.InnerText = IIf(Not IsDBNull(rs("BasicPay")), Format(CDec(rs("BasicPay")), "#,###,##0.00"), "0.00") 'Format(CDec(rs("BasicPay")), "#,###,##0.00")
			LblEWT.InnerText = Format(CDec(rs("WHTax")), "#,###,##0.00")
			LblTtlTaxable.InnerText = Format(CDec(rs("TotalEarning")), "#,###,##0.00")
			LblInputVat.InnerText = Format(CDec(rs("InputVAT")), "#,###,##0.00")
			LblGrandTtl.InnerText = Format(CDec(rs("GrandTotal")), "#,###,##0.00")
			LblTtlDeduc.InnerText = Format(CDec(rs("TotalDeduction") + CDec(rs("WHTax"))), "#,###,##0.00")
			LblNetPay.InnerText = Format(CDec(rs("NetPay")), "#,###,##0.00")

			LblBVATable.InnerText = Format(CDec(rs("TotalEarning")), "#,###,##0.00")
			LblBVatAmt.InnerText = Format(CDec(rs("InputVAT")), "#,###,##0.00")
			LblBGrandTtl.InnerText = Format(CDec(rs("GrandTotal")), "#,###,##0.00")
			LblBEWT.InnerText = "(" & Format(CDec(rs("WHTax")), "#,###,##0.00") & ")"
			LblBNetPay.InnerText = Format(CDec(rs("NetPay")), "#,###,##0.00")
		End If
		rs.Close()

		vSQL = "select sum(Amount) as HistoAmt from tblPayInstruction " _
				& "where BatchNo in (select BatchNo from tblPayInstructionHeader where DatePublish is not null and BatchNo != '" & BatchNo & "' and PayDate < '" & Format(CDate(Paydate), "MM/dd/yyyy") & "') and " _
					& "EmpCode='" & Session("EmpId") & "' and " _
					& "PayElement='BASIC' and " _
					& "year(DateCreated)='" & Format(Now, "yyyy") & "' and Amount is not null"
		EarningHistoTempAmt = GetRef(vSQL, 0)


		If EarningHistoTempAmt.ToString.Trim <> "null" Then
			LblBasicHisto.InnerText = Format(CDec(EarningHistoTempAmt), "#,###,##0.00")
		Else
			LblBasicHisto.InnerText = "0.00"
		End If



		'vSQL = "select Amount,  " _
		'	& "(select Name from tblPayElements where Code=PayElement) as Descr from tblPayInstruction " _
		'	& "where EmpCode='" & Session("EmpId") & "' and BatchNo='" & BatchNo & "' and " _
		'	& "PayElement in (select Code from tblPayElements where IsEarning=0 and active=1 and Code<>'BASIC') " 
		'" & Format(CDec(rs("Amount")), "#,###,##0.00") & "

		vSQL = "select id, Code, Name from tblPayElements where IsEarning=0 and Active=1 and Code not in ('BASIC') order by Name"

		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		Do While rs.Read

			vSQL = "select Amount from tblPayInstruction " _
				& "where BatchNo='" & BatchNo & "' and EmpCode='" & Session("EmpId") & "' and PayElement='" & rs("Code") & "'"
			EarningTempAmt = GetRef(vSQL, 0)

			vSQL = "select sum(Amount) as HistoAmt from tblPayInstruction " _
				& "where BatchNo in (select BatchNo from tblPayInstructionHeader where DatePublish is not null and " _
						& "BatchNo != '" & BatchNo & "' and PayDate < '" & Format(CDate(Paydate), "MM/dd/yyyy") & "') and " _
					& "EmpCode='" & Session("EmpId") & "' and " _
					& "PayElement='" & rs("Code") & "' and " _
					& "year(DateCreated)='" & Format(Now, "yyyy") & "' and Amount is not null"
			EarningHistoTempAmt = GetRef(vSQL, 0)

			If EarningHistoTempAmt.ToString.Trim <> "null" Then
				EarningHistoAmt = EarningHistoTempAmt
			End If

			If EarningTempAmt > 1 Or EarningHistoAmt > 0 Then
				Earnings += "<div class='row Pad4'>" _
				& "<div Class='col-sm-6 text-left'><small>" & rs("Name") & "</small></div>" _
				& "<div class='col-sm-3 text-right'><small><label id='Label7' class='Pad4'>" &
					Format(CDec(EarningTempAmt), "#,###,##0.00") & "</label></small></div>" _
				& "<div class='col-sm-3 text-right'>"

				Earnings += "<div class='row'>" _
								& "<div class='col-sm-10 text-right'><small><label id='Label8'>" _
								& Format(CDec(EarningHistoAmt), "#,###,##0.00") & "</label></small></div>" _
							& "</div>" _
						& "</div>" _
					& "</div>"
				EarningHistoTaxableAmt += EarningHistoAmt
			End If
			EarningHistoAmt = 0
			EarningTempAmt = 0
		Loop

		EarningTempAmt = 0
		rs.Close()


		' GET EARNINGS HISTORICAL PAYROLL 
		' ----------------------------------------------------------------------------------------------------------------- 
		vSQL = "select Allow, AdvanceOperatingFund, AOE, TelephoneAllow, TranspoAllow, AdvanceOperatingFund, Incentive " _
			& "from tblPayrollHistorical " _
			& "where EmpCode='" & EmpCd & "' and Year(DateCreated)='" & Format(Now, "yyyy") & "'"

		'Response.Write(vSQL)
		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		Do While rs.Read
			If Not IsDBNull(rs("Allow")) Then
				TotalEarningsHisto += rs("Allow")
			End If
			If Not IsDBNull(rs("AdvanceOperatingFund")) Then
				TotalEarningsHisto += rs("AdvanceOperatingFund")
			End If
			If Not IsDBNull(rs("AOE")) Then
				TotalEarningsHisto += rs("AOE")
			End If
			If Not IsDBNull(rs("TelephoneAllow")) Then
				TotalEarningsHisto += rs("TelephoneAllow")
			End If
			If Not IsDBNull(rs("TranspoAllow")) Then
				TotalEarningsHisto += rs("TranspoAllow")
			End If
			If Not IsDBNull(rs("AdvanceOperatingFund")) Then
				TotalEarningsHisto += rs("AdvanceOperatingFund")
			End If
			If Not IsDBNull(rs("Incentive")) Then
				TotalEarningsHisto += rs("Incentive")
			End If
		Loop
		rs.Close()

		EarningsHisto += "<div class='row Pad4'>" _
				& "<div Class='col-sm-6 text-left'><small>Allowance (Historical)</small></div>" _
				& "<div class='col-sm-3 text-right'><small><label id='Label7' class='Pad4'>-</label></small></div>" _
				& "<div class='col-sm-3 text-right'>" _
					& "<div class='row'>" _
						& "<div class='col-sm-10 text-right'><small><label id='Label8'>" & Format(CDec(TotalEarningsHisto), "#,###,##0.00") & "</label></small></div>" _
					& "</div>" _
				& "</div>" _
			& "</div>"


		LblTtlTaxableHisto.InnerText = Format(CDec(LblBasicHisto.InnerText) + TotalEarningsHisto + EarningHistoTaxableAmt, "#,###,##0.00")

		' ----------------------------------------------------------------------------------------
		' INPUT VAT This Period
		vSQL = "Select InputVat from tblPayrollHistorical where EmpCode='" & EmpCd & "' and Year(DateCreated)='" & Format(Now, "yyyy") & "'"
		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		EarningTempAmt = 0
		Do While rs.Read
			If rs("InputVat").ToString.Trim <> "" Then
				EarningTempAmt += CDec(rs("InputVat").ToString.Trim)
			End If
		Loop
		rs.Close()

		' ----------------------------------------------------------------------------------------
		' INPUT VAT HISTORICAL
		EarningHistoAmt = 0
		vSQL = "select sum(InputVAT) VatAmnt from tblPayrollSummary " _
				& "where BatchNo in (select BatchNo from tblPayInstructionHeader where DatePublish is not null and " _
						& "BatchNo != '" & BatchNo & "' and PayDate < '" & Format(CDate(Paydate), "MM/dd/yyyy") & "') and " _
				& "EmpCode='" & Session("EmpId") & "' "

		EarningHistoTempAmt = GetRef(vSQL, 0)

		If EarningHistoTempAmt.ToString.Trim <> "null" Then
			EarningHistoAmt = EarningHistoTempAmt
		End If

		EarningTempAmt += EarningHistoAmt

		LblInputVatHisto.InnerText = Format(EarningTempAmt, "#,###,##0.00")

		LblGrandTotalEarningHisto.InnerText = Format(TotalEarningsHisto + CDec(LblInputVatHisto.InnerText) + CDec(EarningHistoAmt), "#,###,##0.00")





		' ----------------------------------------------------------------------------------------
		' WHTAX AMOUNT HISTO 

		vSQL = "Select WHT from tblPayrollHistorical where EmpCode='" & EmpCd & "' and Year(DateCreated)='" & Format(Now, "yyyy") & "'"
		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		EarningTempAmt = 0
		Do While rs.Read
			If rs("WHT").ToString.Trim <> "" Then
				EarningTempAmt += CDec(rs("WHT").ToString.Trim)
			End If
		Loop
		rs.Close()

		'EarningTempAmt = 0
		EarningHistoAmt = 0
		EarningHistoTempAmt = ""

		vSQL = "select sum(WHTax) WHTax from tblPayrollSummary " _
				& "where BatchNo in (select BatchNo from tblPayInstructionHeader where DatePublish is not null and " _
						& "BatchNo != '" & BatchNo & "' and PayDate < '" & Format(CDate(Paydate), "MM/dd/yyyy") & "') and " _
				& "EmpCode='" & Session("EmpId") & "' "

		EarningHistoTempAmt = GetRef(vSQL, 0)

		'If EarningHistoTempAmt.ToString.Trim <> "null" Or EarningHistoTempAmt.ToString.Trim <> "" Then
		'	EarningHistoAmt = EarningHistoTempAmt
		'End If

		Try
			EarningHistoAmt = EarningHistoTempAmt
		Catch ex As Exception

		End Try

		'EarningTempAmt += EarningHistoAmt

		LblEWTHisto.InnerText = Format(EarningTempAmt + EarningHistoAmt, "#,###,##0.00")



		EarningTempAmt = 0
		EarningHistoAmt = 0

		vSQL = "select id, Code, Name from tblPayElements where IsEarning=1 and Active=1 order by Name"
		cm.CommandText = vSQL
		rs = cm.ExecuteReader
		Do While rs.Read

			' GET LIST OF DEDUCTION BASED ON THE PAYROLL RUN
			' -----------------------------------------------------------------------------------------------------------------
			vSQL = "select Amount from tblPayInstruction " _
				& "where BatchNo='" & BatchNo & "' and EmpCode='" & Session("EmpId") & "' and PayElement='" & rs("Code") & "'"
			EarningTempAmt = GetRef(vSQL, 0)


			' GET DEDUCTION HISTORICAL 
			' -----------------------------------------------------------------------------------------------------------------
			vSQL = "select sum(Amount) as HistoAmt from tblPayInstruction " _
				& "where BatchNo in (select BatchNo from tblPayInstructionHeader where DatePublish is not null and " _
						& "BatchNo != '" & BatchNo & "' and PayDate < '" & Format(CDate(Paydate), "MM/dd/yyyy") & "') and " _
					& "EmpCode='" & Session("EmpId") & "' and " _
					& "PayElement='" & rs("Code") & "' and " _
					& "year(DateCreated)='" & Format(Now, "yyyy") & "' and Amount is not null"
			Try
				EarningHistoTempAmt = GetRef(vSQL, 0)

				If EarningHistoTempAmt.ToString.Trim <> "null" Then
					EarningHistoAmt = EarningHistoTempAmt
				End If
			Catch ex As Exception

			End Try




			If EarningTempAmt > 1 Or EarningHistoAmt > 0 Then
				Deduction += "<div class='row Pad4'>" _
				& "<div Class='col-sm-6 text-left'><small>" & rs("Name") & "</small></div>" _
				& "<div class='col-sm-3 text-right'><small><label id='Label7' class='Pad4'>" &
					Format(CDec(EarningTempAmt), "#,###,##0.00") & "</label></small></div>" _
				& "<div class='col-sm-3 text-right'>"


				Deduction += "<div class='row'>" _
									& "<div class='col-sm-10 text-right'><small><label id='Label8'>" & Format(CDec(EarningHistoAmt), "#,###,##0.00") & "</label></small></div>" _
								& "</div>" _
							& "</div>" _
						& "</div>"
				'EarningHistoTaxableAmt += EarningHistoAmt
			End If
			EarningHistoAmt = 0
			EarningTempAmt = 0
		Loop
		rs.Close()

		'vSQL = "select Amount,  " _
		'	& "(select Name from tblPayElements where Code=PayElement) as Descr from tblPayInstruction " _
		'	& "where EmpCode='" & Session("EmpId") & "' and BatchNo='" & BatchNo & "' and " _
		'	& "PayElement in (select Code from tblPayElements where IsEarning=1 and active=1)"

		'cm.CommandText = vSQL

		'rs = cm.ExecuteReader
		'Do While rs.Read
		'	Deduction += "<div class='row Pad4'>" _
		'		& "<div Class='col-sm-6 text-left'><small>" & rs("Descr") & "</small></div>" _
		'		& "<div class='col-sm-3 text-right'><small><label id='Label7' class='Pad4'>(" & Format(CDec(rs("Amount")), "#,###,##0.00") & ")</label></small></div>" _
		'		& "<div class='col-sm-3 text-right'>" _
		'			& "<div class='row'>" _
		'				& "<div class='col-sm-10 text-right'><small><label id='Label8'>-</label>&nbsp;&nbsp;</small></div>" _
		'			& "</div>" _
		'		& "</div>" _
		'	& "</div>"
		'Loop
		'rs.Close()


		' GET DEDUCTION HISTORICAL PAYROLL 
		' -----------------------------------------------------------------------------------------------------------------
		vSQL = "select Misc, GroupFund, CourierMktg, Telephone " _
			& "from tblPayrollHistorical " _
			& "where EmpCode='" & EmpCd & "' and Year(DateCreated)='" & Format(Now, "yyyy") & "'"

		'Response.Write(vSQL)
		cm.CommandText = vSQL

		rs = cm.ExecuteReader
		Do While rs.Read
			If Not IsDBNull(rs("Misc")) Then
				TotalDeductionHisto += rs("Misc")
			End If
			If Not IsDBNull(rs("GroupFund")) Then
				TotalDeductionHisto += rs("GroupFund")
			End If
			If Not IsDBNull(rs("CourierMktg")) Then
				TotalDeductionHisto += rs("CourierMktg")
			End If
			If Not IsDBNull(rs("Telephone")) Then
				TotalDeductionHisto += rs("Telephone")
			End If
		Loop
		rs.Close()

		DeductionHisto += "<div class='row Pad4'>" _
				& "<div Class='col-sm-6 text-left'><small>Deduction (Historical)</small></div>" _
				& "<div class='col-sm-3 text-right'><small><label id='Label7' class='Pad4'>-</label></small></div>" _
				& "<div class='col-sm-3 text-right'>" _
					& "<div class='row'>" _
						& "<div class='col-sm-10 text-right'><small><label id='Label8'>" & Format(CDec(TotalDeductionHisto), "#,###,##0.00") & "</label></small></div>" _
					& "</div>" _
				& "</div>" _
			& "</div>"

		LblTotalDecucHisto.InnerText = Format(CDec(TotalDeductionHisto) + CDec(LblEWTHisto.InnerText), "#,###,##0.00")

		TotalDeductionHisto = 0

		vSQL = "select sum(GrandTotal) from tblPayrollHistorical where EmpCode='" & EmpCd & "' and Year(DateCreated)='" & Format(Now, "yyyy") & "'"
		Try
			TotalDeductionHisto = GetRef(vSQL, 0)
		Catch ex As Exception

		End Try



		vSQL = "select sum(NetPay) from tblPayrollSummary " _
			& "where EmpCode='" & EmpCd & "' and " _
			& "BatchNo in (select BatchNo from tblPayInstructionHeader " _
				& "where DatePublish is not null and BatchNo != '" & BatchNo & "' and PayDate < '" & Format(Paydate, "MM/dd/yyyy") & "')"

		Try
			TotalDeductionHisto += GetRef(vSQL, 0)
		Catch ex As Exception

		End Try


		LblNetHisto.InnerText = Format(CDec(TotalDeductionHisto), "#,###,##0.00")

		c.Close()
		c.Dispose()
		cm.Dispose()

	End Sub

End Class
