
Partial Class ExpiredToken
    Inherits System.Web.UI.Page

    Private Sub LinkForgotAcct_Click(sender As Object, e As EventArgs) Handles LinkForgotAcct.Click
        Response.Redirect("~/AccountRecover")
    End Sub
End Class
