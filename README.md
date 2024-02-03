# EmployeeManager App README

## Overview

EmployeeManager is an application developed as part of the C# course finals exam. This documentation provides step-by-step instructions to ensure the seamless setup and functionality of the EmployeeManager app.

## Prerequisites

Before proceeding with the setup, ensure you have the following prerequisites installed:

- Visual Studio
- SQL Server Management Studio (SSMS)

## Setup Instructions

Follow the steps below to set up and configure EmployeeManager:

### 1. Configure Photo Directory

Update the content of the `PhotoDirectory` string in the `App.xaml` file within the `UserInterface` project. This step is crucial to specify the exact location of user profile photos. The current photos are stored in the `UserPhotos` folder.

```xml
<!-- UserInterface/App.xaml -->

<Application x:Class="UserInterface.App" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Update the following line with the appropriate path -->
                <x:String x:Key="PhotoDirectory">C:\Path\To\UserPhotos</x:String>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>    
</Application>
```

### 2. Configure Database Connection

Update the connection string named `DefaultConnection` in the `appsettings.json` file within the `Web` project. This step is essential for establishing a connection between the database and the application.

```json
// Web/appsettings.json

{
  "ConnectionStrings": {
    // Update the following line with your database connection details
    "DefaultConnection": "Server=YourServer;Database=YourDatabase;Integrated Security=True;TrustServerCertificate=True;"
  },
  // Other configurations
}
```

### 3. Create SQL Database

Execute the `TEST_DOO.sql` script located in the `Sql` folder using SQL Server Management Studio (SSMS). This script will create the necessary SQL database for the EmployeeManager app.

### 4. Update SQL Database

Run the `updateDatabase.sql` script from the Sql folder using SQL Server Management Studio (SSMS). This script updates the SQL database with appropriate values required for the application.

## Conclusion

By following these steps meticulously, you will successfully set up and configure the EmployeeManager app for optimal functionality. 
