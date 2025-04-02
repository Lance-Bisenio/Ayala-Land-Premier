<%@ Page Title="" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeFile="WageTypeCatalog.aspx.vb" Inherits="WageTypeCatalog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="container-fluid">
        <br />
        <h3>Wage Type Catalog</h3>
        <%--<div class="row divPad">
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
                </div>--% >
            </div>


            <div class="col-sm-3"></div>
        </div>--%>

        <div class="row divPad">
            <div class="col-sm-12">
                <div class="col-sm-6">
                    <div class="btn-group"> 
                        <%--<asp:Button ID="BtnReload" runat="server" CssClass="btn btn-sm btn-primary" Text="Search"></asp:Button>
                        <button type="button" id="BtnGenReport" class="btn btn-success btn-sm" data-toggle="modal" data-target="#myModalEdit">Generate Report</button>--%>
                    </div>
                </div>
                <div class="col-sm-6"></div>
            </div>

        </div>
        <br />
        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <asp:GridView ID="tblWageType" runat="server" AllowPaging="True" BorderColor="#CCCCCC" Font-Size="12px"
                        AutoGenerateColumns="False" Width="100%" BorderStyle="Solid" BorderWidth="1px"
                        CssClass="table table-sm table-bordered table-striped" PageSize="30" EnableModelValidation="True"
                        SelectedRowStyle-CssClass="btn btn-info">

                        <Columns>
                            <asp:TemplateField HeaderText="#" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <HeaderStyle Width="30px"></HeaderStyle>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Code" HeaderText="Code"></asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Description"></asp:BoundField>
                            <asp:BoundField DataField="IsTaxable" HeaderText="Taxable"></asp:BoundField>

                            <asp:BoundField DataField="IsEarning" HeaderText="IsDeduction"></asp:BoundField>
                            <asp:BoundField DataField="IsRecurring" HeaderText="IsRecurring"></asp:BoundField> 
                             
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
</asp:Content>

