# 人員資料管理應用程式
後端資料庫程式以SQL server及ASP.NET Web API建立。
前端網頁以ASP.NET Blazor建立。
## 功能

- **註冊**：新用戶可以註冊帳戶。
- **登入**：用戶可以登入系統。
![CRUD](https://raw.githubusercontent.com/HYT90/DemoEmployeeManagementSystemSolution/master/Client/wwwroot/demo_employee_management/update_employee.png)
- **公司部門員工資料**：可以查看和管理公司所有部門和員工的資料。
![檢視資料](https://raw.githubusercontent.com/HYT90/DemoEmployeeManagementSystemSolution/master/Client/wwwroot/demo_employee_management/employee_%E9%A0%81%E9%9D%A2.png)
- **員工差勤**：可以查看和管理員工的差勤記錄。
- **增刪改查**：提供了完整的 CRUD 功能，可以輕鬆地管理資料。
![頁面](https://raw.githubusercontent.com/HYT90/DemoEmployeeManagementSystemSolution/master/Client/wwwroot/demo_employee_management/%E4%B8%BB%E9%A0%81%E9%9D%A2%E6%88%AA%E5%9C%96.png)
![API](https://raw.githubusercontent.com/HYT90/DemoEmployeeManagementSystemSolution/master/Client/wwwroot/demo_employee_management/swagger.png)

## JWT 

使用 JSON Web Token (JWT) 來優化使用者體驗。當用戶登入後，系統會發送一個 JWT 給用戶。用戶可以使用這個 JWT 來存取受保護的路由和服務，而無需再次登入。

