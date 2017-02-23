<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="restore-items.aspx.cs" Inherits="Sitecore.sitecore.admin.RestoreItemsPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:ListBox runat="server" ID="ListBox"/> 
    </div>
    <div>
      <asp:Button runat="server" Text="Refresh" OnClick="Refresh" />
      <asp:Button runat="server" Text="Restore" OnClick="Restore" />
    </div>
    </form>
</body>
</html>
