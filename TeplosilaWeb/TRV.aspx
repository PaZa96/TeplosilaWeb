﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TRV.aspx.cs" Inherits="TRV" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <link href="Content/bootstrap.min.css" rel="stylesheet" />
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link href="Content/css/style.css" rel="stylesheet" />
        <title></title>
    </head>
    <body> 
        <div class="container">
            <form id="form1" runat="server">
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <div class="row">
                    <div class="col-10">
                        <div class="col border">
                            <asp:Label ID="Label1" runat="server" Text="Место установки:"></asp:Label>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="sprRadioButtonList1" runat="server" style="margin-left: 0px" AutoPostBack="True" required="required">
                                        <asp:ListItem>ЦТП</asp:ListItem>
                                        <asp:ListItem>ИТП</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="sprRequiredFieldValidator1" runat="server" ControlToValidate="sprRadioButtonList1" Display="Dynamic" ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">Выберите необходимое значение</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label2" runat="server" Text="Наличие регулятора перепада давления:"></asp:Label>
                                    <asp:RadioButtonList ID="prvRadioButtonList1" required="required" runat="server" AutoPostBack="True" >
                                        <asp:ListItem>Да</asp:ListItem>
                                        <asp:ListItem>Нет</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="prvRequiredFieldValidator1" runat="server" ControlToValidate="prvRadioButtonList1" Display="Dynamic" ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">Выберите необходимое значение</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label3" runat="server" Text="Область применения (система) / Схема присоединения"></asp:Label><br/>                      
                                    <asp:RadioButton ID="aaRadioButton1" runat="server"  Text="Горячее водоснабжение" AutoPostBack="True" required="required" OnCheckedChanged="aaRadioButton1_CheckedChanged"/>
                                    <asp:RadioButtonList ID="aa1RadioButtonList1" required="required" CssClass="childRadio" runat="server" RepeatDirection="Horizontal" Enabled="False"  AutoPostBack="True" OnSelectedIndexChanged="aa1RadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>Открытая</asp:ListItem>
                                        <asp:ListItem>Закрытая (через теплообменник)</asp:ListItem>
                                    </asp:RadioButtonList>
                        
                                    <asp:RadioButton ID="aaRadioButton2" runat="server" Text="Отопление" AutoPostBack="True" required="required" OnCheckedChanged="aaRadioButton2_CheckedChanged"/>
                                    <asp:RadioButtonList ID="aa2RadioButtonList1" required="required" CssClass="childRadio" runat="server" RepeatDirection="Horizontal" Enabled="False" AutoPostBack="True" OnSelectedIndexChanged="aa2RadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>Зависимая</asp:ListItem>
                                        <asp:ListItem>Независимая (через теплообменник)</asp:ListItem>
                                    </asp:RadioButtonList>
                        
                                    <asp:RadioButton ID="aaRadioButton3" runat="server" Text="Вентиляция" AutoPostBack="True"  required="required" OnCheckedChanged="aaRadioButton3_CheckedChanged"/>
                                    <asp:RadioButtonList ID="aa3RadioButtonList1" required="required" CssClass="childRadio" runat="server" RepeatDirection="Horizontal" Enabled="False"  AutoPostBack="True" OnSelectedIndexChanged="aa3RadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>Зависимая</asp:ListItem>
                                        <asp:ListItem>Независимая (через теплообменник)</asp:ListItem>
                                    </asp:RadioButtonList>
                                   
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label4" runat="server" Text="Тип клапана:"></asp:Label>
                                    <asp:RadioButtonList ID="tvRadioButtonList1" runat="server" AutoPostBack="True" required="required" OnSelectedIndexChanged="tvRadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>2-х ходовой седельный</asp:ListItem>
                                        <asp:ListItem>3-х ходовой смесительный</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="tvRequiredFieldValidator1" runat="server" ControlToValidate="tvRadioButtonList1" Display="Dynamic" ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">Выберите необходимое значение</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="col border">
                            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label5" runat="server" Text="Рабочая среда:"></asp:Label>
                                    <asp:RadioButtonList ID="ws2RadioButtonList1" runat="server" AutoPostBack="True"  required="required" OnSelectedIndexChanged="ws2RadioButtonList1_SelectedIndexChanged">
                                        <asp:ListItem>Вода</asp:ListItem>
                                        <asp:ListItem>Этиленгликоль</asp:ListItem>  
                                        <asp:ListItem>Пропиленгликоль</asp:ListItem>                      
                                    </asp:RadioButtonList>
                                    <asp:TextBox ID="ws2TextBox1" runat="server" Enabled="False" type="number" required="required" ></asp:TextBox>
                                    <asp:Label ID="Label6" runat="server" Text="%(от 5% до 65%)"></asp:Label>&nbsp;&nbsp;&nbsp;
                                    <asp:RangeValidator ID="wsRangeValidator1" runat="server" Display="Dynamic"  ControlToValidate="ws2TextBox1" ErrorMessage="Значение должно находится в диапазоне от 5% до 65%" ForeColor="Red" MaximumValue="65" MinimumValue="5" SetFocusOnError="True" Type="Double"></asp:RangeValidator>
                                    <br/>
                                    <asp:TextBox ID="ws2TextBox2" runat="server" Enabled="False" type="number" required="required"></asp:TextBox>
                                    <asp:Label ID="Label7" runat="server" Text="&#8451; (от 0&#8451; до 150&#8451;)"></asp:Label>&nbsp;&nbsp;&nbsp;
                                    <asp:RangeValidator ID="wsRangeValidator2" runat="server" ControlToValidate="ws2TextBox2" Display="Dynamic" ErrorMessage="Значение должно находится в диапазоне от 0&amp;#8451 до 150&amp;#8451" ForeColor="Red" MaximumValue="150" MinimumValue="0" SetFocusOnError="True" Type="Double"></asp:RangeValidator>
                                    <br />
                                    <asp:RequiredFieldValidator ID="ws2RequiredFieldValidator1" runat="server" ControlToValidate="ws2RadioButtonList1" Display="Dynamic" ErrorMessage="Выберите необходимое значение" ForeColor="Red" SetFocusOnError="True">Выберите необходимое значение</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    
                            <div class="col border">
                                <asp:Label ID="Label8" runat="server" Text="Потери давления в системе:"></asp:Label>
                                <div class="col">
                                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                        <ContentTemplate>
                                            <asp:Label ID="Label9" runat="server" Text="&#916;Pсист = "></asp:Label>
                                            <asp:TextBox ID="lpvTextBox2" runat="server" Enabled="False" type="number" TextMode="Number" required="required"></asp:TextBox>
                                            <asp:DropDownList ID="lpvDropDownList2" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="lpvDropDownList2_SelectedIndexChanged">
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                            </asp:DropDownList>
                                            <br /> 
                                            <asp:RangeValidator ID="lpvRangeValidator2" runat="server" ControlToValidate="lpvTextBox2" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MinimumValue="1" SetFocusOnError="True" MaximumValue="999999999999">Неверно указано значение давления</asp:RangeValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="col border">
                                <asp:Label ID="Label11" runat="server" Text="Потери давления в теплообменнике:"></asp:Label>
                                <div class="col">
                                    <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                                        <ContentTemplate>
                                            <asp:Label ID="Label10" runat="server" Text="&#916;Pто = "></asp:Label>
                                            <asp:TextBox ID="lpvTextBox21" runat="server" Enabled="False" type="number" required="required"></asp:TextBox>
                                            <asp:DropDownList ID="lpvDropDownList21" runat="server"  AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="lpvDropDownList21_SelectedIndexChanged"  >
                                            <asp:ListItem>выбрать</asp:ListItem>
                                            <asp:ListItem>МПа</asp:ListItem>
                                            <asp:ListItem>кПа</asp:ListItem>
                                            <asp:ListItem>бар</asp:ListItem>
                                            <asp:ListItem>м. в. ст.</asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                            <asp:RangeValidator ID="lpvRangeValidator21" runat="server" ControlToValidate="lpvTextBox21" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MinimumValue="1" SetFocusOnError="True" MaximumValue="99999999">Неверно указано значение давления</asp:RangeValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        
                        <div class="col border">
                            <asp:Label ID="Label12" runat="server" Text="Потери давления на клапане:"></asp:Label>
                            <div class="col">
                                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                    <ContentTemplate>
                                        <asp:Label ID="Label13" runat="server" Text="&#916;Pкл = "></asp:Label>
                                        <asp:TextBox ID="lpvTextBox1" runat="server" Enabled="False" type="number" required="required"></asp:TextBox>
                                        <asp:DropDownList ID="lpvDropDownList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="lpvDropDownList1_SelectedIndexChanged">
                                        <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList>
                                        <br />
                                        <asp:RangeValidator ID="lpvRangeValidator1" runat="server" ControlToValidate="lpvTextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="99999999" MinimumValue="1" SetFocusOnError="True">Неверно указано значение давления</asp:RangeValidator>
                                               
                                        <asp:CustomValidator ID="lpvCustomValidator1" runat="server" ControlToValidate="lpvTextBox1" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="Red"  SetFocusOnError="True"></asp:CustomValidator>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="col border">
                            <asp:Label ID="Label20" runat="server" Text="Расчёт регулирующего клапана на кавариацию:"></asp:Label>
                            <div class="col">
                                <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                    <ContentTemplate>
                                        <asp:Label ID="Label26" runat="server" Text="Давление перед клапаном:"></asp:Label><br/>
                                        <asp:Label ID="Label21" runat="server" Text="P' = "></asp:Label>
                                        <asp:TextBox ID="calcvTextBox1" runat="server" Enabled="False" type="number" TextMode="Number" required="required" ></asp:TextBox>
                                        <asp:DropDownList ID="calcvDropDownList1" runat="server" AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="calcvDropDownList1_SelectedIndexChanged">
                                        <asp:ListItem>выбрать</asp:ListItem>
                                        <asp:ListItem>МПа</asp:ListItem>
                                        <asp:ListItem>кПа</asp:ListItem>
                                        <asp:ListItem>бар</asp:ListItem>
                                        <asp:ListItem>м. в. ст.</asp:ListItem>
                                        </asp:DropDownList><br />
                                        <asp:RangeValidator ID="calcvRangeValidator1" runat="server" ControlToValidate="calcvTextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="99999999" MinimumValue="1" SetFocusOnError="True">Неверно указано значение давления</asp:RangeValidator>
                                    
                                        <asp:CustomValidator ID="calcvCustomValidator1" runat="server" ErrorMessage="CustomValidator" ControlToValidate="calcvTextBox1" Display="Dynamic" ForeColor="Red"  SetFocusOnError="True"></asp:CustomValidator>
                                        <br/>
                                        <asp:Label ID="Label22" runat="server" Text="Максимальная температура теплоносителя через клапан:"></asp:Label><br/>
                                        <asp:Label ID="Label23" runat="server" Text="T1 = "></asp:Label>
                                        <asp:TextBox ID="calcvTextBox2" runat="server" Enabled="False" type="number" required="required"></asp:TextBox>
                                        <asp:Label ID="Label24" runat="server" Text=" &#8451;"></asp:Label>
                                        <asp:RangeValidator ID="calcvRangeValidator2" runat="server" ErrorMessage="Неверно указано значение температуры" ControlToValidate="calcvTextBox2" Display="Dynamic" ForeColor="Red" MaximumValue="9999999" MinimumValue="0" SetFocusOnError="True" Type="Double">Неверно указано значение температуры</asp:RangeValidator>
                                        <asp:CustomValidator ID="calcvCustomValidator2" runat="server" ErrorMessage="CustomValidator" ControlToValidate="calcvTextBox2" Display="Dynamic" ForeColor="Red" SetFocusOnError="True"></asp:CustomValidator>
                                                
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="col border">
                            <div>
                                <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                                    <ContentTemplate>
                                        <asp:RadioButton ID="fvRadioButton1" runat="server" Text="Задать max величину расхода через клапан:" AutoPostBack="True"   OnCheckedChanged="fvRadioButton1_CheckedChanged"/><br/>
                                        <div class="col">
                                            <asp:Label ID="Label28" runat="server" Text="Gкл = "></asp:Label>
                                            <asp:TextBox ID="fvTextBox1" runat="server" Enabled="False" type="number" required="required"></asp:TextBox>
                                            <asp:DropDownList ID="fvDropDownList1" runat="server"  AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="fvDropDownList1_SelectedIndexChanged"  >
                                                <asp:ListItem>выбрать</asp:ListItem>
                                                <asp:ListItem>м3/ч</asp:ListItem>
                                                <asp:ListItem>л/с</asp:ListItem>
                                                <asp:ListItem>л/мин</asp:ListItem>
                                                <asp:ListItem>л/ч</asp:ListItem>
                                                <asp:ListItem>кг/с</asp:ListItem>
                                                <asp:ListItem>кг/ч</asp:ListItem>
                                                <asp:ListItem>т/ч</asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                            <asp:RangeValidator ID="RangeValidator9" runat="server" ControlToValidate="fvTextBox1" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="99999999" MinimumValue="1" SetFocusOnError="True">Неверно указано значение давления</asp:RangeValidator>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div>
                                <asp:UpdatePanel ID="UpdatePanel14" runat="server">
                                    <ContentTemplate>
                                        <asp:RadioButton ID="fvRadioButton2" runat="server" Text="Вычислить max величину расхода через клапан:" AutoPostBack="True" OnCheckedChanged="fvRadioButton2_CheckedChanged"  /> 
                                        <div class="col">
                                            <table class="table table-bordered col-8">
                                              <thead>
                                                <tr>
                                                  <th scope="col"></th>
                                                  <th scope="col"></th>
                                                  <th scope="col">Температура подающего теплоносителя</th>
                                                  <th scope="col">Температура обратного теплоносителя</th>
                                                </tr>
                                              </thead>
                                              <tbody>
                                                <tr>
                                                 <th rowspan="2" scope="row" class="align-middle">Параметры теплосети</th>
                                                  <td>Зима</td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label27" runat="server" Text="T1 = "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox2" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label29" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label32" runat="server" Text="T2 = "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox3" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label33" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                </tr>
                                                <tr>
                                                  <td>Лето</td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label30" runat="server" Text="T'1 = "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox4" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label31" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label34" runat="server" Text="T'2 = "></asp:Label> 
                                                    <asp:TextBox ID="fvTextBox5" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label35" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                </tr>
                                                <tr>
                                                  <th rowspan="2" scope="row" class="align-middle">Параметры системы</th>
                                                  <td>Отопления</td>
                                                   <td class="text-center">
                                                    <asp:Label ID="Label36" runat="server" Text="T21 = "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox6" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label37" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label38" runat="server" Text="T22= "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox7" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label39" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                </tr>
                                                <tr>
                                                  <td>Вентиляции</td>
                                                   <td class="text-center">
                                                    <asp:Label ID="Label40" runat="server" Text="T11 = "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox8" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label41" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                  <td class="text-center">
                                                    <asp:Label ID="Label42" runat="server" Text="T12= "></asp:Label>
                                                    <asp:TextBox ID="fvTextBox9" runat="server" Enabled="False" type="number" Width="60px" required="required"></asp:TextBox>
                                                    <asp:Label ID="Label43" runat="server" Text=" &#8451;"></asp:Label>
                                                  </td>
                                                </tr>
                                              </tbody>
                                            </table>
                                            
                                            <br />
                                            <br />

                                            <asp:Label ID="Label45" runat="server" Text="Тепловая мощность Q = "></asp:Label>
                                            <asp:TextBox ID="fvTextBox10" runat="server" Enabled="False" type="number" TextMode="Number" required="required"></asp:TextBox>
                                            <asp:DropDownList ID="fvDropDownList2" runat="server"  AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="fvDropDownList2_SelectedIndexChanged">
                                                <asp:ListItem>выбрать</asp:ListItem>
                                                <asp:ListItem Value="кВт">кВт</asp:ListItem>
                                                <asp:ListItem>МВт</asp:ListItem>
                                                <asp:ListItem>Вт</asp:ListItem>
                                                <asp:ListItem>Гкал/ч</asp:ListItem>
                                                <asp:ListItem>ккал/ч</asp:ListItem>
                                            </asp:DropDownList><br />
                                            <asp:RangeValidator ID="fvRangeValidator2" runat="server" ControlToValidate="fvTextBox10" Display="Dynamic" ErrorMessage="Неверно указано значение давления" ForeColor="Red" MaximumValue="99999999" MinimumValue="1" SetFocusOnError="True">Неверно указано значение давления</asp:RangeValidator>
                                    <br/>
                                            <asp:Label ID="Label46" runat="server" Text="Максимальный расход Gкл = "></asp:Label>
                                            <asp:TextBox ID="fvTextBox11" runat="server" Enabled="False" type="number" ReadOnly="True" required="required"></asp:TextBox>
                                            <asp:Label ID="Label48" runat="server" Text=" кг/ч"></asp:Label>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="col border">
                            <asp:Label ID="Label44" runat="server" Text="Характеристики электропривода:"></asp:Label><br/>
                            <div class="col">
                                <asp:UpdatePanel ID="UpdatePanel15" runat="server">
                                    <ContentTemplate>
                                        <asp:Label ID="Label47" runat="server" Text="Напряжение питания:"></asp:Label>
                                        <asp:RadioButtonList ID="tdRadioButtonList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="tdRadioButtonList1_SelectedIndexChanged">
                                            <asp:ListItem Selected="True">230 VAC</asp:ListItem>
                                            <asp:ListItem>24 VAC / VDC</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:Label ID="Label49" runat="server" Text="Напряжение питания:"></asp:Label>
                                        <asp:RadioButtonList ID="tdRadioButtonList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="tdRadioButtonList2_SelectedIndexChanged" >
                                            <asp:ListItem Selected="True">Трёхпозиционное</asp:ListItem>
                                            <asp:ListItem>Аналоговое 4-20 mA (2-10 V)</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:Label ID="Label50" runat="server" Text="Наличие датчика положения 4-20 mA:"></asp:Label>
                                        <asp:RadioButtonList ID="tdRadioButtonList3" runat="server" AutoPostBack="True" OnSelectedIndexChanged="tdRadioButtonList3_SelectedIndexChanged" >
                                            <asp:ListItem>Да</asp:ListItem>
                                            <asp:ListItem Selected="True">Нет</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:Label ID="Label51" runat="server" Text="Наличие возвратного механизма (функция безопасности):"></asp:Label>
                                        <asp:RadioButtonList ID="tdRadioButtonList4" runat="server" AutoPostBack="True" OnSelectedIndexChanged="tdRadioButtonList4_SelectedIndexChanged">
                                             <asp:ListItem> Да</asp:ListItem>
                                            <asp:ListItem Selected="True"> Нет</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <asp:Button ID="vButton" runat="server" type="submit" Text="Рассчитать"  />
                        </div>
                        
                         
                    </div>
                    <div class="col-2">
                    <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                        <ContentTemplate>
                            <asp:Image ID="vPictureBox" runat="server" class="valve-image" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </div>
            </form>
        </div>
    </body>
</html>
