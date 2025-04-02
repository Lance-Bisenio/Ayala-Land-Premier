Imports System.Data
Namespace HelperClass

    Public Module fis
        Public ConnStr As String = "Data Source=localhost\SQLEXPRESS; Initial Catalog=ALSI; " _
                & "Integrated Security=True; MultipleActiveResultSets=True; Connect Timeout=300 "

        Public ProdConnStr As String = "Data Source=10.58.2.87,2014; Initial Catalog=BPOI_DB; User=sa; Password=jeonsoft_2233#; Connect Timeout=180;"

        Public DTQUOTE As String = "'"

        Public SYSTEMNAME As String = "CSAT"
        Public SITENAME As String = "BPOI-CSAT"
        Public SERVERIP As String = "localhost"
        'Public SERVERIP As String = "192.168.1.210"
        Public Const OneInch As Integer = 1440

        Public Function CleanVar(ByVal pStr As String) As String
            CleanVar = pStr.Replace("'", "''")
        End Function
        Public Function ExtractList(ByRef pChk As WebControls.CheckBoxList) As String
            Dim vList As String = ""
            Dim i As Integer

            For i = 0 To pChk.Items.Count - 1
                If pChk.Items(i).Selected Then
                    vList += pChk.Items(i).Value & ","
                End If
            Next
            If vList <> "" Then
                vList = Mid(vList, 1, Len(vList) - 1)
            End If
            Return vList
        End Function
        Public Sub GetList(ByVal pSql As String, ByVal pList As String, ByRef pChk As System.Web.UI.WebControls.CheckBoxList)
            Dim c As New SqlClient.SqlConnection(ConnStr)
            Dim cm As New SqlClient.SqlCommand(pSql, c)
            Dim rs As SqlClient.SqlDataReader
            Dim vList() As String = pList.Split(",")
            Dim i, j As Integer

            c.Open()
            rs = cm.ExecuteReader
            pChk.Items.Clear()
            Do While rs.Read
                pChk.Items.Add(New ListItem(rs(1), rs(0)))
            Loop
            rs.Close()
            c.Close()
            cm.Dispose()
            c.Dispose()

            For i = 0 To UBound(vList)
                For j = 0 To pChk.Items.Count - 1
                    If pChk.Items(j).Value = vList(i) Then
                        pChk.Items(j).Selected = True
                        Exit For
                    End If
                Next
            Next
        End Sub

        Public Function CanRun(ByVal pList As String, ByVal pId As String) As Boolean
            Dim vList() As String
            Dim vLoop As Integer
            Dim VRun As Boolean = False
            vList = pList.Split(",")

            For vLoop = 0 To UBound(vList)
                If vList(vLoop) = pId Then
                    VRun = True
                    Exit For
                End If
            Next vLoop

            Return VRun
        End Function
        Public Sub EventLog(ByVal pId As String, ByVal pRequesterIP As String,
            ByVal pEvent As String, ByVal pOldValues As String,
            ByVal pNewValues As String, ByVal pRemarks As String, ByVal pModule As String)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''''''''''' Description: Logs user activity //////////////////
            ''''''''''' Parameters:
            '''''''''''     pId = the user who triggered the event
            '''''''''''     pRequesterIP = the IP address that the user used
            ''''''''''' 	pEvent = type type of activity ///////////////
            '''''''''''     pOldValues = the original value prior to change
            '''''''''''     pNewValues = the modified values 
            '''''''''''     pRemarks = any remarks
            '''''''''''     pModule  = the module/program that triggers the event
            Dim c As New SqlClient.SqlConnection(ConnStr)
            Dim cm As New SqlClient.SqlCommand(
                "insert into audit (TranDate,TranTime,User_Id,MachineId,Event,OldValues,NewValues,Remarks,Module) " &
                "values ('" & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','" & Format(Now, "HH:mm:ss") & "','" &
                pId & "','" & pRequesterIP & "','" & pEvent & "','" & pOldValues & "','" &
                pNewValues & "','" & pRemarks & "','" & pModule & "')", c)
            c.Open()
            cm.ExecuteNonQuery()
            c.Close()
            cm.Dispose()
            c.Dispose()
        End Sub
        Public Sub EventLog(ByVal pId As String, ByVal pRequesterIP As String,
           ByVal pEvent As String, ByVal pOldValues As String,
           ByVal pNewValues As String, ByVal pRemarks As String, ByVal pModule As String,
           ByRef c As SqlClient.SqlConnection)

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''''''''''' Description: Logs user activity //////////////////
            ''''''''''' Parameters:
            '''''''''''     pId = the user who triggered the event
            '''''''''''     pRequesterIP = the IP address that the user used
            ''''''''''' 	pEvent = type type of activity ///////////////
            '''''''''''     pOldValues = the original value prior to change
            '''''''''''     pNewValues = the modified values 
            '''''''''''     pRemarks = any remarks
            '''''''''''     pModule  = the module/program that triggers the event
            'Dim c As New SqlClient.SqlConnection(connStr)
            Dim cm As New SqlClient.SqlCommand(
                "insert into audit (TranDate,TranTime,User_Id,MachineId,Event,OldValues,NewValues,Remarks,Module) " &
                "values ('" & Format(Now, "yyyy/MM/dd HH:mm:ss") & "','" & Format(Now, "HH:mm:ss") & "','" &
                pId & "','" & pRequesterIP & "','" & pEvent & "','" & pOldValues & "','" &
                pNewValues & "','" & pRemarks & "','" & pModule & "')", c)


            'c.Open()
            cm.ExecuteNonQuery()
            'c.Close()
            cm.Dispose()
            'c.Dispose()

        End Sub
        Public Function GetRef(ByVal pSQL As String, ByVal pCode As String) As String
            Dim cm As New SqlClient.SqlCommand
            Dim dr As SqlClient.SqlDataReader
            Dim c As New SqlClient.SqlConnection
            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = pSQL
            dr = cm.ExecuteReader
            If dr.Read Then
                GetRef = IIf(IsDBNull(dr(0)), "null", dr(0))
            Else
                GetRef = pCode
            End If
            dr.Close()
            cm.Dispose()
            c.Close()
            c.Dispose()
        End Function
        Public Function GetRef(ByVal pSQL As String, ByVal pCode As String, ByRef c As SqlClient.SqlConnection) As String
            Dim cm As New SqlClient.SqlCommand
            Dim dr As SqlClient.SqlDataReader

            cm.Connection = c
            cm.CommandText = pSQL
            dr = cm.ExecuteReader
            If dr.Read Then
                GetRef = IIf(IsDBNull(dr(0)), "null", dr(0))
            Else
                GetRef = pCode
            End If
            dr.Close()
            cm.Dispose()
        End Function
        Public Function GetRef(ByVal pTable As String, ByVal pReturnField As String, ByVal pField As String, ByVal pCode As String) As String
            Dim cm As New SqlClient.SqlCommand
            Dim dr As SqlClient.SqlDataReader
            Dim c As New SqlClient.SqlConnection
            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = "select " & pReturnField & " from " & pTable & " where " & pField & "='" & pCode & "'"
            dr = cm.ExecuteReader
            If dr.Read Then
                GetRef = IIf(IsDBNull(dr(0)), "null", dr(0))
            Else
                GetRef = pCode
            End If
            dr.Close()
            cm.Dispose()
            c.Close()
            c.Dispose()
        End Function
        Public Function GetRef(ByVal pTable As String, ByVal pReturnField As String,
                            ByVal pField As String, ByVal pCode As String, ByRef c As SqlClient.SqlConnection) As String
            Dim cm As New SqlClient.SqlCommand
            Dim dr As SqlClient.SqlDataReader

            cm.Connection = c
            cm.CommandText = "select " & pReturnField & " from " & pTable & " where " & pField & "='" & pCode & "'"
            dr = cm.ExecuteReader
            If dr.Read Then
                GetRef = IIf(IsDBNull(dr(0)), "null", dr(0))
            Else
                GetRef = pCode
            End If
            dr.Close()
            cm.Dispose()
        End Function

        Public Function ExtractData(ByVal pString As String) As String
            If pString.Length > 0 And pString.IndexOf("=>") > 0 Then
                Return pString.Substring(0, InStr(pString, "=>") - 1)
            Else
                Return ""
            End If
        End Function
        Public Function ExtractDescr(ByVal pString As String) As String
            If pString.Length > 0 And pString.IndexOf("=>") > 0 Then
                Return pString.Substring(InStr(pString, "=>") + 1)
            Else
                Return ""
            End If
        End Function

        Public Function MonthEND(ByVal p_date As Date) As Date
            Dim vSubtract As Integer = Day(DateAdd(DateInterval.Day, 4, CDate(Year(p_date) & "/" & Month(p_date) & "/28"))) * -1
            MonthEND = DateAdd(DateInterval.Day, vSubtract, DateAdd(DateInterval.Day, 4, CDate(Year(p_date) & "/" & Month(p_date) & "/28")))
        End Function
        Public Function PointData1(ByVal pSQL As String, Optional ByVal pCombo As System.Web.UI.WebControls.DropDownList = Nothing) As String
            Dim c As New SqlClient.SqlConnection
            Dim cm As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader

            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = pSQL
            rs = cm.ExecuteReader
            If rs.Read Then
                Return rs(0) & "=>" & rs(1)
            Else
                Return "99=>Unknown"
            End If
            rs.Close()
            cm.Dispose()
            c.Close()
            c.Dispose()
        End Function

        Public Function PointData(ByVal pSQL As String, Optional ByVal pCombo As System.Web.UI.WebControls.DropDownList = Nothing) As String
            Dim c As New SqlClient.SqlConnection
            Dim cmd As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader

            c.ConnectionString = ConnStr
            c.Open()
            cmd.Connection = c
            cmd.CommandText = pSQL
            rs = cmd.ExecuteReader

            If rs.Read Then
                Return rs(0)
            Else
                Return "99"
            End If

            rs.Close()
            cmd.Dispose()

            c.Close()
            c.Dispose()
        End Function
        Public Sub BuildCombo(ByVal pSql As String, ByRef pCombo As System.Web.UI.WebControls.DropDownList)
            Dim c As New SqlClient.SqlConnection
            Dim cm As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader

            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = pSql
            rs = cm.ExecuteReader
            pCombo.Items.Clear()
            Do While rs.Read
                pCombo.Items.Add(New ListItem(IIf(IsDBNull(rs(1)), rs(0), rs(1)), rs(0)))
            Loop
            rs.Close()

            cm.Dispose()
            c.Close()
            c.Dispose()
        End Sub


        Public Sub BuildCombo(ByVal pSql As String, ByRef pCombo As System.Web.UI.WebControls.DropDownList, ByRef c As SqlClient.SqlConnection)
            Dim cm As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader

            cm.Connection = c
            cm.CommandText = pSql
            rs = cm.ExecuteReader
            pCombo.Items.Clear()
            Do While rs.Read
                pCombo.Items.Add(New ListItem(IIf(IsDBNull(rs(1)), rs(0), rs(1)), rs(0)))
            Loop
            rs.Close()
            cm.Dispose()
        End Sub
        Public Function GetName(ByVal emp_id As String) As String
            Dim c As New SqlClient.SqlConnection
            Dim cm As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader
            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = "SELECT Emp_Lname, Emp_Fname, Emp_Mname FROM py_emp_master WHERE Emp_Cd='" & emp_id & "'"
            rs = cm.ExecuteReader
            Dim lname As String = "[No Last Name]"
            Dim mname As String = "[No Middle Name]"
            Dim fname As String = "[No First Name]"
            Dim fullname As String = ""
            If rs.HasRows Then
                If rs.Read Then
                    If Not IsDBNull(rs(0)) Then
                        lname = rs(0)
                    End If
                    If Not IsDBNull(rs(1)) Then
                        fname = rs(1)
                    End If
                    If Not IsDBNull(rs(2)) Then
                        mname = rs(2).ToString.Substring(0, 1) & "."
                    End If
                    fullname = lname & ", " & fname & " " & mname
                Else
                    fullname = "Employee ID : " & emp_id & " is not registered properly."
                End If
            Else
                fullname = "Employee ID : " & emp_id & " is not registered properly."
            End If
            cm.Dispose()
            rs.Close()
            c.Close()
            c.Dispose()
            Return fullname
        End Function

        Public Sub ResyncLeave2Logs(ByVal pAppNo As String, ByVal pVoid As Boolean, ByRef c As SqlClient.SqlConnection)

            Dim cm As New SqlClient.SqlCommand
            Dim cmRef As New SqlClient.SqlCommand
            Dim cmChk As New SqlClient.SqlCommand

            Dim rs As SqlClient.SqlDataReader
            Dim rsRef As SqlClient.SqlDataReader
            Dim rsChk As SqlClient.SqlDataReader

            Dim vAgency As String = ""
            Dim vRc As String = ""
            Dim vALeave As Decimal = 0
            Dim vDays As Decimal = 0
            Dim i As Decimal = 1
            Dim vEffectivityDate As Date
            Dim vWithHalfDay As Boolean = False

            cm.Connection = c
            cmRef.Connection = c
            cmChk.Connection = c


            cm.CommandText = "select * from hr_leave_application where ApplicationNo='" & pAppNo & "'"
            rs = cm.ExecuteReader
            If rs.Read Then
                vALeave = rs("DaysLeave")
                vDays = vALeave
                'vWithHalfDay = (vALeave - Int(vALeave)) > 0 And vALeave > 1
                vWithHalfDay = vALeave > 0 And vALeave < 1

                If IsDBNull(rs("EffectivityDate")) Then
                    vEffectivityDate = Nothing
                Else
                    vEffectivityDate = rs("EffectivityDate")
                End If

                cmRef.CommandText = "select Agency_Cd,Rc_Cd from py_emp_master where Emp_Cd='" &
                    rs("Emp_Cd") & "'"
                rsRef = cmRef.ExecuteReader
                If rsRef.Read Then
                    vAgency = rsRef("Agency_Cd")
                    vRc = rsRef("Rc_Cd")
                End If
                rsRef.Close()
                '''''''''''''' added the ShiftCd<>'RD' condition to exclude restday as part of the schedule interpretation'''''''
                '''''''''''''' modified by: Vic Gatchalian ''''''''''''''
                '''''''''''''' Date Modified: 9/7/2011 ''''''''''''''''''
                cmRef.CommandText = "select Date_Sched from py_emp_time_sched where ShiftCd<>'RD' and Emp_Cd='" &
                    rs("Emp_Cd") & "' and Date_Sched >='" & CDate(rs("StartDate")) &
                    "' AND NOT EXISTS (SELECT Holy_Date FROM py_holiday WHERE Holy_Date = Date_Sched " &
                    " AND (Office_Cd LIKE '%*%' OR Office_Cd LIKE '%" & vAgency &
                    "%') AND (Rc_Cd LIKE '%*%' OR Rc_Cd LIKE '%" & vRc & "%')) " &
                    "and not exists " &
                    "(select StartDate from hr_leave_application where ApplicationNo<>'" & pAppNo &
                    "' and LeaveCd<>'OT' and Void=0 " &
                    "and hr_leave_application.Emp_Cd=py_emp_time_sched.Emp_Cd and " &
                    "Date_Sched between StartDate and EndDate)"
                rsRef = cmRef.ExecuteReader

                If vWithHalfDay Then 'applied days leave is more than 1 day and with half day embedded
                    'check first day if it contains logs, if it contains logs, put the remainder in the first day instead
                    If rsRef.Read Then
                        'check date in py_time_log if there's existing log
                        cmChk.CommandText = "select 1 from py_time_log where Tran_Date='" &
                            Format(CDate(rsRef("Date_Sched")), "yyyy/MM/dd") & "' and Emp_Cd='" &
                            rs("Emp_Cd") & "' and (Time_In is not null or Time_in<>'')"
                        rsChk = cmChk.ExecuteReader
                        If rsChk.HasRows Then      'first schedule date has logs, so put the remainder in the first day instead
                            If rs("LeaveCd") = "SAT" Then
                                If CDate(rsRef("Date_Sched")).DayOfWeek = DayOfWeek.Saturday Then
                                    'UPDATE py_time_log_dtl
                                    updateLogs(CDate(rsRef("Date_Sched")), vEffectivityDate,
                                        (vALeave - Int(vALeave)), rs("Emp_Cd"), IIf(rs("Paid") Or rs("Paid") = 1, rs("LeaveCd"), "ABSENT"), pVoid, c)
                                End If
                            Else
                                'UPDATE py_time_log_dtl
                                updateLogs(CDate(rsRef("Date_Sched")), vEffectivityDate,
                                    (vALeave - Int(vALeave)), rs("Emp_Cd"), IIf(rs("Paid") Or rs("Paid") = 1, rs("LeaveCd"), "ABSENT"), pVoid, c)
                            End If
                            i += 1
                            vALeave -= (vALeave - Int(vALeave))
                        End If
                        rsChk.Close()
                    End If
                End If

                Do While rsRef.Read
                    If rs("LeaveCd") = "SAT" Then
                        If CDate(rsRef("Date_Sched")).DayOfWeek = DayOfWeek.Saturday Then
                            updateLogs(CDate(rsRef("Date_Sched")), vEffectivityDate,
                                IIf(vALeave > 1, 1, vALeave), rs("Emp_Cd"), IIf(rs("Paid") Or rs("Paid") = 1, rs("LeaveCd"), "ABSENT"), pVoid, c)
                            If i < Math.Ceiling(vDays) Then
                                i += 1
                                vALeave -= 1
                            Else
                                Exit Do
                            End If
                        End If
                    Else
                        updateLogs(CDate(rsRef("Date_Sched")), vEffectivityDate,
                            IIf(vALeave > 1, 1, vALeave), rs("Emp_Cd"), IIf(rs("Paid") Or rs("Paid") = 1, rs("LeaveCd"), "ABSENT"), pVoid, c)
                        If i < Math.Ceiling(vDays) Then
                            i += 1
                            vALeave -= 1
                        Else
                            Exit Do
                        End If
                    End If
                Loop
                rsRef.Close()
            End If
            rs.Close()
            cmRef.Dispose()
            cm.Dispose()
        End Sub
        Private Sub updateLogs(ByVal iDate As Date, ByVal vEffectivityDate As Date, ByVal vALeave As Decimal,
            ByVal pID As String, ByVal pLeaveCd As String, ByVal pVoid As Boolean, ByRef c As SqlClient.SqlConnection)
            Dim cm As New SqlClient.SqlCommand
            Dim cmRef As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader
            Dim vDays As Decimal = vALeave * 8
            Dim vAmount As Decimal = 0
            Dim vRestDay As Boolean = False
            Dim vSQL As String = ""
            Dim vTranCd As String = ""
            Dim vHrs As Decimal = 0
            Dim vAmt As Decimal = 0

            cm.Connection = c
            cmRef.Connection = c

            cm.CommandText = "select Emp_Cd from py_time_log where Emp_Cd='" & pID &
                "' and Tran_Date='" & Format(iDate, "yyyy/MM/dd") & "'"
            rs = cm.ExecuteReader
            If Not rs.HasRows Then  'no record
                vSQL = "insert into py_time_log (Tran_Date,Emp_Cd) values ('" &
                    Format(iDate, "yyyy/MM/dd") & "','" & pID & "')"
            End If
            rs.Close()
            If vSQL <> "" Then
                cm.CommandText = vSQL
                cm.ExecuteNonQuery()
            End If
            If vEffectivityDate = Nothing Then       'current filed application
                'CLEAN RECORD FIRST IN TIME LOG DTL 
                cm.CommandText = "delete from py_time_log_dtl where Emp_Cd='" &
                    pID & "' and TranDate='" & Format(iDate, "yyyy/MM/dd") &
                    "' and LateFiled=0 and TranCd='" & pLeaveCd & "'"

                cm.ExecuteNonQuery()
                If Not pVoid Then
                    cm.CommandText = "insert into py_time_log_dtl (Emp_Cd,TranDate,TranCd,Hrs_Rendered," &
                        "AmtConv,Reason,LateFiled,EffectivityDate) values ('" & pID & "','" &
                        Format(iDate, "yyyy/MM/dd") & "','" & pLeaveCd & "'," & vDays &
                        ",0,'" & IIf(pLeaveCd = "ABSENT", "Auto-generated", "Posted-LeaveApp") &
                        "',0,'" & Format(iDate, "yyyy/MM/dd") & "')"
                    cm.ExecuteNonQuery()
                End If
            Else        'late filed application
                'CLEAN RECORD FIRST IN TIME LOG DTL 
                cm.CommandText = "delete from py_time_log_dtl where Emp_Cd='" &
                    pID & "' and TranDate='" & Format(iDate, "yyyy/MM/dd") & "' AND LateFiled=1"
                cm.ExecuteNonQuery()

                If Not pVoid Then
                    'get records for adjustment
                    cm.CommandText = "select * from py_time_log_dtl where Emp_cd='" &
                        pID & "' and CAST(TranDate AS DATE)='" & Format(iDate, "yyyy/MM/dd") &
                        "' and EffectivityDate=TranDate and Reason='Auto-generated' and " &
                        "TranCd in (SELECT replace(CreditTo,',',''',''') FROM hr_leave_application " &
                        "WHERE EffectivityDate IS NOT NULL AND StartDate='" & Format(iDate, "yyyy/MM/dd") &
                        "' AND Emp_Cd='" & pID & "')"
                    rs = cm.ExecuteReader

                    Do While rs.Read
                        vTranCd = rs("TranCd")
                        vHrs = rs("Hrs_Rendered") * -1
                        vAmt = rs("AmtConv") * -1
                        cmRef.CommandText = "insert into py_time_log_dtl (Emp_Cd,TranDate,TranCd,Hrs_Rendered," &
                            "AmtConv,Reason,LateFiled,EffectivityDate) values ('" & pID & "','" &
                            Format(iDate, "yyyy/MM/dd") & "','" & vTranCd & "'," & vHrs &
                            "," & vAmt & ",'Posted-LateApp',1,'" & Format(CDate(vEffectivityDate), "yyyy/MM/dd") & "')"
                        cmRef.ExecuteNonQuery()
                    Loop
                    rs.Close()
                End If
            End If
            cm.Dispose()
            cmRef.Dispose()
        End Sub

        Public Function Num2Words(ByVal pAmount As Decimal) As String
            Dim vOnes(0 To 9) As String
            Dim vTens(0 To 9) As String
            Dim vRest(9) As String
            Dim vValue As Decimal
            Dim vMode As Decimal
            Dim vTmp As Decimal
            Dim Flag As Integer
            Dim Flags As Integer
            Dim vPrefix As String = ""
            Dim Worded_Amount As String = ""

            vOnes(1) = "One" : vOnes(2) = "Two" : vOnes(3) = "Three"
            vOnes(4) = "Four" : vOnes(5) = "Five" : vOnes(6) = "Six"
            vOnes(7) = "Seven" : vOnes(8) = "Eight" : vOnes(9) = "Nine"

            vTens(2) = "Twenty" : vTens(3) = "Thirty" : vTens(4) = "Fourty"
            vTens(5) = "Fifty" : vTens(6) = "Sixty" : vTens(7) = "Seventy"
            vTens(8) = "Eighty" : vTens(9) = "Ninety"

            vRest(0) = "Ten" : vRest(1) = "Eleven" : vRest(2) = "Twelve"
            vRest(3) = "Thirteen" : vRest(4) = "Fourteen" : vRest(5) = "Fifteen"
            vRest(6) = "Sixteen" : vRest(7) = "Seventeen" : vRest(8) = "Eighteen"
            vRest(9) = "Nineteen"

            'vvalue = Val(txt_amt.Text)
            vValue = pAmount
            vMode = Int(vValue)
            vMode = vValue - vMode
            vMode = vMode * 100
            vTmp = IIf(vValue < 1000, 0, vValue)

            Try
refresh:
                If vValue > 999999 Then
                    vTmp = vValue - (vValue - (vValue Mod 1000000))
                    vValue = (vValue - vTmp) / 1000000
                    vPrefix = " Million "
                    If vTmp = 0 Then
                        Flags = 5
                    End If
                ElseIf vValue > 999 Then
                    vTmp = vValue - (vValue - (vValue Mod 1000))
                    vValue = (vValue - vTmp) / 1000
                    vPrefix = " Thousand "
                    If vTmp = 0 Then
                        Flags = 5
                    End If
                End If

                If (vValue - (vValue Mod 100)) / 100 <> 0 Then      'hundreds 
                    Worded_Amount = Worded_Amount & vOnes((vValue - (vValue Mod 100)) / 100) & " Hundred "
                    vValue = vValue - (vValue - (vValue Mod 100))
                    GoTo refresh
                ElseIf (vValue - (vValue Mod 10)) / 10 > 1 Then    'tens 
                    Worded_Amount = Worded_Amount & vTens((vValue - (vValue Mod 10)) / 10) & " "
                    vValue = vValue - (vValue - (vValue Mod 10))
                    GoTo refresh
                ElseIf vValue > 0 And vValue < 10 Then
                    Worded_Amount = Worded_Amount & vOnes(Int(vValue))
                    vValue = vTmp
                ElseIf vValue > 9 And vValue < 20 Then
                    Worded_Amount = Worded_Amount & vRest(vValue - 10)
                    vValue = vTmp
                Else
                    vValue = vTmp
                End If

                'worded_amount = worded_amount & IIf(vtmp = 0, "", vprefix)
                If Flags <> 5 Then
                    Worded_Amount = Worded_Amount & IIf(vTmp = 0, "", vPrefix)
                Else
                    Worded_Amount = Worded_Amount & " " & vPrefix
                End If

                vTmp = 0
                If vValue <> 0 Then GoTo refresh

                If Flag = 5 Then
                    vMode = 0
                    Worded_Amount = Worded_Amount & " Centavos"
                End If


                If vMode > 0 Then
                    Flag = 5
                    Worded_Amount = Worded_Amount & " " & "Pesos" & " " & "and" & " "
                    vValue = vMode
                    vTmp = IIf(vValue < 1000, 0, vValue)
                    GoTo refresh
                End If
                If Flag <> 5 Then
                    Worded_Amount = Worded_Amount & " " & "Pesos"
                End If
            Catch ex As Exception
                Worded_Amount = "________________________________________________________________"
            End Try

            Return Worded_Amount
        End Function

        Public Function EOTimeConvert(ByVal uid As String, ByVal SchedDate As Date, ByVal hrsApplied As Decimal,
                                    ByVal remarks As String, ByRef c As SqlClient.SqlConnection) As Decimal

            'Dim c As New SqlClient.SqlConnection
            Dim cmref As New SqlClient.SqlCommand
            Dim rsref As SqlClient.SqlDataReader

            Dim vAgency As String = ""
            Dim vRegDay As Boolean = True
            Dim vRank As String = ""
            Dim vDayTypeCode As String = ""
            Dim vGroupCd As String = ""
            Dim vCredits As Decimal = 0
            Dim vComputed As Decimal = 0

            'c.ConnectionString = connStr
            'c.Open()

            cmref.Connection = c

            Dim vSched As String = "RD"
            Dim OTCd As String = "A1"
            Dim vFactor As Decimal = 1
            Dim vSchedIn As String = ""
            Dim vSchedOut As String = ""
            Dim vSchedHours As Decimal = 8   ''''' default to 8


            cmref.CommandText = "SELECT Agency_Cd,EmploymentType,GroupCd FROM py_emp_master WHERE Emp_Cd = '" & uid & "'"
            rsref = cmref.ExecuteReader
            rsref.Read()
            If IsDBNull(rsref(0)) Then vAgency = "" Else vAgency = rsref(0)
            If IsDBNull(rsref(1)) Then vRank = "" Else vRank = rsref(1)
            If IsDBNull(rsref(2)) Then vGroupCd = "" Else vGroupCd = rsref(2)
            rsref.Close()

            If Not remarks.Contains("Evolve") Then   '''''Used in manila pen
                'If Not remarks.Contains("Migration") Then
                cmref.CommandText = "SELECT Office_Cd,Descr,Official FROM py_holiday WHERE Holy_Date='" & Format(SchedDate, "yyyy/MM/dd") & "'"
                rsref = cmref.ExecuteReader
                If rsref.Read Then
                    If rsref("Official") = True Then
                        'If IsDBNull(rsref("Descr")) Then vDayDesc = "" Else vDayDesc = rsref("Descr")
                        'vTrimDayDesc = vDayDesc
                        'If vDayDesc <> "" And vDayDesc.Length > 11 Then
                        '    vTrimDayDesc = vDayDesc.Substring(0, 7) & "..."
                        'End If
                        '''''Legal Holiday
                        vDayTypeCode = "Legal"
                    Else
                        If rsref("Office_Cd").ToString.IndexOf(vAgency) >= 0 Or rsref("Office_Cd").ToString.Contains("*") Then
                            'If IsDBNull(rsref("Descr")) Then vDayDesc = "" Else vDayDesc = rsref("Descr")
                            'vTrimDayDesc = vDayDesc
                            vDayTypeCode = "Special"
                        End If

                        'If vDayDesc <> "" And vDayDesc.Length > 11 Then
                        '    vTrimDayDesc = vDayDesc.Substring(0, 7) & "..."
                        'End If

                    End If
                    vRegDay = False
                End If
                rsref.Close()

                cmref.CommandText = "select * from py_time_log where Tran_Date='" & Format(CDate(SchedDate), "yyyy/MM/dd") & "' and Emp_Cd='" & uid & "'"
                rsref = cmref.ExecuteReader
                If rsref.Read Then
                    vSchedIn = IIf(Not IsDBNull(rsref("Sched_In")), rsref("Sched_In"), "")
                    vSchedOut = IIf(Not IsDBNull(rsref("Sched_Out")), rsref("Sched_Out"), "")
                    'vSched = IIf(Not IsDBNull(rsref("ShiftCd")), rsref("ShiftCd"), "RD")
                    vSched = IIf(Not IsDBNull(rsref("ShiftCd")), rsref("ShiftCd"), "")
                End If
                rsref.Close()

                If vSchedIn <> "" And vSchedOut <> "" Then
                    vSchedHours = DateDiff(DateInterval.Hour, CDate(vSchedIn), CDate(vSchedOut))
                End If

                If Not vRegDay Then  '''''   Holiday
                    If vDayTypeCode = "Legal" Then   ''''   Legal Holiday
                        If vSched <> "RD" Then
                            OTCd = "C1"    '''' Legal Holiday
                        Else
                            OTCd = "D1"    '''' Legal Holiday on Rest Day
                        End If
                    Else    '''' Special Holiday
                        If vSched <> "RD" Then
                            OTCd = "F1"    ''''' Special Holiday
                        Else
                            OTCd = "B1"    ''''' Special Holiday on Rest Day
                        End If
                    End If
                Else    ''''' Regular day
                    If vSched = "RD" Then   '''' Rest Day 
                        OTCd = "E1"
                    Else
                        OTCd = ""
                    End If
                End If

                cmref.CommandText = "select MaxHrs from py_leave_table where LeaveCd='EO' and EmploymentType='" & vRank &
                                "' and GroupCd='" & vGroupCd & "'"
                rsref = cmref.ExecuteReader
                If rsref.Read Then
                    If hrsApplied > rsref("MaxHrs") Then
                        hrsApplied = rsref("MaxHrs")
                    End If
                End If
                rsref.Close()

                If vSchedHours <> 0 Then
                    If vGroupCd = "ADMIN" Then
                        If vSchedHours > 4 Then
                            vSchedHours -= 1
                        End If
                    End If
                End If


                If vRank <> "" And OTCd <> "" Then
                    cmref.CommandText = "select EmploymentType,Factor from py_ot_ref_dtl where EmploymentType='" & vRank & "' and OtCd='" & OTCd & "'"
                    rsref = cmref.ExecuteReader
                    If rsref.Read Then
                        If IsDBNull(rsref("Factor")) Then
                            vFactor = 1
                        Else
                            vFactor = rsref("Factor")
                        End If
                    End If
                    rsref.Close()
                End If
                ''''' check to py_ot_ref if no record found on the py_ot_ref_dtl
                If vFactor = 1 And OTCd <> "" Then
                    cmref.CommandText = "select OtCd,OtPer from py_ot_ref where OtCd='" & OTCd & "'"
                    rsref = cmref.ExecuteReader
                    If rsref.Read Then
                        vFactor = rsref("OtPer")
                    End If
                    rsref.Close()
                End If
                If vGroupCd = "ADMIN" Then
                    vSchedHours = 9
                Else
                    vSchedHours = 8
                End If
            Else 'Migrated EO's (must contains word "Evolve" in the remarks)
                If vGroupCd = "ADMIN" Then
                    vSchedHours = 9
                Else
                    vSchedHours = 8
                End If
            End If  'not contains "Evolve"    

            If vSchedHours > 0 Then
                vComputed = (hrsApplied * vFactor) / vSchedHours
            End If

            'vTrimDayDesc += "-" & "Hrs Applied:" & Format(hrsApplied, "###.00") & " OT Code:" & OTCd & " Factor:" & vFactor & " Sched Hours:" & vSchedHours
            'vDayDescription = vTrimDayDesc
            'If vTrimDayDesc <> "" And vTrimDayDesc.Length > 11 Then
            '    vTrimDayDesc = vTrimDayDesc.Substring(0, 7) & "..."
            'End If

            cmref.Dispose()
            'c.Dispose()
            'c.Close()

            Return vComputed
            ''''''''''''''''''''''''''''''

            'Catch ex As SqlClient.SqlException
            '    vscript = "alert('Error in opening the database." & ex.Message.Replace(",", "").Replace(";", "") & "')"
            'Finally
            'End Try
        End Function


        Public Function GetMaxCount(ByVal pSQL As String, Optional ByVal pCombo As System.Web.UI.WebControls.DropDownList = Nothing) As String

            Dim c As New SqlClient.SqlConnection
            Dim cm As New SqlClient.SqlCommand
            Dim rs As SqlClient.SqlDataReader

            c.ConnectionString = ConnStr
            c.Open()
            cm.Connection = c
            cm.CommandText = pSQL
            rs = cm.ExecuteReader

            If rs.Read Then
                If IsDBNull(rs(0)) Then
                    Return 1
                Else
                    Return rs(0) + 1
                End If
            End If

            rs.Close()
            cm.Dispose()
            c.Close()
            c.Dispose()

            Return ""

        End Function


        Public Function getEncryptedCode(ByVal inputString As String) As String

            Dim Hash As Byte() = New System.Security.Cryptography.SHA512Managed().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(inputString))
            Dim outputString As New System.Text.StringBuilder()

            For i As Integer = 0 To Hash.Length - 1
                outputString.Append(Hash(i).ToString("x2"))
            Next

            Return outputString.ToString()

        End Function

        Public Sub CreateRecords(ByVal pSQL As String)
            Dim c As New SqlClient.SqlConnection(ConnStr)
            Dim cm As New SqlClient.SqlCommand(pSQL, c)

            c.Open()

            Try
                cm.ExecuteNonQuery()
            Catch ex As Exception

            End Try

            c.Close()
            cm.Dispose()
            c.Dispose()
        End Sub

        Public Function GenerateRandomString(ByRef iLength As Integer) As String
            Dim rdm As New Random()
            Dim allowChrs() As Char = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray()
            Dim sResult As String = ""

            For i As Integer = 0 To iLength - 1
                sResult += allowChrs(rdm.Next(0, allowChrs.Length))
            Next

            Return sResult
        End Function

        Public Function dateparsing(ByVal string_to_date As String) As Date
            Dim vStringToDate As Date
            Dim format() = {"M/d/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "MM/dd/yyyy"}
            Date.TryParseExact(string_to_date, format, System.Globalization.DateTimeFormatInfo.InvariantInfo, Globalization.DateTimeStyles.None, vStringToDate)

            Return vStringToDate
        End Function
    End Module
End Namespace
