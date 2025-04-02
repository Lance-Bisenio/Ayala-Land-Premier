Imports System.Data
Imports HelperClass

Partial Class DocumentType
	Inherits System.Web.UI.Page
	Dim vSQL As String = ""

	Private Sub DocumentType_Load(sender As Object, e As EventArgs) Handles Me.Load

		If Session("uid") = "" Then
			Response.Redirect("~/Login")
			Exit Sub
		End If

		If Not IsPostBack Then
			Dim CanViewApp As Integer = 0

			vSQL = "select Count(User_Id) as Ctr from rights_list where Property_Value='2000' and User_Id='" & Session("uid") & "'"

			CanViewApp = GetRef(vSQL, "")

			If CanViewApp = 0 Then
				'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('You are not authorized to view this page.');", True)
				Response.Redirect("~/AccessDenied")
			End If

			GetDocumentType()
		End If
	End Sub

	Private Sub GetDocumentType()
		Dim c As New SqlClient.SqlConnection
		Dim da As SqlClient.SqlDataAdapter
		Dim ds As New DataSet
		Dim vFilter As String = ""
		Dim vTableName As String = ""
		Dim vSQL As String = ""

		c.ConnectionString = ConnStr
		vSQL = "select Type_Cd, Descr from dm_document_type order by Descr"
		'Response.Write(vSQL)

		da = New SqlClient.SqlDataAdapter(vSQL, c)

		da.Fill(ds, "tblDocumentType")
		tblDocumentType.DataSource = ds.Tables("tblDocumentType")
		tblDocumentType.DataBind()

		da.Dispose()
		ds.Dispose()
	End Sub

	Protected Sub tblDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblDocumentType.SelectedIndexChanged
		Session("TranId") = tblDocumentType.SelectedRow.Cells(1).Text
		Session("ActionType") = "Edit"
		txtCode.Value = tblDocumentType.SelectedRow.Cells(1).Text
		txtDescr.Value = tblDocumentType.SelectedRow.Cells(2).Text


		'Session("tblDocKeyID") = tblDocumentType.SelectedRow.Cells(1).Text
	End Sub

	Private Sub tblDocumentType_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles tblDocumentType.PageIndexChanging
		tblDocumentType.PageIndex = e.NewPageIndex
		tblDocumentType.SelectedIndex = -1
		GetDocumentType()
		txtCode.Value = ""
		txtDescr.Value = ""
	End Sub

	Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
		tblDocumentType.SelectedIndex = -1
		GetDocumentType()
		txtCode.Value = ""
		txtDescr.Value = ""
	End Sub

	Private Sub BtnSave_ServerClick(sender As Object, e As EventArgs) Handles BtnSave.ServerClick
		If TxtNewDesc.Value.Trim <> "" Then
			vSQL = "insert into dm_document_type values ('" & TxtNewDesc.Value & "')"
			CreateRecords(vSQL)
			tblDocumentType.SelectedIndex = -1
			GetDocumentType()
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Saved successfully');", True)
		Else
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Failed to complete. \n\nPlease enter document type description.');", True)
		End If
	End Sub

	Private Sub BtnUpdate_ServerClick(sender As Object, e As EventArgs) Handles BtnUpdate.ServerClick
		If txtDescr.Value.Trim <> "" Then
			vSQL = "update dm_document_type set Descr='" & txtDescr.Value & "' where Type_Cd=" & tblDocumentType.SelectedRow.Cells(1).Text
			CreateRecords(vSQL)
			tblDocumentType.SelectedIndex = -1
			GetDocumentType()
			txtCode.Value = ""
			txtDescr.Value = ""
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected item has been updated');", True)
		Else
			ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('Failed to complete. \n\nPlease enter document type description.');", True)
		End If
	End Sub

	Private Sub BtnDelAction_ServerClick(sender As Object, e As EventArgs) Handles BtnDelAction.ServerClick
		vSQL = "delete from dm_document_type where Type_Cd=" & tblDocumentType.SelectedRow.Cells(1).Text
		CreateRecords(vSQL)
		tblDocumentType.SelectedIndex = -1
		GetDocumentType()
		txtCode.Value = ""
		txtDescr.Value = ""
		ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alert('The selected item has been deleted');", True)
	End Sub
End Class
