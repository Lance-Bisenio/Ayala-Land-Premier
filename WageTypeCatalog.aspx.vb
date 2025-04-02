Imports System.Data
Imports HelperClass
Partial Class WageTypeCatalog
    Inherits System.Web.UI.Page
	Dim vSQL As String = ""

	Private Sub WageTypeCatalog_Load(sender As Object, e As EventArgs) Handles Me.Load
		If Not IsPostBack Then
			GetEmployeeList()
		End If
	End Sub

	Private Sub GetEmployeeList()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""

		c.ConnectionString = ConnStr

		vSQL = "select Code, Name, IsTaxable, IsEarning, Active, IsRecurring from tblPayElements order by IsRecurring, Name"

		'Response.Write(vSQL)

		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblWageType")
		tblWageType.DataSource = ds.Tables("tblWageType")
		tblWageType.DataBind()

		da.Dispose()
		ds.Dispose()
	End Sub
	Private Sub tblEmployees_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblWageType.PageIndexChanging
		tblWageType.PageIndex = e.NewPageIndex
		tblWageType.SelectedIndex = -1

		GetEmployeeList()

	End Sub

	'Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
	'	GetEmployeeList()
	'End Sub
End Class
