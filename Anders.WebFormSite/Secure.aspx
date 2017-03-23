<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Secure.aspx.cs" Inherits="Anders.WebFormSite.Secure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <p>Secure!</p>

        <asp:Repeater ID="rptClaims" runat="server">
            <ItemTemplate>
                <strong><%# Eval("name") %>:</strong> <%# Eval("value") %>
                <br />
            </ItemTemplate>
        </asp:Repeater>

        <p>
            <asp:LinkButton ID="signout" OnClick="signout_Click" runat="server">Sign out</asp:LinkButton>
        </p>
    </form>
</body>
</html>
