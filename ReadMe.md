# Mindex Coding Challenge

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### Endpoints you may call

GET api/employee/{id} - Returns an employee entity based off the ID. Should include compensation and all direct reports.

GET api/employee/getdirectreports/{id} - Gets a ReportingStructure object. This should include # of direct reports.

POST api/employee - Creates an employee using employee JSON.

PUT api/employee - Updates an employee.

POST api/compensation - Creates a compensation.

GET api/compensation/employeeid/{employeeid} - Gets a compensation, and employee for employee with ID.


Notes:
While the challenge asked for the ability to read by employee ID, it is possible in EF to get Compensation with an Includes by using api/employee/{id}.

