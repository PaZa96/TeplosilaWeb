<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalcPrice.aspx.cs" Inherits="CalcPrice" Culture="ru-RU" %>

<%@ Import Namespace="System.Globalization" %>

<!DOCTYPE html>

<html>

<head runat="server">
    <title>Расчет стоимости шкафов управления</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/css/style.css" rel="stylesheet" />
</head>

<body>
    <div class="container">
        <form id="form1" runat="server">
            <div class="jumbotron">

                <!-- Строка исполнения -->

                <div class="card shadow-sm header-card section-space">
                    <div class="card-body text-center">
                        <div class="execution-code">ТШУА-1-1-10-2-IP65-01</div>
                    </div>
                </div>

                <!-- Основные параметры -->

                <div class="row">
                    <div class="col-8 non-padding-right">
                        <div class="parameter-box section-space">


                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Тип датчика температуры</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlSensor" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem Selected="True" Value="А">(A)-Pt1000</asp:ListItem>
                                        <asp:ListItem Value="">( )-DS1820</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Резервирование сети питания</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlReserve" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem Selected="True" Value="1">(1)-без резервирования</asp:ListItem>
                                        <asp:ListItem Value="2">(2)-два ввода электросети</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Контур № 1</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlK1" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>(0)-управление насосами</asp:ListItem>
                                        <asp:ListItem>(1)-система отопления</asp:ListItem>
                                        <asp:ListItem>(2)-система горячего водоснабжения</asp:ListItem>
                                        <asp:ListItem>(3)-система теплоснабжения вентиляции</asp:ListItem>
                                        <asp:ListItem>(4)-подпитка вторичного контура</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Контур № 2</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlK2" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-управление насосами</asp:ListItem>
                                        <asp:ListItem>(1)-система отопления</asp:ListItem>
                                        <asp:ListItem>(2)-система горячего водоснабжения</asp:ListItem>
                                        <asp:ListItem>(3)-система теплоснабжения вентиляции</asp:ListItem>
                                        <asp:ListItem>(4)-подпитка вторичного контура</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Контур № 3</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlK3" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-управление насосами</asp:ListItem>
                                        <asp:ListItem>(1)-система отопления</asp:ListItem>
                                        <asp:ListItem>(2)-система горячего водоснабжения</asp:ListItem>
                                        <asp:ListItem>(3)-система теплоснабжения вентиляции</asp:ListItem>
                                        <asp:ListItem>(4)-подпитка вторичного контура</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Контур № 4</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlK4" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-управление насосами</asp:ListItem>
                                        <asp:ListItem>(1)-система отопления</asp:ListItem>
                                        <asp:ListItem>(2)-система горячего водоснабжения</asp:ListItem>
                                        <asp:ListItem>(3)-система теплоснабжения вентиляции</asp:ListItem>
                                        <asp:ListItem>(4)-подпитка вторичного контура</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Тип интерфейса</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlInterface" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>(2)-RS-485</asp:ListItem>
                                        <asp:ListItem>(3)-RS-485 + Ethernet</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Степень защиты оболочки</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlIp" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>54</asp:ListItem>
                                        <asp:ListItem>65</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Тип модуля управления</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlModule" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>(01)-TTR-01</asp:ListItem>
                                        <asp:ListItem>(02)-TTR-02</asp:ListItem>
                                        <asp:ListItem>(03)-TTR-03</asp:ListItem>
                                        <asp:ListItem>(04)-TTR-04</asp:ListItem>
                                        <asp:ListItem>(05)-TTR-05</asp:ListItem>
                                        <asp:ListItem>(06)-TTR-06</asp:ListItem>
                                        <asp:ListItem>(07)-TTR-07</asp:ListItem>
                                        <asp:ListItem>(08)-TTR-08</asp:ListItem>
                                        <asp:ListItem>(09)-TTR-09</asp:ListItem>
                                        <asp:ListItem>(10)-TTR-10</asp:ListItem>
                                        <asp:ListItem>(11)-TTR-11</asp:ListItem>
                                        <asp:ListItem>(12)-TTR-12</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Управление подпиткой</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlSupport" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>( )-отсутствует или релейная схема</asp:ListItem>
                                        <asp:ListItem>(П)-модулем управления</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Конструктивное исполнение</label>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlConstruction" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>( )-внутренние управление и индикация</asp:ListItem>
                                        <asp:ListItem>(ЩМП)-внешние управление и индикация</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Марка корпуса</label>
                                </div>
                                <div class="col-6">
                                    <asp:TextBox ID="ddlBody" runat="server" CssClass="cp-form-text-box" Enabled="false">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Насосы -->

                    <div class="col-4">
                        <div class="parameter-box section-space">
                            <div class="row margin-top-bottom">
                                <div class="col-12 text-center">
                                    <label class="cp-label-primary">Управление насосами</label>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6 text-center">
                                    <label class="form-label">Основные</label>
                                </div>
                                <div class="col-6 text-center">
                                    <label class="form-label">Резервные</label>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlPumpMain1" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-6">

                                    <asp:DropDownList ID="ddlPumpReserve1" runat="server" CssClass="cp-form-select">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlPumpMain2" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-6">

                                    <asp:DropDownList ID="ddlPumpReserve2" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlPumpMain3" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-6">

                                    <asp:DropDownList ID="ddlPumpReserve3" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <asp:DropDownList ID="ddlPumpMain4" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-6">

                                    <asp:DropDownList ID="ddlPumpReserve4" runat="server" CssClass="cp-form-select" Enabled="false">
                                        <asp:ListItem>(0)-отсутствует</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="parameter-box section-space">

                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Доля ТМЦ (%)</label>
                                </div>
                                <div class="col-6">
                                    <asp:TextBox ID="txtShare" runat="server" Text="55" AutoPostBack="true" CssClass="form-control spinbox" TextMode="Number" step="1" min="0" Enabled="false">
                                    </asp:TextBox>
                                </div>
                            </div>
                            <div class="row margin-top-bottom">
                                <div class="col-6">
                                    <label class="form-label">Курс RUB / BYN</label>
                                </div>
                                <div class="col-4 non-padding-right">
                                    <asp:TextBox ID="txtRate" runat="server" Text="27.31" TextMode="Number" CssClass="form-control spinbox" AutoPostBack="true" step="0.01" min="0">
                                    </asp:TextBox>
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton
                                        ID="btnRefreshRate"
                                        runat="server"
                                        CssClass="refresh-rate-btn"
                                        ToolTip="Обновить курс"
                                        OnClick="btnRefreshRate_Click">
                                        ↻
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Ручной ввод -->

                <div class="parameter-box section-space">
                    <div class="row margin-top-bottom">
                        <div class="col-4">
                            <label class="form-label">Ручной ввод строки исполнения</label>
                        </div>
                        <div class="col-4">
                            <asp:TextBox ID="txtCode" runat="server" CssClass="cp-form-text-box" Text="ТШУА-1-1-10-2-IP65-01">
                            </asp:TextBox>
                        </div>
                        <div class="col-1 non-padding">
                            <asp:LinkButton
                                ID="LinkButton1"
                                runat="server"
                                CssClass="refresh-code-btn"
                                ToolTip="Сбросить введенное значение">
                                ↩
                            </asp:LinkButton>

                        </div>
                    </div>
                    <div class="row non-padding">
                        <div class="col-12">
                         <label class="error-label"></label>
                        </div>
                    </div>
                </div>
                <!-- Таблица стоимости -->

                <div class="price-box section-space">

                    <table class="table table-bordered table-hover align-middle mb-0">
                        <thead class="card-header">
                            <tr>
                                <th>Страна</th>
                                <th>Стоимость ТМЦ</th>
                                <th>Стоимость с долей ТМЦ</th>
                                <th>Розница без НДС</th>
                                <th>Розница с НДС</th>
                            </tr>
                        </thead>

                        <tbody>
                            <tr>
                                <td>РБ, BYN</td>
                                <td>269.84</td>
                                <td>490.63</td>
                                <td>686.88</td>
                                <td class="summary-green">824.25</td>
                            </tr>

                            <tr>
                                <td>РФ, RUB</td>
                                <td>7369.44</td>
                                <td>13398.98</td>
                                <td>30371.02</td>
                                <td class="summary-green">37052.65</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
   
                <div class="row ">
                    <div class="col-12">
                    <asp:Button ID="btnExcel" runat="server" Text="Сохранить в Excel" CssClass="btn btn-primary row-no-margin" style="width: 100%;"/>
                </div>
                    </div>
                <asp:HiddenField ID="hfToken" runat="server" />
            </div>
        </form>
    </div>
</body>

</html>
