<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="EmployeeMaster.aspx.vb" Inherits="EmployeeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.5.1.js"></script>
    <style type="text/css">
        .iDataFrame {
            width: 99%;
            border: solid 0px #e2e2e2;
            height: 96%;
            margin: 0px;
        }

        .divPad {
            border: 0px solid #000;
            padding-top: 5px;
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
    </style>
    <script>
        $(document).ready(function () {

            $('#BtnUpload').click(function () {
                $('#UploadTemplate').modal();
            });
            $('#MainContent_BtnUpdate').click(function () {
                $('#UpdateEmployeeData').modal();
            });
            $('#BtnException').click(function () {
                $('#ExceptionReport').modal();
            });


        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Employee Management</h3>
        <div class="row divPad">
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Search Keywords:</small>
                    <asp:TextBox ID="TxtKeywords" runat="server" CssClass="form-control form-control-sm" placeholder="Enter emp code, first or last name"></asp:TextBox>
                </div>
                <div class="col-md-12">
                    <small>Employment Status:</small>
                    <asp:DropDownList ID="CmdResign" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>
                </div>
            </div>
            <div class="col-sm-3">
                <div class="col-md-12">
                    <small>Employee Status:</small>
                    <asp:DropDownList ID="CmdStatus" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>
                </div>
                <%--<div class="col-md-12">
                    <small>Position:</small>
                    <asp:DropDownList ID="CmdPosition" runat="server" Width="" CssClass="form-control form-control-sm"></asp:DropDownList>
                </div>--%>
            </div>


            <div class="col-sm-3"></div>
        </div>

        <div class="row divPad">
            <div class="col-sm-12">
                <div class="col-sm-6">
                    <div class="btn-group">
                        <button type="button" id="BtnUpload" class="btn btn-primary btn-sm">Upload Template</button>
                        <button type="button" id="BtnUpdate" class="btn btn-primary btn-sm" runat="server">Edit</button>
                        <button type="button" id="BtnException" class="btn btn-primary btn-sm">Exception Report</button>
                        <asp:Button ID="BtnExport" runat="server" CssClass="btn btn-sm btn-primary" Text="Export to Excel"></asp:Button>
                        <asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Search"></asp:Button>
                        <asp:Button ID="BtnSendEmail" runat="server" CssClass="btn btn-sm btn-primary" Text="Send Email Notif"></asp:Button>
                        <%--<button type="button" id="BtnGenReport" class="btn btn-success btn-sm" data-toggle="modal" data-target="#myModalEdit">Generate Report</button>--%>
                    </div>
                </div>
                <div class="col-sm-5"><small><b>Total Record(s):
                    <asp:Label ID="LblRowCount" runat="server" Text="Label"></asp:Label></b></small>
                </div>
            </div>

        </div>
        <br />
        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <asp:GridView ID="tblEmployees" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="12px"
                        AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                        CssClass="table table-sm table-bordered table-striped" PageSize="20" EnableModelValidation="True"
                        SelectedRowStyle-CssClass="btn btn-info">

                        <Columns>
                            <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <HeaderStyle Width="30px"></HeaderStyle>
                            </asp:TemplateField>

                            <asp:BoundField DataField="EmployeeCode" HeaderText="Emp Code"></asp:BoundField>
                            <asp:BoundField DataField="FullName" HeaderText="Full Name"></asp:BoundField>
                            <asp:BoundField DataField="AddressRegistered" HeaderText="Registered Address"></asp:BoundField>

                            <asp:BoundField DataField="BirthDate" HeaderText="Birth Date"></asp:BoundField>
                            <asp:BoundField DataField="DateHired" HeaderText="Date Hired"></asp:BoundField>
                            <asp:BoundField DataField="Active" HeaderText="Active"></asp:BoundField>
                            <asp:BoundField DataField="DateSeparated" HeaderText="Date Separated"></asp:BoundField>
                            <asp:BoundField DataField="TaxCode" HeaderText="Tax Code"></asp:BoundField>
                            <asp:BoundField DataField="Vat" HeaderText="Vat"></asp:BoundField>
                            <asp:BoundField DataField="VatPercent" HeaderText="Vat Percent"></asp:BoundField>
                            <asp:BoundField DataField="Monthlyrate" HeaderText="Basic Allowance"></asp:BoundField>
                            <asp:BoundField DataField="PosName" HeaderText="Position"></asp:BoundField>
                            <asp:BoundField DataField="Gender" HeaderText="Gender"></asp:BoundField>
                            <asp:BoundField DataField="TINNo" HeaderText="TIN"></asp:BoundField>
                            <asp:BoundField DataField="EmailAddress" HeaderText="Email Address"></asp:BoundField>
                            <asp:BoundField DataField="Bank" HeaderText="Bank"></asp:BoundField>
                            <asp:BoundField DataField="BankAccountNo" HeaderText="Bank Acct No"></asp:BoundField>
                            <asp:BoundField DataField="CostCenter" HeaderText="Cost Center"></asp:BoundField>
                            <asp:BoundField DataField="PayGroup" HeaderText="Pay Group"></asp:BoundField>
                            <asp:BoundField DataField="Div" HeaderText="Division"></asp:BoundField>
                            <asp:BoundField DataField="LocName" HeaderText="Group"></asp:BoundField>
                            <asp:BoundField DataField="CustomField2" HeaderText="ATC"></asp:BoundField>
                            <asp:BoundField DataField="Remarks" HeaderText="IPSEWTax"></asp:BoundField>




                            <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="Select">
                                <ItemStyle CssClass="labelC" Width="40px" />
                                <ControlStyle CssClass="btn btn-primary btn-sm" />
                            </asp:CommandField>
                        </Columns>
                        <SelectedRowStyle CssClass="bg-warning" />
                        <PagerStyle Font-Size="8pt" />
                        <HeaderStyle CssClass="titleBar" />
                        <RowStyle CssClass="odd" />
                        <AlternatingRowStyle CssClass="even" />
                    </asp:GridView>
                </div>

            </div>
        </div>
    </div>

    <div id="UploadTemplate" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="LblModalTitle" runat="server" Text="Upload Master Data"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <%--Browse Master Data File: 
                            <asp:FileUpload ID="TxtFileName" runat="server" CssClass="form-control-file border" />
                            <br />--%>
                            Browse Master Data File: 
                            <asp:FileUpload ID="TxtFileUpdateMasterData" runat="server" CssClass="form-control-file border" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSubmitUpload" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="UpdateEmployeeData" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label1" runat="server" Text="Modify Employee Data"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            Enter date resign: 
                            <asp:TextBox ID="TxtDateResign" runat="server" CssClass="form-control form-control-sm col-5" Placeholder="MM/DD/YYYY"></asp:TextBox>
                            <br />
                            Employee Status:
                            <asp:DropDownList ID="CmdEditStatus" runat="server" Width="" CssClass="form-control form-control-sm col-5"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" id="BtnSubmitUpdate" value="Submit" class="btn btn-sm btn-primary" runat="server" />
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="ExceptionReport" class="modal fade" role="dialog">
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="Label2" runat="server" Text="Exception Report"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">

                    <div class="row">
                        <div class="col-sm-12 text-danger">Duplicate Employee Code</div>
                        <div class="col-sm-12">
                            <table class="table table-bordered table-sm" style="font-size: 12px">
                                <thead>
                                    <tr>
                                        <th colspan="3">Employee Master Data</th>
                                        <th colspan="3">Dumplecate from the template</th>
                                    </tr>
                                    <tr>
                                        <th style="width: 200px" class="col-sm-3">Emp Code</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Emp Code</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%=DuplicateEmpCode %>
                                </tbody>
                            </table>
                        </div>
                        <div class="col-sm-12 text-danger"><input type="button" id="BtnClearDuplicateEmpCode" value="Clear duplicate employee code" class="btn btn-sm btn-primary" runat="server" /></div>
                    </div>
                    <br />
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 text-danger">Duplicate Bank Account Number</div>
                        <div class="col-sm-12">
                            <table class="table table-bordered table-sm" style="font-size: 12px">
                                <thead>
                                    <tr>
                                        <th style="width: 200px" class="col-sm-3">Emp Code</th>
                                        <th style="width: 400px">Full Name</th>
                                        <th>Bank Account Number</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%=DuplicateBankAcct %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 text-danger">Duplicate TIN Number</div>
                        <div class="col-sm-12">
                            <table class="table table-bordered table-sm" style="font-size: 12px">
                                <thead>
                                    <tr>
                                        <th style="width: 200px" class="col-sm-3">Emp Code</th>
                                        <th style="width: 400px">Full Name</th>
                                        <th>TIN Number</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%=DuplicateTIN %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 text-danger">Duplicate Email Address</div>
                        <div class="col-sm-12">
                            <table class="table table-bordered table-sm" style="font-size: 12px">
                                <thead>
                                    <tr>
                                        <th style="width: 200px" class="col-sm-3">Emp Code</th>
                                        <th style="width: 400px">Full Name</th>
                                        <th>Email Address</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%=DuplicateEmail %>
                                </tbody>
                            </table>
                        </div>
                    </div>


                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-sm btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
</asp:Content>
