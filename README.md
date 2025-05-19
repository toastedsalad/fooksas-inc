# This app tracks gameplay time for pool tables

Title says it all.

TODOs:

* DONE Need session repo.
* DONE Need schedule repo.
* Refactor schedule repo to not do DTO transformations. Only getting data. 
* Add a schedule service which will do DTO conversions given a repo.
* Need schedule controller. 
* Table management frontend.
* Schedule management frontend.
* Live session dumping and retrieval.
* Retrieval of sessions by table number is not good, we might need to retrieve by table name.
* Should I add saving as part of the call to add something to the repo? Or handle it manually.


Instead of time remaining do play for money.
Switch goes off on standby.
Remainder of money should be accounted.`
One card one discount.


Migrations are done like this:
1. Add your new entity or edit it in code:
2. dotnet ef migrations add AddScheduleWeeklyRates


