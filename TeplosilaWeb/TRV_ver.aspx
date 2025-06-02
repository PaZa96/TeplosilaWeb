<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TRV_ver.aspx.cs" Inherits="TRV_ver" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <title>Программа подбора регулирующего клапана - Поверочный расчет</title>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server" novalidate="novalidate">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="row jumbotron">
                <div class="col-12">
                    <nav class="nav nav-tabs">
                        <a class="nav-link" href="/TRV.aspx">Стандартный расчет</a>
                        <a class="nav-link active" aria-current="page" href="#">Поверочный расчет</a>
                    </nav>
                </div>
                <div class="col-xs-12 col-sm-10">
                    <div class="col border border-non-top">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label1" runat="server" Text="Тип клапана:"></asp:Label>
                                <br />
                                <asp:RadioButton ID="tvRadioButton1" runat="server" Text="2-х ходовой" AutoPostBack="True" OnCheckedChanged="RadioButton1_CheckedChanged" />
                                <div class="col">
                                    <asp:RadioButtonList ID="tvRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="tvRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>TRV</asp:ListItem>
                                        <asp:ListItem>TRV-T</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:RadioButton ID="tvRadioButton2" runat="server" Text="3-х ходовой" AutoPostBack="True" OnCheckedChanged="tvRadioButton2_CheckedChanged" />
                                <div class="col">
                                    <asp:RadioButtonList ID="tvRadioButtonList2" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="tvRadioButtonList2_SelectedIndexChanged">
                                        <asp:ListItem>TRV-3</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label2" runat="server" Text="Номинальное давление PN, бар:"></asp:Label>
                                <asp:RadioButtonList ID="pnRadioButtonList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="pnRadioButtonList1_SelectedIndexChanged">
                                    <asp:ListItem>16</asp:ListItem>
                                    <asp:ListItem>25</asp:ListItem>
                                </asp:RadioButtonList>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div class="col border">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="Label3" runat="server" Text="Диаметр и пропускная способность клапана:"></asp:Label>
                                <div id="aaPane1" class="col" runat="server">
                                    <div class="row">
                                        <div class="col-2">
                                            <asp:Label ID="dnLabel" runat="server" Text="DN, мм:"></asp:Label></div>
                                        <div class="col-2">
                                            <asp:DropDownList ID="dnDropDownList1" runat="server" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="dnDropDownList1_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList></div>
                                    </div>
                                    <div class="row">
                                        <div class="col-2">
                                            <asp:Label ID="kvsLabel" runat="server" Text="Kvs, м3/ч:"></asp:Label></div>
                                        <div class="col-2">
                                            <asp:DropDownList ID="kvsDropDownList1" runat="server" Enabled="False" AutoPostBack="True">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                            </asp:DropDownList></div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
