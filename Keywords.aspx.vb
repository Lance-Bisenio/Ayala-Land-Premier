Imports HelperClass
Imports System.Data

Partial Class Keywords
    Inherits System.Web.UI.Page
    Public vScript As String = ""
    Public KeywordsList As String = ""
    Public ProccessKeywords As String = ""
    Dim vSQL As String = ""

    Private Sub Keywords_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DataRefresh()
        End If
    End Sub

    Private Sub DataRefresh()
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim da As SqlClient.SqlDataAdapter
        Dim ds As New DataSet

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            Response.Write("Error occurred while trying to connect to database. Error is: " _
               & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "")
            c.Dispose()
            Exit Sub
        End Try


        da = New SqlClient.SqlDataAdapter("SELECT * FROM tblKeywords order by Descr", c)
        da.Fill(ds, "keyword")
        tblkeywords.DataSource = ds.Tables("keyword")
        tblkeywords.DataBind()

        da.Dispose()
        ds.Dispose()
        c.Dispose()
    End Sub

    Protected Sub tblkeywords_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles tblkeywords.PageIndexChanging
        tblkeywords.PageIndex = e.NewPageIndex
        DataRefresh()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        Session("KeywordsKey") = ""
        txtDescription.Text = ""
        DDLDataTypeList.SelectedValue = ""

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "$('#myModal').modal('show'); ", True)
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        DataRefresh()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand

        If Session("KeywordsKey") = "" Then

            EventLog(Session("uid"), Request.ServerVariables("REMOTE_ADDR"), "ADD", "", "", "Keyword Item " & "-" _
                     & CleanVar(txtDescription.Text), "Keyword")

            cm.CommandText = "INSERT INTO tblKeywords " _
                & "(Descr,Data_Type,Encoded_By,Date_Encoded)VALUES('" _
                & CleanVar(txtDescription.Text) & "','" & DDLDataTypeList.SelectedValue & "','" _
                & Session("uid") & "','" & Format(Now, "yyyy/MM/dd") & "')"
        Else
            EventLog(Session("uid"), Request.ServerVariables("REMOTE_ADDR"), "EDIT", "", "", "Keyword Item " _
                     & Session("vline") & "-" & CleanVar(txtDescription.Text), "Keyword")

            cm.CommandText = "UPDATE tblKeywords SET " _
                & "Descr='" & CleanVar(txtDescription.Text) & "', " _
                & "Data_Type='" & DDLDataTypeList.SelectedValue & "' " _
                & "WHERE Keyword_Id='" & Session("KeywordsKey") & "'"
        End If

        cm.Connection = c
        c.Open()
        cm.ExecuteNonQuery()

        c.Close()
        cm.Dispose()

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "alert('Successfully saved.'); ", True)

        Session("KeywordsKey") = ""
        DataRefresh()

    End Sub

    Protected Sub tblkeywords_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tblkeywords.SelectedIndexChanged

        Session("KeywordsKey") = tblkeywords.SelectedRow.Cells(0).Text
        txtDescription.Text = tblkeywords.SelectedRow.Cells(1).Text
        DDLDataTypeList.SelectedValue = tblkeywords.SelectedRow.Cells(2).Text

    End Sub

    Protected Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "$('#myModal').modal('show'); ", True)
    End Sub

    Private Sub BtnCreateProcess_Click(sender As Object, e As EventArgs) Handles BtnCreateProcess.Click

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "$('#ModalPrcess').modal('show'); ", True)

        vSQL = "select ProcessId, Descr from tblProcess order by Descr"
        BuildCombo(vSQL, DDLProcessList)
        DDLProcessList.SelectedValue = Request.Item("vCompCd")

        BuildKeyworks("")
    End Sub

    Private Sub BuildKeyworks(IsSave As String)
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader


        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception

            c.Close()
            cm.Dispose()
            c.Dispose()

        End Try


        vSQL = "select Keyword_Id, Descr from tblKeywords order by Descr"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            KeywordsList += "<div class='col-md-12'>" _
                & "<Label Class='form-check-label'>" _
                & "<input type='checkbox' Class='form-check-input' " _
                    & "id='Chk_" & rs("Keyword_Id") & "' " _
                    & "name='Chk_" & rs("Keyword_Id") & "' value='" & rs("Keyword_Id") & "'>" & rs("Descr") _
                & "</label>" _
                & "</div>"


            If IsSave = "SAVE" Then
                If Request.Form("Chk_" & rs("Keyword_Id").ToString) <> "" Then
                    Response.Write("insert " & rs("Keyword_Id") & "<br>")

                    vSQL = "insert into tblProcessProperties (ProcessId, CategoryId, SeqId, KeywordId, CreatedBy, DateCreated) values (" _
                        & "'" & DDLProcessList.SelectedValue & "', " _
                        & "'" & DDLCategory.SelectedValue & "', " _
                        & "0, " _
                        & "'" & rs("Keyword_Id") & "', " _
                        & "'" & Session("uid") & "', " _
                        & "'" & Now & "')"

                    CreateRecords(vSQL)

                End If
            End If



        Loop
        rs.Close()

        c.Close()
        cm.Dispose()
        c.Dispose()
    End Sub

    Private Sub BtnSaveKeys_Click(sender As Object, e As EventArgs) Handles BtnSaveKeys.Click
        BuildKeyworks("SAVE")
    End Sub

    Private Sub BuildProcessKeywords()
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader


        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception

            c.Close()
            cm.Dispose()
            c.Dispose()

        End Try


        vSQL = "select ProcessId, Descr from tblProcess order by Descr"

        cm.CommandText = vSQL
        rs = cm.ExecuteReader
        Do While rs.Read
            ProccessKeywords += "<tr>" _
                & "<td>" & rs("Descr") & "</td>"

            BuildProcessKeywordsList(rs("ProcessId"))

            ProccessKeywords += "</tr>"
        Loop
        rs.Close()

        c.Close()
        cm.Dispose()
        c.Dispose()
    End Sub

    Private Sub BuildProcessKeywordsList(ProcessId As Integer)
        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader


        Try
            c.Open()
            cm.Connection = c
        Catch ex As Exception

            c.Close()
            cm.Dispose()
            c.Dispose()

        End Try


        vSQL = "select KeywordId, " _
            & "(select Descr from tblKeywords where KeywordId=Keyword_Id) as KeyDescr, " _
            & "(select Data_Type from tblKeywords where KeywordId=Keyword_Id) as KeyDescrType " _
            & "from tblProcessProperties where ProcessId=" & ProcessId

        cm.CommandText = vSQL
        rs = cm.ExecuteReader

        ProccessKeywords += "<td>"
        Do While rs.Read

            ProccessKeywords += rs("KeyDescr") & " (" & rs("KeyDescrType") & ")" & "<br>"

        Loop
        ProccessKeywords += "</td><td><input type='button' class='btn btn-primary btn-sm' value='Select'></td>"

        rs.Close()

        c.Close()
        cm.Dispose()
        c.Dispose()
    End Sub

    Private Sub BtnReloadProcess_Click(sender As Object, e As EventArgs) Handles BtnReloadProcess.Click
        BuildProcessKeywords()
    End Sub

    Private Sub BtnEditProcessKeys_Click(sender As Object, e As EventArgs) Handles BtnEditProcessKeys.Click

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "$('#ModalPrcess').modal('show'); ", True)

        vSQL = "select ProcessId, Descr from tblProcess order by Descr"
        BuildCombo(vSQL, DDLProcessList)
        DDLProcessList.SelectedValue = Request.Item("vCompCd")

        BuildKeyworks("")

    End Sub
End Class
