<%@ Page Title="LOG IN" Language="VB" AutoEventWireup="true" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
 
<head runat="server">
    <title>HR Portal Login </title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" /> 
    <link href="Content/bootstrap.css" rel="stylesheet" />    
    <style>
        .xBtn {
            background-color: #0a60cb;
            color: #fff;
        }

        .bg {
            background: url(images/LoginBg.png) no-repeat center center fixed;
            background-size: cover;
            z-index: -9999;
        }
        .divPad {
            margin-bottom: 15px
        }

        .alert {
             font-size: 12px; margin: 0px
        }
    </style>
</head>
<body class="bg">
    <form id="form1" runat="server" autocomplete="off">
        <div class="container">
            <div class="row">
                <div class="col-md-4"></div>
                <div class="col-md-4">
                    <div class="panel panel-primary" style="width: 100%; margin: auto; 
                        box-shadow: 2px 2px 10px #000; margin-top: 30px; border: 
                            solid 0px #bb1100; background-color:#fff">

                        <div style="margin: auto;
                            width: 85%;
                            padding-top: 25px;
                            padding-bottom: 20px;
                            border: solid 0px;">

                            <h2>LOGIN</h2>
                            <h5>Ayala Land Sales, Inc.</h5>
                        </div>

                        <div class="panel-body"> 
                            <div class="row">
                                <div class="col-sm-12">
                                  <%--  <div class="col-sm-12 divPad" >
                                        <input class="form-control form-control-sm col-12" type="text" id="TxtClientCode" runat="server" placeholder="Client Code" autocomplete="off" />
                                    </div>--%>
                                    <div class="col-sm-12 divPad">
                                        <input class="form-control form-control-sm col-12" type="text" id="txtU" runat="server" placeholder="Employee Code" autocomplete="off" />
                                    </div>
                                    <div class="col-sm-12 divPad">
                                        <input class="form-control form-control-sm col-12" type="password" id="txtP" runat="server" placeholder="Password" autocomplete="off" />
                                    </div>
                                </div>
                            </div>

                            <%--<div class="form-group form-group-sm">
                                <div class="col-sm-12">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="alert alert-info" style="padding: 5px; padding-left: 10px" id="Div1">
                                                <p style="font-size: 12px; margin: 0px">
                                                    NOTE: It is recommended that you only use the following password special characters.
                                                    <br />
                                                    ! @ # $ % & * ( ) ~
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>

                            <div class="form-group form-group-sm">
                                <div class="col-sm-12">
                                    <asp:Button ID="btnLogin" CssClass="btn btn-primary btn-sm btn-block xBtn" runat="server" Text="Login" />
                                </div>
                            </div>

                            <div class="form-group form-group-sm">
                                <div class="col-sm-12 text-center">
                                    <asp:LinkButton ID="LinkForgotAcct" CssClass="btn-link" runat="server">Forgot account?</asp:LinkButton>
                                </div>
                            </div>

                            <div class="form-group form-group-sm">
                                <div class="col-sm-12">
                                    <div class="alert alert-danger" id="dvError" visible="false" runat="server">
                                        <strong>Access Denied!</strong>&nbsp;:&nbsp;
                                    <asp:Label ID="lblError" runat="server" ForeColor="#FF3300" Font-Size="Small"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group form-group-sm">
                                <div class="col-sm-12">
                                    <hr />
                                    <small class="text-muted">&copy; <%: DateTime.Now.Year %> Business Process Outsourcing International, Inc.</small>
                                    <br /><br />
                                </div>
                            </div>
                        </div>
                    </div>  
                </div>
                <div class="col-md-4"></div>
            </div>
            <div class="row">
                <div class="col-8"></div>
                <div class="col-4">
                    <div class="fixed-bottom" style="margin-right:20px; margin-bottom:20px">
                        <img alt="" src="images/logo.png" align="right" style="margin-top:70px"/>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>