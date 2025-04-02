Imports HelperClass
Imports System.Data

Partial Class JobOrderTask
    Inherits System.Web.UI.Page
    Dim vSQL As String
    Public vRecordData As String


    Private Sub JobOrderTask_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            vSQL = "select ClientCd, ClientName from tblClient order by ClientName"
            BuildCombo(vSQL, DDLClientList)

            vSQL = "select Status_Cd, Descr from tblRefStatus where GroupName='JO' order by Descr"
            BuildCombo(vSQL, DDLPOStatusList)

            GetPOList()
        End If
    End Sub


    Private Sub BtnAddNew_Click(sender As Object, e As EventArgs) Handles BtnAddNew.Click

        TxtJONO.Text = Format(Now, "MMddyyyyhhmm")


        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
              "$('#myModal').modal('show'); ", True)

        GetPOList()

    End Sub

    Private Sub GetPOList()

        Dim vFilter As String = ""

        Dim c As New SqlClient.SqlConnection(ConnStr)
        Dim cm As New SqlClient.SqlCommand
        Dim rs As SqlClient.SqlDataReader

        Dim cmSub As New SqlClient.SqlCommand

        Dim iCtr As Integer = 1

        Dim vColor As String = ""
        Dim vClass As String = ""

        Try
            c.Open()
        Catch ex As SqlClient.SqlException
            ' = "alert('Error occurred while trying to connect to database. Error code 101; Error is: " & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');"
            c.Dispose()
            cm.Dispose()
            Exit Sub
        End Try

        cm.Connection = c

        'If txtDateFrom.Text.Trim <> "" And txtDateTo.Text.Trim <> "" Then
        '    vFilter += " and DateCreated between '" & txtDateFrom.Text.Trim & "' and '" & txtDateTo.Text.Trim & "'"
        'End If

        'If DDLSupplier.SelectedValue <> "All" Then
        '    vFilter += " and Supp_Cd='" & DDLSupplier.SelectedValue & "'"
        'End If

        'If DDLPOStatus.SelectedValue <> "All" Then
        '    vFilter += " and PONO_Status='" & DDLPOStatus.SelectedValue & "'"
        'End If

        'If TxtPONumber.Text.Trim <> "" Then
        '    vFilter += " and PONO='" & TxtPONumber.Text.Trim & "'"
        'End If

        vRecordData = ""

        vSQL = "select TranId, JONO, Client_Cd,JONO_Status,Remarks,Target_DelDate,CreatedBy,DateCreated,ApprovedBy,DateApproved, " _
            & "(select ClientName from tblClient a where a.ClientCd=c.Client_Cd) As ClientName, " _
            & "(select Descr from tblRefStatus b where b.Status_Cd=c.JONO_Status And b.GroupName='JO') as POStatus " _
            & "from tblJOheader c where JONO is not null "

        'and DateCreated between " _
        '& "'" & txtDateFrom.Text.Trim & "' and '" & txtDateTo.Text.Trim & "' " & vFilter

        'Response.Write(vSQL)

        'Exit Sub

        cm.CommandText = vSQL

        Try
            rs = cm.ExecuteReader
            Do While rs.Read
                vRecordData += "<tr>" _
                    & "<td><input type='button' class='btn btn-primary btn-sm' value='Select' " _
                    & " onclick='ModifyJoHeader(""" & rs("TranId") & """,""" & rs("JONO") & """,""" & rs("Client_Cd") & """)'></td>" _
                    & "<td>" & iCtr & "</td>" _
                    & "<td>" & rs("JONO") & "</td>" _
                    & "<td>" & rs("ClientName") & "</td>" _
                    & "<td>" & rs("Target_DelDate") & "</td>" _
                    & "<td>-</td>" _
                    & "<td>-</td>" _
                    & "<td>-</td>" _
                    & "<td>-</td>" _
                    & "<td>-</td>" _
                & "<td>" & rs("Remarks") & "</td>" _
                    & "<td>" & rs("POStatus") & "</td>" _
                    & "<td>" & rs("CreatedBy") & "</td>" _
                    & "<td>" & rs("DateCreated") & "</td>"
                '& "<td>" & rs("TotalQty") & "</td>" _
                '& "<td></td>"


                vRecordData += "</tr>"

                vColor = ""

                iCtr += 1
            Loop
            rs.Close()

        Catch ex As SqlClient.SqlException
            'vScript = "alert('Error occurred while trying to retrieve Job Order Info. " _
            '    & "Error code 102; Error Is: " _
            '    & ex.Message.Replace(vbCrLf, "").Replace("'", "") & "');"
        End Try

        c.Close()
        c.Dispose()
        cm.Dispose()
        cmSub.Dispose()

        Session("vRecordData") = vRecordData
        h_TranId.Value = ""


        Session("JOHeader") = vRecordData

    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click

        vSQL = "insert into tblJOheader (JONO,Client_Cd,JONO_Status,Remarks,Target_DelDate,CreatedBy,DateCreated) values (" _
            & "'" & TxtJONO.Text & "','" & DDLClientList.SelectedValue & "','" & DDLPOStatusList.SelectedValue & "','" & TxtRemarks.Text.Trim & "'," _
            & "'" & TxtTargetDelDate.Text.Trim & "','" & Session("uid") & "','" & Now & "')"

        CreateRecords(vSQL)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "Script",
                "alert('Successfully saved'); " _
                & " ", True)

        TxtJONO.Text = ""
        TxtRemarks.Text = ""
        TxtTargetDelDate.Text = ""
        DDLPOStatusList.SelectedValue = "PRE-PLAN"

    End Sub

    Private Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        GetPOList()

    End Sub

    Private Sub BtnJODetails_Click(sender As Object, e As EventArgs) Handles BtnJODetails.Click
        DivJODetails.Visible = True
        DivJOAttachment.Visible = False
        DivEstimate.Visible = False
        DivCategory.Visible = False

        BtnJODetails.CssClass = "nav-link active"
        BtnAttachment.CssClass = "nav-link"
        BtnEstimate.CssClass = "nav-link"
        BtnCategories.CssClass = "nav-link"

        vRecordData = Session("JOHeader")
    End Sub

    Private Sub BtnAttachment_Click(sender As Object, e As EventArgs) Handles BtnAttachment.Click
        DivJODetails.Visible = False
        DivJOAttachment.Visible = True
        DivEstimate.Visible = False
        DivCategory.Visible = False

        BtnJODetails.CssClass = "nav-link"
        BtnAttachment.CssClass = "nav-link active"
        BtnEstimate.CssClass = "nav-link"
        BtnCategories.CssClass = "nav-link"
        vRecordData = Session("JOHeader")
    End Sub

    Private Sub BtnEstimate_Click(sender As Object, e As EventArgs) Handles BtnEstimate.Click
        DivJODetails.Visible = False
        DivJOAttachment.Visible = False
        DivEstimate.Visible = True
        DivCategory.Visible = False

        BtnJODetails.CssClass = "nav-link"
        BtnAttachment.CssClass = "nav-link"
        BtnEstimate.CssClass = "nav-link active"
        BtnCategories.CssClass = "nav-link"

        vRecordData = Session("JOHeader")
    End Sub

    Private Sub BtnCategories_Click(sender As Object, e As EventArgs) Handles BtnCategories.Click
        DivJODetails.Visible = False
        DivJOAttachment.Visible = False
        DivEstimate.Visible = False
        DivCategory.Visible = True

        BtnJODetails.CssClass = "nav-link"
        BtnAttachment.CssClass = "nav-link"
        BtnEstimate.CssClass = "nav-link"
        BtnCategories.CssClass = "nav-link active"

        vRecordData = Session("JOHeader")
    End Sub


End Class
