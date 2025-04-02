<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="BIR2307Management.aspx.vb" Inherits="BIR2307Septup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/jquery-3.5.1.js"></script>
    <script>
        $(document).ready(function () {

            $('#BtnCreateNew').click(function () {
                $('#ModalPayrollList').modal();
            });

            $('#MainContent_BtnUpdate').click(function () {
                $('#UpdateEmployeeData').modal();
            });
            $('#BtnException').click(function () {
                $('#ExceptionReport').modal();
            });
        });
        function Downfile(FilePath) {
            alert(FilePath);
            //window.location.assign(FilePath)
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <br />
    <div class="container-fluid">
        <br />
        <h3>2307 Report Management</h3>
        <div class="row">
            <div class="col-sm-12">
                <div class="btn-group">
                    <%--<button type="button" id="BtnCreateNew" class="btn btn-primary btn-sm">Create New</button>
                    <button type="button" id="BtnUpdate" class="btn btn-primary btn-sm" runat="server">Edit</button>
                    --%>
                    <asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Genarate 2307 Summary Report"></asp:Button>

                    <%--<button type="button" id="BtnGenReport" class="btn btn-success btn-sm" data-toggle="modal" data-target="#myModalEdit">Generate Report</button>--%>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-4">
                <div class="table-responsive-sm">
                    <table class="table table-bordered table-sm small">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>BatchNo</th>
                                <th>PayrollPeriod</th>
                                <th>PayDate</th>
                                <th>DatePosted</th>
                                <th>DatePublish</th>
                            </tr>
                        </thead>
                        <tbody>
                            <%=PayrollRunList %>
                        </tbody>
                    </table>
                </div>
                <div class="table-responsive-sm">
                    <asp:GridView ID="tblPosted2307" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="12px"
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

                            <asp:BoundField DataField="Periodfrom" HeaderText="Period From"></asp:BoundField>
                            <asp:BoundField DataField="Periodto" HeaderText="Period To"></asp:BoundField>
                            <asp:BoundField DataField="BatchNo" HeaderText="Batch No"></asp:BoundField> 
                            <asp:BoundField DataField="BatchNo" HeaderText="IsPosted"></asp:BoundField> 




                            <asp:CommandField ButtonType="Button" ShowSelectButton="True" SelectText="Download">
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


<%--                    <table class="table table-bordered table-sm small">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Period From</th>
                                <th>Period To</th>
                                <th>BatchNo</th>
                                <th>Posted</th>
                                <th>Option</th> 
                            </tr>
                        </thead>
                        <tbody>
                            <%=BIR2307PostedList %>
                        </tbody>
                    </table>--%>
                </div>
            </div>
            <div class="col-sm-8">
                <div class="table-responsive-sm">
                    <table class="table table-bordered table-sm small">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Employee Code</th>
                                <th>FullName</th>
                                <th>ATC</th>
                                <th>1st Month</th>
                                <th>2nd Month</th>
                                <th>3nd Month</th>
                                <th>Total</th>
                                <th>Tax WithHeld</th>
                            </tr>
                        </thead>
                        <tbody>
                            <%=Payroll2307Data %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div> 
    </div>

    <div id="ModalPayrollList" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-primary">
                        <asp:Label ID="LblModalTitle" runat="server" Text="Payroll Data"></asp:Label></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <table class="table table-bordered table-sm">
                                <thead>
                                    <tr>
                                        <th>Firstname</th>
                                        <th>Lastname</th>
                                        <th>Email</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
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
</asp:Content>

