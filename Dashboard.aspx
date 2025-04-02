<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style type="text/css">
        .iDataFrame {
            width: 99%;
            border: solid 0px #e2e2e2;
            height: 96%;
            margin: 0px;
        }

        div {
            border: 0px solid #000;
            padding-top: 1px;
        }

        body {
            font-family: Arial;
            font-size: 12px;
        }

        .ZeroPadleft {
            padding-left: 0px;
            margin-left: 0px;
        }

        .Tbl {
            border: solid 1px #CCC;
        }

        small {
            font-size: 12px;
        }

        .hideGridColumn {
            display: none;
        }

        .CirSpan {
            display: inline-block;
            width: 100px;
            height: 100px;
            margin: 6px;
            background-color: #FFC300;
            color: #CCC;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h2>Dashboard</h2>

        <div class="row">
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Select Report Type:</small>
                    <asp:DropDownList ID="CmdReportList" runat="server" CssClass="form-control form-control-sm"></asp:DropDownList>
                </div>
                <div class="col-md-12">
                    <%--<small>Emp Status:</small>
                    <asp:DropDownList ID="CmdStatus" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>--%>
                </div>
            </div>
            <div class="col-sm-9">
                <div class="row">
                    <%=BasicDash %>
                </div>

            </div>
        </div>
        <br />
        <hr />
        <br />
        <div class="row">
            
            <div class="col-sm-3"></div>
            <div class="col-sm-6">
                <table class="table table-bordered">
                    <tr>
                        <td></td>
                         <%=AttestDetailsHeader %>
                    </tr>
                    <tr>
                        <td>Total number of documents attested </td>
                        <%=AttestRead %>
                    </tr>
                    <tr>
                        <td>Total number of unread documents </td>
                        <%=AttestUnRead %>
                    </tr>
              
                    <%=ForMyReview %>
                    <%=ForMyApproval %>
                    <%=ForMyAttension %>
                </table>
            </div>
            <div class="col-sm-3"></div>
        </div>

        <br />
    </div>
</asp:Content>

